using UnityEngine;
using System.Collections;
public class OffscreenShot : InputManager {


    private float lifetime;

    // Use this for initialization
    void Awake () {
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

        lifetime = 5.8f; 
    }
	
	// Update is called once per frame
	void Update () {
		playerMovement.bufferIter = bufferIter;
		if(playerMovement.locked) {
			return;
		}
		char button = GetButtonPress();
        meleeCooldownTimer -= Time.deltaTime;

        if (button == 'D' && meleeCooldownTimer <= 0) {
			reticle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", playerStats.character.ToString(), playerStats.number == 0 ? "" : "Alt", "/Melee"));
			Melee();
		} else if(button != '0' && exponentCooldownTimer <= 0 && !melee) {
			shotCooldownTimer = shotCooldownTime;
            if (button != 'D') // threw everything in here to get this cooldown not to interfere with sword. works.
            {
				if (bufferIter >= mashBufferSize) {
					Fire();
				}
				else {
	                //gameObject.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(fullBufferScale, fullBufferScale, fullBufferScale),(float)bufferIter / (float)mashBufferSize);
	                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(fullBufferScale, fullBufferScale, fullBufferScale),(float)bufferIter / (float)mashBufferSize);
                    mashBuffer.SetValue(button, bufferIter);
   	         
				    if(!mashing) {
					    mashing = true;
				    }
				    if(button == 'B') {
			  	    	OffScreenShot();
			  	    }
			  	    else {
			  	    	ExponentShot();
			  	    }
					if (poweredUpBuffer || bufferIter < mashBufferSize)
                    {
                        bufferIter++;

                    }
            }
           }
		} else if(mashing && button == '0' && !melee ){
			shotCooldownTimer -= Time.deltaTime;

            if (shotCooldownTimer <= 0.0f) {
				Fire();
			}
		}

		if(exponentCooldownTimer > 0) { 
			exponentCooldownTimer -= Time.deltaTime;
			renderer.color = new Color(0.5f, 0.5f, 0.5f);
		}
		else {
			renderer.color = new Color(1f, 1f, 1f);
		}
	}

	public override void Fire ()
	{
		GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
		temp.transform.position = gameObject.transform.position;
		temp.transform.rotation = Quaternion.identity;
		temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("bufferFull");

		//playerMovement.ResetReticle();
		if(exponentCooldownTimer <= 0) {
			if(mashBuffer[mashBuffer.Length - 1] == 'B') {
				bufferIter--;
				OffScreenShot();
			}
			else {
				base.InputEqualsNumber();
			}
		}
		base.ResetBuffer();
	}

	void OffScreenShot() {
		BulletType type = BulletType.Boomerang;
		Direction dir = playerMovement.lastDirection;
		BulletDepot.Volley volley = bullets.types[(int)playerStats.character].projectileTypes[(int)type].volleys[bufferIter];
        


		for(int i = 0; i < volley.volley.Length; i++) {
			BulletDepot.Bullet bullet = volley.volley[i];
			int angle = bullet.angle + (int)playerMovement.CurrentShotAngle();	
	   	    GameObject newBullet = bullets.GetBullet();
	   	    switch(dir) {
	   	    	case Direction.Right:
					newBullet.transform.position = new Vector3(42, -25 + (60 / volley.volley.Length) * i, 0);
					break;
				case Direction.Left:
					newBullet.transform.position = new Vector3(-42, -25 + (60 / volley.volley.Length) * i, 0);
					break;
				case Direction.Down:
					newBullet.transform.position = new Vector3(-42 + (60 / volley.volley.Length) * i, -25, 0);
					break;
				case Direction.Up:
					newBullet.transform.position = new Vector3(-42 + (60 / volley.volley.Length) * i, 25, 0);
					break;
	   	    }

			newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
			BulletLogic bulletLogic = newBullet.GetComponent<BulletLogic>();//((GameObject)Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, angle))).GetComponent<BulletLogic>();
            bulletLogic.Initialize(type, bullet.damage, bullet.speed, bullet.size, lifetime, playerStats.playerColor, playerStats.number, playerStats.character);
            bulletLogic.indirectHomingTime = 10.5f;
            bulletLogic.indirectHomingLimit = 1.0f;
            bulletLogic.indirectCorrectionSpeed = 1.5f;
            newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder = 9 - bufferIter;
            	  
		}
		soundEffects.clip = Resources.Load<AudioClip>("audio/soundEffects/boomerangSound");
		soundEffects.Play();
	}


	void OffScreenFinalShot() {
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
