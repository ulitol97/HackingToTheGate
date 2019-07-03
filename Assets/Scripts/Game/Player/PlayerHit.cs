using Game.Items;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// The PlayerHit class handles game logic that should happen whenever a Player
    /// attacks. PlayerHit objects are attached to the player's attack hit-boxes to handle
    /// this logic.
    /// </summary>
    public class PlayerHit : MonoBehaviour
    {
        /// <summary>
        /// Checks for collision events between the player attacks and other objects with collision capabilities and
        /// triggers the corresponding logic of the object that collided with the attack.
        /// </summary>
        /// <param name="other">Collider object that initiated contact.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Breakable"))
            {
                if (other.GetComponent<Pot>() != null)
                    other.GetComponent<Pot>().Smash();
            }
        }
    }
}
