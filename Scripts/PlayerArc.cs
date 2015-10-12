using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerArc
{
	// Vector3 positions
	public List<Vector3> positionPoints;

	// Precision Properties
	public int numPoints = 70;

	private RaycastHit lastHit;
	private bool lastHitActive = false;


	private PlayerMeshGenerator meshGen = new PlayerMeshGenerator();


	public PlayerArc()
	{
		positionPoints = new List<Vector3>();
	}


	public void GeneratePoints(float curve, float range, float height, float hRatio, Vector3 position, float rotation)
	{
		List<Vector3> rocketPoints = GenerateRocketPath(curve, height, hRatio, range, position, rotation);
		positionPoints = rocketPoints;

		// Debug draw
		for (int i = 0; i < rocketPoints.Count-1; i++)
		{
			Debug.DrawLine(rocketPoints[i], rocketPoints[i+1], Color.green);
		}

		meshGen.GenerateMesh(positionPoints);
	}

	private List<Vector3> GenerateRocketPath(float curve, float height, float hRatio, float distance, Vector3 playerPosition, float rotation)
	{
		List<Vector3> points = new List<Vector3>();
		points.Add(playerPosition);


		// Timestep
		float t = 0;
		float step = (Mathf.PI) / numPoints;
		int currentPoint = 0;

		bool hitObject = false;
		while (hitObject == false)
		{
			// Step forward
			t += step;

			if (t > (Mathf.PI)) // *2
			{
				hitObject = true;
				break;
			}


			// Generate the next point
			Vector3 nextPoint = GenerateRocketPoint(curve, height, hRatio, distance, t, playerPosition, rotation);
			Vector3 prevPoint = points[currentPoint];

			// Calculate difference between current and next point
			Vector3 differenceVector = nextPoint - prevPoint;

			// Raycast
			RaycastHit hit;
			Ray collisionRay = new Ray(prevPoint, differenceVector);
			if (Physics.Raycast(collisionRay, out hit, differenceVector.magnitude))
			{
				// Add prefab hit marker
				//DrawHitMark(hit);
				//GameManager.Instance.CreateMarker(hit);

				lastHit = hit;
				lastHitActive = true;

				//Debug.Log ("Raycast hit!");
				hitObject = true;
				//return points;
				//break;
			}
			else
			{
				points.Add(nextPoint);

				
				// Step forwards
				currentPoint++;
				//Debug.Log (currentPoint);
			}

		}


		return points;
	}

	private Vector3 GenerateRocketPoint(float curve, float height, float hRatio, float distance, float t, Vector3 playerPosition, float rotation)
	{
		float radRotation = rotation * Mathf.Deg2Rad;

		float xCoord = (t) * (distance / 2.5f) * Mathf.Cos(radRotation); //curve * (t / (Mathf.PI * 2)) * Mathf.Cos(radRotation);
		float zCoord = (t) * (distance / 2.5f) * Mathf.Sin(radRotation);
		float yCoord = (-1.66f*Mathf.Pow(t, 4) + 8.71f*Mathf.Pow(t, 3) + -16.72f*Mathf.Pow (t, 2) + 13.29f*t + 0.03f) * (height / 3.5f);
		yCoord = Mathf.Lerp(0, yCoord, hRatio);

		Vector3 p = new Vector3(xCoord, yCoord, zCoord) + playerPosition;
		return p;
	}

	public void DrawMarker(GameObject marker, float rotation)
	{
		if (!lastHitActive)
			return;

		marker.transform.position = lastHit.point;
		marker.transform.rotation = Quaternion.LookRotation(lastHit.normal);
			
		marker.transform.Rotate(Vector3.right * 90);
		marker.transform.Rotate(Vector3.up * rotation);
		marker.transform.Translate(Vector3.up * 0.005f);
		
	}

	public float CalculateArcLength()
	{
		float total = 0;
		for (int i = 0; i < positionPoints.Count-1; i++)
		{
			Vector3 diff = positionPoints[i+1] - positionPoints[i];
			total += diff.magnitude;
		}

		return total;
	}

	public Vector3 GetHitMarkerPosition()
	{
		if (!lastHitActive)
			return new Vector3(0, 0, 0);

		return lastHit.point;
	}

	public List<Vector3> GetPoints()
	{
		return positionPoints;
	}
}
