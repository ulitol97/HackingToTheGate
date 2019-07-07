using UnityEngine;

namespace Game.UI.Menus
{
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
        }
    }
}
