using UnityEngine;

namespace Game.Props.Interactable.Door
{
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
