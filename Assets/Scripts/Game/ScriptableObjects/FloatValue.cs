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
    public class FloatValue : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Initial value held by the FLoatValue instance.
        /// </summary>
        public float initialValue;

        /// <summary>
        /// Runtime value held by the FLoatValue instance.
        /// Since the value of these objects does not reset each game session,
        /// this value can change during the game session while using the initial
        /// value to reset it in the beginning of the game.
        /// </summary>
        [HideInInspector] // Not appear in inspector
        public float runtimeValue;

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

        
        
        public void OnBeforeSerialize()
        {}

        /// <summary>
        /// Reset the runtime value held by the FloatValue to its initial value when the
        /// game session begins.
        /// </summary>
        public void OnAfterDeserialize()
        {
            runtimeValue = initialValue;
        }
    }
}
