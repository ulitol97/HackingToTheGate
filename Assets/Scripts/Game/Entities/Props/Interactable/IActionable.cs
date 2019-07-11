namespace Game.Entities.Props.Interactable
{
    /// <summary>
    /// THe interface actionable serves as a contract to derived objects that will perform
    /// certain actions only when actioned by a toggle mechanism (i.e. a switch, a lever...)
    /// </summary>
    public interface IActionable
    {
        /// <summary>
        /// Logic to be executed by the Actionable element when it receives an action.
        /// </summary>
        /// <param name="actionId">Id of the actor that caused the action.</param>
        void OnActionReceived(int actionId);
    }
}
