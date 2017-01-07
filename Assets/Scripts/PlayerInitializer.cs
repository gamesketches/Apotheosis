#define RELEASE
using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInitializer : MonoBehaviour {

	public int startingHealth;
	public float screenShake;
	public BulletDepot bullets;

	#if DEV
	public GameObject CreatePlayer(string[] controls, Character character, Vector3 position, int number){
		Color color = character == Character.Loholt ? Color.blue : Color.red;
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		Reticle reticle = ((GameObject)Instantiate(Resources.Load("prefabs/Reticle"))).GetComponent<Reticle>();
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		PlayerStats tempStats = temp.GetComponent<PlayerStats>();
		PlayerMovement tempMovement = temp.GetComponent<PlayerMovement>();
		InputInterpretter tempInputManager = temp.GetComponent<InputInterpretter>();
		switch(character) {
			case Character.Orpheus:
				break;
			case Character.Hiruko: 
				temp.AddComponent<OffscreenShot>();
				tempInputManager = temp.GetComponent<OffscreenShot>();
				Destroy(temp.GetComponent<InputInterpretter>());
				break;
			case Character.Loholt:
				temp.AddComponent<RecallShot>();
				tempInputManager = temp.GetComponent<RecallShot>();
				Destroy(temp.GetComponent<InputInterpretter>());
				break;
		};

		tempStats.health = startingHealth;
		tempStats.maxHealth = startingHealth;
		tempStats.character = character;
		tempStats.playerColor = color;
		tempStats.number = number;
		temp.GetComponent<PlayerMovement>().InitializeAxes(controls);
        tempStats.screenShake = screenShake;
		reticle.color = color;
		tempMovement.reticle = reticle;

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);
		tempInputManager.reticle = reticle;

		AnimatorOverrideController animationController = new AnimatorOverrideController();

		animationController.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprites/OptimizedAnimationController");
		string resourcePath = string.Concat("sprites/", character.ToString(), "Animation/p", (number + 1).ToString());
		foreach(AnimationClip clip in Resources.LoadAll<AnimationClip>(resourcePath)) {
				animationController[clip.name] = clip;
			}
	
		tempMovement.SetAnimator(animationController);
		reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/reticle-18");
			//player1Reticle = reticle.gameObject;

        return temp;
	}
	#elif RELEASE
	public GameObject CreatePlayer(InputDevice controls, Character character, Vector3 position, int number){
		Color color = character == Character.Loholt ? Color.blue : Color.red;
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		Reticle reticle = ((GameObject)Instantiate(Resources.Load("prefabs/Reticle"))).GetComponent<Reticle>();
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		PlayerStats tempStats = temp.GetComponent<PlayerStats>();
		PlayerMovement tempMovement = temp.GetComponent<PlayerMovement>();
		InputInterpretter tempInputManager = temp.GetComponent<InputInterpretter>();
		switch(character) {
			case Character.Orpheus:
				break;
			case Character.Hiruko: 
				temp.AddComponent<OffscreenShot>();
				tempInputManager = temp.GetComponent<OffscreenShot>();
				Destroy(temp.GetComponent<InputInterpretter>());
				break;
			case Character.Loholt:
				temp.AddComponent<RecallShot>();
				tempInputManager = temp.GetComponent<RecallShot>();
				Destroy(temp.GetComponent<InputInterpretter>());
				break;
		};

		tempStats.health = startingHealth;
		tempStats.maxHealth = startingHealth;
		tempStats.character = character;
		tempStats.playerColor = color;
		tempStats.number = number;
		temp.GetComponent<PlayerMovement>().InitializeController(controls);
        tempStats.screenShake = screenShake;
		reticle.color = color;
		tempMovement.reticle = reticle;

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);
		tempInputManager.reticle = reticle;

		AnimatorOverrideController animationController = new AnimatorOverrideController();

		animationController.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprites/OptimizedAnimationController");
		string resourcePath = string.Concat("sprites/", character.ToString(), "Animation/p", (number + 1).ToString());
		foreach(AnimationClip clip in Resources.LoadAll<AnimationClip>(resourcePath)) {
				animationController[clip.name] = clip;
			}
	
		tempMovement.SetAnimator(animationController);
		reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/reticle-18");
			//player1Reticle = reticle.gameObject;

        return temp;
	}
	#else
	public GameObject CreatePlayer(string[] controls, Character character, Vector3 position, int number) {
		Debug.LogError("No Preprocessor directive defined for PlayerInitializer");
	}
	#endif
}
