using Game.Audio;
using UnityEngine;

namespace Game.Entities.Props.Interactable.ToggleItems
{
    /// <summary>
    /// The class Lever inherits from Toggle, and adds the capability of staying in their state
    /// permanently when not being interacted with.
    /// </summary>
    public class Lever : Toggle
    {
        private bool _active;

        protected override void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();

            SpriteRenderer.sprite = inactiveSprite;
        }
        /// <summary>
        /// For each frame, check if the player is in range to activate the lever
        /// and if it interacts with it, toggle the linked obstacle logic.
        /// </summary>
        private void Update()
        {
            if (PlayerInRange && Input.GetButtonDown("Interact"))
            {
                ActivateSwitch();
            }
        }

        protected override void ActivateSwitch()
        {
            _active = !_active;
            SpriteRenderer.sprite = _active ? activeSprite : inactiveSprite;
                
            foreach (var obstacle in linkedObstacles)
            {
                if (obstacle != null)
                    obstacle.OnActionReceived(id);
            }
            AudioManager.Instance.PlayEffectClip(AudioManager.ToggleSwitch);
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = true;
                if (context != null)
                    context.Notify();
            }
        }
        
        /// <summary>
        /// Deactivate the toggle mechanism if a player exits the trigger zone.
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = false;
                if (context != null)
                    context.Notify();
            }
        }
    }
}
