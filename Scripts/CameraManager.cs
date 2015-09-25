using UnityEngine;
using System.Collections;


public enum CameraAngle
{
	NullAngle,
	AimingAngle,
	LaunchAngle,
	FlightAngle
}

public interface CameraBehaviour
{
	void SetTarget(GameObject newTarget);
	string GetParentName();
}


// Switch between camera angles
public class CameraManager : MonoBehaviour
{
	// Inspector References to Camera Angles
	public CameraAiming angleAiming;
	public CameraLaunch angleLaunch;

	// Current camera angle
	private CameraBehaviour currentCameraAngle;
	private GameObject currentCameraObject;

	// Current Camera Target
	private GameObject currentCameraTarget = null;
	
	// Camera Shake
	public float shakeAmplitude;

	private float currentShakeAmount;
	private float originalShakeAmount;


	void Awake()
	{
		// Default to aiming angle
		//ChangeAngle(CameraAngle.AimingAngle);
	}

	void LateUpdate()
	{
		if (!currentCameraObject)
			return;

		// Update camera to snap to current angle
		transform.position = currentCameraObject.transform.position;
		transform.rotation = currentCameraObject.transform.rotation;


		// Update Shake
		UpdateCameraShake();



		if (Input.GetKeyUp("f"))
		{
			CreateShakeImpulse(0.25f, 0.3f);
		}
	}

	public void ChangeAngle(CameraAngle newAngle)
	{

		switch (newAngle)
		{

		case CameraAngle.NullAngle:
			currentCameraAngle = null;
			break;

		case CameraAngle.AimingAngle:
			currentCameraAngle = angleAiming;
			currentCameraObject = GameObject.FindGameObjectWithTag(angleAiming.GetParentName());
			Camera.main.fieldOfView = 60;
			break;

		case CameraAngle.LaunchAngle:
			currentCameraAngle = angleLaunch;
			currentCameraObject = GameObject.FindGameObjectWithTag(angleLaunch.GetParentName());
			Camera.main.fieldOfView = 100;
			break;

		}

		// Assign camera angle object
		//currentCameraObject = GameObject.FindGameObjectWithTag(currentCameraAngle.GetParentName());

		// Set Target
		if (currentCameraTarget)
			SetTarget(currentCameraTarget);
	}


	public void SetTarget(GameObject newTarget)
	{
		currentCameraTarget = newTarget;
		//Debug.Log (currentCameraTarget);

		// Update whichever script is currently active
		if (currentCameraAngle != null)
			currentCameraAngle.SetTarget(currentCameraTarget);
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
