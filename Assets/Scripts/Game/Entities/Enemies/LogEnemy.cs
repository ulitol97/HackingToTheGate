using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities.Enemies
{
    /// <summary>
    /// The LogEnemy class is in charge of handling all the logic related to Log enemies
    /// found in game.
    /// </summary>
    public class LogEnemy : Enemy
    {
        /// <summary>
        /// RigidBody containing the enemy's body for physics handling.
        /// </summary>
        protected Rigidbody2D EnemyRigidBody;
        
        /// <summary>
        /// Transform object containing the coordinates the enemy should follow.
        /// </summary>
        public Transform target;
        
        /// <summary>
        /// Transform object containing the coordinates the enemy shall patrol.
        /// </summary>
        public Transform homePosition;

        /// <summary>
        /// Radius of the terrain area the enemy shall patrol.
        /// </summary>
        public float chaseRadius;

        /// <summary>
        /// Minimum distance needed for the enemy to attack. A player won't approach the player
        /// further than its attack radius.
        /// </summary>
        public float attackRadius;
        
        /// <summary>
        /// Distance that must be reached between the enemy and a location point for the enemy
        /// to consider the point as visited.
        /// </summary>
        public FloatValue distanceTolerance;

        /// <summary>
        /// Unity Animator component in charge of animating the enemy sprite to simulate actions.
        /// </summary>
        /// <remarks>The animator attributes are cached below for quicker access</remarks>
        protected Animator EnemyAnimator;
        protected static readonly int AnimatorWakeUp = Animator.StringToHash("wakeUp");
        private static readonly int AnimatorMoveX = Animator.StringToHash("moveX");
        private static readonly int AnimatorMoveY = Animator.StringToHash("moveY");

        /// <summary>
        /// Function called when the Enemy script is loaded into the game.
        /// Sets up the enemy's current state, target and references to the Unity components modified on runtime.
        /// </summary>
        protected virtual void Start()
        {
            currentState = EnemyState.Idle;
            EnemyRigidBody = GetComponent<Rigidbody2D>();
            EnemyAnimator = GetComponent<Animator>();
            
            target = GameObject.FindWithTag("Player").transform;
            
            // Start awake.
            EnemyAnimator.SetBool(AnimatorWakeUp, true);
        }

        /// <summary>
        /// Function called on each frame the Enemy script is present into the game.
        /// Checks for the target's position.
        /// </summary>
        private void FixedUpdate()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                case EnemyState.Walk:
                    CheckDistance();
                    break;
            }
        }

        /// <summary>
        /// Computes the distance between the enemy and its target (<see cref="target"/>)
        /// and updates the enemy's position in case the target is in the chase radius but not in the
        /// attack radius and the enemy is not staggered.
        /// </summary>
        /// <remarks>It also handles the animations in case the enemy wakes up or
        /// goes to sleep.</remarks>
        protected virtual void CheckDistance()
        {
            var position = transform.position;
            float distanceToTarget = Vector3.Distance(position, target.position);
            float distanceToHome = Vector3.Distance(position, homePosition.position);
            
            // Approach but never more than attack radius.
            if (distanceToTarget <= chaseRadius && distanceToTarget > attackRadius)
            {
                if ((currentState == EnemyState.Idle || currentState == EnemyState.Walk) 
                    && currentState != EnemyState.Staggered)
                {
                    Vector3 movement = Vector3.MoveTowards(position, 
                        target.position, moveSpeed * Time.deltaTime);
                    
                    ChangeAnim(movement - position);
                    EnemyRigidBody.MovePosition(movement);
                
                    // Walk and wake up animation if needed.
                    ChangeState(EnemyState.Walk);
                    EnemyAnimator.SetBool(AnimatorWakeUp, true);
                }
            }
            // If not chasing the payer, check if home.
            else if (distanceToTarget > attackRadius)
            {
                // Move home
                if (distanceToHome > distanceTolerance.initialValue)
                {
                    Vector3 movement = Vector3.MoveTowards(position,
                        homePosition.position, moveSpeed * Time.deltaTime);

                    ChangeAnim(movement - position);
                    EnemyRigidBody.MovePosition(movement);
                }
                // Sleep
                else
                {
                    EnemyAnimator.SetBool(AnimatorWakeUp, false);
                }
            }
        }
        /// <summary>
        /// Function managing the animation the enemy should make regarding it's moving direction.
        /// </summary>
        /// <param name="direction"></param>
        protected void ChangeAnim(Vector2 direction)
        {
            direction = direction.normalized;
            EnemyAnimator.SetFloat(AnimatorMoveX, direction.x);
            EnemyAnimator.SetFloat(AnimatorMoveY, direction.y);
        }

    }
}
