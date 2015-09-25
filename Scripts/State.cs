using UnityEngine;
using System.Collections;

public abstract class State : MonoBehaviour
{
	protected GameObject player;
	protected GameObject hitMarker;

	public virtual void SetPlayer(GameObject playerObject)
	{
		player = playerObject;
	}

	public virtual void OnEnter()
	{
		AddListeners();
	}

	public virtual void OnExit()
	{
		RemoveListeners();
		Destroy(this);
	}

	protected virtual void OnDestroy()
	{
		RemoveListeners();
	}


	protected virtual void AddListeners()
	{

	}

	protected virtual void RemoveListeners()
	{

	}

	protected GameObject GetHitMarker()
	{
		if (hitMarker == null)
			hitMarker = Instantiate(Resources.Load ("Prefabs/HitMark")) as GameObject;

		return hitMarker;
	}
}
