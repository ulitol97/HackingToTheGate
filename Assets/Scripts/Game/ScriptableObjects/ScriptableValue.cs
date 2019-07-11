using UnityEngine;
using UnityEngine.Serialization;

namespace Game.ScriptableObjects
{
    public abstract class ScriptableValue <T> : ScriptableObject, ISerializationCallbackReceiver

    {
        /// <summary>
        /// Initial value held by the ScriptableValue instance that should be reset on launch.
        /// </summary>
        [FormerlySerializedAs("defaultValue")] public T initialValue;
        
        /// <summary>
        /// Runtime value held by the ScriptableValue instance.
        /// Since the value of these objects is persistent each game session,
        /// it is needed to store a default value for them to reset each session.
        /// </summary>
        public T runtimeValue;

        public void OnBeforeSerialize()
        {}

        /// <summary>
        /// Reset the runtime value held by the ScriptableValue to its initial value when the
        /// game session begins.
        /// </summary>
        public void OnAfterDeserialize()
        {
            runtimeValue = initialValue;
        }
    }
}