using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// The MenuScreen class handles the behaviour of the game menu of the game.
    /// </summary>
    public class MenuScreen : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            LoadSettings();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Inits a game session on the first level available.
        /// </summary>
        public void NewGame()
        {
            Cursor.visible = false;
            SceneManager.LoadScene("Level1");
        }

        /// <summary>
        /// Reloads the current settings for connection with the remote server.
        /// </summary>
        public void LoadSettings()
        {
            
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        public void QuitToDesktop()
        {
            Application.Quit();
        }
    }
}
