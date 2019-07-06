using UnityEngine;

namespace Game.Props.PickUpItems
{
    /// <summary>
    /// Tje IPickUpItem interface defines the functionality any item that can be picked up from the floor must have.
    /// </summary>
    public interface IPickUpItem
    {
        void PickUp();
    }
}
