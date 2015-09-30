using UnityEngine;
using System.Collections;

public class PlayerStateAiming : RoundState
{
	// Inspector Variables
	public float userRotateSpeed = 0.75f;

	public float maxRange = 100.0f;
	public float minRange = 0.5f;

	// Aiming Variables
	private float range = 10.0f; // default
	private float height = 10.0f; // default
	private float rotation = 0.0f;

	private float camPosRatio = 0.0f;
	private float camRotation = 0.0f;

	// Reference to main camera
	private Camera mainCam;


	//private float cameraRotation = 0.0f;
	private Vector3 cameraLookAtPosition;
	private Vector3 cameraPosition;
	
	// Inspector Variables
	//public Vector3 relativeDisplacement = new Vector3(0, 2, 4);
	private float cameraFollowDistance = 6f;
	private float cameraFollowHeight = 4f;
	public float cameraSmoothDamping = 6.5f;


	protected override void Awake()
	{

	}

	void Update()
	{
		// See if user is pressing buttons
		HandleRotationInput();

		CalculateHeight();



		// Update ARC
		//Debug.Log (horizontalRotation);
		owner.arc.GeneratePoints(0, GetRange(), height, player.transform.position, -rotation+90);
		owner.arc.DrawMarker(GetHitMarker(), rotation);
	}

	void LateUpdate()
	{
		
		CalculateCameraPosition(player);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (cameraPosition, 1f);
	}

	public override void OnEnter()
	{
		Debug.Log("Entering Aiming State");

		mainCam = Camera.main;
		ResetShot();

		// Subscribe to input
		EventInputBroadcaster.OnSubmitUpAction += ConfirmShot;

		
		base.Awake();
	}

	public override void OnExit()
	{	
		// Unsubscribe
		EventInputBroadcaster.OnSubmitUpAction -= ConfirmShot;

		//owner.SetPoints(arc.GetPoints());
		base.OnExit();
	}


	// RESET SHOT (VECTORS + FLOATS)
	void ResetShot()
	{
		// Reset rotation to 0
		rotation = 0.0f;
	}

	void ConfirmShot()
	{
		owner.ChangeState<PlayerStatePower>();
	}

	// HANDLE USER INPUT FOR AIMING
	void HandleRotationInput()
	{
		// Add axis input to rotation
		rotation += Input.GetAxis("Horizontal") * userRotateSpeed;

		range += Input.GetAxis("Vertical") * userRotateSpeed;
		range = Mathf.Clamp(range, minRange, maxRange);


		// Camera Input
		float factor = 0.005f;
		camPosRatio += -Input.GetAxis("CameraVertical") * factor;
		if (Input.GetKey("i"))
			camPosRatio += factor;
		if (Input.GetKey("k"))
			camPosRatio -= factor;

		camPosRatio = Mathf.Clamp01(camPosRatio);


		float rotFactor = 1f;
		camRotation += Input.GetAxis("CameraHorizontal") * rotFactor;
		if (Input.GetKey("j"))
			camRotation += rotFactor;
		if (Input.GetKey("l"))
			camRotation -= rotFactor;

		camRotation = Mathf.Clamp (camRotation, -50, 50);

	}

	void CalculateHeight()
	{
		// Height is exponential to range
		height = Mathf.Pow(2, range / 20);
	}


	void CalculateCameraPosition(GameObject player)
	{
		// Camera Position
		float ratioBetweenPoints = camPosRatio * (owner.arc.positionPoints.Count);
		int pointAIndex = Mathf.FloorToInt(ratioBetweenPoints);
		int pointBIndex = pointAIndex + 1;

		if (pointAIndex > owner.arc.positionPoints.Count - 2)
		{
			pointAIndex = owner.arc.positionPoints.Count - 2;
			pointBIndex = owner.arc.positionPoints.Count - 1;
		}

		//cameraPosition = player.transform.position;
		cameraPosition = Vector3.Lerp(owner.arc.positionPoints[pointAIndex], owner.arc.positionPoints[pointBIndex], ratioBetweenPoints - pointAIndex);
		cameraPosition += Vector3.up * cameraFollowHeight;
		cameraPosition += Quaternion.Euler(0, camRotation + rotation, 0) * Vector3.back * cameraFollowDistance;
		//cameraPosition += Vector3.up * relativeDisplacement.y;
		//cameraPosition += lateralRotation * Vector3.back * (relativeDisplacement.z + cameraViewHeight);		// Move back
		//cameraPosition += lateralRotation * Vector3.right * relativeDisplacement.x;								// Move to side
		//cameraPosition += cameraViewHeight * Vector3.up * relativeDisplacement.y;								// Move up
		
		// LookAt Position
		cameraLookAtPosition = owner.arc.GetHitMarkerPosition();
		//cameraLookAtPosition += lateralRotation * Vector3.forward * cameraViewHeight * relativeDisplacement.z; // Look slightly ahead of player


		mainCam.transform.position = cameraPosition;
		//mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraPosition, Time.deltaTime * cameraSmoothDamping);
		mainCam.transform.rotation = Quaternion.Lerp (mainCam.transform.rotation, Quaternion.LookRotation(cameraLookAtPosition - mainCam.transform.position), Time.deltaTime * cameraSmoothDamping);
	}



	public Quaternion GetLateralRotation()
	{
		return Quaternion.Euler(0, rotation, 0);
	}

	public float GetLateralRotationRaw()
	{
		return rotation;
	}

	public float GetRange()
	{
		return range;
	}
}
