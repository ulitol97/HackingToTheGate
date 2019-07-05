using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Props.PickUpItems
{
    public abstract class FloorObject : MonoBehaviour, IPickUpItem
    {
    
        /// <summary>
        /// Item received by picking up the object, if any.
        /// </summary>
        public Item content;

        /// <summary>
        /// Reference to the player's inventory. Floor obtained items may enter the player's inventory.
        /// </summary>
        public Inventory playerInventory;

        /// <summary>
        /// Sprite rendender in charge of rendering the object in-game.
        /// </summary>
        protected SpriteRenderer SpriteRenderer;
    
        /// <summary>
        /// Boolean flag marked true if the player is in range to interact with the sign.
        /// </summary>
        protected bool PlayerInRange;
    
        /// <summary>
        /// Represents whether the object is already picked up or not. Persists in a Boolean value.
        /// </summary>
        public BooleanValue isPickedUp;

    
        /// <summary>
        /// Dialog UI element where the item description is shown (<see cref="content"/>).
        /// </summary>
        public GameObject dialogBox;

        /// <summary>
        /// Text UI element to be shown after the item has been picked up.
        /// </summary>
        public Text dialogText;
    
        /// <summary>
        /// Called when the item os inserted into the game, sets the reference needed for later logic operations.
        /// </summary>
        protected virtual void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            
            // If it was picked up before, disable.
            if (isPickedUp.runtimeValue)
                Destroy(gameObject);
            
        }
    
        /// <summary>
        /// Signal in charge of observing if the player picked up the object form the floor.
        /// </summary>
        public Signal receiveItem;

        /// <summary>
        /// Function called each frame to check for player distance to the item in order
        /// to trigger the logic executed when the item is picked up.
        /// </summary>
        protected virtual void Update()
        {
            if (PlayerInRange)
            {
                if (!isPickedUp.runtimeValue)
                    PickUp();
            }
        }

        /// <summary>
        /// Handles the logic operations made when the object is picked up.
        /// </summary>
        public virtual void PickUp()
        {
            receiveItem.Notify();
            isPickedUp.runtimeValue = true;
            SpriteRenderer.enabled = false;
        }
    
        /// <summary>
        /// Checks for collision events between the sign and other objects with collision capabilities.
        /// If the object colliding with the object is the player, marks that the player is in range and signals the
        /// player that an interactive object is close.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = true;
            }

        }

        /// <summary>
        /// Checks for end of collision events between the object and other with objects collision capabilities.
        /// If the object that stopped colliding with the interactable is the player, marks that the player is out
        /// of range and signals the player that no interactive object is close.
        /// </summary>
        /// <param name="other">Collider object that finished contact.</param>
        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = false;
            }
        }
    }
}
