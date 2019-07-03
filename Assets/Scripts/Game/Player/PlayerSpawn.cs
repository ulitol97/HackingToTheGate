using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Player
{
    /// <summary>
    /// The PlayerSpawn class handles game logic that should happen whenever a Player
    /// element is spawned in game, that is, at the beginning of each level.
    /// </summary>
    public class PlayerSpawn : MonoBehaviour
    {

        /// <summary>
        /// Represents the GameObject holding the name of each level.
        /// </summary>
        public GameObject text;
        /// <summary>
        /// Represents the text component rendering the text name of each level.
        /// </summary>
        public Text placeText;

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
            StartCoroutine(DrawPlaceName());
        }
        
        /// <summary>
        /// Activates the game object holding the name of the level. Gets the name of the current level and
        /// displays it for an amount of time (<see cref="textDuration"/>) before removing it.
        /// </summary>
        private IEnumerator DrawPlaceName()
        {
            placeText.gameObject.SetActive(true);
            placeText.text = Globals.Instance.LevelNameTable[SceneManager.GetActiveScene().name];
            Debug.Log(placeText.text);
            yield return new WaitForSeconds(textDuration);
            placeText.gameObject.SetActive(false);
        }
    }
}
