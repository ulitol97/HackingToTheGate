using UnityEngine;
using UnityEngine.UI;

namespace Game.Props.Interactable.Door
{
    /// <summary>
    /// The PasswordDoor class inherits from Door.
    /// Represents an in-game door the player can open only after introducing a certain password.
    /// The door will prompt players for a password when interacted with.
    /// </summary>
    public class PasswordDoor : Door
    {
        /// <summary>
        /// Reference to the input field associated with the door that player's will use to write the password.
        /// </summary>
        public InputField inputField;

        /// <summary>
        /// Password that opens the door.
        /// </summary>
        public string password;
        
        
        /// <summary>
        /// Password doors will not reappear if they were opened once before, so they are deleted
        /// across scenes if needed.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            // If already opened before disable door
            if (isOpen.runtimeValue)
                Parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// On each frame and only if the player is in range to interact with the door, check for player input.
        /// If the player interacts with the door and the input field is not active (<see cref="inputField"/>),
        /// activate it.
        /// If the player interacts and the field is active, try to compare the input in the text field with
        /// the correct password (<see cref="ValidatePassword"/>).
        /// </summary>
        protected override void Update()
        {
            if (PlayerInRange)
            {
                if (Input.GetButtonDown("Interact") && !inputField.gameObject.activeInHierarchy)
                    ToggleInputField();
                else if (Input.GetButtonDown("Submit"))
                    ValidatePassword();
            }
        }
        
        /// <summary>
        /// Show the in put field on screen and focus it for players to enter the door password.
        /// </summary>
        private void ToggleInputField()
        {
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
            inputField.Select();
        }
        
        /// <summary>
        /// Hide the input field from screen and return keyboard control to the player.
        /// </summary>
        private void HideInputField()
        {
            inputField.text = "";
            inputField.DeactivateInputField();
            inputField.gameObject.SetActive(false);
        }

        /// <summary>
        /// Validate the current contents of the input field with the door password and open the door
        /// if the password was correct.
        /// </summary>
        private void ValidatePassword()
        {
            if (inputField.text.Trim().Equals(password))
            {
                if (context != null)
                    context.Notify();
                Open();
            }
            else
                HideInputField();
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                PlayerInRange = false;
                if (context != null)
                    context.Notify();
                
                // Additionally, turn of Input text.
                HideInputField();
            }
        }
    }
}
