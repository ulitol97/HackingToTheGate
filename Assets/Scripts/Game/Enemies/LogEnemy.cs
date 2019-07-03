﻿using UnityEngine;

namespace Game.Enemies
{
    /// <summary>
    /// The LogEnemy class is in charge of handling all the logic related to Log enemies
    /// found in game.
    /// </summary>
    public class LogEnemy : Enemy
    {
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
            target = GameObject.FindWithTag("Player").transform;
        }

        /// <summary>
        /// Function called on each frame the Enemy script is present into the game.
        /// Checks for the target's position.
        /// </summary>
        private void Update()
        {
            CheckDistance();
        }

        /// <summary>
        /// Computes the distance between the enemy and its target (<see cref="target"/>)
        /// and updates the enemy's position in case the target is in the chase radius but not in the
        /// attack radius.
        /// </summary>
        private void CheckDistance()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            // Approach but never more than attack radius.
            if (distanceToTarget <= chaseRadius && distanceToTarget > attackRadius)
            {
                transform.position = Vector3.MoveTowards(transform.position, 
                    target.position, moveSpeed * Time.deltaTime);
            }
        }

    }
}