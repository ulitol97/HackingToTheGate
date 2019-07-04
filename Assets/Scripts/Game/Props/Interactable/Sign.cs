using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Props.Interactable
{
    /// <summary>
    /// The Sign class represents an in-game Sign the player can approach and read.
    /// It holds some text to be shown to the player when interacted with.
    /// </summary>
    public class Sign : Interactable
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
        /// Function called on each frame the Sign the script is attached to is present into the game.
        /// Checks for user controller input and if the player is close enough to interact with the sign contents.
        /// </summary>
        private void Update()
        {
            if (Input.GetButtonDown("Interact") && PlayerInRange)
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
        /// Checks for end of collision events between the object and other with objects collision capabilities.
        /// If the object that stopped colliding with the sign is the player, marks that the player is out
        /// of range and signals the player that no interactive object is close and makes the sign dialog
        /// disapperar.
        /// </summary>
        /// <param name="other">Collider object that finished contact.</param>
        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                context.Notify();
                
                PlayerInRange = false;
                dialogBox.SetActive(false);
            }
        }
    }
}
