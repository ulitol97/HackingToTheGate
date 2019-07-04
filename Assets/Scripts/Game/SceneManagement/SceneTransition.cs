using System;
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

        public Vector2 playerPosition;

        public Vector2Value playerLocationStorage;
        
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
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
