using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities.Enemies
{
    /// <summary>
    /// The patrol Log enemy implements the behavior of log enemies
    /// and extends these enemies with patrolling capabilities.
    /// </summary>
    public class PatrolLogEnemy : LogEnemy
    {
        /// <summary>
        /// Array of locations that compose the enemy patrolling path.
        /// </summary>
        public Transform[] path;

        /// <summary>
        /// Value of the last point of the patrolling path (<see cref="path"/>) the enemy visited.
        /// </summary>
        public int currentPoint;
        
        /// <summary>
        /// Current target location to patrol to.
        /// </summary>
        public Transform currentGoal;

        /// <summary>
        /// Distance that must be reached between the enemy and a patrol point for the enemy
        /// to consider the patrol point as visited.
        /// </summary>
        public FloatValue distanceTolerance;

        protected override void Start()
        {
            base.Start();
            if (path.Length > 0)
                currentGoal = path[0];
        }
        
        protected override void CheckDistance()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            // Approach but never more than attack radius.
            if (distanceToTarget <= chaseRadius && distanceToTarget > attackRadius)
            {
                if ((currentState == EnemyState.Idle || currentState == EnemyState.Walk) 
                    && currentState != EnemyState.Staggered)
                {
                    Vector3 movement = Vector3.MoveTowards(transform.position, 
                        target.position, moveSpeed * Time.deltaTime);

                    ChangeAnim(movement - transform.position);
                    EnemyRigidBody.MovePosition(movement);
                
                    EnemyAnimator.SetBool(AnimatorWakeUp, true);
                }
            }
            // If not chasing the payer, move towards next patrol point.
            else if (distanceToTarget > attackRadius)
            {
                if (Vector3.Distance(transform.position, currentGoal.position)
                    > distanceTolerance.initialValue)
                {
                    Vector3 movement = Vector3.MoveTowards(transform.position, 
                        currentGoal.position, moveSpeed * Time.deltaTime);
                
                    ChangeAnim(movement - transform.position);
                    EnemyRigidBody.MovePosition(movement);
                }
                // Change goal
                else
                {
                    ChangeGoal();
                }
            }
        }


        /// <summary>
        /// Check the target point the enemy is directed to and change it to the next patrolling
        /// point in the patrol path.
        /// </summary>
        private void ChangeGoal()
        {
            if (currentPoint == path.Length - 1)
                currentPoint = 0;
            else
                currentPoint++;
            
            currentGoal = path[currentPoint];
        }
    }
}
