using System.Collections;
using UnityEngine;

namespace Game.Entities.Props.Interactable.Door
{
    /// <summary>
    /// The EnemyDoor class inherits from Door.
    /// Represents an in-game door the player can open if a certain amount of enemies have been
    /// eliminated.
    /// </summary>
    public class EnemyDoor : Door
    {
        /// <summary>
        /// Amount of enemies that need to be killed for the door to open.
        /// </summary>
        public int nEnemies;

        /// <summary>
        /// Counter of the amount of enemies eliminated. Will be used to trigger the door open or not.S
        /// </summary>
        private int _enemiesEliminated;

        /// <summary>
        /// Logic to be executed when an enemy elimination event observed by the door takes place.
        /// If the door is open nothing will happen, else will compare the amount of enemies left to eliminate
        /// or open the door if they were already eliminated.
        /// </summary>
        public void OnEnemyEliminated()
        {
            if (isOpen.runtimeValue)
                return;
            
            _enemiesEliminated++;
            if (_enemiesEliminated >= nEnemies)
                StartCoroutine(OnAllEnemiesEliminated());
        }

        /// <summary>
        /// Waits for the frame to finish for all observers to check on eliminated enemies,
        /// then opens the door.
        /// </summary>
        private IEnumerator OnAllEnemiesEliminated()
        {
            yield return new WaitForFixedUpdate();
            Open();
        }

        /// <summary>
        /// Enemy doors must not check for user input each update.
        /// </summary>
        protected override void Update()
        {}
        
        protected override void Start()
        {
            base.Start();
            _enemiesEliminated = 0;
            
            // If already opened before disable door
            if (isOpen.runtimeValue)
                Parent.gameObject.SetActive(false);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = true;
            }

        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = false;
            }
        }
    }
}
