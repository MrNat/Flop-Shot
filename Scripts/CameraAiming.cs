using UnityEngine;
using System.Collections;

// Place camera behind player, look towards end of aim arc
public class CameraAiming : MonoBehaviour, CameraBehaviour
{
	// Reference to target body and aim script
	private Transform playerTransform;
	public PlayerStateAiming playerAimingScript;

	// Current camera properties
	private float cameraViewHeight = 0f;

	//private float cameraRotation = 0.0f;
	private Vector3 cameraLookAtPosition;
	private Vector3 cameraPosition;

	// Inspector Variables
	public Vector3 relativeDisplacement = new Vector3(0, 0, 0);
	public float cameraMinHeight = 0.0f;
	public float cameraMaxHeight = 10.0f;
	public float cameraSmoothDamping = 4.0f;


	void Update()
	{
		// If no target, return
		if (!playerTransform)
			return;

		CalculateCameraPosition();

		// Apply position and rotation to camera angle
		transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraSmoothDamping);
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation(cameraLookAtPosition - transform.position), Time.deltaTime * cameraSmoothDamping);

	}

	public void SetTarget(GameObject newTarget)
	{
		if (newTarget)
			playerTransform = newTarget.transform;
	}

	public string GetParentName()
	{
		return "CameraAngleAiming";
	}


	void CalculateCameraPosition()
	{

		// Clamp height of camera
		cameraViewHeight = Mathf.Lerp(cameraMinHeight, cameraMaxHeight, -playerAimingScript.GetVerticalRotation() / playerAimingScript.shotAngleVerticalMax);

		// Save rotation
		Quaternion lateralRotation = playerAimingScript.GetLateralRotation();
		//Vector3 lateralVector = playerAimingScript.GetShotLateralVector();


		// Camera Position
		cameraPosition = playerTransform.position;
		cameraPosition += lateralRotation * Vector3.back * (relativeDisplacement.z + cameraViewHeight);		// Move back
		cameraPosition += lateralRotation * Vector3.right * relativeDisplacement.x;								// Move to side
		cameraPosition += cameraViewHeight * Vector3.up * relativeDisplacement.y;								// Move up

		// LookAt Position
		cameraLookAtPosition = playerTransform.position;
		cameraLookAtPosition += lateralRotation * Vector3.forward * cameraViewHeight * relativeDisplacement.z; // Look slightly ahead of player
	}

}
