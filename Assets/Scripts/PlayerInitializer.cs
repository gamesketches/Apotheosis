using UnityEngine;
using System.Collections;

public class PlayerInitializer : MonoBehaviour {

	public int startingHealth;
	public float screenShake;
	public BulletDepot bullets;
	public float orpheusSpeed;
	public float loholtSpeed;
	public float hirukoSpeed;
	public float bastetSpeed;

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
				tempMovement.speed = orpheusSpeed;
				break;
			case Character.Hiruko: 
				tempMovement.speed = hirukoSpeed;
				temp.AddComponent<OffscreenShot>();
				tempInputManager = temp.GetComponent<OffscreenShot>();
                Debug.Log(temp.GetComponent<InputManager>().mashBufferSize + " dogmngs");
                Debug.Log(tempInputManager.mashBufferSize + " dogmngs");
                Destroy(temp.GetComponent<InputManager>());
                break;
            case Character.Loholt:
				tempMovement.speed = loholtSpeed;
				temp.AddComponent<RecallShot>();
				tempInputManager = temp.GetComponent<RecallShot>();
				Destroy(temp.GetComponent<InputManager>());
				break;
			case Character.Bastet:
				tempMovement.speed = bastetSpeed;
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

		for(int i = 0; i < System.Enum.GetValues(typeof(BulletType)).Length; i++) {
			if(bullets.types[(int)character].projectileTypes[i].volleys.Length != tempInputManager.mashBufferSize) {
                Debug.LogError("Mismatch between MashBufferSize and number of volleys specified for character " + character.ToString());
                Debug.LogError("mashbuffer size  " + tempInputManager.mashBufferSize);
            }
        }
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
