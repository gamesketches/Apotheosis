using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private int roundsToWin;
    public float screenShake;
    #endregion

    #region CharacterSelect Vars
    CharacterSelectManager characterSelectManager;
	#endregion
    SpriteRenderer titleLogo;
    SpriteRenderer infoScreen;
	SpriteRenderer background;
    Text pressStart;
    Text roundTimer;
    Text victoryText;

    SpriteRenderer[] HorusWinsIconsSR;
    SpriteRenderer[] SetWinsIconsSR;

    private bool debug_on;

    // Use this for initialization
    void Start () {
        p1LifeBar = GameObject.Find("P1LifeBar");
        p1BufferBar = GameObject.Find("P1BufferBar");
        p2LifeBar = GameObject.Find("P2LifeBar");
		p2BufferBar = GameObject.Find("P2BufferBar");
        ToggleUI(false);

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
		string[,] bulletDescriptions = new string[numCharacters, numCharacters];
        for(int i = 0; i < numCharacters; i++){
			bulletDescriptions[i,0] = bullets.types[i].projectileTypes[1].bulletDescription;
			bulletDescriptions[i,1] = bullets.types[i].projectileTypes[2].bulletDescription;
			bulletDescriptions[i,2] = bullets.types[i].projectileTypes[0].bulletDescription;
        }

        characterSelectManager.SetBulletDescriptions(bulletDescriptions);
        AnalyticsEngine.Initialize(new string[] {"LoholtBulletsFired", "OrpheusBulletsFired", "HirukoBulletsFired"});
		characterSelectManager.Reset();
        currentUpdateFunction = CharacterSelect;
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
		player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		StartRound();
	}

  

	void CreateBars() {
		Vector3 lifebarOffset = new Vector3(9f, 19.6f, 0);
		Vector3 bufferBarOffset = new Vector3(-10, 20, 0);

		ToggleUI(true);
        player1Stats = player1.GetComponent<PlayerStats>();
        player1Stats.lifeBar = p1LifeBar;
        p1LifeBar.transform.localScale = Vector3.one;
        player1Stats.bufferBar = p1BufferBar;
        p1BufferBar.transform.localScale = Vector3.one;

        player2Stats = player2.GetComponent<PlayerStats>();
		player2Stats.lifeBar = p2LifeBar;
		p2LifeBar.transform.localScale = Vector3.one;
		player2Stats.bufferBar = p2BufferBar;
        p2BufferBar.transform.localScale = Vector3.one;

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
		player1 = playerFactory.CreatePlayer(player1Controls, characterSelectManager.p1Character, player1Pos, 0);
        player2 = playerFactory.CreatePlayer(player2Controls, characterSelectManager.p2Character, player2Pos, 1);

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
		p1LifeBar.SetActive(mode);
		p1BufferBar.SetActive(mode);
		p2LifeBar.SetActive(mode);
		p2BufferBar.SetActive(mode);
	}
}
