using System;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Props.Interactable
{

    /// <summary>
    /// Enum representing the different types of doors that may be found in game.
    /// </summary>
    public enum DoorType
    {
        Key, // Opened with key
        Enemy, // Opened by finishing enemies
        Button // Opened by switch
    }

    /// <summary>
    /// The Door class inherits from Interactable.
    /// Represents an in-game door the player can interact with.
    /// </summary>
    public class Door : Interactable
    {

        /// <summary>
        /// Represents the type of that the instance of the Door class is.
        /// </summary>
        [Header("Door variables")] // Header to help in Unity editor.
        public DoorType doorType;

        /// <summary>
        /// Boolean representing whether the door is open or not.
        /// </summary>
        public bool isOpen;

        /// <summary>
        /// Reference to the player's inventory, used to check for player's keys.
        /// </summary>
        public Inventory playerInventory;

        /// <summary>
        /// SpriteRenderer in charge of rendering the door in-game appearance.
        /// </summary>
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Collider managing the door's collision with other objects.
        /// </summary>
        private BoxCollider2D _physicsCollider;
        
       
        /// <summary>
        /// Function called when the Door is inserted into the game.
        /// Sets up references for later use in the Door logic.
        /// </summary>
        private void Start()
        {
            // Look for the components in the Door game object parent to modify from here.
            var parent = transform.parent;
            _spriteRenderer = parent.GetComponent<SpriteRenderer>();
            _physicsCollider = parent.GetComponent<BoxCollider2D>();
            
        }

        /// <summary>
        /// Function called on each frame the Door script is present into the game.
        /// Checks the player input and conditions that trigger interaction with the door.
        /// </summary>
        private void Update()
        {
            if (PlayerInRange && doorType == DoorType.Key)
            {
                if (Input.GetButtonDown("Interact") && playerInventory.keysRuntimeValue > 0)
                {
                    // If the player has a key, open.
                    playerInventory.SubtractKey();
                    Open();
                }
            }
        }

        /// <summary>
        /// Handles the logic operation when a door is opened:
        /// - Disable the sprite
        /// - Disable collisions
        /// - Update door status
        /// </summary>
        private void Open()
        {
            // Turn off sprite renderer and physics on door.
            _spriteRenderer.enabled = false;
            _physicsCollider.enabled = false;
            isOpen = true;

        }

        public void Close()
        {
            
        }
    }
}
