using System.Collections;
using Game.Entities.Props;
using UnityEngine;

namespace Game.Entities.Enemies
{
    /// <summary>
    /// The turret Log enemy implements the behavior of log enemies
    /// and extends these enemies to act like a stationary turret when detecting a player.
    /// </summary>
    public class TurretEnemy : Enemy
    {
        /// <summary>
        /// Represents the delay between turret shots.
        /// </summary>
        public float fireCadence;

        /// <summary>
        /// Represents whether if the turret can shoot or not. Used for interval between shots.
        /// </summary>
        private bool _canShoot;
        
        /// <summary>
        /// Reference to the game object to be spawned when the turret enemy launches a projectile.
        /// </summary>
        public GameObject projectile;
        
        /// <summary>
        /// Initiates the log but also sets its attack radius, which measures when the enemy is close enough to
        /// the player, to 0, since it won't be used and turrets do not walk towards the player.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            attackRadius = 0;
            _canShoot = true;
        }
        
        /// <summary>
        /// Checks the current distance from the enemy to the the target. Fire's a projectile <see cref="projectile"/>
        /// if close enough, else makes the enemy go to sleep. 
        /// </summary>
        protected override void CheckDistance()
        {
            var position = transform.position;
            float distanceToTarget = Vector3.Distance(position, target.position);
            
            // Wake up and fire when player is close enough.
            if (distanceToTarget <= chaseRadius)
            {
                if ((currentState == EnemyState.Idle || currentState == EnemyState.Walk) 
                    && currentState != EnemyState.Staggered && _canShoot)
                {
                    ChangeState(EnemyState.Walk);
                    EnemyAnimator.SetBool(AnimatorWakeUp, true);
                    Shoot(position);
                }
            }
            // Sleep if player too far
            else
            {
                EnemyAnimator.SetBool(AnimatorWakeUp, false);
            }
        }

        /// <summary>
        /// Represents the turret's act of firing a projectile. Spawns the projectile and initiates the
        /// reload process <see cref="ReloadShot"/>.
        /// </summary>
        /// <param name="targetPosition">Target location to fire the projectile.</param>
        private void Shoot(Vector3 targetPosition)
        {
            _canShoot = false;
            
            // Spawn projectile regarding target position.
            Vector3 projectileLaunch = target.transform.position - targetPosition;
            GameObject projectileShot = Instantiate(projectile, targetPosition, Quaternion.identity);
            projectileShot.GetComponent<Projectile>().LaunchProjectile(projectileLaunch);

            StartCoroutine(ReloadShot());
        }

        /// <summary>
        /// Coroutine in charge of waiting N seconds (<see cref="fireCadence"/>) before allowing the
        /// turret to shoot again.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReloadShot()
        {
            yield return new WaitForSeconds(fireCadence);
            _canShoot = true;
        }
    }
}
