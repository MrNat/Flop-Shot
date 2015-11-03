using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	//private float cameraFollowDistance = 6f;
	//private float cameraFollowHeight = 4f;

	// Sound FX
	public AudioClip clickSound;
	private AudioSource clickSource;
	private float prevRotation = 0.0f;
	private float prevRange = 0.0f;

	private Vector3[] points;


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
		//owner.arc.SetProperties(50, 0.5f, player.transform.position, -rotation + 90);
		owner.arc.SetProperties(range, heightRatio, player.transform.position, -rotation + 90);
		LineRenderer line = player.GetComponent<LineRenderer>();

		points = owner.arc.GenerateFullPath(0.17f);
		line.SetVertexCount(owner.arc.finalPointIndex);

		for (int i = 0; i < owner.arc.finalPointIndex; i++)
		{
			line.SetPosition(i, points[i]);
		}
		//Debug.Log (horizontalRotation);
		//owner.arc.GeneratePoints(GetRange(), heightRatio, player.transform.position, -rotation+90);
		//owner.arc.DrawMarker(GetHitMarker(), rotation);
		//owner.arc.GenerateFullPath();

		
		// Camera
		CalculateCameraPosition(player);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		for (int i = 0; i < owner.arc.finalPointIndex; i++)
		{
			Gizmos.DrawSphere(points[i], 1.0f);
		}
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

		camPosRatio = Mathf.Clamp(camPosRatio, 0.05f, 0.75f);


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
		// Find arc vector
		Vector3 diff = owner.arc.Evaluate(camPosRatio) - owner.arc.Evaluate(camPosRatio-0.05f);
		Vector3 rot = Quaternion.Euler(0, rotation, camRotation) * Vector3.right;
		Vector3 camPos = Vector3.Cross(diff, rot).normalized * 2;

		Debug.DrawLine(owner.arc.Evaluate(camPosRatio), owner.arc.Evaluate(camPosRatio)-diff*10, Color.cyan);
		Debug.DrawLine(owner.arc.Evaluate(camPosRatio), owner.arc.Evaluate(camPosRatio)+rot*10, Color.red);
		Debug.DrawLine(owner.arc.Evaluate(camPosRatio), owner.arc.Evaluate(camPosRatio)+camPos*5, Color.green);


		//cameraPosition = player.transform.position;
		cameraPosition = camPos*6 + owner.arc.Evaluate(camPosRatio);
		//cameraPosition += Vector3.up * cameraFollowHeight;
		//cameraPosition += Quaternion.Euler(0, camRotation + rotation, 0) * Vector3.back * cameraFollowDistance;
		
		// LookAt Position
		cameraLookAtPosition = owner.arc.finalPoint;


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
