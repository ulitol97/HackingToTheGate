using UnityEngine;

namespace Game.Props.Interactable.Door
{
    /// <summary>
    /// The NonKeyDoor class inherits from Door.
    /// Represents an in-game door the player can open if interacted with.
    /// </summary>
    public class NonKeyDoor : Door
    {
        public override void Open()
        {
            Parent.gameObject.SetActive(false);
        }
    }
}
