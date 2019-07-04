using UnityEngine;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// The Vector2Value class acts a holder of a vector 2 value that can be managed across
    /// scenes. It may hold a vector2 value that an external script can need.
    /// </summary>
    /// <remarks>This class does not inherit from Mono-Behavior
    /// so it can be left outside the scene so that it survives scene changes.
    /// </remarks>
    [CreateAssetMenu]
    public class Vector2Value : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Initial value held by the Vector2Value instance.
        /// </summary>
        public Vector2 initialValue;

        /// <summary>
        /// Default value held by the Vector2Value instance.
        /// Since the value of these objects does not reset each game session,
        /// it is needed to store a default value for them to reset each session.
        /// </summary>
        public Vector2 defaultValue;
        
        
        public void OnBeforeSerialize()
        {}

        /// <summary>
        /// Reset the runtime value held by the Vector2Value to its initial value when the
        /// game session begins.
        /// </summary>
        public void OnAfterDeserialize()
        {
            initialValue = defaultValue;
        }
    }
}
