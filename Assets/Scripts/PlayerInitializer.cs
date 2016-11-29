using UnityEngine;
using System.Collections;

public class PlayerInitializer : MonoBehaviour {

	public int startingHealth;
	public float screenShake;
	public BulletDepot bullets;

	public GameObject CreatePlayer(string[] controls, Character character, Vector3 position, int number){
		Color color = character == Character.Loholt ? Color.blue : Color.red;
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		Reticle reticle = ((GameObject)Instantiate(Resources.Load("prefabs/Reticle"))).GetComponent<Reticle>();
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		PlayerStats tempStats = temp.GetComponent<PlayerStats>();
		PlayerMovement tempMovement = temp.GetComponent<PlayerMovement>();
		InputManager tempInputManager = temp.GetComponent<InputManager>();
		switch(character) {
			case Character.Orpheus:
				break;
			case Character.Hiruko: 
				temp.AddComponent<OffscreenShot>();
				tempInputManager = temp.GetComponent<OffscreenShot>();
				Destroy(temp.GetComponent<InputManager>());
				break;
			case Character.Loholt:
				temp.AddComponent<RecallShot>();
				tempInputManager = temp.GetComponent<RecallShot>();
				Destroy(temp.GetComponent<InputManager>());
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
}
