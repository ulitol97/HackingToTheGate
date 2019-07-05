using System.Collections;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities.Enemies
{
    public class Enemy : MonoBehaviour
    {

        public float health;
        public string enemyName;

        public FloatValue maxHealth;
        public int baseAttack;
        public float moveSpeed;
        
        /// <summary>
        /// Enum structure holding the enemy possible states.
        /// </summary>
        public enum EnemyState
        {
            Idle,
            Walk,
            Staggered
        }

        /// <summary>
        /// Current state of the enemy.
        /// </summary>
        public EnemyState currentState;

        private void Awake()
        {
            health = maxHealth.initialValue;
        }

        /// <summary>
       /// Reduces the health of an enemy and eliminates it if it goes below 0.
       /// </summary>
       /// <param name="damage">Amount of health to be reduced.</param>
       private void TakeDamage(float damage)
       {
           health -= damage;
           if (health <= 0)
           {
               gameObject.SetActive(false);
           }
       }

        /// <summary>
        /// Arranges the end of knockback logic.
        /// </summary>
        /// <param name="rigidBody"></param>
        /// <param name="knockTime"></param>
        /// <param name="damage">Damage inflicted by the knock back.</param>
        public void Knock(Rigidbody2D rigidBody, float knockTime, float damage)
        {
            StartCoroutine(EndKnock(rigidBody, knockTime));
            TakeDamage(damage);
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
