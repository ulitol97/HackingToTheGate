using Remote_Terminal;
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
        public Color disconnectedColor;
        
        /// <summary>
        /// Color in which <see cref="connectionCircle"/> is rendered when the remote
        /// server is online.
        /// </summary>
        public Color connectedColor;

        
        /// <summary>
        /// Image UI element holding the image template of the connection status on UI.
        /// </summary>
        public Image connectionCircle;
        
        private void Start()
        {
            UpdateOnlineStatus();
        }

        /// <summary>
        /// Update the color of the UI regarding on the remote server availability.
        /// </summary>
        public void UpdateOnlineStatus()
        {
            if (VncManager.GetInstance(true) != null)
                connectionCircle.color = VncManager.GetInstance(true).ConnectionStatus ? connectedColor : disconnectedColor;
        }
    }
}
