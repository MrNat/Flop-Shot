using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatePower : RoundState
{
	private float power = 0f;
	private float powerRate = 40.0f; // units/sec
	private float powerStartTime;
	private bool poweringUp = false;

	private Image powerBar;

	public override void OnEnter()
	{
		Debug.Log ("Entering Power State");
		powerBar = GameObject.FindGameObjectWithTag("PowerBar").GetComponent<Image>();

		// Subscribe to Input Events
		EventInputBroadcaster.OnSubmitDownAction += StartPoweringUp;
		EventInputBroadcaster.OnSubmitUpAction += StopPoweringUp;


		base.OnEnter();
	}

	public override void OnExit()
	{
		// Unsubscribe to Input Events
		EventInputBroadcaster.OnSubmitDownAction -= StartPoweringUp;
		EventInputBroadcaster.OnSubmitUpAction -= StopPoweringUp;
	}


	void StartPoweringUp()
	{
		Debug.Log ("Powering up!");

		power = 0;
		powerStartTime = Time.time;
		poweringUp = true;
	}

	void StopPoweringUp()
	{
		float timeElapsed = Time.time - powerStartTime;
		power = Mathf.Clamp(timeElapsed * powerRate, 0, 100);
		poweringUp = false;

		// Change state
		owner.power = power;
		owner.ChangeState<PlayerStateLaunch>();
	}

	void Update()
	{
		if (!poweringUp)
			return;

		float timeElapsed = Time.time - powerStartTime;
		powerBar.fillAmount = Mathf.Clamp(timeElapsed * powerRate, 0, 100) / 100;
	}

	/*
	void Update()
	{
		// Input
		if (!poweringUp)
		{
			if (Input.GetKeyDown("space"))
			{
				power = 0;
				poweringUp = true;
			}
		}
		else
		{
			if (Input.GetKey("space"))
			{
				power += powerRate;
			}
			if (Input.GetKeyUp("space"))
			{
				power = Mathf.Clamp(power, 0, 100);

				Debug.Log ("POW: " + power);
				poweringUp = false;

				// Change State
				owner.power = power;
				owner.ChangeState<PlayerStateLaunch>();
			}
		}


		// Render bar
		powerBar.fillAmount = power / 100;
	}
	*/
}
