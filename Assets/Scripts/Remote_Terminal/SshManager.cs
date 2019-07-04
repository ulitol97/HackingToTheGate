using Renci.SshNet;

namespace Remote_Terminal
{
    /// <summary>
    /// The SshManager class is in charge of establishing an SHH connection to a remote target host and
    /// handle the authentication process if needed.
    /// It allows to perform port forwarding in order to tunnel the VNC connection made by the game through the SSH
    /// protocol, cyphering the data shared between and server in the process.
    /// As a singleton, a maximum of one instance of this class will be managed by the game.
    /// </summary>
    public class SshManager
    {
        /// <summary>
        /// Holds the current and only simultaneous instance of the SshManager in use by the game.
        /// </summary>
        private static SshManager _instance;
    
        /// <summary>
        /// Ssh client in charge of connecting with the remote host.
        /// </summary>
        private SshClient _client;

        /// <summary>
        /// Object representation of the port forwarding used by the SshClient to tunnel the VNC connection to the host.
        /// </summary>
        private ForwardedPortLocal _forwardedPort;

        /// <summary>
        /// Represents whether the current manager instance is using port forwarding or not.
        /// </summary>
        public bool Forward { get; private set; }

        public bool Connected
        {
            get { return _client.IsConnected; }
        }
        
        public static SshManager GetInstance()
        {
            return _instance;
        }
        
        /// <summary>
        /// Creates, configures and returns a new singleton instance of the SshManager if none has been created,
        /// or returns the currently existing one.
        /// </summary>
        /// <param name="ip">Address of the target host</param>
        /// <param name="port">Port in which the host is listening for connections</param>
        /// <param name="username">Username used for authentication</param>
        /// <param name="password">Password used for authentication</param>
        /// <returns>A new SshManager instance or the currently existing singleton instance.</returns>
        public static SshManager GetInstance(string ip, int port, string username, string password)
        {
            return _instance ?? (_instance = new SshManager(ip, port, username, password));
        }
        
        /// <summary>
        /// Creates, configures and returns a new singleton instance of the SshManager if none has been created,
        /// or returns the currently existing one.
        /// </summary>
        /// <param name="ip">Address of the target host</param>
        /// <param name="port">Port in which the host is listening for connections</param>
        /// <param name="username">Username used for authentication</param>
        /// <param name="keyFileName">Route of the file containing the key used for authentication</param>
        /// <param name="passPhrase">Passphrase (if any) used to secure the key used for authentication</param>
        /// <returns>A new SshManager instance or the currently existing singleton instance.</returns>
        public static SshManager GetInstance(string ip, int port, string username, string keyFileName, 
            string passPhrase)
        {
            return _instance ?? (_instance = new SshManager(ip, port, username, keyFileName, passPhrase));
        }

        /// <summary>
        /// Creates a new SshClient holding a user and a password for authentication.
        /// </summary>
        /// <remarks>Made private to prevent non-singleton constructor use.</remarks>
        /// <param name="ip">Address of the target host</param>
        /// <param name="port">Port in which the host is listening for connections</param>
        /// <param name="username">Username used for authentication</param>
        /// <param name="password">Password used for authentication</param>
        private SshManager(string ip, int port, string username, string password)
        {
            // Initialize SSH connection info for the client
            var conn = new ConnectionInfo(ip, port, username, new PasswordAuthenticationMethod(username, password));

            _client = new SshClient (conn);
        }
        
        
        /// <summary>
        /// Creates a new SshClient holding a user and a private key for authentication.
        /// </summary>
        /// <remarks>Made private to prevent non-singleton constructor use.</remarks>
        /// <param name="ip">Address of the target host</param>
        /// <param name="port">Port in which the host is listening for connections</param>
        /// <param name="username">Username used for authentication</param>
        /// <param name="keyFileName">Route of the file containing the key used for authentication</param>
        /// <param name="passPhrase">Passphrase (if any) used to secure the key used for authentication</param>
        private SshManager(string ip, int port, string username, string keyFileName, string passPhrase)
        {
            // Initialize SSH connection info for the client
            var conn = new ConnectionInfo(ip, port, username, new PrivateKeyAuthenticationMethod(username, 
                new PrivateKeyFile(keyFileName, passPhrase)));

            _client = new SshClient (conn);
        }

        /// <summary>
        /// Adds the ability to port tunnel/forward to the SshClient.
        /// </summary>
        /// <param name="boundHost">Address of the ssh sender whose port is to be forwarded</param>
        /// <param name="boundPort">Port number from which the bound-host will send the data</param>
        /// <param name="host">Address of the ssh receiver of the forwarded data</param>
        /// <param name="remotePort">Port number from which the host will receive the data</param>
        /// <returns></returns>
        public void ForwardPort(string boundHost, uint boundPort, string host, uint remotePort)
        {
            _forwardedPort = new ForwardedPortLocal (boundHost, boundPort, host, remotePort);
            Forward = true;
        }

        /// <summary>
        /// Starts a connection using the SshClient already created and forwarding specific ports if specified
        /// in the SshClient object.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">Thrown when the connection could not
        /// be established</exception>
        public void Connect()
        {
            if (!Forward)
            {
                try
                {
                    _client.Connect();
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }
            else
            {
                try
                {
                    //Add forwarded port
                    _client.Connect();
                    _client.AddForwardedPort(_forwardedPort);
                    _forwardedPort.Start();
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Closes the SshClient and stops the port forwarding if it's taking place. Then, sets the singleton instance
        /// to null to allow a new SshClient to be initialized.
        /// </summary>
        public void Dispose ()
        {
            if (_forwardedPort != null)
                _forwardedPort.Dispose();
            
            if (_client != null)
                _client.Dispose();

            _instance = null;
        }
    }
}