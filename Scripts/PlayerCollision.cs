using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
	// Debug
	private Vector3 startingPos;
	private bool measuring = false;
	private float range = 0;

	public void MeasureDistance(Vector3 pos, float targetRange)
	{
		startingPos = pos;
		measuring = true;
		range = targetRange;
	}

	private void OnCollisionEnter(Collision col)
	{
		if (measuring)
		{
			measuring = false;

			Vector3 endingPos = transform.position;

			// Measure distance, ignore height
			Vector3 distance = endingPos - startingPos;
			//Debug.Log ("Launch dist.: " + distance.magnitude);
			//Debug.Log ("Target Range: " + range);
		}
	}
}
