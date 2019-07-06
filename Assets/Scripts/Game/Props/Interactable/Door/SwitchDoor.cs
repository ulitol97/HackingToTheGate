using UnityEngine;

namespace Game.Props.Interactable.Door
{
    public class SwitchDoor : Door
    {
        /// <summary>
        /// The correct order that the switches linked should be pressed in order to open a door.
        /// </summary>
        public int[] actionableSequence;

        /// <summary>
        /// Points out the number of switches pressed in the order specified in
        /// <see cref="actionableSequence"/> by the player in order to compare future switch pressed.
        /// </summary>
        private int _currentProgressInSequence;

        
        protected override void Start()
        {
            base.Start();
            _currentProgressInSequence = 0;
            
            // If already opened before disable door
            if (isOpen.runtimeValue)
                Parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Checks if the opening sequence has been fulfilled and opens the door if so.
        /// </summary>
        public void OnSwitchPressed(int switchId)
        {
            if (isOpen.runtimeValue)
                return;

            CompareWithActionableSequence(switchId);
            
            //Check if all switches have been pressed correctly.
            if (_currentProgressInSequence == actionableSequence.Length)
                Open();
        }

        /// <summary>
        /// Checks for the state of the switches associated to the door and
        /// restarts the sequence if the user pressed switch was incorrect.
        /// </summary>
        private void CompareWithActionableSequence(int id)
        {
            // Is the switch pressed correct?
            if (id == actionableSequence[_currentProgressInSequence])
                _currentProgressInSequence++;
            
            // If incorrect reset the opening sequence
            else
                _currentProgressInSequence = 0;
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
