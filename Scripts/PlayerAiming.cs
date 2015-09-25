using UnityEngine;
using System.Collections;

// This script allows the player to aim their shot.
// They cannot fire from here, only point the arc.
public class PlayerAiming : MonoBehaviour
{
	// Inspector Variables
	public float userRotateSpeed = 0.5f; // How fast the user can rotate the shot
	public float shotAngleVerticalMax = 75.0f; // Limit angle up

	// Rotation
	private float horizontalRotation = 0.0f;
	private float verticalRotation = 0.0f;

	// Vectors (direction of shot)
	private Vector3 shotFullVector;
	private Vector3 shotLateralVector;
	private Vector3 shotInitForwardVector;

	// Is the player allowed to be aiming?
	private bool activeAiming = false; // Default, false

	// Reference to Arrow
	public GameObject shotArrow;

	// MONOBEHAVIOUR FUNCTIONS

	void Awake()
	{
		// Reset variables
		ResetShot();

		this.enabled = false;
	}

	void Update()
	{
		// Early exit, if not active don't handle input
		if (!activeAiming)
			return;

		// Player leaves aiming
		HandleExitInput();

		// See if user is pressing buttons
		HandleRotationInput();

		// Calculate Vectors and Point Arrow
		CalculateShotVector();
		AimShotArrow();
	}


	void HandleExitInput()
	{
		if (Input.GetKeyUp("space") || Input.GetButtonUp("Submit"))
		{
			GameManager.Instance.cameraManager.ChangeAngle(CameraAngle.LaunchAngle);
		}
	}


	// HANDLE USER INPUT FOR AIMING
	void HandleRotationInput()
	{
		// Add axis input to rotation
		verticalRotation -= Input.GetAxis("Vertical") * userRotateSpeed;
		horizontalRotation += Input.GetAxis("Horizontal") * userRotateSpeed;

		// Clamp vertical rotation
		verticalRotation = Mathf.Clamp(verticalRotation, -shotAngleVerticalMax, 0);

	}

	void CalculateShotVector()
	{
		shotFullVector = Quaternion.Euler (verticalRotation, horizontalRotation, 0) * shotInitForwardVector;
		shotLateralVector = Quaternion.Euler(0, horizontalRotation, 0) * shotInitForwardVector;
	}

	void AimShotArrow()
	{
		shotArrow.transform.rotation = Quaternion.Euler(verticalRotation + 90, horizontalRotation, 0); // +90 degrees for Blender bullshit
		shotArrow.transform.position = transform.position + shotFullVector * 2; // 2m away from player center
	}


	// RESET SHOT (VECTORS + FLOATS)
	void ResetShot()
	{
		// Reset rotation to 0
		verticalRotation = 0.0f;
		horizontalRotation = 0.0f;

		// Reset Vectors (when goal is in play, default by aiming towards it)
		shotInitForwardVector = Vector3.forward;
		shotFullVector = shotInitForwardVector;
	}


	// ACTIVE AIMING PUBLIC TOGGLE
	public void EnableAiming()
	{
		activeAiming = true;
	}

	public void DisableAiming()
	{
		activeAiming = false;
	}

	// GET SHOT VECTORS
	public Vector3 GetShotVector()
	{
		return shotFullVector;
	}

	public Vector3 GetShotLateralVector()
	{
		return shotLateralVector;
	}

	public Quaternion GetLateralRotation()
	{
		return Quaternion.Euler(0, horizontalRotation, 0);
	}

	public float GetVerticalRotation()
	{
		return verticalRotation;
	}


	void OnEnable()
	{
		activeAiming = true;
	}

	void OnDisable()
	{
		activeAiming = false;
	}
}
