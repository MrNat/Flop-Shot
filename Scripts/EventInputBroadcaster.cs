using UnityEngine;
using System.Collections;

public class EventInputBroadcaster : MonoBehaviour
{
	public delegate void EventInputAction();

	public static event EventInputAction OnSubmitUpAction;
	public static event EventInputAction OnSubmitDownAction;

	void Update()
	{
		if (Input.GetButtonUp("Submit"))
			if (OnSubmitUpAction != null)
				OnSubmitUpAction();

		if (Input.GetButtonDown("Submit"))
			if (OnSubmitDownAction != null)
				OnSubmitDownAction();

	}
}
