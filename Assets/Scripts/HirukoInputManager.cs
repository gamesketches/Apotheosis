﻿using UnityEngine;
using System.Collections;

public class HirukoInputManager : InputManager {

	// Use this for initialization
	void Start () {
	base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	base.Update();
	}

	public override void Fire ()
	{
		//playerMovement.ResetReticle();
		if(exponentCooldownTimer <= 0) {
				OffScreenShot();
		}
		for(int i = 0; i < mashBufferSize; i++){
			mashBuffer.SetValue('*', i);
		}
		// This will be the hardest part to get right
		int lockFrames = (bufferIter * (bufferIter + 1));
		if(bufferIter < mashBufferSize) {
			lockFrames = lockFrames / 2;
		}
		// TODO: CHANGE THIS TO SOMETHIGN REAL
		exponentCooldownTimer = lockFrames * Time.deltaTime;
		bufferIter = 0;
		mashing = false;
		gameObject.transform.localScale = new Vector3(1, 1, 1);
	}

	void OffScreenShot() {
		for(int i = 0; i < 4; i++) {
			GameObject newBullet = bullets.GetBullet();
			newBullet.transform.position = new Vector3(-30 + (15 * i), 20, 0);
			newBullet.transform.rotation = Quaternion.Euler(0f,0f,270f);
			BulletLogic bulletLogic = newBullet.GetComponent<BulletLogic>();
			// just values I'm making up
			bulletLogic.Initialize(BulletType.Boomerang, 10, 20, 2, 10, playerStats.playerColor, playerStats.number, playerStats.character);
			newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder = 9 - bufferIter;
		}
		for(int i = 0; i < 4; i++) {
			GameObject newBullet = bullets.GetBullet();
			newBullet.transform.position = new Vector3(-30 + (15 * i), -20, 0);
			newBullet.transform.rotation = Quaternion.Euler(0f,0f,90f);
			BulletLogic bulletLogic = newBullet.GetComponent<BulletLogic>();
			// just values I'm making up
			bulletLogic.Initialize(BulletType.Boomerang, 10, 20, 2, 10, playerStats.playerColor, playerStats.number, playerStats.character);
			newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder = 9 - bufferIter;
		}
		for(int i = 0; i < 4; i++) {
			GameObject newBullet = bullets.GetBullet();
			newBullet.transform.position = new Vector3(-35, -15 + (i * 8), 0);
			newBullet.transform.rotation = Quaternion.Euler(0f,0f,0f);
			BulletLogic bulletLogic = newBullet.GetComponent<BulletLogic>();
			// just values I'm making up
			bulletLogic.Initialize(BulletType.Boomerang, 10, 20, 2, 10, playerStats.playerColor, playerStats.number, playerStats.character);
			newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder = 9 - bufferIter;
		}
		for(int i = 0; i < 4; i++) {
			GameObject newBullet = bullets.GetBullet();
			newBullet.transform.position = new Vector3(35, -15 + (i * 8), 0);
			newBullet.transform.rotation = Quaternion.Euler(0f,0f,0f);
			BulletLogic bulletLogic = newBullet.GetComponent<BulletLogic>();
			// just values I'm making up
			bulletLogic.Initialize(BulletType.Boomerang, 10, 20, 2, 10, playerStats.playerColor, playerStats.number, playerStats.character);
			newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder = 9 - bufferIter;
		}
	}
}
