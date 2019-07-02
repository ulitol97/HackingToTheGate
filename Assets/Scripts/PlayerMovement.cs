using UnityEngine;

/// <summary>
/// The Player Movement class is in charge of detecting input from the player's controller and transforming it into
/// player movement actions.
/// </summary>
public class PlayerMovement : MonoBehaviour
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

	/// <summary>
	/// Function called when the PlayerMovement script is loaded into the game.
	/// Sets up all references to the Unity components modified on runtime.
	/// </summary>
	void Start ()
	{
		_playerAnimator = GetComponent<Animator>();
		_playerRigidBody = GetComponent<Rigidbody2D>();
	}
	
	/// <summary>
	/// Function called on each frame the PlayerMovement script is present into the game.
	/// Checks for user controller input to manage future movement operations.
	/// </summary>
	/// <remarks>GetAxisRaw allows digital input instead of analog input, either the movement signal is sent or not.
	/// </remarks>
	void Update () {
		
		_change.x = Input.GetAxisRaw("HorizontalPAD");
		_change.y = Input.GetAxisRaw("VerticalPAD");
	}

	/// <summary>
	/// Function called after each game update to handle physics events if user input was detected.
	/// </summary>
	private void FixedUpdate()
	{
		UpdateAnimationAndMove();
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
}