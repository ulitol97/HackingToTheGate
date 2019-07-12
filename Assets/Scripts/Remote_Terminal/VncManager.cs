using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Game.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityObserver;
using VncSharp4Unity2D;

namespace Remote_Terminal
{
    /// <summary>
    /// The VncManager class is in charge of establishing an VNC connection to a remote target host via an SSH tunnel
    /// if available and handle the VNC authentication process if needed.
    /// Once the connection has been established, it requests periodic updates from the host's remote desktop and
    /// updates the in-game sprites representing that remote desktop.
    /// As a singleton, a maximum of one instance of this class will be managed by the game.
    /// As an observed subject, it has a connection status <see cref="ConnectionStatus"/> which is notified to the
    /// registered observers if changed.
    /// </summary>
    public class VncManager : UnitySingleton<VncManager>
    {
        // Sprite and texture updating
        
        /// <summary>
        /// Sprite holding the live image of the remote desktop.
        /// </summary>
        public Sprite remoteDesktopSprite;
        
        /// <summary>
        /// Boolean flag representing if the byte array holding the current desktop image has changed
        /// and should be refreshed with a new image.
        /// </summary>
        private bool _bytesChanged;

        // SSH Forwarding for VNC

        /// <summary>
        /// Manager in charge of handling the Ssh connection to the remote host.
        /// </summary>
        private SshManager _sshManager;
		
		/// <summary>
        /// Username used in the Ssh authentication process.
        /// </summary>
        private string _sshUserName;
        /// <summary>
        /// Password used in the Ssh authentication process.
        /// </summary>
        private string _sshPassword;
        
        /// <summary>
        /// Port used to establish the Ssh connection with the remote host.
        /// </summary>
		private int _sshPort;

        /// <summary>
        /// True if the Ssh authentication should be made via user and private key.
        /// False if the Ssh authentication should be made via user and password.
        /// </summary>
        private bool _sshConnectViaKey;
        
        /// <summary>
        /// Path of the key file used in the Ssh authentication process.
        /// </summary>
        private string _sshKeyPath;
        /// <summary>
        /// Passphrase of the private key used in the Ssh authentication process.
        /// </summary>
        private string _sshKeyPassphrase;

        // VNC

        /// <summary>
        /// Boolean flag marked true while the game is periodically launching connection attempts against the
        /// VNC server.
        /// </summary>
        private bool _attemptingConnection;

        /// <summary>
        /// Constant representing the localhost IP used when forwarding VNC via SSH.
        /// </summary>
        private const string Localhost = "127.0.0.1";

        /// <summary>
        /// Target host to which the VNC client should connect.
        /// </summary>
        private string _vncHost;

        /// <summary>
        /// Port used to establish the VNC connection with the remote host.
        /// </summary>
        private uint _vncPort;
        
        /// <summary>
        /// Password used in the VNC authentication process.
        /// </summary>
        private string _vncPassword;

        // Bitmap handling
        
        /// <summary>
        /// Bitmap object representation of the currently displayed image of the remote host.
        /// </summary>
        private Bitmap _desktopImage;
        
        /// <summary>
        /// Byte array storing the the currently displayed image of the remote host.
        /// </summary>
        private byte[] _desktopImageBytes;
        
        /// <summary>
        /// VNC client managed by the VncSharp API.
        /// </summary>
        private RemoteDesktop _client;

        /// <summary>
        /// Interval between connection checks.
        /// </summary>
        private int _checkConnectionInterval;
        
        /// <summary>
        /// Minimum amount of time passed between connection attempts/checks.
        /// </summary>
        private const int MinConnectionInterval = 8;

        // Keyboard operations
        
        /// <summary>
        /// Manager in charge of handling the keyboard events send to the remote host.
        /// </summary>
        public KeyboardManager keyboardManager;
        
        // Threading

        /// <summary>
        /// Thread object representing an independent thread in charge of operating the BitmapManager to avoid
        /// interrupting the game.
        /// </summary>
        private Thread _imageConversionThread;
        
        /// <summary>
        /// Boolean flag indicating if the image processing separate thread is running or not.
        /// </summary>
        private bool _threadRunning;
        
        /// <summary>
        /// Holds a delegate containing the actions the VncManager should take each frame update to refresh the
        /// remote desktop image.
        /// </summary>
        private Action _pendingAction; // Refresh screen action.
        
        
        // Observer pattern based on signals (observers)

        /// <summary>
        /// Signal observing changes in the connection status of the server.
        /// </summary>
        [FormerlySerializedAs("serverStatusSignal")] public SignalSubject serverStatus;

        // Getters / Setters

        private RemoteDesktop RemoteDesktop
        {
            get { return _client; }
        }


        /// <summary>
        /// Returns true if there's a SSH connection currently established between the SSH client and remote host,
        /// otherwise returns false.
        /// </summary>
        private bool SshConnected
        {
            get { return _sshManager != null && _sshManager.Connected; }
        }
        
        /// <summary>
        /// Returns true if there's a VNC connection currently established between the VNC client and remote host,
        /// otherwise returns false.
        /// </summary>
        private bool VncConnected
        {
            get { return _client != null && _client.IsConnected; }
        }

        /// <summary>
        /// Allows external objects observing the VncManager to know the state of the connection and act in consequence.
        /// </summary>
        public bool ConnectionStatus
        {
            get { return SshConnected && VncConnected; }
        }


        /// <summary>
        /// Function called when the VncManager is inserted into the game.
        /// Checks if an VncManager is running already. If it is not, sets up all the SSH and VNC clients involved
        /// in the connection and begins the image refresh loop.
        /// </summary>
        private void Awake()
        {
            if (GameObject.FindGameObjectsWithTag("RemoteServer").Length > 1)
                gameObject.SetActive(false);
            
            // Set to not destroy between scenes.
            else
                DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SetUpFromConfiguration();
            if (_checkConnectionInterval < MinConnectionInterval)
                _checkConnectionInterval = MinConnectionInterval;
            
            _sshManager = new SshManager();
            ConnectToHost();
        }

        private void SetUpFromConfiguration()
        {
            if (ConfigurationManager.Instance == null)
                return;
            _sshUserName = ConfigurationManager.Instance.connectionConfig.sshConnectionInfo.username;
            _sshPassword = ConfigurationManager.Instance.connectionConfig.sshConnectionInfo.password;
            _sshPort = ConfigurationManager.Instance.connectionConfig.sshConnectionInfo.port;
            _sshConnectViaKey = ConfigurationManager.Instance.connectionConfig.sshConnectionInfo.publicKeyAuth.preferSshPublicKey;
            _sshKeyPath = ConfigurationManager.Instance.connectionConfig.sshConnectionInfo.publicKeyAuth.path;
            _sshKeyPassphrase = ConfigurationManager.Instance.connectionConfig.sshConnectionInfo.publicKeyAuth.passPhrase;
            _vncHost = ConfigurationManager.Instance.connectionConfig.vncConnectionInfo.targetHost;
            _vncPassword = ConfigurationManager.Instance.connectionConfig.vncConnectionInfo.vncServerPassword;
            _vncPort = (uint) ConfigurationManager.Instance.connectionConfig.vncConnectionInfo.port;
            _checkConnectionInterval = ConfigurationManager.Instance.connectionConfig.secondsBetweenConnectionAttempts;
        }

        /// <summary>
        /// Launches an attempt to connect the SSH and VNC clients to the VNC server machine.
        /// If it fails on its first attempt, will launch a periodic process in charge of trying to connect
        /// periodically to the specified host <see cref="AttemptToConnect"/>.
        /// If the connection attempt succeeds, calls the process that will arrange the live desktop visualization.
        /// </summary>
        private void ConnectToHost()
        {
            bool connect = true;
            try
            {
                // Attempt to contact the server both via SSH and VNC.
                SetUpSshConnection();
                SetUpVncConnection();
            }
            catch (Exception) // If the server could not be contacted, periodically try again.
            {
                connect = false;

                if (!_attemptingConnection)
                    StartCoroutine(AttemptToConnect());
            }
            
            if (connect)
            {
                StartCoroutine(ConfirmVncConnection());
                StartCoroutine(CheckConnectionStatus());
                serverStatus.Notify();
            }
        }

        /// <summary>
        /// Coroutine in charge of periodically requesting the game a new connection attempt to the server in case
        /// either the SSH or VNC client has not reached the host.
        /// </summary>
        /// <remarks>The periodic attempts will be run each N number of seconds
        /// <see cref="_checkConnectionInterval"/>.</remarks>
        private IEnumerator AttemptToConnect()
        {
            _attemptingConnection = true;
            while (!SshConnected || !VncConnected)
            {
                ConnectToHost();
                yield return new WaitForSeconds(_checkConnectionInterval);
            }
            _attemptingConnection = false;
        }

        /// <summary>
        /// Function called on each frame the VncManager is present into the game.
        /// Checks for pending actions involving refreshing the remote desktop in-game image and executes them.
        /// </summary>
        /// <remarks>In order to balance efficiency and usability, the VncManager handles a maximum of
        /// one screen refresh per frame</remarks>
        private void Update()
        {
            if (_pendingAction != null) // 1 refresh max per update
            {
                _pendingAction.Invoke();
                _pendingAction = null;
            }
        }

        /// <summary>
        /// Initializes a new SshManager in charge of logging into the remote server via Ssh and start a tunnel
        /// between client and servers VNC port (590X).
        /// </summary>
        /// <remarks>In case there's already a connected SSH client, this function will return automatically.</remarks>
        private void SetUpSshConnection()
        {
            if (SshConnected)
                return;
            
            // If no key path has been specified, don't try to connect this way.
            if (_sshConnectViaKey && !_sshKeyPath.Trim().Equals(""))
                _sshManager.SetUpManager(_vncHost, _sshPort, _sshUserName, _sshKeyPath, _sshKeyPassphrase);

            else
                _sshManager.SetUpManager(_vncHost, _sshPort, _sshUserName, _sshPassword);

            _sshManager.CreateForwardPort(Localhost, _vncPort, _vncHost, _vncPort);
            _sshManager.Connect();
        }

        /// <summary>
        /// Initializes a new RemoteDesktop client in charge of logging into the remote server via VNC.
        /// </summary>
        /// <remarks>In case there's already a connected VNC client, this function will return automatically.</remarks>
        private void SetUpVncConnection()
        {
            // If a VNC client is up and running
            if (VncConnected)
                return;

            // If we are tunneling the VNC connection through SSH, connect to "localhost", not remote machine address.
            if (SshConnected && _sshManager.Forward)
                _vncHost = Localhost;

            // Not specifying a display, the target tty will depend on the server port of choice
            _client = new RemoteDesktop(_vncHost, (int) _vncPort, _vncPassword);

            // Connect via VNC
            _client.Connect();
        }


        /// <summary>
        /// Co-routine in charge of checking if the connection to the remote desktop
        /// is successful on startup or not and proceed to update the in-game graphics if it is.
        /// </summary>
        private IEnumerator ConfirmVncConnection()
        {
            // Wait until connection has been established by VNC client
            yield return new WaitUntil(() => VncConnected);
            
            // With the VNC client connected, proceed
            StartCoroutine(StartVncRequests());
        }

        /// <summary>
        /// Co-routine in charge of creating a separate execution thread that will change the initial static Sprite
        /// to the remote desktop image periodically without blocking the game main thread.
        /// </summary>
        private IEnumerator StartVncRequests()
        {
            if (!_threadRunning)
            {
                _imageConversionThread = new Thread(RetrieveScreenBytes);
                _imageConversionThread.Start();
            }
            yield return new WaitForFixedUpdate();
        }


        /// <summary>
        /// Co-routine in charge of creating a new sprite from the bytes received from the server and un-marking the
        /// _bytesChanged flag for the game to request new sprite updates.
        /// In case a sprite texture is already in use, destroy it when replaced to prevent memory leaks.
        /// </summary>
        private IEnumerator<float> UpdateVncSprite()
        {
            // Load a texture form the remote image.
            try
            {
                Texture2D newTexture = BitmapManager.BitmapToTexture2D(_desktopImage, _desktopImageBytes);
                Texture2D oldTexture = remoteDesktopSprite.texture;

                remoteDesktopSprite = Sprite.Create(newTexture,
                    new Rect(0, 0, newTexture.width, newTexture.height),
                    new Vector2(0.5f, 0.5f));

                Destroy(oldTexture);
            }
            finally
            {
                _desktopImage.Dispose();
                _desktopImage = null;
                _bytesChanged = false;
                _pendingAction = null;
            }
            // Wait 1 frame.
            yield return new float();
        }

        /// <summary>
        /// Method that runs in a parallel thread to the main game execution.
        /// In charge of querying the VNC client for the remote desktop image representation, retrieving the image bytes
        /// and arranging the transformation of that image into an in-game sprite.
        /// </summary>
        private void RetrieveScreenBytes()
        {
            _threadRunning = true;
            while (VncConnected && _threadRunning)
            {
                if (!_bytesChanged && _pendingAction == null)
                {
                    _desktopImage = _client.Desktop.Clone() as Bitmap;
                    
                    _desktopImageBytes = BitmapManager.Bitmap2RawBytes(_desktopImage);
                    _bytesChanged = true;
                    _pendingAction = () => StartCoroutine(UpdateVncSprite());
                }
            }
        }
        
        /// <summary>
        /// Checks if a key event code corresponds to the "None" key event in Unity. If it doesn't, sends
        /// the keystroke over to the server write buffer via the VNC API.
        /// </summary>
        /// <param name="vKey">Hexadecimal virtual keycode sent to the server buffer.</param>
        /// <param name="pressed">True if the key stroke is a key press, false if it is a key release.</param>
        public void SendKeyToServer(uint vKey, bool pressed)
        {
            if (vKey != 0x00 && GetInstance(true).RemoteDesktop != null)
                RemoteDesktop.VncClient.WriteKeyboardEvent(vKey, pressed);
        }


        /// <summary>
        /// Co-routine in charge of checking the VNC connection status periodically and act in consequence.
        /// If the connection status is offline, will stop the in-game screen refreshing
        /// and order the start of a new whole connection process.
        /// </summary>
        /// <remarks>The periodic checks will be run each N number of seconds
        /// <see cref="_checkConnectionInterval"/>.</remarks>
        private IEnumerator CheckConnectionStatus()
        {
            while (_client != null) // Run as long as there's a remote  desktop component active
            {
                serverStatus.Notify();
                if (!ConnectionStatus)
                {
                    // Cancel any pending refreshing actions.
                    CancelScreenRefresh();
                    // Notify all observers about the disconnection
                    serverStatus.Notify();
                    // Re-start the connection process
                    ConnectToHost();
                }
                yield return new WaitForSeconds(_checkConnectionInterval);
            }
        }
        

        /// <summary>
        /// Cancels the next screen update before it can be shown in the game world.
        /// </summary>
        private void ResetScreenRefresh()
        {
            // Cancel any pending refreshing actions.
            _bytesChanged = false;
            _pendingAction = null;
            _attemptingConnection = false;
        }

        /// <summary>
        /// Cancels the next screen update and prevents the game from displaying any more images
        /// coming from the VNC server. 
        /// </summary>
        private void CancelScreenRefresh()
        {
            ResetScreenRefresh();
            // Stop threaded screen refreshing.
            if (_threadRunning)
            {
                // Mark threaded work as not begun.
                _threadRunning = false;

                // Wait until the thread exits, ensuring any cleanup we do after this is safe. 
                _imageConversionThread.Join();
            }
        }

        /// <summary>
        /// Checks if the SSH and VNC clients are running and disconnects them form the host. Then sets their values
        /// to null in order to allow new clients to be created via singleton instantiation.
        /// </summary>
        private void RemoveClients()
        {
            if (VncConnected)
            {
                _client.Dispose(false);
                _client = null;
            }

            if (SshConnected)
                _sshManager.Dispose();
        }

        private void DisconnectClients()
        {
            if (VncConnected)
            {
                _client.Disconnect();
                _client = null;
            }
            if (SshConnected)
                _sshManager.Disconnect();
        }

        /// <summary>
        /// Called when the game is quit (e.g: on game closing) to interrupt the connection to the remote host nicely.
        /// </summary>
        private void OnDestroy()
        {
            // Stop the remote desktop refresh process
            if (gameObject.activeInHierarchy || SceneManager.GetActiveScene().name.
                    Equals(ConfigurationManager.MenuScene))
            {
                CancelScreenRefresh();
                DisconnectClients();
            }
        }
        
        /// <summary>
        /// Called when the game is quit (e.g: on game closing) to interrupt the connection to the remote host nicely.
        /// </summary>
        private void OnApplicationQuit()
        {
            // Stop the remote desktop refresh process
            CancelScreenRefresh();
            RemoveClients();
        }
    }
}
