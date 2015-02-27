// C# version of 3rd person controller
// Uses Arduino gyroscope and accelerometer

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class ArduinoController : MonoBehaviour {
	public AnimationClip
		idleAnimation,
		walkAnimation,
		runAnimation,
		jumpPoseAnimation;

	public float
		walkMaxAnimationSpeed = 0.75f,
		trotMaxAnimationSpeed = 1f,
		runMaxAnimationSpeed = 1f,
		jumpAnimationSpeed = 1.15f,
		landAnimationSpeed = 1f;

	private Animation _animation;

	enum CharacterState {
		Idle = 0,
		Walking = 1,
		Trotting = 2,
		Running = 3,
		Jumping = 4
	}

	private CharacterState _characterState;

	public float
		walkSpeed = 2f,
		trotSpeed = 4f,
		runSpeed = 6f,
		inAirControlAcceleration = 3f,
		jumpHeight = 0.5f,
		gravity = 20f,
		speedSmoothing = 10f,
		rotateSpeed = 500f,
		trotAfterSeconds = 3f;

	public bool canJump = true;

	private float
		jumpRepeatTime = 0.05f,
		jumpTimeout = 0.15f,
		groundedTimeout = 0.25f,
		lockCameraTimer = 0f;

	private Vector3 moveDirection = Vector3.zero;

	private float
		verticalSpeed = 0f,
		moveSpeed = 0f;

	private CollisionFlags collisionFlags;

	private bool
		jumping = false,
		jumpingReachedApex = false,
		movingBack = false,
		isMoving = false;

	private float
		walkTimeStart = 0f,
		lastJumpButtonTime = -10f,
		lastJumpTime = -1f,
		lastJumpStartHeight = 0f;

	private Vector3 inAirVelocity = Vector3.zero;

	private float lastGroundedTime = 0f;

	private bool isControllable = true;

	void Awake() {
		moveDirection = transform.TransformDirection(Vector3.forward);

		_animation = transform.GetChild (0).GetComponent<Animation> ();

		if(!_animation)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");

		/*
		public var idleAnimation : AnimationClip;
		public var walkAnimation : AnimationClip;
		public var runAnimation : AnimationClip;
		public var jumpPoseAnimation : AnimationClip;	
		*/

		if(!idleAnimation) {
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}

		if(!walkAnimation) {
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}

		if(!runAnimation) {
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}

		if(!jumpPoseAnimation && canJump) {
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}

	void UpdateSmoothedMovementDirection() {
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();

		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward.Normalize();

		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		// float v = Input.GetAxisRaw("Vertical");
		// float h = Input.GetAxisRaw("Horizontal");
		float v = GetVerticalMovement();
		float h = GetHorizontalMovement();

		// Are we moving backwards or looking backwards
		movingBack = (v < -0.2);

		bool wasMoving = isMoving;
		isMoving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);

		// Target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;

		// Grounded controls
		if (grounded) {
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving)
				lockCameraTimer = 0f;

			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.

			if (targetDirection != Vector3.zero)
			{
				// If we are really slow, just snap to the target direction
				if (moveSpeed < walkSpeed * 0.9f && grounded)
				{
					moveDirection = targetDirection.normalized;
				}

				// Otherwise smoothly turn towards it
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

					moveDirection = moveDirection.normalized;
				}
			}

			// Smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;

			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

			_characterState = CharacterState.Idle;

			// Pick speed modifier

			/// EDIT: Always Run
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;

			/******************
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			{
				targetSpeed *= runSpeed;
				_characterState = CharacterState.Running;
			}
			else if (Time.time - trotAfterSeconds > walkTimeStart)
			{
				targetSpeed *= trotSpeed;
				_characterState = CharacterState.Trotting;
			}
			else
			{
				targetSpeed *= walkSpeed;
				_characterState = CharacterState.Walking;
			
			********************/

			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

			if (moveSpeed < walkSpeed * 0.3f)
				walkTimeStart = Time.time;
		}
		else { // In air controls
			// Lock camera while in air
			if (jumping)
				lockCameraTimer = 0f;

			if (isMoving)
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
		}
	}

	void ApplyJumping() {
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time)
			return;

		if (IsGrounded()) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void ApplyGravity() {
		if (isControllable) { // don't move player at all if not controllable.
			// Apply gravity
			// bool jumpButton = Gesture.IsJumping();

			// When we reach the apex of the jump we send out a message
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0)
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}

			if (IsGrounded ())
				verticalSpeed = 0;
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}

	float CalculateJumpVerticalSpeed(float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}

	void DidJump() {
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;

		_characterState = CharacterState.Jumping;
	}

	bool isJumping = false;

	void Update() {
		if (!isControllable)
			Input.ResetInputAxes();

		if (Gesture.IsJumping ()) {
			lastJumpButtonTime = Time.time;
		}

		UpdateSmoothedMovementDirection();

		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();

		// Apply jumping logic
		ApplyJumping ();

		// Calculate actual motion
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;

		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);

		// Animation
		if (_animation) {
			if(_characterState == CharacterState.Jumping) {
				if(!jumpingReachedApex) {
					_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				}
				else {
					_animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				}
			}
			else
			{
				if(controller.velocity.sqrMagnitude < 0.1f) {
					_animation.CrossFade(idleAnimation.name);
				}
				else
				{
					if(_characterState == CharacterState.Running) {
						_animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, runMaxAnimationSpeed);
						_animation.CrossFade(runAnimation.name);
					}
					else if(_characterState == CharacterState.Trotting) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, trotMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);
					}
					else if(_characterState == CharacterState.Walking) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, walkMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);
					}
				}
			}
		}


		// Set rotation to the move direction
		if (IsGrounded()) {
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		else {
			Vector3 xzMove = movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001f) {
				transform.rotation = Quaternion.LookRotation(xzMove);
			}
		}

		// We are in jump mode but just became grounded
		if (IsGrounded()) {
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping) {
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void OnCoontrollerColliderHit(ControllerColliderHit hit) {
		if (hit.moveDirection.y > 0.01f)
			return;
	}

	public float GetSpeed() {
		return moveSpeed;
	}

	public bool IsJumping() {
		return jumping;
	}

	public bool IsGrounded() {
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}

	public bool IsMovingBackwards() {
		return movingBack;
	}

	public float GetLockCameraTimer() {
		return lockCameraTimer;
	}

	public float GetVerticalMovement() {
		return 10;
	}

	public float GetHorizontalMovement() {
		return 0;
	}

	public bool IsMoving() {
		return Mathf.Abs(GetVerticalMovement()) + Mathf.Abs(GetHorizontalMovement()) > 0.5f;
	}

	public bool HasReachedApex() {
		return jumpingReachedApex;
	}

	public bool IsGroundedWithTimeout() {
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	void Reset() {
		gameObject.tag = "Player";
	}
	
	
}