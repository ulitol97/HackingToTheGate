using System.Collections;
using Game.Audio;
using UnityEngine;

namespace Game.Props
{
    /// <summary>
    /// The Pot class represents an in-game pot object the player can approach and interact with.
    /// It may block the player's path unless attacked and broken.
    /// </summary>
    public class Pot : MonoBehaviour
    {

        /// <summary>
        /// Unity Animator component in charge of animating the pot sprite to simulate actions.
        /// </summary>
        /// <remarks>The animator attributes are cached below for quicker access</remarks>
        private Animator _animator;
        private static readonly int AnimatorSmash = Animator.StringToHash("smash");

        /// <summary>
        /// Function called when the Pot script is loaded into the game.
        /// Sets up the references to the Unity components modified on runtime.
        /// </summary>
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Triggers the activation of the pot break animation.
        /// </summary>
        public void Smash()
        {
            _animator.SetTrigger(AnimatorSmash);
            AudioManager.Instance.PlayEffectClip(AudioManager.BreakPot);
            StartCoroutine(Break());
        }

        /// <summary>
        /// Coroutine in charge of waiting for the por breaking animation to finish
        /// and then disabling it's colliding properties.
        /// </summary>
        private IEnumerator Break()
        {
            yield return new WaitForSeconds(0.3f);
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
