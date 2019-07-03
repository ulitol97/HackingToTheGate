using System.Collections;
using UnityEngine;

namespace Game.Enemies
{
    public class Enemy : MonoBehaviour
    {

        public int health;
        public string enemyName;

        public int baseAttack;
        public float moveSpeed;
        
        /// <summary>
        /// Enum structure holding the enemy possible states.
        /// </summary>
        public enum EnemyState
        {
            Idle,
            Walk,
            Attack,
            Staggered
        }

        /// <summary>
        /// Current state of the enemy.
        /// </summary>
        public EnemyState currentState;
        
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Arranges the end of knockback logic.
        /// </summary>
        /// <param name="rigidBody"></param>
        /// <param name="knockTime"></param>
        public void Knock(Rigidbody2D rigidBody, float knockTime)
        {
            StartCoroutine(EndKnock(rigidBody, knockTime));
        }

        /// <summary>
        /// Co routine in charge of stopping the knockback effect on the knocked back game enemies
        /// after a certain time has passed by.
        /// </summary>
        /// <param name="rigidBody">Body of the knocked back element to be managed.</param>
        /// <param name="knockTime">Time before stopping the knockback force.</param>
        /// <returns></returns>
        private IEnumerator EndKnock(Rigidbody2D rigidBody, float knockTime)
        {
            if (rigidBody != null)
            {
                yield return new  WaitForSeconds(knockTime);
                rigidBody.velocity = Vector2.zero;
            
                // Reset enemy state.
                currentState = EnemyState.Idle;
            }
        }

        public void ChangeState(EnemyState state)
        {
            currentState = state;
        }
    }
}
