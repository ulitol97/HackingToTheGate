using Game.Audio;
using Game.ScriptableObjects;
using Game.UnityObserver;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Entities.Props.Interactable
{
    /// <summary>
    /// The Chest class inherits from AbstractInteractable.
    /// Represents an in-game treasure chest.
    /// </summary>
    public class Chest : AbstractInteractable
    {

        /// <summary>
        /// Item contained inside the chest.
        /// </summary>
        public Item content;

        /// <summary>
        /// Reference to the player's inventory. Chest obtained items will enter the player's inventory.
        /// </summary>
        public Inventory playerInventory;
        
        
        /// <summary>
        /// Stores if the chest was opened for persistence.
        /// </summary>
        public BooleanValue persistenceOpen;

        /// <summary>
        /// Signal in charge of observing if the player received an object form the chest.
        /// </summary>
        public SignalSubject receiveItem;

        /// <summary>
        /// Dialog UI element where the item description is shown (<see cref="content"/>).
        /// </summary>
        public GameObject dialogBox;

        /// <summary>
        /// Text to be shown after the chest has been open.
        /// </summary>
        public Text dialogText;

        /// <summary>
        /// Reference to the chest's animator to handle open animation.
        /// </summary>
        private Animator _chestAnimator;
        private static readonly int AnimatorOpen = Animator.StringToHash("open");
        
        /// <summary>
        /// Function called when the chest is inserted into the game.
        /// Sets up references for later use.
        /// </summary>
        private void Start()
        {
            _chestAnimator = GetComponent<Animator>();
            _chestAnimator.SetBool(AnimatorOpen, persistenceOpen.runtimeValue);
        }

        
        /// <summary>
        /// Function called on each frame the Chest the script is attached to is present into the game.
        /// Checks for user controller input and if the player is close enough to interact with the chest.
        /// </summary>
        private void Update()
        {
            if (Input.GetButtonDown("Interact") && PlayerInRange)
            {
                if (!persistenceOpen.runtimeValue)
                    OpenChest();
                else
                    OnOpenedChest();
            }
        }

        /// <summary>
        /// Manages the logic when the chest is opened by the player.
        /// Activates the dialog and re-arranges the player's inventory, for that, it raises a signal
        /// to the player and set the chest to already open (<see cref="persistenceOpen"/>).
        /// </summary>
        private void OpenChest()
        {
            // Set up dialogue
            dialogBox.SetActive(true);
            dialogText.text = content.itemDescription;
            AudioManager.Instance.PlayEffectClip(AudioManager.PickUpItem);

            // Set up player inventory and notify player for animation.
            playerInventory.AddItem(content);
            playerInventory.currentItem = content;
            receiveItem.Notify();
            
            // Disable context clue on top of player.
            context.Notify();

            persistenceOpen.runtimeValue = true;
            _chestAnimator.SetBool(AnimatorOpen, true);

        }

        /// <summary>
        /// Manages the logic to be executed when a player interacts with a chest already open.
        /// Deactivates the dialog and empties the chest. Signal the player not to animate holding
        /// an object.
        /// </summary>
        private void OnOpenedChest()
        {
                dialogBox.SetActive(false);
                receiveItem.Notify();
                PlayerInRange = false;
                AudioManager.Instance.PlayEffectClip(AudioManager.Confirm);
        }
        
        /// <summary>
        /// Checks for collision events between the sign and other objects with collision capabilities.
        /// If the object colliding with the object is the player, marks that the player is in range and signals the
        /// player that an interactive object is close.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger && !persistenceOpen.runtimeValue)
            {
                PlayerInRange = true;
                context.Notify();
            }

        }

        /// <summary>
        /// Checks for end of collision events between the object and other with objects collision capabilities.
        /// If the object that stopped colliding with the interactable is the player, marks that the player is out
        /// of range and signals the player that no interactive object is close.
        /// </summary>
        /// <param name="other">Collider object that finished contact.</param>
        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger && !persistenceOpen.runtimeValue)
            {
                context.Notify();
                PlayerInRange = false;
            }
        }
    }
}