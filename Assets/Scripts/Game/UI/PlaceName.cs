using System.Collections;
using Game.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// The PlaceName class handles, at the beginning of each level, the appearance of a descriptive text containing
    /// the name of the level.
    /// </summary>
    public class PlaceName : MonoBehaviour
    {
        /// <summary>
        /// Represents the text component rendering the text name of each level.
        /// </summary>
        private Text _placeText;

        /// <summary>
        /// Amount of seconds the name of the level should be shown.
        /// </summary>
        /// <remarks>A default value of "3.5" is enough.</remarks>
        [System.ComponentModel.DefaultValue(3.5f)]
        public float textDuration;
        
        /// <summary>
        /// Function called when the Player game element id loaded into the game.
        /// Arranges the logical operations that must take place when the player spawns.
        /// </summary>
        private void Start()
        {
            _placeText = GetComponent<Text>();
            StartCoroutine(DrawPlaceName());
        }
        
        /// <summary>
        /// Activates the game object holding the name of the level. Gets the name of the current level and
        /// displays it for an amount of time (<see cref="textDuration"/>) before removing it.
        /// </summary>
        private IEnumerator DrawPlaceName()
        {
            _placeText.gameObject.SetActive(true);
            _placeText.text = GameConfigurationManager.Instance.LevelNameTable[int.Parse(GetLevelNumber())];

            yield return new WaitForSeconds(textDuration);
            _placeText.gameObject.SetActive(false);
        }
        
        private static string GetLevelNumber()
        {
            string text = SceneManager.GetActiveScene().name;
            return text.StartsWith("Level") ? text.Substring("Level".Length) : text;
        }
    }
}
