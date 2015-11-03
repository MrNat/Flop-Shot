using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
	// Debug
	private Vector3 startingPos;
	private bool measuring = false;
	private float range;
	private float time;
	private float startTime;

	// Reference to goal
	private Collider goalCollider;

	void Awake()
	{
		goalCollider = GameObject.FindGameObjectWithTag("Finish").GetComponent<Collider>();
	}

	public void MeasureHit(Vector3 pos, float targetRange, float targetTime)
	{
		startingPos = pos;
		measuring = true;
		range = targetRange;
		time = targetTime;
		startTime = Time.time;
	}

	void FixedUpdate()
	{
		float rate = Mathf.Clamp(GetComponent<Rigidbody>().velocity.magnitude / 2, 0, 200);
		ParticleSystem rocketTrail = GameObject.Find ("RocketTrail").GetComponent<ParticleSystem>();
		rocketTrail.emissionRate = rate;
	}

	private void OnCollisionEnter(Collision col)
	{

		if (measuring)
		{
			measuring = false;

			Debug.Log ("--------------------");

			Vector3 endingPos = transform.position;

			// Measure distance, ignore height
			Vector3 distance = endingPos - startingPos;
			Debug.Log ("Distance: " + distance.magnitude + "\tTarget Range: " + range);
			Debug.Log ("Flight Time: " + (Time.time - startTime) + "\tTarget Time: " + time);
		}


		if (col.collider.tag == "Terrain" && Mathf.Abs(GetComponent<Rigidbody>().velocity.y) > 3.5f)
		{
			ParticleSystem clouds = GameObject.Find ("Cloud-Burst").GetComponent<ParticleSystem>();
			clouds.transform.position = transform.position - new Vector3(0, 1.2f, 0);
			clouds.Play();
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.Equals(goalCollider))
		{
			Debug.Log ("We did it!");

			/*
			Time.timeScale = 0.5f;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
			*/
			ParticleSystem confetti = GameObject.Find ("Goal Confetti").GetComponent<ParticleSystem>();
			confetti.Play();
		}

	}
}
