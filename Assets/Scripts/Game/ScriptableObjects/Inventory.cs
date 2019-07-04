using System.Collections.Generic;
using UnityEngine;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// The Inventory class acts a persistence object holding the player's inventory that can be managed across
    /// scenes.
    /// </summary>
    /// <remarks>This class does not inherit from Mono-Behavior
    /// so it can be left outside the scene so that it survives scene changes.
    /// </remarks>
    [CreateAssetMenu]
    public class Inventory : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// List holding hte items present in the inventory.
        /// </summary>
        public List<Item> items = new List<Item>();
        
        /// <summary>
        /// Current active item in the inventory.
        /// </summary>
        public Item currentItem;

        /// <summary>
        /// Number of keys present in the players inventory in the beginning of a game session.
        /// </summary>
        public int keysInitialValue;
        
        /// <summary>
        /// Runtime value representing the amount of keys present in the inventory
        /// during the game session.
        /// </summary>
        [HideInInspector] // Not appear in inspector
        public int keysRuntimeValue;

        /// <summary>
        /// Represents whether the player has acquired a sword or not. A boolean value is used as a global variable.
        /// </summary>
        public BooleanValue hasSword;
        
        /// <summary>
        /// Adds an item to the inventory list of items (<see cref="items"/>) if it is
        /// not present already and updates the number of keys.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            if (item.isKey)
                keysRuntimeValue++;
            else if (item.isSword)
                hasSword.runtimeValue = true;
            else if (!items.Contains(item))
                items.Add(item);
        }

        public void OnBeforeSerialize()
        {}

        /// <summary>
        /// Reset the runtime value hof keys held by the player when the
        /// game session begins.
        /// </summary>
        public void OnAfterDeserialize()
        {
            keysRuntimeValue = keysInitialValue;
        }
    }
}
