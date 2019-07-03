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
            Stagger
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

        public void ChangeState(EnemyState state)
        {
            currentState = state;
        }
    }
}
