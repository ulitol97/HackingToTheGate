using UnityEngine;

namespace Game.Props.PickUpItems
{
    public class PickUpKey : FloorObject
    {
        
       protected override void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer.sprite = content.itemSprite;
        }
        
        protected override void Update()
        {
            if (PlayerInRange)
            {
                if (!IsPickedUp)
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
