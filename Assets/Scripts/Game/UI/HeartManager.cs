using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// The heart manager class is attached to the UI health bar to manage the rendering
    /// of heart depending on the player's health.
    /// </summary>
    public class HeartManager : StatusBarManager
    {
        /// <summary>
        /// Sprite holding the image for half a life point.
        /// </summary>
        public Sprite halfHeart;

        
        /// <summary>
        /// Sprite holding the image for an empty life point.
        /// </summary>
        public Sprite emptyHeart;

        /// <summary>
        /// Check player's health and compute how many heart should be rendered.
        /// </summary>
        /// <remarks>It is divided by two since each UI hart counts as 2 life points.</remarks>
        public override void UpdateStatusBar()
        {
            float tempHealth = currentItemsValue.runtimeValue / 2;
            for (int i = 0; i < maxItemsValue.initialValue; i++)
            {
                // Full heart
                if (i <= tempHealth - 1)
                {
                    images[i].sprite = itemPresent;
                }
                // Empty heart
                else if (i >= tempHealth)
                {
                    images[i].sprite = emptyHeart;
                }
                // Half heart
                else
                {
                    images[i].sprite = halfHeart;
                }
            }
        }

    }
}
