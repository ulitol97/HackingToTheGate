namespace Game.UnityObserver
{
    public interface IObserver
    {
        /// <summary>
        /// Updates the corresponding observer status with the observed subject status.
        /// </summary>
        void UpdateObserver();
    }
}