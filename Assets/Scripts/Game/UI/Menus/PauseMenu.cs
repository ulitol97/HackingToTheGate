using Game.Audio;
using Game.Configuration;
using Game.UnityObserver;
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
        public SignalSubject pauseSignal;
        
        /// <summary>
        /// Function running on first frame to ensure the game is not marked as paused when loaded.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _isPaused = false;
        }
        
        protected override void Update()
        {
            if (Input.GetButtonDown("Pause"))
                TogglePause();
            
            else if (Input.GetButtonDown("Attack") && _isPaused)
            {
                if(IsShowingHelp)
                    ToggleHelp();
                else
                    TogglePause();
                AudioManager.Instance.PlayEffectClip(AudioManager.Back);
            }
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
            AudioManager.Instance.PlayEffectClip(AudioManager.Pause);
        }

        /// <summary>
        /// Exits the game session and loads the main menu.
        /// </summary>
        public override void QuitMenu()
        {
            SceneManager.LoadScene(ConfigurationManager.MenuScene);
            Time.timeScale = 1f;
        }
    }
}
