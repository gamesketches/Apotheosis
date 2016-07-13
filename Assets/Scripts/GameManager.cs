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
	GameObject p1LifeBar, p2LifeBar;
	#endregion

	#region CharacterSelect Vars
	GameObject characterSelectElements;
	Character p1Character, p2Character;
	Sprite[] p1CharacterPortraits, p2CharacterPortraits, p1InfoPanes, p2InfoPanes;
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

        p1CharacterPortraits = Resources.LoadAll<Sprite>("characterSelect/p1/portraits");
        p1InfoPanes = Resources.LoadAll<Sprite>("characterSelect/p1/summons");
        p2CharacterPortraits = Resources.LoadAll<Sprite>("characterSelect/p2/portraits");
		p2InfoPanes = Resources.LoadAll<Sprite>("characterSelect/p2/summons");
    }

    #region Pre-Battle

    void TitleScreen()
    {
        if (Input.GetButtonUp("ButtonA0"))
        {
            titleLogo.enabled = false;
            titleLogo.transform.GetChild(0).gameObject.SetActive(false);
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

    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(0).gameObject,
    										 p1Character, p1CharacterPortraits, p1InfoPanes);
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(1).gameObject,
    										 p2Character, p2CharacterPortraits, p2InfoPanes);

    	if(Input.GetButtonUp("ButtonA0")) {
			titleLogo.enabled = false;
            pressStart.enabled = false;
			background.enabled = true;
			characterSelectElements.SetActive(false);
            InitializeGameSettings();
    	}

    }

    void UpdateInfoCharacterSelect(GameObject player, Character highlightedCharacter,
    														 Sprite[] portraits,
    														 Sprite[] infoPanes) {
    	SpriteRenderer portrait = player.GetComponentInChildren<SpriteRenderer>();
    	portrait.sprite = portraits[(int)highlightedCharacter];
    	SpriteRenderer infoPane = player.GetComponentsInChildren<SpriteRenderer>()[2];
    	infoPane.sprite = infoPanes[(int)highlightedCharacter];
    	Text nameText = player.GetComponentInChildren<Text>();
    	nameText.text = highlightedCharacter.ToString();
    	Text[] bulletDescriptions = player.GetComponentsInChildren<Text>();
    	int firstText = bulletDescriptions.Length - 3;
    	bulletDescriptions[firstText + 0].text = bullets.types[(int)highlightedCharacter].projectileTypes[1].bulletDescription;
    	bulletDescriptions[firstText + 1].text = bullets.types[(int)highlightedCharacter].projectileTypes[2].bulletDescription;
		bulletDescriptions[firstText + 2].text = bullets.types[(int)highlightedCharacter].projectileTypes[0].bulletDescription;   
    }

    #endregion

    #region Initialization Code

	void InitializeGameSettings() {
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

	GameObject CreatePlayer(string[] controls, Character character, Vector3 position, int number){
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
		tempStats.number = number;
		temp.GetComponent<PlayerMovement>().InitializeAxes(controls);

		reticle.color = color;
		tempMovement.reticle = reticle;

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);
		tempInputManager.reticle = reticle;

		AnimatorOverrideController animationController = new AnimatorOverrideController();
		animationController.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprites/LoholtAnimation/p1/PlayerAnimationController");
		string resourcePath = string.Concat("sprites/", character.ToString(), "Animation/p", (number + 1).ToString());
		foreach(AnimationClip clip in Resources.LoadAll<AnimationClip>(resourcePath)) {
				animationController[clip.name] = clip;
			}
		tempMovement.SetAnimator(animationController);
		reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/reticle-18");
			player1Reticle = reticle.gameObject;

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
		Vector3 lifebarOffset = new Vector3(0, -2, 0);
		player1 = CreatePlayer(player1Controls, p1Character, player1Pos, 0);
		p1LifeBar = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SetLifeBar"), player1Pos + lifebarOffset, Quaternion.identity);
		p1LifeBar.transform.parent = player1.transform;
		player2 = CreatePlayer(player2Controls, p2Character, player2Pos, 1);
		p2LifeBar = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SetLifeBar"), player2Pos + lifebarOffset, Quaternion.identity);
		p2LifeBar.transform.parent = player2.transform;
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
	        victoryText.text = playerNum == 1 ? player2Stats.character.ToString().ToUpper() : player1Stats.character.ToString().ToUpper();
			victoryText.text += roundsWon == 3 ? "\n IS \n   VICTORIOUS" : "\nWINS";
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
   		        titleLogo.transform.GetChild(0).gameObject.SetActive(true);
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
		p1LifeBar.transform.localScale = new Vector3(player1HealthProportion * 0.5f, 1, 1);
		p2LifeBar.transform.localScale = new Vector3(player2HealthProportion * 0.5f, 1, 1);
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
	}

	void UnlockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = false;
		player2.GetComponent<PlayerMovement>().locked = false;
	}

	IEnumerator ReadyFightMessageChange() {
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("audio/soundEffects/swordSwing");
		victoryText.enabled = true;
		if(player1RoundWins + player2RoundWins == 0) {
			victoryText.text = "PREPARE \n YOURSELF";
		}
		yield return new WaitForSeconds(2.0f);
		victoryText.text = "FIGHT";
		yield return new WaitForSeconds(0.5f);
		audio.Play();
		victoryText.text = "";
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
