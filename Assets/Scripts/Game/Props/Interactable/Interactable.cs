using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Props.Interactable
{
    /// <summary>
    /// The interactable class acts as a parent to all objects that may trigger an
    /// interaction with the player.
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        
        /// <summary>
        /// Boolean flag marked true if the player is in range to interact with the sign.
        /// </summary>
        protected bool PlayerInRange;
        
        /// <summary>
        /// Signal in charge of observing if an object is currently interactable by the player.
        /// </summary>
        public Signal context;
        
        /// <summary>
        /// Checks for collision events between the sign and other objects with collision capabilities.
        /// If the object colliding with the object is the player, marks that the player is in range and signals the
        /// player that an interactive object is close.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        protected  virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
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
        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                context.Notify();
                PlayerInRange = false;
            }
        }
    }
}
