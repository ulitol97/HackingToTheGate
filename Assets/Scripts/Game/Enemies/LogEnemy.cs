using UnityEngine;

namespace Game.Enemies
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
        /// Function called when the Enemy script is loaded into the game.
        /// Sets up the enemy's current state, target and references to the Unity components modified on runtime.
        /// </summary>
        private void Start()
        {
            currentState = EnemyState.Idle;
            target = GameObject.FindWithTag("Player").transform;
            _enemyRigidBody = GetComponent<Rigidbody2D>();
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
        private void CheckDistance()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            // Approach but never more than attack radius.
            if (distanceToTarget <= chaseRadius && distanceToTarget > attackRadius
                && (currentState == EnemyState.Idle || currentState == EnemyState.Walk) 
                && currentState != EnemyState.Stagger)
            {
                Vector3 movement = Vector3.MoveTowards(transform.position, 
                    target.position, moveSpeed * Time.deltaTime);
                
                _enemyRigidBody.MovePosition(movement);
                ChangeState(EnemyState.Walk);
            }
        }

    }
}
