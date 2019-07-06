using UnityEngine;

namespace Game.Props.Interactable.Door
{
    public class SwitchDoor : Door
    {
//        /// <summary>
//        /// List of switches the door is linked to.
//        /// </summary>
//        public List<Switch> switches;

        /// <summary>
        /// The correct order that the switches linked should be pressed in order to open a door.
        /// </summary>
        public int[] openingSequence;

        /// <summary>
        /// Points out the number of switches pressed in the order specified in
        /// <see cref="openingSequence"/> by the player in order to compare future switch pressed.
        /// </summary>
        private int _currentProgressInSequence;

        
        protected override void Start()
        {
            base.Start();
            _currentProgressInSequence = 0;
            
            // If already opened before destroy door
            if (isOpen.runtimeValue)
                Destroy(Parent.gameObject);
        }

        /// <summary>
        /// Checks for the state of the switches associated to the door and
        /// opens it if the order of pressing them was correct.
        /// </summary>
        public void OnSwitchPressed(int switchId)
        {
            if (isOpen.runtimeValue)
                return;

            // Is the switch pressed correct?
            if (switchId == openingSequence[_currentProgressInSequence])
                _currentProgressInSequence++;
            
            // If incorrect reset the opening sequence
            else
                _currentProgressInSequence = 0;
            
            //Check if all switches have been pressed correctly.
            if (_currentProgressInSequence == openingSequence.Length)
                Open();
        }

        // Override for no context clues.
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
