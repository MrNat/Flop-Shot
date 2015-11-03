using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateLaunch : RoundState
{
	
	// Inspector Variables
	public Vector3 relPositionStart = new Vector3(-3f, -3.6f, -6f);
	public Vector3 relPositionEnd = new Vector3(-4.5f, 9f, 6f);
	public Vector3 relativeLookAtPosition = new Vector3(0, 0, 0); //new Vector3(1, 3, 1);

	private float cutoffRatio = 0.35f;
	private float startTime;
	private float totalTime;
	private Vector3 origPos;

	//private float startTime = 0.0f;
	//private float endTime = 0.0f;
	//private float speed = 15f; // m/s


	
	// Positions
	private Vector3 lookAtPosition;


	private Quaternion lateralRotation;
	public Camera mainCam;

	private Rigidbody playerBody;


	private bool updating = false;


	// Debug Stats
	private PlayerCollision playerCol;

	// Launch Velocity
	private float launchForce;
	private Vector3 launchVector;


	public override void OnEnter()
	{
		Debug.Log("Entering Launch State");

		mainCam = Camera.main;

		playerCol = player.GetComponent<PlayerCollision>();
		
		playerBody = player.GetComponent<Rigidbody>();

		base.OnEnter();

		// Begin Following
		CalculateLaunchVector();
		StartParticles();
		StartLaunching();
	}

	void CalculateLaunchVector()
	{
		float range = owner.arc.distance;
		float g = 9.81f;//Mathf.Abs(Physics.gravity.y);

		
		Vector3 testPointA = owner.arc.Evaluate(0.0f);
		Vector3 testPointB = owner.arc.Evaluate(0.01f);

		float dY = testPointA.y - testPointB.y;
		float dX = Vector3.Distance(new Vector3(testPointB.x, 0, testPointB.z), new Vector3(testPointA.x, 0, testPointA.z));
		float angle = Mathf.Abs(Mathf.Atan2(dY, dX) * Mathf.Rad2Deg);

		float radAngle = angle * Mathf.Deg2Rad;

		launchForce = (Mathf.Sqrt(range) * Mathf.Sqrt(g)) / Mathf.Sqrt(Mathf.Sin(2 * radAngle));
		//Debug.Log ("Num: " + (Mathf.Sqrt(range) * Mathf.Sqrt(g)));
		//Debug.Log ("Den: " + Mathf.Sqrt(Mathf.Sin(2 * radAngle)));

		//float lat = launchForce * Mathf.Cos(angle);
		//float rot = owner.arc.rotation * Mathf.Deg2Rad;
		Vector3 diff = testPointB - testPointA;
		launchVector = diff.normalized * launchForce;

		//Debug.Log ("Launch angle: " + angle);
		//Debug.Log ("Launch force: " + launchForce);
		totalTime = owner.arc.FlightTime(launchVector);
		startTime = Time.time;
	}

	void StartParticles()
	{

		
	}

	void StartLaunching()
	{
		// Set variables
		//startTime = Time.time;
		//endTime = startTime + (owner.arc.ArcLength() / speed);

		// Disable player gravity
		
		origPos = playerBody.position;

		/*
		Vector3 testPointA = owner.arc.Evaluate(0.0f);
		Vector3 testPointB = owner.arc.Evaluate(0.01f);

		Vector3 diff = testPointB - testPointA;
		//Vector3 mag = owner.arc.Evaluate(0.5f) - owner.arc.Evaluate(0.0f);
		Vector3 force = diff.normalized * owner.arc.distance * 222;
		*/
		playerBody.AddForce(launchVector * playerBody.mass * 50);

		// Debug
		//playerCol.MeasureHit(playerBody.position, owner.arc.distance, totalTime);
	}

	void Update()
	{
		if ((playerBody.velocity.magnitude <= 0.05f) && updating)
		{
			Invoke ("ResetState", 3.5f);
			updating = false;
		}

	}

	void FixedUpdate()
	{
		
		CalculateCameraPosition();
	}

	void ResetState()
	{
		owner.ChangeState<PlayerStateAiming> ();
	}

	public void SavePlayerRotation(Quaternion rotation)
	{
		lateralRotation = rotation;
	}


	void CalculateCameraPosition()
	{
		Vector3 camPos;
		Vector3 lookAt;

		// Is the ball past the cutoff?
		if (((Time.time - startTime) / totalTime) < cutoffRatio)
		{
			// Before cutoff
			// stay behind the ball

			camPos = RelativePosition(origPos, new Vector3(6, 4, 0), owner.arc.rotation);
			lookAt = playerBody.position;
		}
		else
		{
			// be near goal

			camPos = RelativePosition(GameObject.FindGameObjectWithTag("Finish").transform.position, new Vector3(9, 6, -5), 0);
			lookAt = playerBody.position;
		}

		owner.cameraManager.desiredPosition = camPos;
		owner.cameraManager.lookAtPosition = lookAt;
	}

	Vector3 RelativePosition(Vector3 position, Vector3 relative, float rotation)
	{
		// Calc quaternion rotation
		Quaternion latRot = Quaternion.Euler(0, rotation, 0);
		
		Vector3 newPos = position;
		newPos += latRot * Vector3.forward * relative.z;
		newPos += latRot * Vector3.right * relative.x;
		newPos += Vector3.up * relative.y;
		
		return newPos;
	}
}
