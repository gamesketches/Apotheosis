using UnityEngine;
using System.Collections;

public class BulletTestSceneManager : MonoBehaviour {

	public Character character;
	GameObject player1, player2;
	PlayerInitializer playerFactory;
	public Vector3 player1Pos;
	BulletDepot bullets;
	// Use this for initialization
	void Start () {
		bullets = ScriptableObject.CreateInstance<BulletDepot>();
		bullets.Load();

		playerFactory = GetComponent<PlayerInitializer>();
    	playerFactory.bullets = bullets;
		player1 = playerFactory.CreatePlayer(CreateControlScheme(0), character, player1Pos, 0);
		playerFactory.CreatePlayer(CreateControlScheme(1), Character.Orpheus, new Vector3(10, 0, 0), 1);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)) { 
			bullets.Load();
		}
	}

	string[] CreateControlScheme(int playerNum) {
		string[] controlArray = new string[6];
		controlArray[0] = string.Concat("Horizontal", playerNum.ToString());
		controlArray[1] = string.Concat("Vertical", playerNum.ToString());
		controlArray[2] = string.Concat("ButtonA", playerNum.ToString());
		controlArray[3] = string.Concat("ButtonB", playerNum.ToString());
		controlArray[4] = string.Concat("ButtonC", playerNum.ToString());
		controlArray[5] = string.Concat("ButtonD", playerNum.ToString());
		return controlArray;
	}
}
