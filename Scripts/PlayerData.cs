using UnityEngine;
using System.Collections;


// Used for storing player data,
// not a mono behaviour subclass
public class PlayerData
{
	// ID, player number, name, etc.
	public int ID;
	public string name;

	// Scoring
	public int totalScore;
	public int[] roundScores;

	// State
	//public PlayerState currentState;
	private MonoBehaviour currentBehaviour;

	// Reference to GameObject
	public GameObject gameObj;


	// Reference to Player Scripts
	//public PlayerAiming scriptAiming;

	public PlayerData(GameObject player)
	{
		gameObj = player;
	}

	public void Init()
	{
		//scriptAiming = GameObject.FindGameObjectWithTag("PlayerBody").GetComponent<PlayerAiming>();
	}

}
