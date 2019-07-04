using UnityEngine;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// The Item class acts a persistence object holding an item representation that can be managed across
    /// scenes.
    /// </summary>
    /// <remarks>This class does not inherit from Mono-Behavior
    /// so it can be left outside the scene so that it survives scene changes.
    /// </remarks>
    [CreateAssetMenu]
    public class Item : ScriptableObject
    {

        /// <summary>
        /// Sprite holding the item image that the game should render.
        /// </summary>
        public Sprite itemSprite;

        /// <summary>
        /// Text description of the item.
        /// </summary>
        public string itemDescription;

        /// <summary>
        /// Boolean flag marking if the item is a key or not.
        /// </summary>
        public bool isKey;
    }
}
