namespace Game.Entities.Props.Interactable.Door
{
    /// <summary>
    /// The IDoor interface defines the functionality any Door in the game must implement. 
    /// </summary>
    public interface IDoor : IInteractable
    {
        void Open();

    }
}
