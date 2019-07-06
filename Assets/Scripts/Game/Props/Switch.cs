using Game.Props.Interactable.Door;
using UnityEngine;

namespace Game.Props
{
    public class Switch : MonoBehaviour
    {
        /// <summary>
        /// Boolean flag marking if the switch is pressed or not.
        /// </summary>
        public bool active;

        /// <summary>
        /// Identifier of the switch, used for multi-switch doors that may require a specific order.
        /// </summary>
        public int id;
        
        /// <summary>
        /// Sprite attached to the switch when active.
        /// </summary>
        public Sprite activeSprite;
        
        /// <summary>
        /// Sprite attached to the switch when not active.
        /// </summary>
        public Sprite inactiveSprite;

        /// <summary>
        /// Reference to the sprite renderer in charge of rendering the sprite in game.
        /// </summary>
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// The Door element the switch is linked to.
        /// A switch may send an "open" signal to the door when acivated.
        /// </summary>
        public SwitchDoor linkedDoor;
        
        /// <summary>
        /// Function called when the Switch is inserted into the game.
        /// Sets up references for changing the sprite of the switch later on.
        /// </summary>
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = inactiveSprite;
        }

        /// <summary>
        /// Mark the switch active, change its appearance and send an open signal to the door.
        /// </summary>
        private void ActivateSwitch()
        {
            active = true;
            _spriteRenderer.sprite = activeSprite;
            if (linkedDoor != null)
                linkedDoor.OnSwitchPressed(id);
        }
        
        /// <summary>
        /// Mark the switch inactive and change its appearance.
        /// </summary>
        private void DeactivateSwitch()
        {
            active = false;
            _spriteRenderer.sprite = inactiveSprite;
        }

        /// <summary>
        /// Press the switch if entered by a player.
        /// </summary>
        /// <param name="other">Object colliding with the switch</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
                ActivateSwitch();
        }
        
        /// <summary>
        /// Un-press the switch if a player exits the trigger zone.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
                DeactivateSwitch();
        }
    }
}
