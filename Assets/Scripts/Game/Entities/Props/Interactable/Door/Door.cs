using Game.Audio;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities.Props.Interactable.Door
{
    /// <summary>
    /// The Door class inherits from Interactable.
    /// Serves as a template to any an in-game door the player can interact with.
    /// </summary>
    public abstract class Door : Interactable, IDoor
    {
        /// <summary>
        /// Boolean representing whether the door is open or not.
        /// A BooleanValue helps keep this value across scenes.
        /// </summary>
        public BooleanValue isOpen;

        /// <summary>
        /// GameObject referencing the parent of the script, which contains the door rendered in game.
        /// </summary>
        protected Transform Parent;
        
       
        /// <summary>
        /// Function called when the Door is inserted into the game.
        /// Sets up references for later use in the Door logic.
        /// </summary>
        protected virtual void Start()
        {
            // Look for the components in the Door game object parent to modify from here.
            Parent = transform.parent;
        }

        /// <summary>
        /// Function called on each frame the Door script is present into the game.
        /// Checks the player input and conditions that trigger interaction with the door.
        /// </summary>
        protected virtual void Update()
        {
            if (PlayerInRange && Input.GetButtonDown("Interact"))
            {
                Open();
            }
        }

        /// <summary>
        /// Handles the logic operation when a door is opened. Stores it was opened and destroys it.
        /// </summary>
        public virtual void Open()
        {
            // Store it was opened and destroy.
            isOpen.runtimeValue = true;
            Parent.gameObject.SetActive(false);
            AudioManager.Instance.PlayEffectClip(AudioManager.OpenDoor);
        }
    }
}
