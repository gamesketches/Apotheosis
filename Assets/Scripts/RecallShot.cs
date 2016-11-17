using UnityEngine;
using System.Collections;

public class RecallShot : InputManager {

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

		float maxDistance = 0;
		for(int i = 0; i < activeBullets.Length; i++) {
			if(activeBullets[i].layer == gameObject.layer) {
				float distance = Vector3.Distance(transform.position, activeBullets[i].transform.position);
				if(distance > maxDistance) {
					maxDistance = distance;
				}
			}
		}

		for(int i = 0; i < activeBullets.Length; i++) {
			if(activeBullets[i].layer == gameObject.layer && Vector3.Distance(transform.position, activeBullets[i].transform.position) == maxDistance) {
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
