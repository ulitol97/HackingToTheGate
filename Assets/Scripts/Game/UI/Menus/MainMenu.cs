using Game.Audio;
using Game.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI.Menus
{
    /// <summary>
    /// The MainMenu class handles the behaviour of the main menu of the game.
    /// </summary>
    public class MainMenu : GameMenu
    {
        
        /// <summary>
        /// Boolean flag to control if the player can start or not a new game. The game configuration must be
        /// valid for that to happen
        /// </summary>
        private bool _canStartGame;

        private GameObject _loadingConfigText;
        private GameObject _configOkText;
        private GameObject _configErrorText;

        public Text currentSettingText;
        
        
        /// <summary>
        /// When inserted into the game, the main menu will check if a connection
        /// to the remote host is running and will stop it to restart it when a new game session begins.
        /// Then it will load the user connection settings if they are not valid.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            Cursor.visible = true;
            SetUpConfigFileText();
            
            // Set to not destroy between scenes.
            GameObject remoteClient = GameObject.FindWithTag("RemoteServer");
            if (remoteClient != null)
                Destroy(remoteClient);
            
            if (!ConfigurationManager.IsValid)
                LoadSettings();
            UpdateUi();
            
            AudioManager.Instance.PlayMusicClip(AudioManager.MainTheme);
        }

        private void SetUpConfigFileText()
        {
            _loadingConfigText = GameObject.FindWithTag("ConfigFileLoading");
            _loadingConfigText.SetActive(false);
            
            _configOkText = GameObject.FindWithTag("ConfigFileOK");
            _configOkText.SetActive(false);
            
            _configErrorText = GameObject.FindWithTag("ConfigFileErrors");
            _configErrorText.SetActive(false);
        }


        /// <summary>
        /// Inits a game session on the first level of the game.
        /// </summary>
        public void NewGame()
        {
            if (!ConfigurationManager.IsValid)
            {
                AudioManager.Instance.PlayEffectClip(AudioManager.Error);
                return;
            }
            Cursor.visible = false;
            AudioManager.Instance.PlayEffectClip(AudioManager.NewGame);
            AudioManager.Instance.PlayMusicClip(AudioManager.BackgroundTheme);
            
            SceneManager.LoadScene("Level1");
        }

        /// <summary>
        /// Loads the current settings for connection with the remote server, substituting the existing ones.
        /// </summary>
        private void LoadSettings()
        {
            _configErrorText.SetActive(false);
            _configOkText.SetActive(false);
            _loadingConfigText.SetActive(true);
            AudioManager.Instance.PlayEffectClip(AudioManager.Confirm);
            ConfigurationManager.Instance.LoadGameConfiguration();
            
            _loadingConfigText.SetActive(false);
            UpdateUi();
        }

        private void UpdateUi()
        {
            if (ConfigurationManager.IsValid)
                _configOkText.SetActive(true);
            else
            {
                _configErrorText.SetActive(true);
                AudioManager.Instance.PlayEffectClip(AudioManager.Error);
            }

            currentSettingText.text = ConfigurationManager.Instance.ToString();
        }
    }
}
