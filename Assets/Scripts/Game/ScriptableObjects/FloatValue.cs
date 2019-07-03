using System;
using UnityEngine;

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
