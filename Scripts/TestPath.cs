using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestPath : MonoBehaviour
{
	// Points
	private List<Vector3> points;
	private bool gizmosOn = false;

	[Range(0.1f, 75.0f)]
	public float distance = 40f;

	[Range(0.1f, 10.0f)]
	public float height = 3.5f;

	[Range(1, 200)]
	public int numPoints = 15;

	[Range(-6.0f, 6.0f)]
	public float curve = 0.0f;

	//private int currentStep = 0;

	void Awake()
	{
		points = new List<Vector3>();
		gizmosOn = true;


	}

	void Update()
	{
		GeneratePoints();

		if (Input.GetKeyUp("t"))
		{
			StartCoroutine("FollowPoints");
		}
	}

	void GeneratePoints()
	{
		// Reset Points
		points.Clear();

		//float b = Mathf.PI;

		// Calculate
		for (float t = 0; t < Mathf.PI*2; t += (Mathf.PI*2 / numPoints))
		//for (float t = -b; t < b; t += (b * 2) / numPoints)
		//for (float t = 0; t < 2.5f; t += (2.5f / numPoints))
		{
			// Coordinate Values
			float xCoord = curve * (t / (Mathf.PI * 2));
			float zCoord = (t / 6) * distance;
			float yCoord = (Mathf.Sin (t - (Mathf.PI / 2))+1) * (height / 2);

			//float zCoord = ((t / (Mathf.PI * 2)) + 0.5f) * distance;
			//float yCoord = ((-t * t) + 10) * (height / 10);

			//float zCoord = (t) * (distance / 2.5f);
			//float yCoord = (-1.66f*Mathf.Pow(t, 4) + 8.71f*Mathf.Pow(t, 3) + -16.72f*Mathf.Pow (t, 2) + 13.29f*t + 0.03f) * (height / 3.5f);

			Vector3 p = new Vector3(xCoord, yCoord, zCoord);
			//p += transform.position;
			points.Add(p);
		}


		//Debug.Log (points[0]);
	}

	IEnumerator FollowPoints()
	{
		Debug.Log ("Started follow points");
		for (int i = 0; i < points.Count; i++)
		{
			GetComponent<Rigidbody>().MovePosition(points[i]);
			
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		if (!gizmosOn)
			return;

		Gizmos.color = Color.cyan;

		foreach (Vector3 p in points)
		{
			Gizmos.DrawSphere(p, 0.1f);
		}
	}
}
