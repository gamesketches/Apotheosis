using UnityEngine;
using System.Collections;
using System;

public class RecallShot : InputInterpretter {

	float recallButtonHoldTime = 0.5f;
	float recallButtonHeldTimer = 0f;

	// Use this for initialization
	void Start () {
		mashBufferSize = 8;
		shotCooldownTime = 0.25f;
		exponentCooldownTime = 0.1f;
		meleePressWindow = 0.25f;
		jabTime = 0.08f;
		jabSpeed = 150f;
		spinTime = 0.75f;
		spinSpeed = 1155.27f;
		spinRadius = 2.5f;
		fullBufferScale = 2f;

		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();
		if(Input.GetButton(buttonB)) {
			if(recallButtonHeldTimer < recallButtonHoldTime) {
				recallButtonHeldTimer += Time.fixedDeltaTime;
			}
			else {
				Fire();
				recallButtonHeldTimer = 0;
			}
		}
	}

	public override void Fire ()
	{
		if (shotCooldownTimer <= 0.0f) {
				base.ResetBuffer();
		}
		if(recallButtonHeldTimer < recallButtonHoldTime) {
			return;
		}
		GameObject[] activeBullets = GameObject.FindGameObjectsWithTag("Boomerang");

		Array.Sort(activeBullets, delegate(GameObject bulletX, GameObject bulletY){
										float myDist = Vector3.Distance(transform.position, bulletX.transform.position);
										float theirDist = Vector3.Distance(transform.position, bulletY.transform.position);
										return myDist.CompareTo(theirDist);});

		for(int i = (int)(activeBullets.Length *  0.7f); i < activeBullets.Length; i++) {
			if(activeBullets[i].layer == gameObject.layer) {
				StartCoroutine(BulletRecall(activeBullets[i].transform, transform.position));
			}
		}
	}

	IEnumerator BulletRecall(Transform bullet, Vector3 endPosition) {
		float t = 0;
		Vector3 startPosition = bullet.position;
		BulletLogic stats = bullet.GetComponent<BulletLogic>();
		while(t < 1) {
			stats.lifetime = bulletLifetime;
			bullet.position = Vector3.Lerp(startPosition, endPosition, t);
			t += Time.deltaTime;
			yield return null;
		}
	}
}
