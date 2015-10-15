using UnityEngine;
using System.Collections;


// Switch between camera angles
public class CameraManager : MonoBehaviour
{
	// Movement
	public Vector3 desiredPosition = new Vector3(0, 0, 0);
	public Vector3 lookAtPosition = new Vector3(0, 0, 0);
	public float moveSpeed = 3.5f;
	public float rotateSpeed = 0.5f;

	// Camera Shake
	public float shakeAmplitude;

	private float currentShakeAmount;
	private float originalShakeAmount;


	void Awake()
	{

	}

	void LateUpdate()
	{
		// Lerp to desired position
		//UpdateTransform();
		
		transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSpeed * Time.smoothDeltaTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtPosition - transform.position), moveSpeed * Time.smoothDeltaTime);

		// Update Shake
		UpdateCameraShake();
	}

	// MOVEMENT
	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	public void UpdateTransform()
	{
	}


	// CAMERA SHAKE (might move to separate class later)
	public void CreateShakeImpulse(float duration, float amplitude)
	{
		// Assign variables
		shakeAmplitude = amplitude;

		// Add shake to current counter
		originalShakeAmount = duration;
		currentShakeAmount = originalShakeAmount;
	}

	void UpdateCameraShake()
	{
		if (currentShakeAmount > 0)
		{
			transform.position += Random.insideUnitSphere * (shakeAmplitude * (currentShakeAmount / originalShakeAmount));
			currentShakeAmount -= Time.deltaTime;
		}
		else
		{
			currentShakeAmount = 0f;
			//transform.localPosition = originalCameraTransform;
		}
	}
}
