using System.Collections;
using UnityEngine;

namespace Game.Enemies
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
        /// Checks for start-collision events affecting the game element attached to this scrip and
        /// triggers the corresponding logic of the object that collided with the attack.
        /// If the enemy entered the collision, it is knocked back proportionally to the distance to the player.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Rigidbody2D otherBody = other.GetComponent<Rigidbody2D>();
                if (otherBody != null)
                {
                    Vector2 distance = otherBody.transform.position - transform.position;
                    // Apply multiplying factor
                    distance = distance.normalized * thrust;
                    
                    otherBody.AddForce(distance, ForceMode2D.Impulse);
                    
                    // Start knockback disable
                    StartCoroutine(EndKnock(otherBody));
                }
            }
        }

        /// <summary>
        /// Co routine in charge of stopping the knockback effect on the knocked back game elements
        /// after a certain time has passed by (<see cref="knockTime"/>).
        /// </summary>
        /// <param name="other">Body of the knocked back element to be managed.</param>
        /// <returns></returns>
        private IEnumerator EndKnock(Rigidbody2D other)
        {
            if (other != null)
            {
                yield return new  WaitForSeconds(knockTime);
                other.velocity = Vector2.zero;
            }
        }
    }
}
