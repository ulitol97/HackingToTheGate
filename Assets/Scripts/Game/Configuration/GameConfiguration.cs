using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Configuration
{
    /// <summary>
    /// The GameConfiguration class encapsulates all the configuration the game need to run. Instances of this
    /// class can be loaded from a Json file.
    /// </summary>
    [System.Serializable]
    public class GameConfiguration
    {
        public VncConnectionInfo vncConnectionInfo;
        public SshConnectionInfo sshConnectionInfo;
        public int secondsBetweenConnectionAttempts;
        
        [System.Serializable]
        public class VncConnectionInfo
        {
            public string targetHost;
            public string vncServerPassword;
            public int port;
        }

        [System.Serializable]
        public class PublicKeyAuth
        {
            public string path;
            public string passPhrase;
            public bool preferSshPublicKey;
        }

        [System.Serializable]
        public class SshConnectionInfo
        {
            public string username;
            public string password;
            public int port;
            public PublicKeyAuth publicKeyAuth;
        }

        public static GameConfiguration CreateFromJson(string jsonFilePath)
        {
            return JsonUtility.FromJson<GameConfiguration>(jsonFilePath);
        }


        public override string ToString()
        {
            string ret = "";
            ret += "Target host: " + vncConnectionInfo.targetHost + "\n";
            ret += "VNC port: " + vncConnectionInfo.port + "\n";
            ret += "SSH login username: " + sshConnectionInfo.username + "\n";
            ret += "SSH login password: " + Regex.Replace(sshConnectionInfo.password, ".", "*") + "\n";
            ret += "SSH port: " + sshConnectionInfo.port + "\n";
            ret += "Prefer public key authentication: " + sshConnectionInfo.publicKeyAuth.preferSshPublicKey +"\n";
            return ret;
        }
    }
}
