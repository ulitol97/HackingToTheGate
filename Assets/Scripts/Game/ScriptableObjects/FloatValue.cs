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
    public class FloatValue : ScriptableObject
    {
        /// <summary>
        /// Initial value held by the FLoatValue instance.
        /// </summary>
        public float initialValue;
    }
}
