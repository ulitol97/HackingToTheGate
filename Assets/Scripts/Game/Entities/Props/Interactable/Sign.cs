using System;
using Game.Audio;
using Game.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Entities.Props.Interactable
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
        /// Dummy text to show when no specific text is found for the id specified (<see cref="dialogId"/>).
        /// </summary>
        private string _defaultText = "Nothing to read here...";
        
        /// <summary>
        /// Text displayed by the sign.
        /// </summary>
        private string _signText = "Nothing to read here...";

        /// <summary>
        /// Id identifying the text from the list of clues that was retrieved from the JSON file
        /// that the sign must show.
        /// </summary>
        public int dialogId;

        private void Start()
        {
            SetSignText();
        }

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
        
        private void SetSignText()
        {
            try
            {
                _signText = ConfigurationManager.Instance.challengesConfig.GetClues()[dialogId];
            }
            catch (ArgumentOutOfRangeException)
            {
                _signText = _defaultText;
            }
        }
        
        /// <summary>
        /// If the sign's text is being displayed, it is hidden.
        /// If the sign's text is no hidden
        /// </summary>
        private void ToggleSignText()
        {
            dialogText.text = _signText;
            dialogBox.SetActive(!dialogBox.activeInHierarchy);
            AudioManager.Instance.PlayEffectClip(AudioManager.Confirm);
        }
        
        /// <summary>
        /// Checks for end of collision events between the object and other with objects collision capabilities.
        /// If the object that stopped colliding with the sign is the player, marks that the player is out
        /// of range and signals the player that no interactive object is close and makes the sign dialog
        /// disappear.
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
