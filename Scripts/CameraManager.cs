using UnityEngine;
using System.Collections;


// Switch between camera angles
public class CameraManager : MonoBehaviour
{
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

		// Update Shake
		UpdateCameraShake();



		if (Input.GetKeyUp("f"))
		{
			CreateShakeImpulse(0.25f, 0.3f);
		}
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
