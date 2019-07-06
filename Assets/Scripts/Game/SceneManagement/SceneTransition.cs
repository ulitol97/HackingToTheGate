using System.Collections;
using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneManagement
{
    /// <summary>
    /// The SceneTransition class handles the change from one game level to another.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {

        /// <summary>
        /// Name of the scene to be loaded by the scene transition.
        /// </summary>
        public string targetScene;

        /// <summary>
        /// Vector holding the player's desired position the moment of the transition.
        /// </summary>
        public Vector2 playerPosition;

        /// <summary>
        /// Vector value in charge of storing the player's position (<see cref="playerPosition"/>) when the player leaves a scene
        /// in order to spawn the player in the correct scene entrance if he/she returns.
        /// </summary>
        public Vector2Value playerLocationStorage;
        
        
        /// <summary>
        /// A colored panel that will be instantiated from a prefab to create a scene enter animation.
        /// </summary>
        public GameObject fadeInPanel;
        /// <summary>
        /// A colored panel that will be instantiated from a prefab to create a scene enter animation.
        /// </summary>
        public GameObject fadeOutPanel;

        /// <summary>
        /// Waiting time (seconds) before ending the scene transition animation.
        /// </summary>
        public FloatValue fadeWait;
        
        private void Awake()
        {
            // Hide cursor from game
            Cursor.visible = false;
            
            if (fadeInPanel != null)
            {
                GameObject panelIn = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity);
                Destroy(panelIn, 1);
            }

        }

        /// <summary>
        /// Checks for collision events with the Player in order to arrange a transition to a new scene.
        /// </summary>
        /// <param name="other">The object colliding with the SceneTransition
        /// in-game representation</param>
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                playerLocationStorage.initialValue = playerPosition;
                StartCoroutine(FadeScene());
            }
        }

        private IEnumerator FadeScene()
        {
            if (fadeOutPanel != null)
            {
                Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(fadeWait.runtimeValue);
            
            // Load async to show fade animation and hide loading time.
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene);

            // Wait for scene to load
            while (!asyncOperation.isDone)
                yield return null;
        }
    }
}
