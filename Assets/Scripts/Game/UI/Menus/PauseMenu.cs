using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Menus
{
    /// <summary>
    /// The PauseMenu class handles the behaviour of the pause menu of the game.
    /// </summary>
    public class PauseMenu : GameMenu
    {
        /// <summary>
        /// Boolean flag for internal control of the state of the pause menu.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Signal raised when the game is paused or un-paused.
        /// </summary>
        public Signal pauseSignal;
        
        /// <summary>
        /// Function running on first frame to ensure the game is not paused when launched.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _isPaused = false;
        }

        /// <summary>
        /// Each frame check for player input to load or unload the pause screen.
        /// </summary>
        protected override void Update()
        {
            if (Input.GetButtonDown("Pause"))
                TogglePause();
            else if (Input.GetButtonDown("Attack") && IsShowingHelp)
                ToggleHelp();
        }

        /// <summary>
        /// Logic for activation of the pause menu. Shows or hides the menu on screen
        /// along with the player cursor. Freezes the game timescale while on pause.
        /// </summary>
        public void TogglePause()
        {
            _isPaused = !_isPaused;
            mainPanel.SetActive(_isPaused);
            manualPanel.SetActive(false);
            Cursor.visible = !Cursor.visible;
            
            pauseSignal.Notify();

            if (_isPaused)
                Time.timeScale = 0;
            else
            {
                Time.timeScale = 1f;
                IsShowingHelp = false;
            }
        }

        /// <summary>
        /// Exits the game session and loads main menu.
        /// </summary>
        public override void QuitMenu()
        {
            SceneManager.LoadScene("TitleMenu");
            Time.timeScale = 1f;
        }
    }
}
