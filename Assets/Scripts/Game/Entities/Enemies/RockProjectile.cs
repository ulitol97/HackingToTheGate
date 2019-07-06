using Game.Props;
using UnityEngine;

namespace Game.Entities.Enemies
{
    public class RockProjectile : Projectile
    {
        /// <summary>
        /// Checks for start-collision events affecting the projectile and
        /// triggers the corresponding logic of the projectile if needed. If the projectile collided with a player,
        /// it will be destroyed.
        /// </summary>
        /// <param name="other">Collider object that initiated contact with the projectile.</param>
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.isTrigger)
                base.OnTriggerEnter2D(other);
        }
    }
}
