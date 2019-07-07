using UnityEngine;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// The BooleanValue class acts a holder of a boolean value that can be managed across
    /// scenes. It may hold a boolean value value that an external script can need.
    /// </summary>
    /// <remarks>This class does not inherit from Mono-Behavior
    /// so it can be left outside the scene so that it survives scene changes.
    /// </remarks>
    [CreateAssetMenu]
    public class BooleanValue : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Runtime value held by the BooleanValue instance.
        /// </summary>
        public bool runtimeValue;

        /// <summary>
        /// Default value held by the BooleanValue instance.
        /// Since the value of these objects does not reset each game session,
        /// it is needed to store a default value for them to reset each session.
        /// </summary>
        public bool defaultValue;

        public void OnBeforeSerialize()
        {}

        /// <summary>
        /// Reset the runtime value held by the BooleanValue to its initial value when the
        /// game session begins.
        /// </summary>
        public void OnAfterDeserialize()
        {
            runtimeValue = defaultValue;
            
        }

        /// <summary>
        /// Setup in order not to unload this instance from memory when entering a scene
        /// that does not use it.
        /// </summary>
        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}