using Game.Entities.Props.Interactable.Obstacles;

namespace Game.Entities.Props.Interactable.Door
{
    /// <summary>
    /// The SwitchDoor class inherits from Actionable, which also encapsulates the door
    /// functionality.
    /// Represents an in-game door the player can open if a number of switches or levers are pressed
    /// in the correct sequence.
    /// </summary>
    public class SwitchDoor : Obstacle
    {
        /// <summary>
        /// Switch doors must not check for user input each update.
        /// </summary>
        protected override void Update()
        {}

        /// <summary>
        /// Open the door when the obstacle is activated correctly.
        /// </summary>
        protected override void Activate()
        {
            Open();
        }
    }
}
