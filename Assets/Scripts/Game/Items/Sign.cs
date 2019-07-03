using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
    /// <summary>
    /// The Sign class represents an in-game Sign the player can approach and read.
    /// It holds some text to be shown to the player when interacted with.
    /// </summary>
    public class Sign : MonoBehaviour
    {
        /// <summary>
        /// UI element containing the dialog template image.
        /// </summary>
        public GameObject dialogBox;
        
        /// <summary>
        /// UI element in charge of rendering the dialog text.
        /// </summary>
        public Text dialogText;

        /// <summary>
        /// String identifying the dialog the sign must show the player.
        /// </summary>
        public string dialogKey;

        /// <summary>
        /// Boolean flag marked true if the player is in range to interact with the sign.
        /// </summary>
        private bool _playerInRange;

        /// <summary>
        /// Function called on each frame the Sign the script is attached to is present into the game.
        /// Checks for user controller input and if the player is close enough to interact with the sign contents.
        /// </summary>
        private void Update()
        {
            if (Input.GetButtonDown("Interact") && _playerInRange)
            {
                ToggleSignText();
            }
        }
        
        /// <summary>
        /// If the sign's text is being displayed, it is hidden.
        /// If the sign's text is no hidden
        /// </summary>
        private void ToggleSignText()
        {
            if (dialogBox.activeInHierarchy)
                dialogBox.SetActive(false);
            else
            {
                var text = "";
                try
                {
                    text = Globals.Instance.TipsTable[dialogKey];
                }
                catch
                {
                    text = Globals.Instance.TipsTable["placeholder"];
                }
                finally
                {
                    dialogText.text = text;
                    dialogBox.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Checks for collision events between the sign and other objects with collision capabilities.
        /// If the object colliding with the sign is the player, marks that the player is in range.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }

        }

        /// <summary>
        /// Checks for end of collision events between the sign and other with objects collision capabilities.
        /// If the object that stopped colliding with the sign is the player, marks that the player is out of range.
        /// </summary>
        /// <param name="other">Collider object that finished contact.</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                dialogBox.SetActive(false);
            }
        }
    }
}
