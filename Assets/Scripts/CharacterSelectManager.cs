#define RELEASE
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;
using System;

public class CharacterSelectManager : MonoBehaviour {

	GameObject characterSelectElements;
	public Character p1Character, p2Character;
	Sprite[] p1CharacterPortraits, p2CharacterPortraits, p1InfoPanes, p2InfoPanes;
	public bool charactersSelected;
	private bool p1Selected, p2Selected, p1ButtonDown, p2ButtonDown;
	private int numCharacters;
	private string[,] bulletDescriptions;
	InputDevice player1Controller, player2Controller;
	AudioSource audioSource;
	// Use this for initialization
	void Awake () {
		p1Selected = false;
		p2Selected = false;
		p1ButtonDown = true;
		p2ButtonDown = true;
		charactersSelected = false;
		characterSelectElements = GameObject.Find("CharacterSelectElements");
        characterSelectElements.SetActive(false);
		p1CharacterPortraits = Resources.LoadAll<Sprite>("characterSelect/p1/portraits");
        p1InfoPanes = Resources.LoadAll<Sprite>("characterSelect/p1/info");
        p2CharacterPortraits = Resources.LoadAll<Sprite>("characterSelect/p2/portraits");
		p2InfoPanes = Resources.LoadAll<Sprite>("characterSelect/p2/info");
		numCharacters = System.Enum.GetValues(typeof(Character)).Length;
		audioSource = GetComponent<AudioSource>();
	}

	public void Reset() {
		p1Selected = false;
		p2Selected = false;
		p1ButtonDown = true;
		p2ButtonDown = true;
		charactersSelected = false;
		characterSelectElements.SetActive(true);
        p1Character = Character.Loholt;
        p2Character = Character.Orpheus;
        audioSource.clip = Resources.Load<AudioClip>("audio/music/characterSelect/characterSelectSound");
	}

	public void CharacterSelectUpdate() {
		CheckDirectionals();
		if(!p1Selected && GetPlayer1XAxis() != 0 && !p1ButtonDown) {
			p1ButtonDown = true;
    		p1Character = CycleThroughCharacters(p1Character, GetPlayer1XAxis());
    	}
		if(!p2Selected && GetPlayer2XAxis() != 0 && !p2ButtonDown) {
			p2ButtonDown = true;
    		p2Character = CycleThroughCharacters(p2Character, GetPlayer2XAxis());
    	}
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(0).gameObject,
    										 p1Character, p1CharacterPortraits, p1InfoPanes);
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(1).gameObject,
    										 p2Character, p2CharacterPortraits, p2InfoPanes);

    	if(GetPlayer1ConfirmButton()) {
			p1Selected = true;
			audioSource.Play();
            Debug.Log("P1 Selected");
        }
    	if(GetPlayer2ConfirmButton()) {
    		p2Selected = true;
    		audioSource.Play();
            Debug.Log("P2 Selected");
        }
    	if(p1Selected && p2Selected) {
			characterSelectElements.SetActive(false);
			charactersSelected = true;
    	}
    }

	public void TrainingModeCharacterSelectUpdate() {
    	CheckDirectionals();
    		if(GetPlayer1XAxis() != 0 && !p1ButtonDown) {
    			p1ButtonDown = true;
    			if(p1Selected) {
					p2Character = CycleThroughCharacters(p2Character, GetPlayer1XAxis());
    			}
    			else {
					p1Character = CycleThroughCharacters(p1Character, GetPlayer1XAxis());
	    		}
    		}

		if(GetPlayer1ConfirmButton()) {
			if(p1Selected) {
				audioSource.Play();
				p2Selected = true;
			}
			else { 
				audioSource.Play();
				p1Selected = true;
			}
        }
		UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(0).gameObject,
    										 p1Character, p1CharacterPortraits, p1InfoPanes);
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(1).gameObject,
    										 p2Character, p2CharacterPortraits, p2InfoPanes);

        if(p1Selected && p2Selected) {
        	characterSelectElements.SetActive(false);
        	charactersSelected = true;
        }
    }

	void CheckDirectionals() {
    	if(GetPlayer1XAxis() == 0) {
    		p1ButtonDown = false;
    	}
    	if(GetPlayer2XAxis() == 0) {
    		p2ButtonDown = false;
    	}
    }

    float GetPlayer1XAxis() {
    	if(Application.isEditor) {
    		return Input.GetAxis("Horizontal0");
    	}
    	else {
    		return player1Controller.Direction.X;
    	}
    }

    float GetPlayer2XAxis(){
		if(Application.isEditor) {
    		return Input.GetAxis("Horizontal1");
    	}
    	else {
    		return player2Controller.Direction.X;
    	}
    }

    bool GetPlayer1ConfirmButton() {
    	if(Application.isEditor) {
    		return Input.GetButtonDown("ButtonB0");
    	}
    	else {
    		return player1Controller.Action1.WasPressed;
    	}
    }

    bool GetPlayer2ConfirmButton() {
    	if(Application.isEditor) {
    		return Input.GetButtonDown("ButtonB1");
    	}
    	else {
    		return player2Controller.Action1.WasPressed;
    	}
    }

    void UpdateInfoCharacterSelect(GameObject player, Character highlightedCharacter,
    														 Sprite[] portraits,
    														 Sprite[] infoPanes) {
    	SpriteRenderer portrait = player.GetComponentInChildren<SpriteRenderer>();
    	portrait.sprite = portraits[(int)highlightedCharacter];
    	SpriteRenderer infoPane = player.GetComponentsInChildren<SpriteRenderer>()[2];
    	infoPane.sprite = infoPanes[(int)highlightedCharacter];
    }

	Character CycleThroughCharacters(Character character, float xVal) {
		if(xVal < 0) {
				int temp = (int)character;
				temp -= 1;
   		 		if(temp < 0) {
   		 			temp = numCharacters -1;
   		 		}
				return (Character)temp;//System.Enum.GetValues(typeof(Character)).GetValue(temp);

   		 	}
    	else if(xVal > 0) {
    			character += 1;
    			if((int)character == numCharacters) {
    				return (Character)System.Enum.GetValues(typeof(Character)).GetValue(0);
    			}
    		}
    	return character;
    }

    public void SetControllers(InputDevice player1, InputDevice player2) {
    	player1Controller = player1;
    	player2Controller = player2;
    }
}
