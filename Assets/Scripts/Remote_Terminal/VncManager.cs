using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using Game;
using Game.ScriptableObjects;
using MEC;
using UnityEngine;
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
    public class VncManager : Singleton<VncManager>
    {
        // Sprite and texture updating
        
        /// <summary>
        /// Sprite holding the live image of the remote desktop.
        /// </summary>
        private Sprite _remoteDesktopSprite;
        
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
        [SerializeField][DefaultValue("tfg")] 
        private string sshUserName;
        /// <summary>
        /// Password used in the Ssh authentication process.
        /// </summary>
        [SerializeField][DefaultValue("")] 
        private string sshPassword;
        
        /// <summary>
        /// Port used to establish the Ssh connection with the remote host.
        /// </summary>
        [SerializeField][DefaultValue(22)] 
		private int sshPort;

        /// <summary>
        /// True if the Ssh authentication should be made via user and private key.
        /// False if the Ssh authentication should be made via user and password.
        /// </summary>
        [SerializeField][DefaultValue(false)] 
        private bool sshConnectViaKey;
        
        /// <summary>
        /// Path of the key file used in the Ssh authentication process.
        /// </summary>
        [SerializeField][DefaultValue("")] 
        private string sshKeyPath;
        /// <summary>
        /// Passphrase of the private key used in the Ssh authentication process.
        /// </summary>
        [SerializeField][DefaultValue("")] 
        private string sshKeyPassphrase;

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
        [SerializeField] [DefaultValue(Localhost)]
        private string vncHost;

        /// <summary>
        /// Port used to establish the VNC connection with the remote host.
        /// </summary>
        [SerializeField] [DefaultValue(5900)] 
        private uint vncPort;
        

        /// <summary>
        /// Password used in the VNC authentication process.
        /// </summary>
        [SerializeField] [DefaultValue("")]
        private string vncPassword;

        /// <summary>
        /// Manager in charge of handling the key strokes exchange during the connection to the remote host.
        /// </summary>
        private KeyboardManager _keyboardManager;

        // Bitmap handling
        
        /// <summary>
        /// Bitmap object representation of the currently displayed image of the remote host.
        /// </summary>
        private Bitmap _desktop;
        
        /// <summary>
        /// Byte array storing the the currently displayed image of the remote host.
        /// </summary>
        private byte[] _desktopBytes;
        
        /// <summary>
        /// VNC client managed by the VncSharp API.
        /// </summary>
        private RemoteDesktop _rd;

        /// <summary>
        /// Interval between connection checks.
        /// </summary>
        [SerializeField] [DefaultValue(7)] 
        private int checkConnectionInterval;
        
        /// <summary>
        /// Minimum amount of time passed between connection attempts/checks.
        /// </summary>
        private const int MinConnectionInterval = 8;

        // Threading

        /// <summary>
        /// Thread object representing an independent thread in charge of operating the BitmapManager to avoid
        /// interrupting the game.
        /// </summary>
        private Thread _thread;
        
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
        public Signal serverStatusSignal;
            
//
//        private List<IObserver> _observers;
//        
//        public List<IObserver> Observers
//        {
//            get { return _observers ?? (_observers = new List<IObserver>()); }
//            set { _observers = value; }
//        }
//        
//        public void Attach(IObserver observer)
//        {
//            if (!Observers.Contains(observer))
//                Observers.Add(observer);
//        }
//
//        public void Detach(IObserver observer)
//        {
//            
//            if (Observers.Contains(observer))
//                Observers.Remove(observer);
//        }
//
//        public void Notify()
//        {
//            Debug.Log(Observers.Count);
//            foreach (IObserver observer in Observers)
//                observer.UpdateObserver();
//        }

        

        // Getters / Setters
        
        public RemoteDesktop RemoteDesktop
        {
            get { return _rd; }
        }

        public Sprite RemoteDesktopSprite
        {
            get { return _remoteDesktopSprite; }
            private set { _remoteDesktopSprite = value; }
        }

        public string VncHost
        {
            get { return vncHost; }
            private set { vncHost = value; }
        }
        public uint VncPort
        {
            get { return vncPort; }
            private set { vncPort = value; }
        }
        public string VncPassword
        {
            get { return vncPassword; }
            private set { vncPassword = value; }
        }

        
        /// <summary>
        /// Returns true if there's a SSH connection currently established between the SSH client and remote host,
        /// otherwise returns false.
        /// </summary>
        private bool SshConnected
        {
            get { return SshManager.GetInstance() != null && SshManager.GetInstance().Connected; }
        }
        
        /// <summary>
        /// Returns true if there's a VNC connection currently established between the VNC client and remote host,
        /// otherwise returns false.
        /// </summary>
        private bool VncConnected
        {
            get { return _rd != null && _rd.IsConnected; }
        }

        /// <summary>
        /// Allows external objects observing the VncManager to know the state of the connection and act in consequence.
        /// </summary>
        public bool ConnectionStatus
        {
            get { return SshConnected && VncConnected; }
        }

        /// <summary>
        /// Creates a new VncManager.
        /// </summary>
        /// <remarks>Made private to prevent non-singleton constructor use.</remarks>
        protected VncManager()
        {
        }


        /// <summary>
        /// Function called when the VncManager is inserted into the game.
        /// Sets up all the SSH and VNC clients involved in the connection and begins the image refresh loop.
        /// </summary>
        private void Awake()
        {
            if (checkConnectionInterval < MinConnectionInterval)
                checkConnectionInterval = MinConnectionInterval;

            // Force KeyboardManager initialization.
            _keyboardManager = KeyboardManager.Instance;
            ConnectToHost();
        }

        /// <summary>
        /// Launches an attempt to connect the SSH and VNC clients to the VNC server machine.
        /// If it fails on its first attempt, will launch a periodic process in charge of trying to connect
        /// periodically to the specified host <see cref="AttemptToConnect"/>.
        /// If the connection attempt succeeds, calls the process that will arrange the live desktop visualization.
        /// </summary>
        private void ConnectToHost()
        {
            Debug.Log("Attempting to connect to host...");
            bool connect = true;
            try
            {
                // Attempt to contact the server both via SSH and VNC.
                SetUpSshConnection(sshPort);
                SetUpRemoteDesktop();
            }
            catch (Exception e) // If the server could not be contacted, periodically try again.
            {
                connect = false;

                    Debug.Log("Connection to host not successful");
                    Debug.Log("SSH CONNECTED: " + SshConnected);
                    Debug.Log("VNC CONNECTED: " + VncConnected);
                    Debug.Log(e);
                    if (!_attemptingConnection)
                        StartCoroutine(AttemptToConnect());
            }
            
            if (connect)
            {
                StartCoroutine(ConfirmVncConnection());
                StartCoroutine(CheckOnVncConnection());
                serverStatusSignal.Notify();
            }
        }

        /// <summary>
        /// Coroutine in charge of periodically requesting the game a new connection attempt to the server in case
        /// either the SSH or VNC client has not reached the host.
        /// </summary>
        /// <remarks>The periodic attempts will be run each N number of seconds
        /// <see cref="checkConnectionInterval"/>.</remarks>
        private IEnumerator AttemptToConnect()
        {
            _attemptingConnection = true;
            while (!SshConnected || !VncConnected)
            {
                ConnectToHost();
                yield return new WaitForSeconds(checkConnectionInterval);
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
            }
        }

        /// <summary>
        /// Initializes a new SshManager in charge of logging into the remote server via Ssh and start a tunnel
        /// between client and servers VNC port (590X).
        /// </summary>
        /// <param name="sshPort">Port used by the SshClient to establish the connection.</param>
        /// <remarks>In case there's already a connected SSH client, this function will return automatically.</remarks>
        private void SetUpSshConnection(int sshPort)
        {
            if (SshConnected)
                return;
            
            this.sshPort = sshPort;

            // If no key path has been specified, don't try to connect this way.
            if (sshConnectViaKey && !sshKeyPath.Equals(""))
                _sshManager = SshManager.GetInstance(vncHost, this.sshPort, sshUserName, sshKeyPath, sshKeyPassphrase);

            else
                _sshManager = SshManager.GetInstance(vncHost, this.sshPort, sshUserName, sshPassword);

            _sshManager.ForwardPort(Localhost, vncPort, vncHost, vncPort);
            _sshManager.Connect();
            Debug.Log("SSH connected to host");
        }

        /// <summary>
        /// Initializes a new RemoteDesktop client in charge of logging into the remote server via VNC.
        /// </summary>
        /// <remarks>In case there's already a connected VNC client, this function will return automatically.</remarks>
        private void SetUpRemoteDesktop()
        {
            // If a VNC client is up and running
            if (VncConnected)
                return;

            // If we are tunneling the VNC connection through SSH, connect to "localhost", not remote machine address.
            if (SshConnected && _sshManager.Forward)
                vncHost = Localhost;

            // Not specifying a display, the target tty will depend on the server port of choice
            _rd = new RemoteDesktop(vncHost, (int) vncPort, vncPassword);
            Debug.Log("Remote Desktop host: " + vncHost);

            // Connect via VNC
            _rd.Connect();
            Debug.Log("Remote Desktop connected to: " + vncHost);
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
            StartCoroutine(ChangeToRemoteDesktop());
        }

        /// <summary>
        /// Co-routine in charge of creating a separate execution thread that will change the initial static Sprite
        /// to the remote desktop image periodically without blocking the game main thread.
        /// </summary>
        private IEnumerator ChangeToRemoteDesktop()
        {
            if (!_threadRunning)
            {
                _thread = new Thread(RetrieveBytes);
                _thread.Start();
            }
            yield return new WaitForFixedUpdate();
        }


        /// <summary>
        /// Co-routine in charge of creating a new sprite from the bytes received from the server and un-marking the
        /// _bytesChanged flag for the game to request new sprite updates.
        /// </summary>
        private IEnumerator<float> UpdateVncSprite()
        {
            try
            {
                Texture2D texture = BitmapManager.GetInstance().BitmapToTexture2D(_desktop, _desktopBytes);
                
                _remoteDesktopSprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
            }
            finally // Whether an error occurs or not processing the image, prepare to refresh the screen again.
            {
                _desktop.Dispose();
                _pendingAction = null;
                _bytesChanged = false;
            }

            yield return new float();
        }

        /// <summary>
        /// Method that runs in a parallel thread to the main game execution.
        /// In charge of querying the VNC client for the remote desktop image representation, retrieving the image bytes
        /// and arranging the transformation of that image into an in-game sprite.
        /// </summary>
        private void RetrieveBytes()
        {
            _threadRunning = true;
            while (VncConnected && _threadRunning)
            {
                if (_bytesChanged == false && _pendingAction == null)
                {
                    _desktop = (Bitmap) _rd.Desktop.Clone(); // Operate on a copy not to interrupt the flow of the connection
                    _desktopBytes = BitmapManager.GetInstance().Bitmap2RawBytes(_desktop);
                    _bytesChanged = true;
                    _pendingAction = () => { Timing.RunCoroutine(UpdateVncSprite()); };
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
            if (vKey != 0x00 && Instance.RemoteDesktop != null)
                RemoteDesktop.VncClient.WriteKeyboardEvent(vKey, pressed);
        }


        /// <summary>
        /// Co-routine in charge of checking the VNC connection status periodically and act in consequence.
        /// If the connection status is offline, will stop the in-game screen refreshing
        /// and order the start of a new whole connection process.
        /// </summary>
        /// <remarks>The periodic checks will be run each N number of seconds
        /// <see cref="checkConnectionInterval"/>.</remarks>
        private IEnumerator CheckOnVncConnection()
        {
            while (_rd != null) // Run as long as there's a remote  desktop component active
            {
                Debug.Log("Checking remote desktop socket status: " + VncConnected);
                serverStatusSignal.Notify();
                if (!ConnectionStatus)
                {
                    Debug.Log("Attempting to reconnect...");
                    // Cancel any pending refreshing actions.
                    CancelScreenRefresh();
                    // Notify all observers about the disconnection
                    serverStatusSignal.Notify();
                    // Re-start the connection process
                    ConnectToHost();
                }
                yield return new WaitForSeconds(checkConnectionInterval);
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
                _thread.Join();
            }
        }

        /// <summary>
        /// Checks if the SSH and VNC clients are running and disconnects them form the host. Then sets their values
        /// to null in order to allow new clients to be created via singleton instatiaton.
        /// </summary>
        private void ResetClients()
        {
            if (VncConnected)
            {
                _rd.Dispose(false);
                _rd = null;
            }
            if (SshConnected)
                _sshManager.Dispose();
        }
        
        /// <summary>
        /// Stops all data flow between the game and the server, ensuring all threads and clients related are stopped
        /// or disconnected.
        /// </summary>
        private void StopConnection()
        {
            // Stop the remote desktop refresh process
            CancelScreenRefresh();
            
            // Disconnect VNC and SSH clients to ensure connection is restarted safely.
            ResetClients();
        }

        /// <summary>
        /// Called when the game is quit (e.g: on game closing) to interrupt the connection to the remote host nicely.
        /// </summary>
        private void OnApplicationQuit()
        {
            StopConnection();
        }
    }
}
