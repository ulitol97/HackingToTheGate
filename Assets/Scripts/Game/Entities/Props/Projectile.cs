using System.Collections;
using UnityEngine;

namespace Game.Entities.Props
{
    public abstract class Projectile : MonoBehaviour
    {
        /// <summary>
        /// Multiplying factor to the speed at which the projectile moves.
        /// </summary>
        public int speed;
        
        /// <summary>
        /// Direction in which the projectile moves.
        /// </summary>
        public Vector2 direction;
        
        /// <summary>
        /// Seconds that the projectile will remain loaded on screen.
        /// </summary>
        public float lifetime;

        /// <summary>
        /// Reference to the projectile's in-game rigid-body
        /// </summary>
        public Rigidbody2D myRigidBody;
        
        /// <summary>
        /// Function called when the Projectile script is loaded into the game.
        /// Sets up the references to the Unity components modified on runtime for the projectile to work.
        /// </summary>
        protected virtual void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            StartCoroutine(DestroyProjectile());
        }
        
        /// <summary>
        /// Sets up the projectile initial velocity in the game world.
        /// </summary>
        /// <param name="initialVelocity"></param>
        public virtual void LaunchProjectile(Vector2 initialVelocity)
        {
            myRigidBody.velocity = initialVelocity * speed;
        }

        /// <summary>
        /// Coroutine in charge of waiting for the project's lifetime seconds before destroying the game object.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DestroyProjectile()
        {
            yield return new WaitForSeconds(lifetime);
            Destroy(gameObject);
        }

        /// <summary>
        /// Checks for start-collision events affecting the projectile and
        /// triggers the corresponding logic of the projectile if needed.
        /// </summary>
        /// <param name="other">Collider object that initiated contact with the projectile.</param>
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            Destroy(gameObject);
        }
    }
}
