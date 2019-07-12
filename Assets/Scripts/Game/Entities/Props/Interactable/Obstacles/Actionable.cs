using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities.Props.Interactable.Obstacles
{
    /// <summary>
    /// The abstract class Actionable encapsulates the common and default behaviour to all
    /// game objects that will be interacted by means of a toggle mechanism (switches and levers).
    /// </summary>
    public abstract class Actionable : Door.Door, IActionable
    {
        /// <summary>
        /// The correct order that the toggles linked to the actionable should be
        /// activated in order to activate the mechanism.
        /// </summary>
        public int[] actionableSequence;
        
        /// <summary>
        /// Points out the number of toggles pressed in the order specified in
        /// <see cref="actionableSequence"/> by the player in order to compare future toggle activations.
        /// </summary>
        public int currentProgressInSequence;

        /// <summary>
        /// Retrieves a reference to the obstacle parent to deactivate it if obstacle destroyed.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            currentProgressInSequence = 0;
            
            // If already opened before disable obstacle
            if (isOpen == null)
                isOpen = ScriptableObject.CreateInstance<BooleanValue>();
            else if (isOpen.runtimeValue)
                Parent.gameObject.SetActive(false);
        }
        
        public virtual void OnActionReceived(int actionId)
        {
            if (isOpen.runtimeValue)
                return;

            CompareWithActionableSequence(actionId);
            
            //Check if all switches have been pressed correctly.
            if (currentProgressInSequence == actionableSequence.Length)
                Activate();
        }
        
        /// <summary>
        /// Checks for the state of the switches associated to the door and
        /// restarts the sequence if the user pressed switch was incorrect.
        /// </summary>
        private void CompareWithActionableSequence(int id)
        {
            // Is the switch pressed correct?
            if (id == actionableSequence[currentProgressInSequence])
                currentProgressInSequence++;
            
            // If incorrect reset the opening sequence
            else
                currentProgressInSequence = 0;
        }

        /// <summary>
        /// Logic to be executed when the obstacle is activated correctly.
        /// </summary>
        protected virtual void Activate()
        {}
    }
}
