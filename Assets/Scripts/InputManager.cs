using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	public int mashBufferSize;

	public float shotCooldownTime;
	public float exponentCooldownTime;
	public float meleePressWindow;
	public float jabTime;
	public float jabSpeed;
	public float spinTime;
	public float spinSpeed;
	public float spinRadius;
	public float fullBufferScale = 2f;

	public Reticle reticle;

	protected GameObject bulletPrefab;

	public BulletDepot bullets;

	protected int bufferIter = 0;

	protected float shotCooldownTimer;
	protected float meleeCooldownTimer;
	protected float exponentCooldownTimer;
	protected float bulletLifetime = 5;

	protected string buttonA;
	protected string buttonB;
	protected string buttonC;
	protected string buttonD;

	protected char[] mashBuffer;

	protected bool mashing;
	protected bool melee;

	protected PlayerStats playerStats;
    protected new SpriteRenderer renderer;
	protected PlayerMovement playerMovement;
	protected AudioSource soundEffects;

	public void Start() {
		soundEffects = GetComponent<AudioSource>();
		bulletPrefab = Resources.Load<GameObject>("prefabs/Bullet");
		playerStats = GetComponent<PlayerStats>();
		playerMovement = GetComponent<PlayerMovement>();
		renderer = GetComponent<SpriteRenderer>();
		//mashBufferSize = 8;
		mashBuffer = new char[mashBufferSize];
		for(int i = 0; i < mashBufferSize; i++){
			mashBuffer.SetValue('*', i);
		}
	}

	public void Update() {
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
				if (bufferIter >= mashBufferSize - 1) {
					Fire();
				}
				else {
	                //gameObject.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(fullBufferScale, fullBufferScale, fullBufferScale),(float)bufferIter / (float)mashBufferSize);
	                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(fullBufferScale, fullBufferScale, fullBufferScale),(float)bufferIter / (float)mashBufferSize);
                    mashBuffer.SetValue(button, bufferIter);
   	         
				    if(!mashing) {
					    mashing = true;
				    }
			  	    ExponentShot();
				    bufferIter++;
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

	public char GetButtonPress() {
		if(Input.GetButtonDown(buttonA)) {
			return 'A';
		}
		else if(Input.GetButtonDown(buttonB)) {
			return 'B';
		}
		else if(Input.GetButtonDown(buttonC)) {
			return 'C';
		}
		else if(Input.GetButtonDown(buttonD)) {
			return 'D';
		}
		else {
			return '0';
		}
	}

	public void ExponentShot() {
		BulletType type = BulletType.Knife;
		switch(GetButtonPress()) {
			case 'A':
				type = BulletType.Knife;
				break;
			case 'B':
				type = BulletType.Boomerang;
				break;
			case 'C':
				type = BulletType.Shield;
				break;
			case 'D':
				break;
		}
		BulletDepot.Volley volley = bullets.types[(int)playerStats.character].projectileTypes[(int)type].volleys[bufferIter];
        //Debug.Log(" volley = " + bufferIter); //ski
		foreach(BulletDepot.Bullet bullet in volley.volley) {
			CreateBullet(bullet, type);
		}
		switch(type) {
		case BulletType.Boomerang:
			soundEffects.clip = Resources.Load<AudioClip>("audio/soundEffects/boomerangSound");
			break;
		case BulletType.Knife:
			soundEffects.clip = Resources.Load<AudioClip>("audio/soundEffects/knifeSound");
			break;
		case BulletType.Shield:
			soundEffects.clip = Resources.Load<AudioClip>("audio/soundEffects/shieldSound");
			break;
		}
		soundEffects.Play();
	}

	void RecallShot() {
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

	public virtual void Fire() {
		//playerMovement.ResetReticle();
		if(exponentCooldownTimer <= 0) {
				InputEqualsNumber();
		}
		ResetBuffer();
	}

	public void ResetBuffer() {
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

	public void InputEqualsNumber() {
        int bulletNumber = 0;
		List<char> meaningfulInput = new List<char>();
		for(int i = 0; i < mashBufferSize; i++) {
			if(mashBuffer[i] != '*') {
				bulletNumber++;
				meaningfulInput.Add(mashBuffer[i]);
			}
		}
		float angleDifference = 90.0f / mashBufferSize;
        List<float> bulletAngles = new List<float>();
		List<BulletType> bulletTypes = new List<BulletType>();
		//BulletType theBulletType;
		if(meaningfulInput.Count <= 0) {
			return;
		}

        /*
        //commented is currently never runs. debug? ski
		char bulletTypeChar = meaningfulInput[meaningfulInput.Count - 1];
        
		if(bulletTypeChar == 'A') {
			theBulletType = BulletType.Knife;
		} else if(bulletTypeChar == 'B') {
			theBulletType = BulletType.Boomerang;
		} else {
			theBulletType = BulletType.Shield;
		}
        */

        for (int i = 0; i < bulletAngles.Count; i++) {
			BulletDepot.Bullet bullet = bullets.types[(int)playerStats.character].projectileTypes[(int)bulletTypes[i]].volleys[0].volley[0];
			bullet.angle = (int)bulletAngles[i];
			CreateBullet(bullet, bulletTypes[i]);
		}
	}

	public void CreateBullet(BulletDepot.Bullet bullet, BulletType type = BulletType.Knife) {
		int angle = bullet.angle + (int)playerMovement.CurrentShotAngle();
        //Debug.Log("CreateBullet(): angle =" + angle + " playerMovement.CurrentShotAngle = " + (int)playerMovement.CurrentShotAngle());  //ski

        GameObject newBullet = bullets.GetBullet();
		newBullet.transform.position = gameObject.transform.position;
		newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
		BulletLogic bulletLogic = newBullet.GetComponent<BulletLogic>();//((GameObject)Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, angle))).GetComponent<BulletLogic>();
		bulletLogic.Initialize(type, bullet.damage, bullet.speed, bullet.size, 5, playerStats.playerColor, playerStats.number, playerStats.character);
        newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder = 9 - bufferIter;
        Debug.Log("Sorting layer debug thing " + newBullet.GetComponentInChildren<SpriteRenderer>().sortingOrder);
    }

	public void InitializeControls(string[] controls) {
		buttonA = controls[2];
		buttonB = controls[3];
		buttonC = controls[4];
		buttonD = controls[5];
	}

	public void Melee() {
		if(melee && !playerMovement.locked) {
			StopCoroutine("MeleeWindow");
			StartCoroutine("Spin");
		} else {
			StartCoroutine("MeleeWindow");
		}
	}

	public IEnumerator MeleeWindow() {
		melee = true;
		float windowTimer = meleePressWindow;
		while(windowTimer > 0.0f) {
			windowTimer -= Time.deltaTime;
			yield return 0;
		}
		StartCoroutine("Jab");
	}

	public IEnumerator Jab() {
		playerMovement.GetRigidbody2D().velocity = Vector2.zero;
		playerMovement.locked = true;
		reticle.melee = true;
		reticle.GetRigidbody2D().velocity = new Vector2(Mathf.Cos(playerMovement.CurrentShotAngle() * Mathf.Deg2Rad), Mathf.Sin(playerMovement.CurrentShotAngle() * Mathf.Deg2Rad)) * jabSpeed;
		yield return new WaitForSeconds(jabTime);
		reticle.GetRigidbody2D().velocity = new Vector2(Mathf.Cos((playerMovement.CurrentShotAngle() + 180.0f) * Mathf.Deg2Rad), Mathf.Sin((playerMovement.CurrentShotAngle() + 180.0f) * Mathf.Deg2Rad)) * jabSpeed;
		yield return new WaitForSeconds(jabTime);
		reticle.GetRigidbody2D().velocity = Vector2.zero;
		playerMovement.SetReticle();
		melee = false;
		reticle.melee = false;
		playerMovement.locked = false;
		reticle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/reticle-18");
    }

    public IEnumerator Spin() {
		playerMovement.GetRigidbody2D().velocity = Vector2.zero;
		playerMovement.locked = true;
		reticle.melee = true;
		reticle.spinning = true;
		float angle = reticle.GetRigidbody2D().rotation;
		float spinTimer = spinTime;
		while(spinTimer > 0.0f) {
			spinTimer -= Time.deltaTime;
			angle += spinSpeed * Time.deltaTime;
			float radians = angle * Mathf.Deg2Rad;
			reticle.GetRigidbody2D().MovePosition((Vector2)transform.position + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * spinRadius);
			reticle.GetRigidbody2D().MoveRotation(angle - 90.0f);
			yield return 0;
		}
		playerMovement.SetReticle();
		melee = false;
		reticle.melee = false;
		reticle.spinning = false;
		playerMovement.locked = false;
		reticle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/reticle-18");
    }

    public void SetExponentCooldownTimer(float value) {
		exponentCooldownTimer = value;
	}
}
