using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Props.Interactable.Door
{
    /// <summary>
    /// The KeyDoor class inherits from Door.
    /// Represents an in-game door the player can open if he/she has a key available in the inventory.
    /// </summary>
    public class KeyDoor : Door
    {
        /// <summary>
        /// Reference to the player's inventory, used to check for player's keys.
        /// </summary>
        public Inventory playerInventory;
        
        /// <summary>
        /// Key doors will not reappear if they were opened once before, so they are deleted across scenes if needed.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            // If already opened before disable door
            if (isOpen.runtimeValue)
                Parent.gameObject.SetActive(false);
        }
        
        protected override void Update()
        {
            if (PlayerInRange && Input.GetButtonDown("Interact") && playerInventory.keysRuntimeValue > 0)
            {
                playerInventory.SubtractKey();
                    Open();
            }
        }
    }
}