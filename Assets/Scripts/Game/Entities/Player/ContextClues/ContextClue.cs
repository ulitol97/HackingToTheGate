using UnityEngine;

namespace Game.Entities.Player.ContextClues
{
    /// <summary>
    /// ContextClue acts as a template class for representing special information above the player avatar,
    /// to act like contextual clues to solve the game.
    /// </summary>
    public class ContextClue : MonoBehaviour
    {
        /// <summary>
        /// Game object to be interacted when the context clue must be shown.
        /// </summary>
        public GameObject contextClue;

        /// <summary>
        /// Current status of the clue on screen, either shown or not shown (<see cref="contextClue"/>).
        /// </summary>
        public bool contextActive;

        /// <summary>
        /// Toggle the context clue, making the game object representing the clue appear or disappear regarding its
        /// current state.
        /// </summary>
        public void ChangeContext()
        {
            contextActive = !contextActive;

            contextClue.SetActive(contextActive);
        }
    }
}
