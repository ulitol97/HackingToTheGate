using UnityEngine;

namespace Game.Props.Interactable
{
    public interface IInteractable
    {
        bool IsPlayerInRange { get; set; }
    }
}
