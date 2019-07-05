using UnityEngine;

namespace Game.Props.Interactable.Door
{
    public class NonKeyDoor : Door
    {
        public override void Open()
        {
            Destroy(Parent.gameObject);
        }
    }
}
