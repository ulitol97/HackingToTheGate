using Game.Audio;
using Game.Props.Interactable;
using UnityEngine;

namespace Game.Props.Obstacles
{
    public class Obstacle : ActionableObstacle
    {
        /// <summary>
        /// In game representation of the obstacle when it is activated.
        /// </summary>
        public Sprite obstacleActive;
        
        /// <summary>
        /// In game representation of the obstacle when it is not activate.
        /// </summary>
        public Sprite obstacleInactive;
        /// <summary>
        /// Reference to the sprite renderer of the game object for appearance manipulation.
        /// </summary>
        private SpriteRenderer _spriteRenderer;
        
        /// <summary>
        /// Reference to the collisions collider of the game object for appearance manipulation.
        /// </summary>
        private BoxCollider2D _boxCollider;

        /// <summary>
        /// Init the obstacle with the corresponding sprite regarding it's state.
        /// </summary>
        protected override void Start()
        {
            Parent = transform.parent;
            currentProgressInSequence = 0;
            _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
            _boxCollider = Parent.GetComponent<BoxCollider2D>();
            
            UpdateObstacleState();
            
        }
        
        protected override void Activate()
        {
            isOpen.runtimeValue = !isOpen.runtimeValue;
            UpdateObstacleState();
            
        }

        /// <summary>
        /// Change the obstacle sprite representation and activate or deactivate collisions
        /// with it depending on if the obstacle is opened or not.
        /// </summary>
        private void UpdateObstacleState()
        {
            _spriteRenderer.sprite = isOpen.runtimeValue ? obstacleInactive : obstacleActive;
            _boxCollider.enabled = !isOpen.runtimeValue;
            AudioManager.Instance.PlayEffectClip(AudioManager.LowerSpikes);
        }
    }
}
