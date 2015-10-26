using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcCurve
{
	// Public Properties
	public float distance = 100.0f; // in meters
	public float apex = 50.0f; // in meters

	public float rangeRatio = 1.0f;
	public float heightRatio = 1.0f;
	public float cutoffRatio = 0.65f;

	public float rotation = 0.0f;
	public Vector3 position; 

	public Vector3 finalPoint = new Vector3(0, 0, 0);



	public void SetProperties(float range, float hRatio, Vector3 playerPosition, float playerRotation)
	{
		distance = range;
		apex = Mathf.Pow(2, range / 20);

		heightRatio = hRatio;

		position = playerPosition;
		rotation = playerRotation;
	}


	public Vector3 Evaluate(float t)
	{
		float radRotation = rotation * Mathf.Deg2Rad;
		float xCoord = t * distance * Mathf.Cos(radRotation);
		float zCoord = t * distance * Mathf.Sin(radRotation);

		float yCoord = (-Mathf.Pow(t * 2 - 1, 2) + 1) * apex;
		yCoord = Mathf.Lerp(0, yCoord * 2, heightRatio);

		return new Vector3(xCoord, yCoord, zCoord) + position;
	}


	public List<Vector3> GenerateFullPath(float pointsPerDistance)
	{
		List<Vector3> points = new List<Vector3>();
		points.Add(position);

		bool calculating = true;
		int step = 1;

		while(calculating)
		{
			// Calculate t
			float arcLength = distance / 6;
			float t = (1/(arcLength * pointsPerDistance)) * step;

			// Make point
			Vector3 newPoint = Evaluate(t);
			Vector3 prevPoint = points[step-1];

			// Test sweep
			if (TestRaycast(prevPoint, newPoint))
			{
				points.Add(finalPoint);
				calculating = false;
			}
			else
			{
				points.Add(newPoint);
			}

			// Step
			step++;
			if ((step / (arcLength * pointsPerDistance)) > 1.2f)
				calculating = false;
		}

		return points;
	}

	private bool TestRaycast(Vector3 p1, Vector3 p2)
	{
		Vector3 difference = p2 - p1;
		RaycastHit hit;
		Ray collisionRay = new Ray(p1, difference);

		if (Physics.Raycast(collisionRay, out hit, difference.magnitude))
		{
			finalPoint = hit.point;
			return true;
		}

		return false;
	}

	public float ArcLength()
	{
		// Estimate with 10 points
		List<Vector3> points = GenerateFullPath(10);

		float total = 0;
		for (int i = 0; i < points.Count-1; i++)
		{
			Vector3 diff = points[i+1] - points[i];
			total += diff.magnitude;
		}

		return total;
	}
}
