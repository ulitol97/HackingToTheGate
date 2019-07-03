using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class RemoteServerManager : MonoBehaviour
    {
        /// <summary>
        /// Color in which <see cref="connectionCircle"/> is rendered when the remote
        /// server is offline.
        /// </summary>
        public Color disconnected;
        
        /// <summary>
        /// Color in which <see cref="connectionCircle"/> is rendered when the remote
        /// server is online.
        /// </summary>
        public Color connected;

        
        /// <summary>
        /// Image UI element holding the image template of the connection status on UI.
        /// </summary>
        public Image connectionCircle;
    
    
        /// <summary>
        /// Function called when the script is loaded, sets the variables values
        /// used in future logic operations.
        /// </summary>
        private void Start()
        {
            connectionCircle.color = connected;
        }
    
        /// <summary>
        /// Check player's health and compute how many heart should be rendered.
        /// </summary>
        /// <remarks>It is divided by two since each UI hart counts as 2 life points.</remarks>
        public void UpdateOnlineStatus()
        {
            // Get VNC Manager connection status and set the color depending on it!!
            connectionCircle.color = connected;
        }

    }
}
