using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// The FloatValue class acts a counter for a float value that can be managed along
    /// scenes. It may hold a float value that an external script can need.
    /// </summary>
    /// <remarks>This class does not inherit from Mono-Behavior
    /// so it can be left outside the scene so that it survives scene changes.
    /// </remarks>
    [CreateAssetMenu]
    public class FloatValue : ScriptableValue<float>
    {
        public FloatValue(float initialValue)
        {
            this.initialValue = initialValue;
        }

        /// <summary>
        /// Function called when the FloatValue is enabled to be part of a scene.
        /// Subscribes the scene manager to the event in <see cref="OnLevelFinishedLoading"/>.
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        /// <summary>
        /// Function called when the FloatValue is disabled from being part of a scene.
        /// Un-subscribes the scene manager to the event in <see cref="OnLevelFinishedLoading"/>.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        /// <summary>
        /// Event that resets the FloatValue to it's initial value when a new level it's loaded
        /// if the value was below zero. Meant to reset player's health on game over.
        /// </summary>
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (runtimeValue <= 0)
                runtimeValue = initialValue;
        }
    }
}
