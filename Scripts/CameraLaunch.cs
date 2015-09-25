using UnityEngine;
using System.Collections;

public class CameraLaunch : MonoBehaviour, CameraBehaviour
{

	// Reference to player 
	private Transform playerTransform;
	private PlayerAiming playerAimingScript;


	// Inspector Variables
	public Vector3 relativePosition = new Vector3(0, 0, 0);
	public Vector3 relativeLookAtPosition = new Vector3(0, 0, 0);

	public float smoothDamping = 5.0f;
	public int FOV = 100;


	// Positions
	private Vector3 desiredPosition;
	private Vector3 lookAtPosition;


	void Update()
	{
		if (!playerTransform)
			return;

		// Calculate Positions
		Quaternion lateralRotation = playerAimingScript.GetLateralRotation();

		desiredPosition = playerTransform.position;
		desiredPosition += lateralRotation * Vector3.forward * relativePosition.z;
		desiredPosition += lateralRotation * Vector3.right * relativePosition.x;
		desiredPosition += Vector3.up * relativePosition.y;

		lookAtPosition = playerTransform.position;
		lookAtPosition += lateralRotation * Vector3.forward * relativeLookAtPosition.z;
		lookAtPosition += lateralRotation * Vector3.right * relativeLookAtPosition.x;
		lookAtPosition += Vector3.up * relativeLookAtPosition.y;

		// Apply position and rotation
		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothDamping);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtPosition - transform.position), Time.deltaTime * smoothDamping);

	}


	public void SetTarget(GameObject newTarget)
	{
		if (newTarget)
		{
			playerTransform = newTarget.transform;
			playerAimingScript = newTarget.GetComponent<PlayerAiming>();
		}
	}

	public string GetParentName()
	{
		return "CameraAngleLaunch";
	}
}
