using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour
{
	public virtual State currentState
	{
		get { return state; }
		set { Transition(value); }
	}
	protected State state;
	protected bool inTransition;


	//protected GameObject currentPlayer;


	public virtual T GetState<T>()
		where T : State
	{
		T target = GetComponent<T>();

		if (target == null)
		{
			target = gameObject.AddComponent<T>();
		}

		return target;
	}


	public virtual void ChangeState<T>()
		where T : State
	{
		currentState = GetState<T>();
		Debug.Log ("state changed");
	}


	protected virtual void Transition(State value)
	{
		if (state == value || inTransition)
			return;


		inTransition = true;

		if (state != null)
			state.OnExit();

		state = value;

		if (state != null)
			state.OnEnter();

		inTransition = false;
	}
}
