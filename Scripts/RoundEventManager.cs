using UnityEngine;
using System.Collections;
using System.Collections.Generic;




// Handles state changes within an actual round of the gameplay
// i.e. makes sure the player inputs and camera angles are acting correctly
public class RoundEventManager : StateMachine
{
	// Player Information
	//private PlayerData[] playerList;
	private List<PlayerData> playerList;
	private PlayerData currentActivePlayer;

	// Reference to Camera Manager
	public CameraManager cameraManager;

	// Player Arc Generator
	public PlayerArc arc;
	public float power;

	void Awake()
	{
		//cameraManager = GameManager.Instance.cameraManager;

		// Init Arc
		arc = new PlayerArc();

		// Init list
		playerList = new List<PlayerData>();

		// Cheat, add one player
		AddPlayer("Jebediah", GameObject.FindGameObjectWithTag("PlayerBody"));
		//playerList[0].gameObj = GameObject.FindGameObjectWithTag("PlayerBody");
		//Debug.Log (playerList[0].gameObj);

		// Start of Game
		Debug.Log ("Game Start!");

		// for now, default to player one going first
		SwitchActivePlayer(1);

		
		ChangeState<PlayerStateAiming>();
	}

	void Start()
	{

	}


	void AddPlayer(string name, GameObject player)
	{
		PlayerData newPlayer = new PlayerData(player);
		newPlayer.Init ();
		newPlayer.ID = playerList.Count + 1;
		newPlayer.name = name;

		playerList.Add(newPlayer);
		 
		Debug.Log("Player " + newPlayer.ID + " ('" + newPlayer.name + "') added");
	}


	void SwitchActivePlayer(PlayerData player)
	{
		// Assign variable
		currentActivePlayer = player;

		// Switch Camera
		//GameManager.Instance.cameraManager.SetTarget(currentActivePlayer.gameObj);

		Debug.Log("Current Player is now: " + currentActivePlayer.ID);
	}

	void SwitchActivePlayer(int playerID)
	{
		// Refer to overload
		SwitchActivePlayer(playerList[playerID-1]);
	}

	public GameObject GetActivePlayer()
	{
		return currentActivePlayer.gameObj;
	}

	public void NextPlayer()
	{
		// If one or no players
		if (playerList.Count < 2)
			return;

		// Move to next player in list
		if (currentActivePlayer.ID == playerList.Count)
			SwitchActivePlayer(1);
		else
			SwitchActivePlayer(currentActivePlayer.ID + 1);
	}

	public void SetPoints(List<Vector3> points)
	{
		arc.positionPoints = points;
	}

	public List<Vector3> GetPoints()
	{
		return arc.positionPoints;
	}
}