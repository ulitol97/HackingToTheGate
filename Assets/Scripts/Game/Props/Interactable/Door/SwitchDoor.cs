namespace Game.Props.Interactable.Door
{
    /// <summary>
    /// The SwitchDoor class inherits from ActionableObstacle, which also encapsulates the door
    /// functionality.
    /// Represents an in-game door the player can open if a number of switches or levers are pressed
    /// in the correct sequence.
    /// </summary>
    public class SwitchDoor : ActionableObstacle
    {
        /// <summary>
        /// Open the door when the obstacle is activated correctly.
        /// </summary>
        protected override void Activate()
        {
            Open();
        }
    }
}
