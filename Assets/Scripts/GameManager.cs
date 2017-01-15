using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Character {Bastet, Hiruko, Loholt, Orpheus};

public class GameManager : MonoBehaviour {

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
    public float attractModeTimer;

    // Use this for initialization
    void Start () {
        GameObject[] HorusWinsIcons;
        GameObject[] SetWinsIcons;

        SetWinsIcons = GameObject.FindGameObjectsWithTag("SetWinsIcon");
        HorusWinsIcons = GameObject.FindGameObjectsWithTag("HorusWinsIcon");
        HorusWinsIconsSR = new SpriteRenderer[HorusWinsIcons.Length];
        SetWinsIconsSR = new SpriteRenderer[SetWinsIcons.Length];
        p1LifeBar = GameObject.Find("P1LifeBar");
        p1BufferBar = GameObject.Find("P1BufferBar");
        p2LifeBar = GameObject.Find("P2LifeBar");
		p2BufferBar = GameObject.Find("P2BufferBar");
		UIElements = GameObject.Find("InGameUIElements");
        ToggleUI(false);
        player1Wins = 0;
        player2Wins = 0;
        roundsToWin = 2;

        int j = 0;
        for (int i = 1; i >= 0; i--)
        {
            HorusWinsIconsSR[j] = HorusWinsIcons[i].GetComponent<SpriteRenderer>();
            SetWinsIconsSR[j] = SetWinsIcons[i].GetComponent<SpriteRenderer>();
            j++;
        }

        timerStarted = false;

		//bullets = new BulletDepot(); // clearing a warning w/next line - ski
        bullets = ScriptableObject.CreateInstance<BulletDepot>();
		bullets.Load();

		playerFactory = GetComponent<PlayerInitializer>();
    	playerFactory.bullets = bullets;
        player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		currentRoundTime = roundTime;
        titleLogo = GameObject.FindGameObjectWithTag("TitleLogo").GetComponent<SpriteRenderer>();
        pressStart = titleLogo.GetComponent<Text>();
        infoScreen = GameObject.FindGameObjectWithTag("InfoScreen").GetComponent<SpriteRenderer>();
		background = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        victoryText = GameObject.FindGameObjectWithTag("VictoryText").GetComponent<Text>();
       
        titleLogo.enabled = true;
        pressStart.enabled = true;
        currentUpdateFunction = TitleScreen;

        characterSelectManager = GetComponent<CharacterSelectManager>();
        int numCharacters = System.Enum.GetNames(typeof(Character)).Length;
		string[,] bulletDescriptions = new string[numCharacters, numCharacters];
        for(int i = 0; i < numCharacters; i++){
			bulletDescriptions[i,0] = bullets.types[i].projectileTypes[1].bulletDescription;
			bulletDescriptions[i,1] = bullets.types[i].projectileTypes[2].bulletDescription;
			bulletDescriptions[i,2] = bullets.types[i].projectileTypes[0].bulletDescription;
        }

        AnalyticsEngine.Initialize(new string[] {"LoholtBulletsFired", "OrpheusBulletsFired", "HirukoBulletsFired"});
    }

    #region Pre-Battle

    void TitleScreen()
    {
		if (Input.GetButtonUp("ButtonA0") || Input.GetButtonUp("ButtonB0") || Input.GetButtonUp("ButtonC0") 
			|| Input.GetButtonUp("ButtonD0") || Input.GetButtonUp("ButtonA1") || Input.GetButtonUp("ButtonB1")
			|| Input.GetButtonUp("ButtonC1") || Input.GetButtonUp("ButtonD1"))
        {
            titleLogo.enabled = false;
            titleLogo.transform.GetChild(0).gameObject.SetActive(false);
            pressStart.enabled = false;
			background.enabled = true;
			ChangeBackgroundMusic("audio/music/characterSelect/StrengthOfWillCut");
            characterSelectManager.Reset();
            currentUpdateFunction = CharacterSelect;
        }

        attractModeTimer -= Time.deltaTime;
        if(attractModeTimer < 0) {
        	SceneManager.LoadScene(1);
        }
        /*       else if (Input.GetButtonUp("ButtonD0") || Input.GetButtonUp("ButtonC0") || Input.GetButtonUp("ButtonB0"))
               {
                   titleLogo.enabled = false;
                   pressStart.enabled = false;
                   infoScreen.enabled = true;
                   currentUpdateFunction = InfoScreen;
               }*/

        //ski 
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            //debug to start round directly
            titleLogo.enabled = false;
            titleLogo.transform.GetChild(0).gameObject.SetActive(false);
            pressStart.enabled = false;
            background.enabled = true;
            characterSelectManager.p1Character = Character.Loholt;
            characterSelectManager.p2Character = Character.Orpheus;
            InitializeGameSettings();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            //debug to start round directly
            titleLogo.enabled = false;
            titleLogo.transform.GetChild(0).gameObject.SetActive(false);
            pressStart.enabled = false;
            background.enabled = true;
            characterSelectManager.p1Character = Character.Orpheus;
            characterSelectManager.p2Character = Character.Hiruko;
            InitializeGameSettings();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            //debug to start round directly
            titleLogo.enabled = false;
            titleLogo.transform.GetChild(0).gameObject.SetActive(false);
            pressStart.enabled = false;
            background.enabled = true;
            characterSelectManager.p1Character = Character.Hiruko;
            characterSelectManager.p2Character = Character.Loholt;
            InitializeGameSettings();
        }

        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            //debug to start round directly
            titleLogo.enabled = false;
            titleLogo.transform.GetChild(0).gameObject.SetActive(false);
            pressStart.enabled = false;
            background.enabled = true;
            characterSelectManager.p1Character = Character.Orpheus;
            characterSelectManager.p2Character = Character.Bastet;
            InitializeGameSettings();
        }

    }

    void InfoScreen()
    {
        if (Input.GetButtonUp("ButtonD0") || Input.GetButtonUp("ButtonC0") || Input.GetButtonUp("ButtonB0"))
        {
            infoScreen.enabled = false;
            titleLogo.enabled = true;
            pressStart.enabled = true;
            currentUpdateFunction = TitleScreen;
        }
    }

    #endregion

    #region CharacterSelect
    void CharacterSelect() {
    	characterSelectManager.CharacterSelectUpdate();
    	if(characterSelectManager.charactersSelected) {
			titleLogo.enabled = false;
            pressStart.enabled = false;
			background.enabled = true;
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
		currentRoundTime = roundTime;	
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        roundTimer.enabled = true;

        //Instantiate(Resources.Load<GameObject>("prefabs/PowerUpZone"), powerUpZonePosition, Quaternion.identity);
        
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

		if(player1RoundWins >= roundsToWin || player2RoundWins >= roundsToWin)
        {
        	victoryText.text = "X For Rematch\n O to Quit";
        	while(!Input.GetButtonUp("ButtonB0") && !Input.GetButtonUp("ButtonC0")) {
        		yield return null;
        	}
        	if(Input.GetButtonUp("ButtonB0")) {
        		ClearObjects();
        		InitializeGameSettings();
        	}
        	else {
				ResetGame();
			}
		}
		else {
			RoundReset();
		}
		victoryText.enabled = false;
        victoryText.text = "";
    }

    /*	// Use this for initialization
        void Start () {
            // Fill in the MenuUpdate function
            // then uncomment line 27 and delete line 28
            // currentUpdateFunction = MenuUpdate;
            //InitializeGameSettings();
        }*/
   
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Reset") != 0f) {
			SceneManager.LoadScene(0);
		}
		currentUpdateFunction();
	}

	void InGameUpdate(){
		if(timerStarted) {
			currentRoundTime -= Time.deltaTime;
   	     roundTimer.text = Mathf.RoundToInt(currentRoundTime).ToString();
   	     }

		if(player1Stats.health <= 0 || player2Stats.health <= 0 || currentRoundTime <= 0) {
			LockPlayers();
			if(player1Stats.health <= 0 && player2Stats.health <= 0 ||
							player1Stats.health == player2Stats.health) {
					StartCoroutine(DisplayVictoryText(5, 0));
			}
			else if(player1Stats.health <= 0 || player1Stats.health < player2Stats.health) {
                player2RoundWins++;
                StartCoroutine(DisplayVictoryText(2, player2RoundWins));
                SetWinsIconsSR[player2RoundWins - 1].enabled = true;
			}
			else if (player2Stats.health <= 0 || player2Stats.health < player1Stats.health){
                player1RoundWins++;
                StartCoroutine(DisplayVictoryText(1, player1RoundWins));
                HorusWinsIconsSR[player1RoundWins - 1].enabled = true;
			}
			currentUpdateFunction = RoundEndUpdate;
			ClearBullets();
			AnalyticsEngine.PrintRow();
		}
	}

	void RoundEndUpdate() {
			if(player1RoundWins > 2 || player2RoundWins > 2){
          	  if (player1RoundWins > 2) player1Wins++;
            	else player2Wins++;
				Invoke("ResetGame", 3f);
				Debug.Log("Called in RoundEndUpdate");
				return;
			}
	}

	void ResetGame() {
		ClearObjects();
		titleLogo.enabled = true;
   		titleLogo.transform.GetChild(0).gameObject.SetActive(true);
        pressStart.enabled = true;
		background.enabled = true;
		roundTimer.enabled = false;
		ChangeBackgroundMusic("audio/music/menu/LandOfTwoFields");
		currentUpdateFunction = TitleScreen;
		ToggleUI(false);
	}

	void ClearObjects() {
		Destroy(player1Reticle);
		Destroy(player2Reticle);
		// TODO ERASE THIS GARBAGE
		foreach(GameObject reticle in GameObject.FindGameObjectsWithTag("Reticle")) {
			Destroy(reticle);
		}
		Destroy(player1);
		Destroy(player2);

		foreach(SpriteRenderer renderer in HorusWinsIconsSR) {
			renderer.enabled = false;
		}
		foreach(SpriteRenderer renderer in SetWinsIconsSR) {
			renderer.enabled = false;
		}
	}

	void FightIntro() {
		LockPlayers();
		StartCoroutine(ReadyFightMessageChange());
		// Probably put some ready fight shit over here
		Invoke("UnlockPlayers", 2.5f);
		ChangeBackgroundMusic("audio/music/battleTheme/RenewYourSoul");
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
        Debug.Log("GameManager.cs: ROUND NUMBER = " + (player1RoundWins + player2RoundWins + 1).ToString());
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

	void ChangeBackgroundMusic(string path){
		AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
		backgroundMusic.clip = Resources.Load<AudioClip>(path);
		backgroundMusic.Play();
	}
}
