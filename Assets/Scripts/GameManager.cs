using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Character {Loholt, Orpheus};

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
	public Vector3 player1Pos, player2Pos;
	public int startingHealth;
	string[] player1Controls, player2Controls;
	PlayerStats player1Stats, player2Stats;
	BulletDepot bullets;
	GameObject[] SetLifeBar;
	GameObject[] HorusLifeBar;
	#endregion

	#region CharacterSelect Vars
	GameObject characterSelectElements;
	Character p1Character, p2Character;
	Sprite[] characterPortraits;
	#endregion
    SpriteRenderer titleLogo;
    SpriteRenderer infoScreen;
	SpriteRenderer background;
    Text pressStart;
    Text roundTimer;
    Text victoryText;

    SpriteRenderer[] HorusWinsIconsSR;
    SpriteRenderer[] SetWinsIconsSR;

    // Use this for initialization
    void Start () {
        GameObject[] HorusWinsIcons;
        GameObject[] SetWinsIcons;

        SetLifeBar = GameObject.FindGameObjectsWithTag("SetLifeBar");
        HorusLifeBar = GameObject.FindGameObjectsWithTag("HorusLifeBar");
        SetWinsIcons = GameObject.FindGameObjectsWithTag("SetWinsIcon");
        HorusWinsIcons = GameObject.FindGameObjectsWithTag("HorusWinsIcon");
        HorusWinsIconsSR = new SpriteRenderer[HorusWinsIcons.Length];
        SetWinsIconsSR = new SpriteRenderer[SetWinsIcons.Length];
        int j = 0;
        for (int i = 2; i >= 0; i--)
        {
            HorusWinsIconsSR[j] = HorusWinsIcons[i].GetComponent<SpriteRenderer>();
            SetWinsIconsSR[j] = SetWinsIcons[i].GetComponent<SpriteRenderer>();
            j++;
        }

		bullets = new BulletDepot();
		bullets.Load();
		player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		currentRoundTime = roundTime;
        titleLogo = GameObject.FindGameObjectWithTag("TitleLogo").GetComponent<SpriteRenderer>();
        pressStart = titleLogo.GetComponent<Text>();
        infoScreen = GameObject.FindGameObjectWithTag("InfoScreen").GetComponent<SpriteRenderer>();
		background = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        victoryText = GameObject.FindGameObjectWithTag("VictoryText").GetComponent<Text>();
        characterSelectElements = GameObject.Find("CharacterSelectElements");
        characterSelectElements.SetActive(false);
        titleLogo.enabled = true;
        pressStart.enabled = true;
        currentUpdateFunction = TitleScreen;

        characterPortraits = Resources.LoadAll<Sprite>("sprites/CharacterPortraits");
    }

    #region Pre-Battle

    void TitleScreen()
    {
        if (Input.GetButtonUp("ButtonA0"))
        {
            titleLogo.enabled = false;
            pressStart.enabled = false;
			background.enabled = true;
            characterSelectElements.SetActive(true);
            p1Character = Character.Loholt;
            p2Character = Character.Orpheus;
            currentUpdateFunction = CharacterSelect;
        }
        else if (Input.GetButtonUp("ButtonD0") || Input.GetButtonUp("ButtonC0") || Input.GetButtonUp("ButtonB0"))
        {
            titleLogo.enabled = false;
            pressStart.enabled = false;
            infoScreen.enabled = true;
            currentUpdateFunction = InfoScreen;
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
    	if(Input.GetAxis("Horizontal0") < 0) {
    		p1Character = Character.Loholt;
    	}
    	else if(Input.GetAxis("Horizontal0") > 0) {
    		p1Character = Character.Orpheus;
    	}
    	if(Input.GetAxis("Horizontal1") < 0) {
    		p2Character = Character.Loholt;
    	}
    	else if(Input.GetAxis("Horizontal1") > 0) {
    		p2Character = Character.Orpheus;
    	}

    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(0).gameObject, p1Character);
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(1).gameObject, p2Character);

    	if(Input.GetButtonUp("ButtonA0")) {
			titleLogo.enabled = false;
            pressStart.enabled = false;
			background.enabled = true;
			characterSelectElements.SetActive(false);
            InitializeGameSettings();
    	}

    }

    void UpdateInfoCharacterSelect(GameObject player, Character highlightedCharacter) {
    	SpriteRenderer portrait = player.GetComponentInChildren<SpriteRenderer>();
    	portrait.sprite = characterPortraits[(int)highlightedCharacter];
    	Text nameText = player.GetComponentInChildren<Text>();
    	nameText.text = highlightedCharacter.ToString();
    	Text[] bulletDescriptions = player.GetComponentsInChildren<Text>();
    	int firstText = bulletDescriptions.Length - 3;
    	for(int i = 0; i < 3; i++) {
    		bulletDescriptions[firstText + i].text = bullets.types[(int)highlightedCharacter].projectileTypes[i].bulletDescription;
    	}
    }

    #endregion

    #region Initialization Code

	void InitializeGameSettings() {
		SetLifeBar = GameObject.FindGameObjectsWithTag("SetLifeBar");
		HorusLifeBar = GameObject.FindGameObjectsWithTag("HorusLifeBar");
		player1RoundWins = 0;
		player1Wins = 0;
		player2RoundWins = 0;
		player2Wins = 0;
		bullets = new BulletDepot();
		bullets.Load();
		player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		StartRound();
	}

	GameObject CreatePlayer(string[] controls, Character character, Vector3 position){
		Color color = character == Character.Loholt ? Color.blue : Color.red;
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		Reticle reticle = ((GameObject)Instantiate(Resources.Load("prefabs/Reticle"))).GetComponent<Reticle>();
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		PlayerStats tempStats = temp.GetComponent<PlayerStats>();
		PlayerMovement tempMovement = temp.GetComponent<PlayerMovement>();
		InputManager tempInputManager = temp.GetComponent<InputManager>();

		tempStats.health = startingHealth;
		tempStats.maxHealth = startingHealth;
		tempStats.character = character;
		tempStats.playerColor = color;
		temp.GetComponent<PlayerMovement>().InitializeAxes(controls);

		reticle.color = color;
		tempMovement.reticle = reticle;

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);
		tempInputManager.reticle = reticle;

		if(character == Character.Loholt) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillBlackWhite");
			tempMovement.SetAnimator(Resources.Load<RuntimeAnimatorController>("sprites/LoholtAnimation/p1/PlayerAnimationController"));
			tempStats.number = 0;
			reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Khopesh/khopeshHorus");
			player1Reticle = reticle.gameObject;
		} else if(character == Character.Orpheus) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillWhiteBlack");
			tempMovement.SetAnimator(Resources.Load<RuntimeAnimatorController>("sprites/OrpheusAnimation/p1/PlayerAnimationController"));
			tempStats.number = 1;
			reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Khopesh/khopeshSet");
			player2Reticle = reticle.gameObject;
		}
		return temp;
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
		player1 = CreatePlayer(player1Controls, p1Character, player1Pos);
		player2 = CreatePlayer(player2Controls, p2Character, player2Pos);
		player1Stats = player1.GetComponent<PlayerStats>();
		player2Stats = player2.GetComponent<PlayerStats>();
		currentUpdateFunction = InGameUpdate;
		currentRoundTime = roundTime;	
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        roundTimer.enabled = true;
        FightIntro();
    }

	#endregion

    IEnumerator DisplayVictoryText(int playerNum, int roundsWon)
    {
		if(playerNum == 5) {
			victoryText.text = "DRAW\nGAME";
		}
		else {
			if(playerNum == 4 || playerNum == 3) {
				victoryText.text = "DRAW\nGAME";
				victoryText.enabled = true;
				yield return new WaitForSeconds(1.5f);
				playerNum -= 2;
			}
	        victoryText.text = playerNum == 1 ? "<color=Blue>HORUS" : "<color=Red>SET";
			victoryText.text += roundsWon == 3 ? "\n IS \n   VICTORIOUS</color>" : "\n</color>WINS";
		}
		victoryText.enabled = true;
        yield return new WaitForSeconds(3.0f);
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
		currentRoundTime -= Time.deltaTime;
        roundTimer.text = Mathf.RoundToInt(currentRoundTime).ToString();

		UpdateLifeBars();

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
		}
	}

	void RoundEndUpdate() {
			if(player1RoundWins > 2 || player2RoundWins > 2){
				Destroy(player1Reticle);
				Destroy(player2Reticle);
				Destroy(player1);
				Destroy(player2);
				titleLogo.enabled = true;
                pressStart.enabled = true;
				background.enabled = true;
				roundTimer.enabled = false;
				foreach(SpriteRenderer renderer in HorusWinsIconsSR) {
					renderer.enabled = false;
				}
				foreach(SpriteRenderer renderer in SetWinsIconsSR) {
					renderer.enabled = false;
				}
				AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
				backgroundMusic.clip = Resources.Load<AudioClip>("audio/music/menu/LandOfTwoFields");
				backgroundMusic.Play();
				currentUpdateFunction = TitleScreen;
				return;
			}
			RoundReset();
	}

	void UpdateLifeBars() {
		float player1HealthProportion, player2HealthProportion;
		if(player1Stats.health > 0) {
			player1HealthProportion = (player1Stats.health / player1Stats.maxHealth);
		}
		else {
			player1HealthProportion = 0;
		}
		if(player2Stats.health > 0) {
			player2HealthProportion = (player2Stats.health / player2Stats.maxHealth);
		}
		else {
			player2HealthProportion = 0;
		}
		SetLifeBar[0].transform.localScale = new Vector3(player1HealthProportion * 10f, 6, 1);
		SetLifeBar[1].transform.localScale = new Vector3(player1HealthProportion * 10f, 6, 1);
		SetLifeBar[2].transform.localScale = new Vector3(player1HealthProportion * 10.5062f, 6, 1);
		HorusLifeBar[0].transform.localScale = new Vector3(player2HealthProportion * 10f, 6, 1);
		HorusLifeBar[1].transform.localScale = new Vector3(player2HealthProportion * 10f, 6, 1);
		HorusLifeBar[2].transform.localScale = new Vector3(player2HealthProportion * 10.5062f, 6, 1);
	}

	void FightIntro() {
		LockPlayers();
		// Probably put some ready fight shit over here
		Invoke("UnlockPlayers", 2.5f);
		AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
		backgroundMusic.clip = Resources.Load<AudioClip>("audio/music/battleTheme/RenewYourSoul");
		backgroundMusic.Play();
	}

	void LockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = true;
		player2.GetComponent<PlayerMovement>().locked = true;
	}

	void UnlockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = false;
		player2.GetComponent<PlayerMovement>().locked = false;
	}

	void ClearBullets() {
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Gator")) {
			Destroy(bullet);
		}
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Hippo")) {
			Destroy(bullet);
		}
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Crane")) {
			Destroy(bullet);
		}
	}

	void RoundReset() {
		Destroy(player1Reticle);
		Destroy(player2Reticle);
		Destroy(player1);
		Destroy(player2);
		StartRound();
	}


}
