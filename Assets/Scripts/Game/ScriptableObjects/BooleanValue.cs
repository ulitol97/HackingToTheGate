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
    public class BooleanValue : ScriptableValue<bool>
    {
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