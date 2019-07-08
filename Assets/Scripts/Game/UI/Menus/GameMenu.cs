using Game.Audio;
using UnityEngine;

namespace Game.UI.Menus
{
    /// <summary>
    /// THe GameMenu abstract class implements some base functionality used by all the menus in the game.
    /// </summary>
    public abstract class GameMenu : MonoBehaviour, IMenu
    {
        /// <summary>
        /// Boolean flag to control if the user manual should be rendered or not.
        /// </summary>
        protected bool IsShowingHelp;
        
        
        /// <summary>
        /// Game object containing the main UI elements that form the game menu and will be
        /// shown or hidden to the player.
        /// </summary>
        public GameObject mainPanel;
        
        /// <summary>
        /// Game object containing the UI elements that form the user manual that players can access
        /// form the menu.
        /// </summary>
        public GameObject manualPanel;
        
        /// <summary>
        /// Run when the menu is inserted into the game. Ensure that the user manual is not being shown initially.
        /// </summary>
        protected virtual void Start()
        {
            IsShowingHelp = false;
        }
        
        /// <summary>
        /// Each frame check for player input to load or unload the user manual screen.
        /// </summary>
        protected virtual void Update()
        {
            if (Input.GetButtonDown("Attack") && IsShowingHelp)
                ToggleHelp();
        }
        
        public virtual void QuitMenu()
        {
            AudioManager.Instance.PlayEffectClip(AudioManager.Confirm);
            Application.Quit();
        }

        /// <summary>
        /// Shows or hides the user manual form the UI, hiding or showing the menu that opened to the manual in the
        /// first place.
        /// </summary>
        public void ToggleHelp()
        {
            IsShowingHelp = !IsShowingHelp;
            manualPanel.SetActive(IsShowingHelp);
            mainPanel.SetActive(!IsShowingHelp);

            AudioManager.Instance.PlayEffectClip(IsShowingHelp ? AudioManager.Confirm : AudioManager.Back);
        }
    }
}
