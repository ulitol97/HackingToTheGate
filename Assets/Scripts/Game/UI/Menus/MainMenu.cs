using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Menus
{
    /// <summary>
    /// The MainMenu class handles the behaviour of the main menu of the game.
    /// </summary>
    public class MainMenu : GameMenu
    {
        /// <summary>
        /// When inserted into the game, the main menu will check if a connection
        /// to the remote host is running and will stop it to restart it when a new game session begins.
        /// Then it will load the user connection settings.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            
            // Set to not destroy between scenes.
            GameObject remoteClient = GameObject.FindWithTag("RemoteServer");
            if (remoteClient != null)
                Destroy(remoteClient);
            
            LoadSettings();
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
    }
}
