using UnityEngine;
using System.Collections;



[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerRedux : SVBLM.Core.FSM {

	public const float CHARACTER_HEIGHT = 1.8f;
	public const float MOVE_SPEED = 60.0f;
	public const float LOOK_SPEED = 4.0f;
	public const float ROLL_AMOUNT = 0.5f;
	public const float MAX_VERTICAL_LOOK_ANGLE = 70f;
	public const float MAX_XZ_VELOCITY = 10f;
	public const float XZ_DRAG = 8f;
	public const float HEAD_BOB_RANGE = 0.3f;
	public const float GRAVITY = 20f;
	public const float JUMP_SPEED = 10f;
	public const float SPRINT_BOOST = 1.5f;
	public const float CROUCH_MOVEMENT_PENALTY = 0.5f;
	public const float HOOKSHOT_METERS_PER_SECOND = 70.0f;
	public const float VAULT_TIMEOUT = 0.3f;

	public enum States {
		GROUNDED,
		FALLING,
		JUMPING,
		VAULTING,
		STOPPED,
		FIRING_HOOKSHOT,
		GRAPPLING,
		RETRACTING_HOOKSHOT,
		ZIPLINING,
		JUMP_RUNE,
		DASH_RUNE,
		WARP_RUNE,
		THROWING,
		DEAD,
		SLIDING,
		NONE
	}

	private Vector3 initialPosition;

	public Transform head;
	public AudioClip warpSound;
	public AudioClip targetBecameSelectedSound;
	public AudioClip targetBecameUnselectedSound;

	public Vector3 velocity = Vector3.zero;
	private Vector2 lookRotation = Vector2.zero;
	private float headRoll = 0;
	private float initialHeadHeight;
	private float headBobFrequency = 6f;
	private bool slowmoTargeting = false;
	private float hookshotTargetingTimeout = 0;
	public AudioClip foundHookshotTarget;
	private float ziplineTimeout;
	private float quitCountdown = 3.1f;
	
	private Vector3 vaultPoint;
	private float vaultLookOffset = 0f;
	private float headRollDirecton = 1f;
	private bool canMove = true;

	private Vector3 dashForwardDirection;

	private bool sprinting = false;
	private bool crouching = false;

	public States haltState;
	private float vaultTimeout = 0;

	private Footsteps _footstep;
	private Footsteps footstep {
		get {
			if(_footstep == null) {
				_footstep = GetComponent<Footsteps>();
			}

			return _footstep;
		}
	}

	private float footstepTimer;
	private Rigidbody carriedObject;

	private float throwCharge = 0;
	private bool isChargingThrow = false;
	private float minTorque = 0;
	private float maxTorque = 5;

	float slideTimeout = 0;
	public ParticleSystem slidingSystem;
	public AudioClip slideLoop;

	void Start() {
		initialPosition = transform.position;
		if (head == null) {
			Debug.LogError("You need to assign the head transform of the player controller.");
			return;
		}

		// Parent and reset transforms
		Camera.main.transform.parent = head;
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;
		Camera.main.transform.localScale = Vector3.zero;
		lookRotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);

		initialHeadHeight = head.localPosition.y;

		// Kinematic rigidbody.
		rigidbody.isKinematic = true;
		currentState = States.FALLING;
		controller.height = CHARACTER_HEIGHT;
		tag = "Player";
	}

	void OnDestroy() {
	}

	IEnumerator JUMPING_EnterState() {
		velocity.y = JUMP_SPEED;
		currentState = States.FALLING;
		yield break;
	}

	IEnumerator VAULTING_EnterState() {
		headRollDirecton = Random.Range (-1f, 1f);
		yield break;
	}

	void VAULTING_Update() {
		footstep.PlayJump ();
		headRoll = Mathf.Lerp (headRoll, Mathf.Sign (headRollDirecton) * 20f, 0.3f);
		if (transform.position.y - (controller.height * 0.5f) < vaultPoint.y) {
			Vector3 newPosition = transform.position;
			newPosition.y = Mathf.Lerp(transform.position.y, vaultPoint.y + controller.height, 0.1f);
			transform.position = newPosition;
		} else {
			currentState = States.FALLING;
		}

		if (Vector3.Distance (transform.position, vaultPoint) > 1.0f) {
			currentState = States.FALLING;
		}
	}

	IEnumerator VAULTING_ExitState() {
		vaultTimeout = 0.3f;
		yield break;
	}

	void FALLING_Update() {
		if ((controller.collisionFlags & CollisionFlags.Below) != 0) {
			currentState = States.GROUNDED;
		}

		CalculateGravity ();
		PerformVaultCheck ();
	}

	IEnumerator GROUNDED_EnterState() {
		yield break;
	}

	void GROUNDED_Update() {
		velocity.y = -5f;
		if ((controller.collisionFlags & CollisionFlags.Below) == 0) {
			velocity.y = 0;
			currentState = States.FALLING;
		}

		if (Input.GetButtonDown ("Jump")) currentState = States.JUMPING; 

		if (footstepTimer > 0.3f) {
			footstepTimer = 0;
		} else {
			footstepTimer += Time.deltaTime * Mathf.Lerp(0, 1.3f, XZVelocity().magnitude/MAX_XZ_VELOCITY);
		}



		CalculateHeadBob ();
		PerformVaultCheck ();
	}

	IEnumerator STOPPED_EnterState() {
		canMove = false;
		yield return null;
	}

	IEnumerator STOPPED_ExitState() {
		canMove = true;
		yield return null;
	}

	protected override void Always_AfterUpdate() {
		if (Input.GetKey (KeyCode.Escape)) {
			quitCountdown -= Time.deltaTime;
			if (quitCountdown < 0) {
				Application.Quit();
			}
		}
		if (!canMove) return;
		vaultTimeout -= Time.deltaTime;
		hookshotTargetingTimeout -= Time.deltaTime;
		sprinting = Input.GetButton ("Sprint");
		if(CanExitCrouch()) crouching = Input.GetButton ("Crouch");
		ziplineTimeout -= Time.deltaTime;

		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			PlayerPrefs.DeleteAll ();
			UI.ToastDebug("Prefs cleared.");
		}

		CalculateRootMovement ();
		CalculateXZDragAndClampMaxSpeed ();
		CalculateLook ();
		CalculateCrouchHeight ();
		controller.Move (velocity * Time.deltaTime);
	}

	void CalculateLook() {
		vaultLookOffset = Mathf.Lerp (vaultLookOffset, (States)currentState == States.VAULTING ? -40f : 0f, 0.1f);

		headRoll -= Input.GetAxis ("LookHorizontal") * ROLL_AMOUNT;
		lookRotation.y += Input.GetAxis ("LookHorizontal") * LOOK_SPEED;
		lookRotation.x += Input.GetAxis ("LookVertical") * LOOK_SPEED;

		if (lookRotation.x > 360f) lookRotation.x -= 360f;
		if (lookRotation.x < -360f) lookRotation.x += 360f;
		if (lookRotation.y > 360f) lookRotation.y -= 360f;
		if (lookRotation.y < -360f) lookRotation.y += 360f;

		lookRotation.x = Mathf.Clamp (lookRotation.x, -MAX_VERTICAL_LOOK_ANGLE, MAX_VERTICAL_LOOK_ANGLE); 

		Quaternion headRotation = Quaternion.AngleAxis(lookRotation.x + vaultLookOffset, Vector3.left);
		Quaternion bodyRotation = Quaternion.AngleAxis(lookRotation.y, Vector3.up);

		headRotation *= Quaternion.AngleAxis (headRoll, Vector3.forward);


		head.localRotation = Quaternion.Slerp(head.localRotation, headRotation, TimeHelper.ExpLerpCoefficient(6f));
		transform.localRotation = Quaternion.Slerp(transform.localRotation, bodyRotation, TimeHelper.ExpLerpCoefficient(6f));

		headRoll *= 0.93f * Time.deltaTime;
	}
	
	void CalculateRootMovement() {
		velocity += transform.forward * Time.deltaTime * Input.GetAxis ("Vertical") * ((States) currentState == States.SLIDING ? MOVE_SPEED * 0.3f : MOVE_SPEED) * (sprinting ? SPRINT_BOOST : 1) * (crouching ? CROUCH_MOVEMENT_PENALTY : 1);
		velocity += transform.right * Time.deltaTime * Input.GetAxis ("Horizontal") * ((States) currentState == States.SLIDING ? MOVE_SPEED * 0.3f : MOVE_SPEED) * (sprinting ? SPRINT_BOOST : 1) * (crouching ? CROUCH_MOVEMENT_PENALTY : 1);
	}

	void CalculateXZDragAndClampMaxSpeed() {
		Vector2 xzVelocity = new Vector2 (velocity.x, velocity.z);
		xzVelocity = Vector2.Lerp (xzVelocity, Vector2.zero, TimeHelper.ExpLerpCoefficient((States) currentState == States.SLIDING ? 1f :  XZ_DRAG));
		xzVelocity = Vector2.ClampMagnitude (xzVelocity, MAX_XZ_VELOCITY * (sprinting ? SPRINT_BOOST : 1));
		velocity.x = xzVelocity.x;
		velocity.z = xzVelocity.y;
	}

	void CalculateGravity() {
		velocity.y -= GRAVITY * Time.deltaTime;
	}

	void CalculateHeadBob() {
		head.transform.localPosition = Vector3.up * initialHeadHeight + (Vector3.up *  HEAD_BOB_RANGE * Mathf.Abs(Mathf.Sin (Time.time * headBobFrequency * (sprinting ? SPRINT_BOOST : 1))) * (XZVelocity().magnitude/MAX_XZ_VELOCITY));
		if (sprinting) {
			headRoll += HEAD_BOB_RANGE * 15f * Mathf.Abs(Mathf.Sin (Time.time * headBobFrequency * SPRINT_BOOST)) * (XZVelocity().magnitude/MAX_XZ_VELOCITY);
		}

		if (crouching) {
			headRoll += HEAD_BOB_RANGE * 50f * Mathf.Abs(Mathf.Sin (Time.time * headBobFrequency * CROUCH_MOVEMENT_PENALTY)) * (XZVelocity().magnitude/MAX_XZ_VELOCITY);

		}
	}

	bool CanExitCrouch() {
		RaycastHit hit;
		if (Physics.SphereCast (transform.position, controller.radius, Vector3.up, out hit, CHARACTER_HEIGHT / 2)) {
			return false;
		}

		return true;
	}

	void CalculateCrouchHeight() {
		float newHeight = Mathf.Lerp (controller.height, crouching ? (CHARACTER_HEIGHT * 0.5f) : CHARACTER_HEIGHT, 0.1f);
		float difference = controller.height - newHeight;
		controller.height = newHeight;
		controller.Move (Vector3.down * difference);
		Vector3 newHeadHeight = head.transform.localPosition;
		newHeadHeight.y = Mathf.Lerp (newHeadHeight.y, crouching ? 0 : initialHeadHeight, 0.1f);
		head.transform.localPosition = newHeadHeight;
	}

	Vector3 RaycastLook() {
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f)) {
			return hit.point;
		} else {
			return Camera.main.transform.position + Camera.main.transform.forward * 100f;
		}
	}

	void PerformVaultCheck() {
		if(sprinting && vaultTimeout < 0) {
			RaycastHit hit;
			if(Physics.SphereCast(transform.position, controller.radius, transform.forward, out hit, controller.radius + 0.1f)) {
				if(Physics.SphereCast(transform.position + (Vector3.up * controller.height/2) + (transform.forward * controller.radius), controller.radius, - transform.up, out hit, controller.height)) {
					vaultPoint = hit.point;
					if(Vector3.Angle(hit.normal, Vector3.up) < 20f) {
						currentState = States.VAULTING;
					}
				}
			}
		}
	}

	Vector2 XZVelocity() {
		return new Vector2(velocity.x, velocity.z);
	}

	RaycastHit checkDownWithOffset(Vector3 offset) {
		RaycastHit hit;
		Physics.SphereCast (transform.position + (offset.normalized * 0.3f), controller.radius * 0.8f, Vector3.down, out hit, controller.bounds.extents.y + 0.1f, ~ (1 << LayerMask.NameToLayer ("Player")));
		return hit;
	}

	Vector3 getMostLikelyNormalOfGround() {
		RaycastHit[] hits = new RaycastHit[8];
		for (int i = 0; i < 8; i++) {
			Vector2 offset = new Vector2(Mathf.Sin(i/8f * 2 * Mathf.PI), Mathf.Cos(i/8f * 2 * Mathf.PI));
			hits[i] = checkDownWithOffset(new Vector3(offset.x, 0, offset.y));
		}

		Vector3 mostLikely = hits[0].normal;
		foreach (RaycastHit hit in hits) {
			Debug.DrawRay(hit.point, hit.normal);
			if (Vector3.Angle(hit.normal, Vector3.up) < Vector3.Angle(mostLikely, Vector3.up)) {
				mostLikely = hit.normal;
			}
		}

		return mostLikely * (1f/8f);
	}

	public void Halt() {
		canMove = false;
		StopAllCoroutines ();
		if((States) haltState != States.STOPPED) haltState = (States) currentState;
		currentState = States.STOPPED;
	}

	public void Resume() {
		if(haltState == States.NONE) {
			UI.ToastWarning("Player was resumed when he wasn't halted :(");
			return;
		}
		currentState = States.FALLING;
	}

	public bool IsGrounded() {
		return (States)currentState == States.GROUNDED;
	}

}