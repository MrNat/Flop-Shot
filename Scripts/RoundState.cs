using UnityEngine;
using System.Collections;

public class RoundState : State
{
	protected RoundEventManager owner;
	//public CameraManager cameraManager { get { return owner.cameraManager; } }

	protected virtual void Awake()
	{
		owner = GetComponent<RoundEventManager>();
		player = owner.GetActivePlayer();
	}

	protected override void AddListeners()
	{

	}

	
	protected override void RemoveListeners()
	{
		
	}



	protected virtual void OnComplete(object sender)
	{

	}
}
