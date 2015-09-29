using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateLaunch : RoundState
{
	
	// Inspector Variables
	public Vector3 relPositionStart = new Vector3(-1f, -1.2f, -2f);
	public Vector3 relPositionEnd = new Vector3(-1.5f, 3f, 2);
	public Vector3 relativeLookAtPosition = new Vector3(0, 0, 0); //new Vector3(1, 3, 1);
	
	public float smoothDamping = 5.0f;
	public int FOV = 100;

	public float timeToEnd = 3.0f;
	private float startTime = 0.0f;
	private float timeRatio = 0.0f;

	
	// Positions
	private Vector3 lookAtPosition;


	private Quaternion lateralRotation;
	public Camera mainCam;

	private List<Vector3> positionPoints;
	private Rigidbody playerBody;

	public override void OnEnter()
	{
		mainCam = Camera.main;

		positionPoints = owner.GetPoints();
		base.OnEnter();
	}

	public void SavePlayerRotation(Quaternion rotation)
	{
		lateralRotation = rotation;
	}

	void Update()
	{
		if (Input.GetKeyUp("a"))
			owner.ChangeState<PlayerStateAiming>();

		if (Input.GetKeyUp("t"))
		{
			StartCoroutine("FollowPoints");
		}

		//CalculateCameraPosition();
	}

	/*
	public override void Reason(GameObject player, PlayerStateManager system)
	{
		if (Input.GetKeyUp("space"))
		{
			//system.SetTransition(Transition.NullTransition);
		}
	}
	*/
	/*
	public override void Act(GameObject player)
	{
		CalculateCameraPosition(player);
	}


	public override void DoBeforeEntering()
	{

	}
	*/

	IEnumerator FollowPoints()
	{
		startTime = 0.0f;
		timeToEnd = positionPoints.Count * 0.06f; // 60ms per segment
		timeRatio = 0.0f;

		playerBody = player.GetComponent<Rigidbody>();

		bool inRange = true;

		Debug.Log ("Points: " + positionPoints.Count);
		Debug.Log ("Duration: " + timeToEnd);

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

			//Debug.Log (positionPoints[i]);
			playerBody.MovePosition(Vector3.Lerp(positionPoints[startPositionPoint], positionPoints[nextPositionPoint], ratioBetweenPoints - startPositionPoint));
			
			CalculateCameraPosition();

			yield return null;
		}

		// Once at the end of the trail
		Vector3 diff = positionPoints[positionPoints.Count-1] - positionPoints[positionPoints.Count-2];
		Debug.Log (diff);
		playerBody.AddForce (diff * 4000);
	}


	void CalculateCameraPosition()
	{
		/*
		desiredPosition = player.transform.position;
		desiredPosition += lateralRotation * Vector3.forward * relativePosition.z;
		desiredPosition += lateralRotation * Vector3.right * relativePosition.x;
		desiredPosition += Vector3.up * relativePosition.y;
		*/

		Vector3 startPos = RelativePosition(player.transform.position, relPositionStart);
		Vector3 endPos = RelativePosition(player.transform.position, relPositionEnd);

		lookAtPosition = player.transform.position;
		/*
		lookAtPosition += lateralRotation * Vector3.forward * relativeLookAtPosition.z;
		lookAtPosition += lateralRotation * Vector3.right * relativeLookAtPosition.x;
		lookAtPosition += Vector3.up * relativeLookAtPosition.y;
		*/

		// Apply position and rotation
		mainCam.transform.position = Vector3.Lerp(startPos, endPos, timeRatio);
		//mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, desiredPosition, Time.deltaTime * smoothDamping);
		mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, Quaternion.LookRotation(lookAtPosition - mainCam.transform.position), Time.deltaTime * smoothDamping);
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
