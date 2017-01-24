using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InControl;

public class TrainingModeManager : MonoBehaviour {

	delegate void UpdateFunction();
	UpdateFunction currentUpdateFunction;
	#region Battle Variables
	GameObject player1, player2;
	GameObject player1Reticle;
	GameObject player2Reticle;
	int player1RoundWins, player1Wins, player2RoundWins, player2Wins;
    public float roundTime;
    private float currentRoundTime;
	private bool timerStarted;
	public Vector3 player1Pos, player2Pos;
	public Vector3 powerUpZonePosition;
	string[] player1Controls, player2Controls;
	PlayerInitializer playerFactory;
	PlayerStats player1Stats, player2Stats;
	BulletDepot bullets;
	GameObject p1LifeBar, p2LifeBar, p1BufferBar, p2BufferBar;
	GameObject UIElements;
    private int roundsToWin;
    public float screenShake;
    InputDevice player1Controller, player2Controller;
    #endregion

    #region CharacterSelect Vars
    CharacterSelectManager characterSelectManager;
	#endregion
	SpriteRenderer background;
    Text pressStart;
    Text roundTimer;
    Text victoryText;

    MenuController menuController;

    SpriteRenderer[] HorusWinsIconsSR;
    SpriteRenderer[] SetWinsIconsSR;

    private bool debug_on;

    // Use this for initialization
    void Start () {
        p1LifeBar = GameObject.Find("P1LifeBar");
		p1BufferBar = GameObject.Find("P1BufferBarSegments");
        p2LifeBar = GameObject.Find("P2LifeBar");
		p2BufferBar = GameObject.Find("P2BufferBarSegments");
		UIElements = GameObject.Find("InGameUIElements");
        ToggleUI(false);

		victoryText = GameObject.FindGameObjectWithTag("VictoryText").GetComponent<Text>();

		//bullets = new BulletDepot(); // clearing a warning w/next line - ski
        bullets = ScriptableObject.CreateInstance<BulletDepot>();
		bullets.Load();

		playerFactory = GetComponent<PlayerInitializer>();
    	playerFactory.bullets = bullets;
        player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		background = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();

        characterSelectManager = GetComponent<CharacterSelectManager>();
        int numCharacters = System.Enum.GetNames(typeof(Character)).Length;

        AnalyticsEngine.Initialize(new string[] {"LoholtBulletsFired", "OrpheusBulletsFired", "HirukoBulletsFired"});
		characterSelectManager.Reset();
        currentUpdateFunction = CharacterSelect;
        menuController = GameObject.Find("Canvas").GetComponent<MenuController>();
		player1Controller = InputManager.ActiveDevice;
    	foreach(InputDevice controller in InputManager.Devices) {
    		if(controller != InputManager.ActiveDevice) {
    			player2Controller = controller;
    		}
    	}
        //menuController.Toggle();
    }

    #region CharacterSelect
    void CharacterSelect() {
    	characterSelectManager.TrainingModeCharacterSelectUpdate();
    	if(characterSelectManager.charactersSelected) {
            InitializeGameSettings();
    	}
    }

    #endregion

    #region Initialization Code

	void InitializeGameSettings() {
		player1RoundWins = 0;
		player2RoundWins = 0;
        //bullets = new BulletDepot(); //clearing warning with next line - ski
        bullets = ScriptableObject.CreateInstance<BulletDepot>();

        bullets.Load();
		StartRound();
	}

  

	void CreateBars() {

		ToggleUI(true);
        player1Stats = player1.GetComponent<PlayerStats>();
        player1Stats.lifeBar = p1LifeBar;
        p1LifeBar.transform.localScale = Vector3.one;
        player1Stats.bufferBar = p1BufferBar.GetComponentsInChildren<Transform>();

        player2Stats = player2.GetComponent<PlayerStats>();
		player2Stats.lifeBar = p2LifeBar;
		p2LifeBar.transform.localScale = Vector3.one;
		player2Stats.bufferBar = p2BufferBar.GetComponentsInChildren<Transform>();

	}

	void CreateObstacles() {
		// We gon do this real shitty for now. The obstacles are just blocks.
		// Create them wherever you'd like using code like this:
		GameObject obstacle = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/Obstacle"), Vector3.zero, Quaternion.identity);
		// obstacle.transform.localScale = whatever box shape you want!
			
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
    
	void StartRound() {
		if(Application.isEditor) {
			player1 = playerFactory.CreatePlayerInEditor(CreateControlScheme(0), characterSelectManager.p1Character, player1Pos, 0);
	        player2 = playerFactory.CreatePlayerInEditor(CreateControlScheme(1), characterSelectManager.p2Character, player2Pos, 1);
		}
		else {
			player1 = playerFactory.CreatePlayerWithController(player1Controller, characterSelectManager.p1Character, player1Pos, 0);
	        player2 = playerFactory.CreatePlayerWithController(player2Controller, characterSelectManager.p2Character, player2Pos, 1);
	    }
        CreateBars();
        CreateObstacles();
        currentUpdateFunction = InGameUpdate;
        
        FightIntro();
    }

	#endregion

    IEnumerator DisplayVictoryText(int playerNum, int roundsWon)
    {

        if (roundTimer.text == "0")
        {
            Debug.Log("time over");
            victoryText.text = "TIME'S\nUP";
            victoryText.enabled = true;
            yield return new WaitForSeconds(1.5f);
            victoryText.text = "";
            yield return new WaitForSeconds(0.5f);
        }

        if (playerNum == 5) {
            Debug.Log("time over + playerNum =" + playerNum);
            victoryText.text = "DRAW\nGAME";
		}
		else {
         
            if (playerNum == 4 || playerNum == 3) {
                victoryText.text = "DRAW\nGAME";
                victoryText.enabled = true;
				yield return new WaitForSeconds(1.5f);
				playerNum -= 2;
			}
			if (debug_on) Debug.Log(playerNum);
	        victoryText.text = playerNum == 2 ? "Player Two" : "Player One";
			victoryText.text += roundsWon == roundsToWin ? "\n IS \n   VICTORIOUS" : "\nWINS";
		}
		victoryText.enabled = true;
        yield return new WaitForSeconds(3.0f);
        victoryText.enabled = false;
        victoryText.text = "";

		if(player1RoundWins >= roundsToWin || player2RoundWins >= roundsToWin)
        {
			ResetGame();
		}
		else {
			RoundReset();
		}
    }
   
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Reset") != 0f) {
			SceneManager.LoadScene(0);
		}
		currentUpdateFunction();
	}

	void InGameUpdate(){
		if(player1Stats.health <= 0) {
			player1Stats.TakeDamage(-playerFactory.startingHealth);
		}
		if(player2Stats.health <= 0) {
			player2Stats.TakeDamage(-playerFactory.startingHealth);
		}
		if(Input.GetKeyDown(KeyCode.Return)) {
			if(menuController.active) {
				UnlockPlayers();
			}
			else {
				LockPlayers();
			}
			menuController.Toggle();
		}
	}

	void ResetGame() {
		Destroy(player1Reticle);
		Destroy(player2Reticle);
		// TODO ERASE THIS GARBAGE
		foreach(GameObject reticle in GameObject.FindGameObjectsWithTag("Reticle")) {
			Destroy(reticle);
		}
		Destroy(player1);
		Destroy(player2);
	}



	void FightIntro() {
		LockPlayers();
		StartCoroutine(ReadyFightMessageChange());
		// Probably put some ready fight shit over here
		Invoke("UnlockPlayers", 2.5f);
		AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
		backgroundMusic.clip = Resources.Load<AudioClip>("audio/music/battleTheme/RenewYourSoul");
		backgroundMusic.Play();
	}

	void LockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = true;
		player2.GetComponent<PlayerMovement>().locked = true;
		timerStarted = false;
	}

	void UnlockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = false;
		player2.GetComponent<PlayerMovement>().locked = false;
		timerStarted = true;
	}

	IEnumerator ReadyFightMessageChange() {
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("audio/soundEffects/swordSwing");
		victoryText.enabled = true;
		if(player1RoundWins + player2RoundWins == 0) {
            victoryText.text = "PREPARE \n YOURSELF";
            yield return new WaitForSeconds(1.0f);
            victoryText.text = "";
            yield return new WaitForSeconds(0.2f);
        }

        int currentRound = (player1RoundWins + player2RoundWins + 1);
        //victoryText.text = ("ROUND " + (player1RoundWins + player2RoundWins + 1.0f).ToString());
        victoryText.text = "ROUND " + currentRound.ToString();
        yield return new WaitForSeconds(1.0f);
        victoryText.text = "";
        yield return new WaitForSeconds(0.2f);
        victoryText.text = "FIGHT!";
		yield return new WaitForSeconds(0.5f);
		audio.Play();
		victoryText.text = "";
	}

	void ClearBullets() {
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Knife")) {
			Destroy(bullet);
		}
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Shield")) {
			Destroy(bullet);
		}
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Boomerang")) {
			Destroy(bullet);
		}
	}

	void RoundReset() {
		Destroy(player1Reticle);
		Destroy(player2Reticle);
		foreach(GameObject reticle in GameObject.FindGameObjectsWithTag("Reticle")) {
			Destroy(reticle);
		}
		Destroy(player1);
		Destroy(player2);
		StartRound();
	}

	void ToggleUI(bool mode) {
		UIElements.SetActive(mode);
	}

	public void ChangeCharacter() {
		player1Pos = player1.transform.position;
		characterSelectManager.p1Character += 1;
		if((int)characterSelectManager.p1Character == System.Enum.GetValues(typeof(Character)).Length) {
				characterSelectManager.p1Character = (Character)System.Enum.GetValues(typeof(Character)).GetValue(0);
			}
		Destroy(player1Reticle);
		Destroy(player1);
		if(Application.isEditor) {
			player1 = playerFactory.CreatePlayerInEditor(player1Controls, characterSelectManager.p1Character, player1Pos, 0);
		}
		else {
			player1 = playerFactory.CreatePlayerWithController(player1Controller, characterSelectManager.p1Character, player1Pos, 0);
		}
		player1Stats = player1.GetComponent<PlayerStats>();
        player1Stats.lifeBar = p1LifeBar;
        p1LifeBar.transform.localScale = Vector3.one;
        player1Stats.bufferBar = p1BufferBar.GetComponentsInChildren<Transform>();
        //p1BufferBar.transform.localScale = Vector3.one;
	}

	public void MainMenu() {
		SceneManager.LoadScene(0);
	}
}
