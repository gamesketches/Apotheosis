using UnityEngine;
using System.Collections;

public class BulletDeathBehavior : MonoBehaviour {

	bool started;
	Animator anim;
	// Use this for initialization
	void Awake () {
		started = false;
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlayAnimation(string animationName, BulletType type) {
		AnimatorOverrideController animationController = new AnimatorOverrideController();
		animationController.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprites/BulletDeathAnimationController");
		string resourcePath = string.Concat("sprites/weaponDeaths/", animationName);
		foreach(AnimationClip clip in Resources.LoadAll<AnimationClip>(resourcePath)) {
				animationController[clip.name] = clip;
			}
		anim.runtimeAnimatorController = animationController;
		anim.SetInteger("type", (int) type);
		anim.SetTrigger("play");
	}
}
