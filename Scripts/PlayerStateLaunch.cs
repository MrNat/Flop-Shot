using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateLaunch : RoundState
{
	
	// Inspector Variables
	public Vector3 relPositionStart = new Vector3(-3f, -3.6f, -6f);
	public Vector3 relPositionEnd = new Vector3(-4.5f, 9f, 6f);
	public Vector3 relativeLookAtPosition = new Vector3(0, 0, 0); //new Vector3(1, 3, 1);
	
	public float smoothDamping = 5.0f;
	public int FOV = 100;

	public float timeToEnd = 3.0f;
	private float startTime = 0.0f;
	private float timeRatio = 0.0f;
	private float speed = 15f; // m/s


	
	// Positions
	private Vector3 lookAtPosition;


	private Quaternion lateralRotation;
	public Camera mainCam;

	private List<Vector3> positionPoints;
	private Rigidbody playerBody;


	private bool tracking = true;
	private bool updating = false;

	public override void OnEnter()
	{
		Debug.Log("Entering Launch State");

		mainCam = Camera.main;

		positionPoints = owner.GetPoints();
		base.OnEnter();

		StartCoroutine("FollowPoints");
	}

	void Update()
	{
		
		if (!tracking)
			CalculateCameraPosition();

		if ((playerBody.velocity.magnitude <= 0.05f) && updating)
		{
			Invoke ("ResetState", 3.5f);
			updating = false;
		}

	}

	void ResetState()
	{
		owner.ChangeState<PlayerStateAiming> ();
	}

	public void SavePlayerRotation(Quaternion rotation)
	{
		lateralRotation = rotation;
	}

	IEnumerator FollowPoints()
	{
		// Disable gravity while following points
		playerBody = player.GetComponent<Rigidbody>();
		playerBody.useGravity = false;
		tracking = true;

		// Add torque (temp)
		playerBody.AddTorque(new Vector3(20000, 0, 0));

		// Start following points
		startTime = 0.0f;
		timeToEnd = owner.arc.CalculateArcLength() / speed;
		timeRatio = 0.0f;


		bool inRange = true;

		Debug.Log ("Points: " + positionPoints.Count);
		Debug.Log ("Duration: " + timeToEnd);

		int endPoint = 0;

		while ((startTime <= timeToEnd) && inRange)
		{
			startTime += Time.deltaTime;
			timeRatio = startTime / timeToEnd;

			float ratioBetweenPoints = positionPoints.Count * timeRatio;
			int startPositionPoint = Mathf.FloorToInt(ratioBetweenPoints);
			int nextPositionPoint = startPositionPoint + 1;

			if (nextPositionPoint >= positionPoints.Count-1)
			{
				inRange = false;
				yield return null;
			}

			// Find points
			Vector3 pointA = positionPoints[startPositionPoint];
			Vector3 pointB = positionPoints[nextPositionPoint];

			// Sweep test
			RaycastHit hit;
			if (playerBody.SweepTest(pointB - pointA, out hit, (pointB - pointA).magnitude * 2))
			{
				Debug.Log ("Hit");
				endPoint = startPositionPoint;
				break;
			}
			else
			{
				playerBody.MovePosition(Vector3.Lerp(pointA, pointB, ratioBetweenPoints - startPositionPoint));
			}

			// Cutoff Test
			if (timeRatio >= owner.arc.cutoffRatio)
			{
				endPoint = startPositionPoint;
				break;
			}
			
			CalculateCameraPosition();

			endPoint = startPositionPoint;
			yield return null;
		}

		// Once at the end of the trail
		Vector3 diff = (positionPoints [endPoint] - positionPoints [endPoint-1]) * 12500;
		Debug.Log (diff);
		playerBody.AddForce (diff);

		// Re-enable gravity
		playerBody.useGravity = true;
		tracking = false;
		updating = true;
	}


	void CalculateCameraPosition()
	{
		Vector3 startPos = RelativePosition(player.transform.position, relPositionStart);
		Vector3 endPos = RelativePosition(player.transform.position, relPositionEnd);

		lookAtPosition = player.transform.position;


		// Apply position and rotation
		owner.cameraManager.desiredPosition = Vector3.Lerp(startPos, endPos, timeRatio);
		owner.cameraManager.lookAtPosition = lookAtPosition;
	}

	Vector3 RelativePosition(Vector3 position, Vector3 relative)
	{
		Vector3 newPos = position;
		newPos += lateralRotation * Vector3.forward * relative.z;
		newPos += lateralRotation * Vector3.right * relative.x;
		newPos += Vector3.up * relative.y;

		return newPos;
	}
}
