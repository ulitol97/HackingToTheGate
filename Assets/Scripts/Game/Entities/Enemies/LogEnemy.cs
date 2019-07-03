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
        private Rigidbody2D _enemyRigidBody;
        
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
        /// Unity Animator component in charge of animating the enemy sprite to simulate actions.
        /// </summary>
        /// <remarks>The animator attributes are cached below for quicker access</remarks>
        private Animator _enemyAnimator;
        private static readonly int AnimatorWakeUp = Animator.StringToHash("wakeUp");
        private static readonly int AnimatorMoveX = Animator.StringToHash("moveX");
        private static readonly int AnimatorMoveY = Animator.StringToHash("moveY");

        /// <summary>
        /// Function called when the Enemy script is loaded into the game.
        /// Sets up the enemy's current state, target and references to the Unity components modified on runtime.
        /// </summary>
        private void Start()
        {
            currentState = EnemyState.Idle;
            _enemyRigidBody = GetComponent<Rigidbody2D>();
            _enemyAnimator = GetComponent<Animator>();
            
            target = GameObject.FindWithTag("Player").transform;
            
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
        private void CheckDistance()
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
                    _enemyRigidBody.MovePosition(movement);
                
                    // Walk and wake up animation if needed.
                    ChangeState(EnemyState.Walk);
                    _enemyAnimator.SetBool(AnimatorWakeUp, true);
                }
            }
            else if (distanceToTarget > attackRadius)
            {
                _enemyAnimator.SetBool(AnimatorWakeUp, false);
            }
        }
        /// <summary>
        /// Function managing the animation the enemy should make regarding it's moving direction.
        /// </summary>
        /// <param name="direction"></param>
        private void ChangeAnim(Vector2 direction)
        {
            direction = direction.normalized;
            _enemyAnimator.SetFloat(AnimatorMoveX, direction.x);
            _enemyAnimator.SetFloat(AnimatorMoveY, direction.y);
        }

    }
}
