using System;
using System.Collections;
using Game.ScriptableObjects;
using UnityEngine;

namespace Game.Entities.Player
{
	/// <summary>
	/// The Player class is in charge of detecting input from the player's controller and transforming it into
	/// player movement and attack actions.
	/// </summary>
	public class Player : MonoBehaviour
	{
		/// <summary>
		/// A multiplying factor to the player's character speed.
		/// </summary>
		/// <remarks>A default value of "4" is enough for an average speed.</remarks>
		[System.ComponentModel.DefaultValue(4f)]
		public float speed;
	
		/// <summary>
		/// Unity RigidBody component representing the player's character body in game.
		/// </summary>
		private Rigidbody2D _playerRigidBody;
	
		/// <summary>
		/// Vector used to store the difference between the player's current position and his/her target position.
		/// </summary>
		private Vector2 _change;

		/// <summary>
		/// Unity Animator component in charge of animating the player sprite to simulate actions.
		/// </summary>
		/// <remarks>The animator attributes are cached below for quicker access</remarks>
		private Animator _playerAnimator;
		private static readonly int AnimatorMoveX = Animator.StringToHash("moveX");
		private static readonly int AnimatorMoveY = Animator.StringToHash("moveY");
		private static readonly int AnimatorMoving = Animator.StringToHash("moving");
		private static readonly int AnimatorAttacking = Animator.StringToHash("attacking");

		/// <summary>
		/// Enum structure holding the main character possible states.
		/// </summary>
		public enum  PlayerState
		{
			Idle,
			Walk,
			Attack,
			Staggered
		}

		/// <summary>
		/// Current state of the player character.
		/// </summary>
		public PlayerState currentState;
		
		/// <summary>
		/// FloatValue storing the player's initial current health.
		/// </summary>
		public FloatValue currentHealth;

		/// <summary>
		/// Signal observing players health.
		/// </summary>
		public Signal playerHealthSignal;

		private const float JoystickTolerance = 0.1f;
		
		/// <summary>
		/// Function called when the Player script is loaded into the game.
		/// Sets up the character current state and all references to the Unity components modified on runtime.
		/// </summary>
		private void Start ()
		{
			currentState = PlayerState.Walk;
			_playerAnimator = GetComponent<Animator>();
			_playerRigidBody = GetComponent<Rigidbody2D>();
			
			// Start the animator with the character facing down.
			_playerAnimator.SetFloat(AnimatorMoveX, 0);
			_playerAnimator.SetFloat(AnimatorMoveY, -1);
		}
	
		/// <summary>
		/// Function called on each frame the Player script is present into the game.
		/// Checks for user controller input either on the joystick or the D-PAD to manage future player operations.
		/// </summary>
		/// <remarks>GetAxisRaw allows digital input instead of analog input, either the movement signal is sent or not.
		/// </remarks>
		private void Update () {
			
			// Check player input
			_change.x = Input.GetAxisRaw("Horizontal");
			if (Math.Abs(_change.x) < JoystickTolerance)
				_change.x = Input.GetAxisRaw("HorizontalPAD");
			
			_change.y = Input.GetAxisRaw("Vertical");
			if (Math.Abs(_change.y) < JoystickTolerance)
				_change.y = Input.GetAxisRaw("VerticalPAD");

			// Check attack input
			if (Input.GetButtonDown("Attack") && currentState != PlayerState.Attack
			    && currentState != PlayerState.Staggered)
			{
				StartCoroutine(Attack());
			}
			else if  (currentState == PlayerState.Idle || currentState == PlayerState.Walk)
			{
				UpdateAnimationAndMove();
			}
		}

		/// <summary>
		/// Function called after each game update to handle physics events if user input was detected.
		/// Resets next movement orders the next movement.
		/// </summary>
		private void FixedUpdate()
		{
			_change = Vector2.zero;
			UpdateAnimationAndMove();
		}

		private IEnumerator Attack()
		{
			_playerAnimator.SetTrigger(AnimatorAttacking);
			currentState = PlayerState.Attack;
			yield return new WaitForFixedUpdate(); // Wait one frame
			yield return new WaitForSeconds(0.3f); // Wait for length of the animation
			currentState = PlayerState.Walk;
		}

		/// <summary>
		/// Function summoned each physics frame update.
		/// Invokes the characters moving and animation logic:
		/// -> <see cref="MoveCharacter"/>
		/// -> <see cref="AnimateCharacter"/>
		/// </summary>
		private void UpdateAnimationAndMove()
		{
			if (_change != Vector2.zero)
			{
				_playerAnimator.SetBool(AnimatorMoving, true);
				MoveCharacter();
				AnimateCharacter();
			}
			else
			{
				_playerAnimator.SetBool(AnimatorMoving, false);
			}
		}

		/// <summary>
		/// Changes the position of the game character regarding the user input, character speed and
		/// time passed between updates.
		/// </summary>
		/// <remarks>The player's change in position is multiplied by delta time to move the character
		/// slightly each frame, which is smoother.</remarks>
		private void  MoveCharacter()
		{
			_playerRigidBody.MovePosition(
				_playerRigidBody.position + speed * Time.deltaTime * _change.normalized);
		}

		/// <summary>
		/// Animates the player character sprite regarding the user input and the target location it is moving
		/// </summary>
		private void AnimateCharacter()
		{
			_playerAnimator.SetFloat(AnimatorMoveX, _change.x);
			_playerAnimator.SetFloat(AnimatorMoveY, _change.y);
		}
		
		
		/// <summary>
		/// Applies damage to player and arranges the end of knockback logic if the player has
		/// health left.
		/// </summary>
		/// <param name="knockTime"></param>
		public void Knock(float knockTime, float damage)
		{
			currentHealth.runtimeValue -= damage;
			playerHealthSignal.Notify();
			if (currentHealth.runtimeValue > 0)
			{
				StartCoroutine(EndKnock(knockTime));
			}
			// Player death
			else
			{
				this.gameObject.SetActive(false);
			}
		}
		
		/// <summary>
		/// Co routine in charge of stopping the knockback effect on the knocked back game enemies
		/// after a certain time has passed by.
		/// </summary>
		/// <param name="knockTime">Time before stopping the knockback force.</param>
		/// <returns></returns>
		private IEnumerator EndKnock( float knockTime)
		{
			if (_playerRigidBody != null)
			{
				yield return new  WaitForSeconds(knockTime);
				_playerRigidBody.velocity = Vector2.zero;
				currentState = PlayerState.Idle;
			}
		}
		
		public void ChangeState(PlayerState state)
		{
			currentState = state;
		}
	}
}