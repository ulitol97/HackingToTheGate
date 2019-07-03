﻿using Game.Entities.Enemies;
using Game.Entities.Player;
using Game.Items;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities
{
    /// <summary>
    /// The Knockback class use's Unity's RigidBody components to apply knockback forces
    /// between interacting objects.
    /// </summary>
    public class Knockback : MonoBehaviour
    {

        /// <summary>
        /// A multiplying factor to the knockback force applied.
        /// </summary>
        public float thrust;

        /// <summary>
        /// Amount of time the knock back is meant to last.
        /// </summary>
        public float knockTime;

        /// <summary>
        /// Damage inflicted by the knockback
        /// </summary>
        public FloatValue damage;
        
        /// <summary>
        /// Checks for start-collision events affecting the game element attached to this scrip and
        /// triggers the corresponding logic of the object that collided with the attack.
        /// If the enemy entered the collision, it is knocked back proportionally to the distance to the player.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {

            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
            {
                Rigidbody2D hitBody = other.GetComponent<Rigidbody2D>();
                if (hitBody != null)
                {
                    // Knockback calculation
                    Vector2 distance = hitBody.transform.position - transform.position;
                    // Apply multiplying factor
                    distance = distance.normalized * thrust;
                    hitBody.AddForce(distance, ForceMode2D.Impulse);
                    
                    // If hit enemy hurt box
                    if (other.gameObject.CompareTag("Enemy"))
                    {
                        // Update state and end knockback logic.
                        other.GetComponent<Enemy>().ChangeState(Enemy.EnemyState.Staggered);
                        other.GetComponent<Enemy>().Knock(hitBody, knockTime, damage.initialValue);
                    }
                    else
                    {
                        if (other.GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.Staggered)
                        {
                            // Update state and end knockback logic.
                            other.GetComponent<PlayerMovement>().ChangeState(PlayerMovement.PlayerState.Staggered);
                            other.GetComponent<PlayerMovement>().Knock(knockTime);
                        }
                    }
                }
            }
            
            // Only player can break stuff
            else if (other.CompareTag("Breakable") && this.gameObject.CompareTag("Player"))
            {
                if (other.GetComponent<Pot>() != null)
                    other.GetComponent<Pot>().Smash();
            }
        }
    }
}