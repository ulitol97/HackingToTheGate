using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
    public class Sign : MonoBehaviour
    {
        public GameObject dialogBox;
        public Text dialogText;

        public string dialogKey;

        private bool _playerInRange;
        
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            // Redefinir un axis.
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) && _playerInRange)
            {
                Debug.Log(dialogBox.activeInHierarchy);
                if (dialogBox.activeInHierarchy)
                    dialogBox.SetActive(false);
                else
                {
                    dialogText.text = Globals.Instance.TipsTable[dialogKey];
                    dialogBox.SetActive(true);
                }
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }

        }

        /// <summary>
        /// 
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
