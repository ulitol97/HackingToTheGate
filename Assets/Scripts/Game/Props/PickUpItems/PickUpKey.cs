using UnityEngine;

namespace Game.Props.PickUpItems
{
    /// <summary>
    /// The PickUpKey class represents a key that can be picked up from the floor
    /// by the player by passing through it.
    /// </summary>
    public class PickUpKey : FloorObject
    {
        protected override void Start()
        {
            base.Start();
            SpriteRenderer.sprite = content.itemSprite;
        }
        
        protected override void Update()
        {
            if (PlayerInRange)
            {
                if (!isPickedUp.runtimeValue)
                    PickUp();

                else if (Input.GetButtonDown("Interact"))
                {
                    dialogBox.SetActive(false);
                    receiveItem.Notify();
                    PlayerInRange = false;
                    gameObject.SetActive(false);
                }
            }
        }

        public override void PickUp()
        {
            // Set up dialogue
            dialogBox.SetActive(true);
            dialogText.text = content.itemDescription;

            // Set up player inventory and notify player for animation.
            playerInventory.AddItem(content);
            playerInventory.currentItem = content;
            
            base.PickUp();
        }
    }
}
