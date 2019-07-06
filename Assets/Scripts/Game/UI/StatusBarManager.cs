using Game.ScriptableObjects;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Game.UI
{
    /// <summary>
    /// The StatusBarManager represents any UI element represent on the health bar zone that is composed
    /// by an array of items indicating how many items of a kind a player has.
    /// </summary>
    public class StatusBarManager : MonoBehaviour
    {
        /// <summary>
        /// Array containing all the Image UI elements in the status bar.
        /// </summary>
        /// <returns></returns>
        public Image[] images;

        /// <summary>
        /// Sprite holding the image representing each item on the status bar.
        /// </summary>
        public Sprite itemPresent;

        /// <summary>
        /// Value holding the max amount of items of the kind represented that can be held by the player.
        /// </summary>
        public FloatValue maxItemsValue;
        
        /// <summary>
        /// Value holding the max amount of items of the kind represented that are held by the player.
        /// </summary>
        public FloatValue currentItemsValue;
        
        /// <summary>
        /// Function called when the StatusBarManager script is loaded into the game
        /// attached to a UI element.
        /// Sets up the status bar and renders the item images on screen.
        /// </summary>
        protected virtual void Start()
        {
            for (int i = 0; i < maxItemsValue.initialValue; i++)
                images[i].gameObject.SetActive(true);
            
            UpdateStatusBar();
        }
        
        /// <summary>
        /// Updates the state of the status bar. Checks how many items should be drawn and proceeds to enable or disable
        /// the images attached to it in consequence.
        /// </summary>
        /// <remarks>.</remarks>
        public virtual void UpdateStatusBar()
        {
            float currentAmountOfItems = currentItemsValue.runtimeValue;
            for (int i = 0; i < maxItemsValue.initialValue; i++)
            {
                // Draw item
                if (i <= currentAmountOfItems - 1)
                {
                    images[i].enabled = true;
                    images[i].sprite = itemPresent;
                }
                // No item
                else if (i >= currentAmountOfItems)
                    images[i].enabled = false;
                
            }
        }
    }
}
