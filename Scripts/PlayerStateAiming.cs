using UnityEngine;
using System.Collections;

public class PlayerStateAiming : RoundState
{
	// Inspector Variables
	public float userRotateSpeed = 0.75f;

	public float maxRange = 100.0f;
	public float minRange = 4f;

	// Aiming Variables
	private float range = 10.0f; // default
	private float rotation = 0.0f;

	private float camPosRatio = 0.0f;
	private float camRotation = 0.0f;

	// Height control
	private float heightRatio = 0.5f;
	private float heightSpeed = 0.2f;


	//private float cameraRotation = 0.0f;
	private Vector3 cameraLookAtPosition;
	private Vector3 cameraPosition;
	
	// Inspector Variables
	//public Vector3 relativeDisplacement = new Vector3(0, 2, 4);
	private float cameraFollowDistance = 6f;
	private float cameraFollowHeight = 4f;

	// Sound FX
	public AudioClip clickSound;
	private AudioSource clickSource;
	private float prevRotation = 0.0f;
	private float prevRange = 0.0f;


	protected override void Awake()
	{
		clickSound = Resources.Load("Sounds/HiHat1") as AudioClip;
		clickSource = Camera.main.GetComponent<AudioSource>();
		clickSource.clip = clickSound;
	}

	void Update()
	{
		// See if user is pressing buttons
		HandleRotationInput();

		// Temp, adjust cutoffRatio
		if (Input.GetKey ("m"))
			owner.arc.cutoffRatio += 0.1f;
		if (Input.GetKey ("n"))
			owner.arc.cutoffRatio -= 0.1f;
		if (Input.GetKeyUp ("b"))
			Debug.Log (owner.arc.cutoffRatio);

		owner.arc.cutoffRatio = Mathf.Clamp01 (owner.arc.cutoffRatio);

		// Update ARC
		//Debug.Log (horizontalRotation);
		owner.arc.GeneratePoints(GetRange(), heightRatio, player.transform.position, -rotation+90);
		owner.arc.DrawMarker(GetHitMarker(), rotation);

		
		// Camera
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

		ResetShot();

		// Subscribe to input
		EventInputBroadcaster.OnSubmitUpAction += ConfirmShot;

		// Sound
		prevRange = range;
		prevRange = rotation;
		
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

		range += Input.GetAxis("Vertical") * userRotateSpeed / 2;
		range = Mathf.Clamp(range, minRange, maxRange);

		heightRatio += Input.GetAxis("ArcHeight") * heightSpeed;
		if (Input.GetKey("x"))
			heightRatio += heightSpeed;
		if (Input.GetKey("z"))
			heightRatio -= heightSpeed;

		heightRatio = Mathf.Clamp01(heightRatio);

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


		// Play sound
		float rangeDiff = Mathf.Abs(range - prevRange);
		float rotDiff = Mathf.Abs(rotation - prevRotation);

		float dRa = 1.5f;
		float dRo = 2.5f;
		if ((rangeDiff > dRa) || (rotDiff > dRo))
		{
			clickSource.PlayOneShot(clickSound, 0.03f);

			prevRange = range;
			prevRotation = rotation;
		}
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
		
		// LookAt Position
		cameraLookAtPosition = owner.arc.GetHitMarkerPosition();


		owner.cameraManager.desiredPosition = cameraPosition;
		owner.cameraManager.lookAtPosition = cameraLookAtPosition;
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
