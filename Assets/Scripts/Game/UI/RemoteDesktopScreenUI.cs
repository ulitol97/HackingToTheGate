using Remote_Terminal;
using UnityEngine;
using UnityEngine.UI;
using UnityObserver;

namespace Game.UI
{
    /// <summary>
    /// The RemoteDesktopScreenUI watches for changes in the VncManager.
    /// While the latter is active, updates the in-game object it is attached to with the remote desktop image,
    /// while the VncManager is not active, waits for new updates.
    /// As an observer object, it has a status <see cref="_statusOnline"/> which is notified by
    /// the observed VncManager if changed.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class RemoteDesktopScreenUI : MonoBehaviour, IObserver
    {
        /// <summary>
        /// Default image that will be displayed when there is no remote desktop image available.
        /// </summary>
        public Sprite defaultSprite; // Sprite displayed by default
        
        /// <summary>
        /// Unity managed component that handles the sprites shown in game.
        /// </summary>
        private Image _imageRenderer;
        
        /// <summary>
        /// Represents the state of the RemoteDesktopScreenUI and whether it should request Image updates
        /// to the VncManager or not.
        /// </summary>
        private bool _statusOnline;
        
        /// <summary>
        /// Represents the state of the RemoteDesktopScreenUI in-game. If toggled, it should display it's
        /// assigned sprite.
        /// </summary>
        private bool _statusOnScreen;


        /// <summary>
        /// Function called when the RemoteDesktopUI is inserted into the game.
        /// Arranges the registration onto the observed subjects and sets up the ImageRenderer component in charge
        /// of displaying the desktop image.
        /// </summary>
        private void Awake()
        {
            // Initialize remote desktop sprite with sprite by default.
            SetUpImageRenderer();
        }

        /// <summary>
        /// Function called on each frame the RemoteDesktopUI is present into the game.
        /// Polls the state of the object to determine if it should show remote desktop images or not.
        /// </summary>
        private void Update()
        {
            if (!_statusOnline || !_statusOnScreen)
                return;

            _imageRenderer.sprite = VncManager.GetInstance(true).remoteDesktopSprite;
        }
        
        
        /// <summary>
        /// Initializes the ImageRenderer in a disabled state
        /// before the remote desktop can be displayed.
        /// </summary>
        private void SetUpImageRenderer()
        {
            _imageRenderer = GetComponent<Image>();
            _imageRenderer.sprite = defaultSprite;
            _imageRenderer.enabled = false;
        }
        
        /// <summary>
        /// Updates the observing screen status with the current connection status of the VncManager observed.
        /// and sets the variables needed for the screen to behave properly in case of disconnection.
        /// </summary>
        public void UpdateObserver()
        {
            // Invert if it has to be shown on screen.
            _statusOnScreen = !_statusOnScreen;
            
            // Update status regarding the client status
            if (VncManager.GetInstance(true) != null)
                _statusOnline = VncManager.GetInstance(true).ConnectionStatus;

            // Put placeholder image in case of disconnection.
            if (!_statusOnline)
            {
                _imageRenderer.sprite = defaultSprite;

            }
            else if (VncManager.GetInstance(true) != null)
                _imageRenderer.sprite = VncManager.GetInstance(true).remoteDesktopSprite;
            

            // Deactivate game element if terminal mode is not toggled
            _imageRenderer.enabled = _statusOnScreen;
        }
    }
}