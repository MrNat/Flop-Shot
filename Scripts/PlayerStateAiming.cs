using UnityEngine;
using System.Collections;

public class PlayerStateAiming : RoundState
{
	// Inspector Variables
	public float userRotateSpeed = 5f; // How fast the user can rotate the shot
	public float shotAngleVerticalMax = 45.0f; // Limit angle up
	
	// Rotation
	private float horizontalRotation = 0.0f;
	private float verticalRotation = 0.0f;
	
	// Vectors (direction of shot)
	private Vector3 shotFullVector;
	private Vector3 shotLateralVector;
	private Vector3 shotInitForwardVector;

	// Reference to main camera
	private Camera mainCam;

	
	// Current camera properties
	private float cameraViewHeight = 0f;
	
	//private float cameraRotation = 0.0f;
	private Vector3 cameraLookAtPosition;
	private Vector3 cameraPosition;
	
	// Inspector Variables
	public Vector3 relativeDisplacement = new Vector3(0, 2, 4);
	public float cameraMinHeight = 0.0f;
	public float cameraMaxHeight = 10.0f;
	public float cameraSmoothDamping = 6.5f;


	protected override void Awake()
	{
		mainCam = Camera.main;
		ResetShot();

		base.Awake();
	}

	void Update()
	{
		if (Input.GetKeyUp("space") || Input.GetButtonUp("Submit"))
		{
			owner.ChangeState<PlayerStateLaunch>();
		}


		//Debug.Log ( owner.GetActivePlayer());
		//Debug.Log (player);

		// See if user is pressing buttons
		HandleRotationInput();
		
		// Calculate Vectors and Point Arrow
		CalculateShotVector();
		//AimShotArrow();

		//Debug.color
		Debug.DrawRay(player.transform.position, shotFullVector * 100, Color.red);

		CalculateCameraPosition(player);



		// Update ARC
		//Debug.Log (horizontalRotation);
		owner.arc.GeneratePoints(0, -verticalRotation / shotAngleVerticalMax, 100, 65, player.transform.position, -horizontalRotation+90);
		owner.arc.DrawMarker(GetHitMarker());
	}

	public override void OnEnter()
	{
		// Reset all the variables
		ResetShot();

		Debug.Log (player);
	}

	public override void OnExit()
	{
		//nextState.SavePlayerRotation(GetLateralRotation());
		//Debug.Log (player);
		Debug.Log ("exiting aiming");

		//owner.SetPoints(arc.GetPoints());
		base.OnExit();
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


	// HANDLE USER INPUT FOR AIMING
	void HandleRotationInput()
	{
		// Add axis input to rotation
		verticalRotation += Input.GetAxis("Vertical") * userRotateSpeed;
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
		//shotArrow.transform.rotation = Quaternion.Euler(verticalRotation + 90, horizontalRotation, 0); // +90 degrees for Blender bullshit
		//shotArrow.transform.position = transform.position + shotFullVector * 2; // 2m away from player center
	}


	void CalculateCameraPosition(GameObject player)
	{
		
		// Clamp height of camera
		cameraViewHeight = Mathf.Lerp(cameraMinHeight, cameraMaxHeight, -GetVerticalRotation() / shotAngleVerticalMax);
		
		// Save rotation
		Quaternion lateralRotation = GetLateralRotation();
		//Vector3 lateralVector = playerAimingScript.GetShotLateralVector();
		
		
		// Camera Position
		cameraPosition = player.transform.position;
		cameraPosition += lateralRotation * Vector3.back * (relativeDisplacement.z + cameraViewHeight);		// Move back
		cameraPosition += lateralRotation * Vector3.right * relativeDisplacement.x;								// Move to side
		cameraPosition += cameraViewHeight * Vector3.up * relativeDisplacement.y;								// Move up
		
		// LookAt Position
		cameraLookAtPosition = player.transform.position;
		cameraLookAtPosition += lateralRotation * Vector3.forward * cameraViewHeight * relativeDisplacement.z; // Look slightly ahead of player



		mainCam.transform.position = cameraPosition;
		//mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraPosition, Time.deltaTime * cameraSmoothDamping);
		mainCam.transform.rotation = Quaternion.Lerp (mainCam.transform.rotation, Quaternion.LookRotation(cameraLookAtPosition - mainCam.transform.position), Time.deltaTime * cameraSmoothDamping);
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

	public float GetLateralRotationRaw()
	{
		return horizontalRotation;
	}
	
	public float GetVerticalRotation()
	{
		return verticalRotation;
	}
}
