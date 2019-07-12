using UnityEngine;

namespace Game.Entities.Props.Projectiles
{
    /// <summary>
    /// The RockProjectile class extends the projectile class specifying that it should only react to collisions
    /// with players, as RockProjectile instances will be used by enemies.
    /// </summary>
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
