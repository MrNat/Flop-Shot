using UnityEngine;
using System.Collections;

public enum GameState
{
	NullState,
	Splash,
	MainMenu,
	Game
}

// Handles the states changes for the different parts of the game
// e.g. main menu, game play, transitions, etc.
public class GameManager : MonoBehaviour
{
	// Singleton Instance
	private static GameManager instance = null;
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				// if not already made, instantiate
				instance = Object.FindObjectOfType(typeof(GameManager)) as GameManager;

				if (instance == null)
				{
					GameObject gm = new GameObject("GameManager");
					DontDestroyOnLoad(gm);

					instance = gm.AddComponent<GameManager>();
				}
			}

			return instance;
		}
	}


	// Current Game State
	private GameState currentGameState;

	// Reference to Camera Manager
	public CameraManager cameraManager;

	// Reference to Round Manager
	public RoundEventManager roundManager;
	
	public void OnApplicationQuit()
	{
		instance = null;
	}

	public void CreateMarker(RaycastHit hit)
	{
		GameObject marker = Instantiate(Resources.Load ("Prefabs/HitMark"), hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
		marker.transform.Rotate(Vector3.right * 90);
		marker.transform.Translate(Vector3.up * 0.005f);
		
		Destroy(marker, 0.1f);
	}

	void Awake()
	{

	}
}
