using System.Collections;
using System.Collections.Generic;
using Game.Props.Interactable;
using Game.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class FloorObject : MonoBehaviour
{
    
    /// <summary>
    /// Item contained inside the chest.
    /// </summary>
    public Item content;

    /// <summary>
    /// Reference to the player's inventory. Chest obtained items will enter the player's inventory.
    /// </summary>
    public Inventory playerInventory;

    private SpriteRenderer _spriteRenderer;
    
    /// <summary>
    /// Boolean flag marked true if the player is in range to interact with the sign.
    /// </summary>
    protected bool PlayerInRange;
    
    /// <summary>
    /// Represents whether the chest is already open or not.
    /// </summary>
    public bool isPickedUp;

    
    /// <summary>
    /// Dialog UI element where the item description is shown (<see cref="content"/>).
    /// </summary>
    public GameObject dialogBox;

    /// <summary>
    /// Text to be shown after the chest has been open.
    /// </summary>
    public Text dialogText;
    
    // Start is called before the first frame update
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = content.itemSprite;
    }
    
    /// <summary>
    /// Signal in charge of observing if the player received an object form the chest.
    /// </summary>
    public Signal receiveItem;

    // Update is called once per frame
    private void Update()
    {
        if (PlayerInRange)
        {
            if (!isPickedUp)
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

    private void PickUp()
    {
        // Set up dialogue
        dialogBox.SetActive(true);
        dialogText.text = content.itemDescription;

        // Set up player inventory and notify player for animation.
        playerInventory.AddItem(content);
        playerInventory.currentItem = content;
        receiveItem.Notify();

        isPickedUp = true;
        _spriteRenderer.sprite = null;
        
    }
    
    /// <summary>
    /// Checks for collision events between the sign and other objects with collision capabilities.
    /// If the object colliding with the object is the player, marks that the player is in range and signals the
    /// player that an interactive object is close.
    /// </summary>
    /// <param name="other">Collider object that initiated contact.</param>
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            PlayerInRange = true;
        }

    }

    /// <summary>
    /// Checks for end of collision events between the object and other with objects collision capabilities.
    /// If the object that stopped colliding with the interactable is the player, marks that the player is out
    /// of range and signals the player that no interactive object is close.
    /// </summary>
    /// <param name="other">Collider object that finished contact.</param>
    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            PlayerInRange = false;
        }
    }
}
