using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Audio
{
    public class AudioManager : UnitySingleton<AudioManager>
    {

        private AudioSource _musicSource;
        private AudioSource _effectsSource;
        private string _musicPath;
        private string _effectsPath;

        private AudioClip _audioClip;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _musicPath = "file://" + Application.streamingAssetsPath + "/Audio/Music/";
            _effectsPath = "file://" + Application.streamingAssetsPath + "/Audio/Effects/";
        }

        // Start is called before the first frame update
        private void Start()
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.volume = 0.22f;

            _effectsSource = gameObject.AddComponent<AudioSource>();
            _effectsSource.loop = false;
        }
        
        public void PlayMusicClip(string filename)
        {
            StartCoroutine(PlayAudioClip(filename, false));
        }
        
        public void PlayEffectClip(string filename)
        {
            StartCoroutine(PlayAudioClip(filename, true));
        }

        private IEnumerator PlayAudioClip(string filename, bool isEffect)
        {
            var path = isEffect ? string.Format(_effectsPath + "{0}", filename) : 
                string.Format(_musicPath + "{0}", filename);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (!www.isNetworkError)
                {
                    _audioClip = DownloadHandlerAudioClip.GetContent(www);
                    if (!isEffect)
                    {
                        _musicSource.clip = _audioClip;
                        _musicSource.Play();
                    }
            
                    else{
                        _effectsSource.clip = _audioClip;
                        _effectsSource.Play();
                    }
                }
            }
        }

        // Music
        public const string MainTheme = "main_theme.wav";
        public const string BackgroundTheme = "background_dungeon.wav";
        
        // Effects
        public const string Attack = "attack.wav";
        public const string EnemyDead = "enemy_dead.wav";
        public const string PlayerHit = "player_hit.wav";
        public const string PickUpItem = "item_pickup.wav";
        public const string OpenDoor = "open_door.wav";
        public const string ToggleSwitch = "toggle_actionable.wav";
        public const string LowerSpikes = "lower_spikes.wav";
        public const string BreakPot = "pot_break.wav";
        public const string SceneTransition = "transition.wav";
            // Menu and UI
        public const string Pause = "pause.wav";
        public const string ConnectionChange = "change_connection.wav";
        public const string Confirm = "confirm.wav";
        public const string Back = "toggle_actionable.wav";
        public const string Error = "error.wav";
        public const string NewGame = "new_game.wav";
    }
}
