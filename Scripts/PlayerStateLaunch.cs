using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateLaunch : RoundState
{
	
	// Inspector Variables
	public Vector3 relativePosition = new Vector3(1, -0.4f, -2);
	public Vector3 relativeLookAtPosition = new Vector3(1, 3, 1);
	
	public float smoothDamping = 5.0f;
	public int FOV = 100;
	
	
	// Positions
	private Vector3 desiredPosition;
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

		CalculateCameraPosition();
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
		playerBody = player.GetComponent<Rigidbody>();

		Debug.Log (positionPoints.Count);
		for (int i = 0; i < positionPoints.Count-1; i++)
		{
			//Debug.Log (positionPoints[i]);
			playerBody.MovePosition(positionPoints[i]);
			
			yield return null;
		}

		Vector3 diff = positionPoints[positionPoints.Count-1] - positionPoints[positionPoints.Count-2];
		Debug.Log (diff);
		playerBody.AddForce (diff * 4000);
	}


	void CalculateCameraPosition()
	{
		desiredPosition = player.transform.position;
		desiredPosition += lateralRotation * Vector3.forward * relativePosition.z;
		desiredPosition += lateralRotation * Vector3.right * relativePosition.x;
		desiredPosition += Vector3.up * relativePosition.y;
		
		lookAtPosition = player.transform.position;
		lookAtPosition += lateralRotation * Vector3.forward * relativeLookAtPosition.z;
		lookAtPosition += lateralRotation * Vector3.right * relativeLookAtPosition.x;
		lookAtPosition += Vector3.up * relativeLookAtPosition.y;
		
		// Apply position and rotation
		mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, desiredPosition, Time.deltaTime * smoothDamping);
		mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, Quaternion.LookRotation(lookAtPosition - mainCam.transform.position), Time.deltaTime * smoothDamping);
	}
}
