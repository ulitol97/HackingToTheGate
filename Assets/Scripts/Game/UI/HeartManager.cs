using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// The heart manager class is attached to the UI health bar to manage the rendering
    /// of heart depending on the player's health.
    /// </summary>
    public class HeartManager : MonoBehaviour
    {
        /// <summary>
        /// Array containing all the heart UI elements in the health bar.
        /// </summary>
        /// <returns></returns>
        public Image[] hearts;

        /// <summary>
        /// Sprite holding the image for a full life point.
        /// </summary>
        public Sprite fullHeart;
        
        /// <summary>
        /// Sprite holding the image for half a life point.
        /// </summary>
        public Sprite halfHeart;

        
        /// <summary>
        /// Sprite holding the image for an empty life point.
        /// </summary>
        public Sprite emptyHeart;
        
        /// <summary>
        /// Value holding the max amount of life points held by the player.
        /// </summary>
        public FloatValue heartContainers;
        
        /// <summary>
        /// Value holding the amount of life points held by the player.
        /// </summary>
        public FloatValue playerCurrentHealth;
        
        /// <summary>
        /// Function called when the HeartManager script is loaded into the game
        /// attached to a UI element.
        /// Sets up the health bar and renders the hearts on screen.
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < heartContainers.initialValue; i++)
            {
                hearts[i].gameObject.SetActive(true);
                hearts[i].sprite = fullHeart;
            }
        }

        /// <summary>
        /// Check player's health and compute how many heart should be rendered.
        /// </summary>
        /// <remarks>It is divided by two since each UI hart counts as 2 life points.</remarks>
        public void UpdateHearts()
        {
            float tempHealth = playerCurrentHealth.runtimeValue / 2;
            for (int i = 0; i < heartContainers.initialValue; i++)
            {
                // Full heart
                if (i <= tempHealth - 1)
                {
                    hearts[i].sprite = fullHeart;
                }
                // Empty heart
                else if (i >= tempHealth)
                {
                    hearts[i].sprite = emptyHeart;
                }
                // Half heart
                else
                {
                    hearts[i].sprite = halfHeart;
                }
            }
        }

    }
}
