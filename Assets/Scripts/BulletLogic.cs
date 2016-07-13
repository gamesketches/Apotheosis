using UnityEngine;
using System.Collections;

public enum BulletType {Gator, Hippo, Crane};

public class BulletLogic : MonoBehaviour {

	public BulletType type;
	int damage;
	float velocity;
	float lifetime;
	public float velocityMultiplier = 10f;
	public float indirectCorrectionSpeed = 5f;
	public float indirectHomingTime = 0.5f;
	delegate void BulletFunction();
	BulletFunction bulletFunction;
	private Vector2 travelVector;
	private Transform target;
	private Vector3 targetPosition;
	private float headingTime, shieldOscillationTime;
    private Sprite sprite;
    private int animFrame;
	AudioSource audio;
	SpriteRenderer renderer;
	private Sprite[] animation; 
	AnimationCurve shieldVelocity;
   
    // Use this for initialization
    void Start () {
		renderer = GetComponentInChildren<SpriteRenderer>();
		audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("audio/soundEffects/rpsBulletCancel");
		Keyframe[] keyFrames = new Keyframe[10];
		float modulation = 0.1f;
		for(int i = 0; i < keyFrames.Length; i++) {
			keyFrames[i] = new Keyframe(0.1f * i, modulation);
			modulation *= -1;
		}
		shieldVelocity = new AnimationCurve(keyFrames);
    }
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if(lifetime <= 0f) {
			Destroy(gameObject);
		}
		bulletFunction();
		gameObject.transform.position += new Vector3(travelVector.x, travelVector.y) * Time.deltaTime;
  	}

	public void Initialize(BulletType bulletType, int bulletDamage, float Velocity, float size,
													float Lifetime, Color bulletColor, int playerNum,
													Character character){
		type = bulletType;
		damage = bulletDamage;
		transform.localScale = new Vector3(size, size, size);
		gameObject.tag = bulletType.ToString();
		velocity = Velocity * velocityMultiplier;
		if(velocity == 0) {
			velocity = 5f;
		}
		Vector3 tempVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
		travelVector = new Vector2(tempVector.x, tempVector.y);
		lifetime = Lifetime;
		gameObject.layer = 8 + playerNum;
		switch(type) {
			case BulletType.Crane:
				bulletFunction = IndirectLogic;
				sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", character.ToString(), playerNum == 0 ? "" : "Alt", "/Boomerang"));
				headingTime = 0f;
				foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
					if(player.layer != gameObject.layer) {
						target = player.transform;
						}
				}
				break;
			case BulletType.Gator:
                sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", character.ToString(), playerNum == 0 ? "" : "Alt", "/Knife"));
			Debug.Log(string.Concat("sprites/weapons/", character.ToString(), playerNum == 0 ? "" : "Alt", "/Knife"));
                // TODO: change this
                transform.Rotate(new Vector3(0f, 0f, -90f));
				bulletFunction = StraightLogic;
				break;
			// Hippo situation
			default:
			sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", character.ToString(), playerNum == 0 ? "" : "Alt", "/Shield"));
				bulletFunction = SlowShotLogic;
			velocity = 2.5f;
			GetComponent<CircleCollider2D>().radius = 0.5f;
			shieldOscillationTime = 0;
			//lifetime = Lifetime / 2;
			lifetime = Lifetime / 0.25f;
			tempVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
			travelVector = new Vector2(tempVector.x, tempVector.y);	
			break;
		}
		//renderer.sprite = sprite;
		GetComponentInChildren<SpriteRenderer>().sprite = sprite;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == "Boundary") {
			Destroy(gameObject);
			return;
		}
		if(other.gameObject.layer != gameObject.layer) {
			if(other.gameObject.tag == "Player") {
					other.gameObject.GetComponent<PlayerStats>().health -= damage;
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");	
					Destroy(gameObject);
					return;
			}
			else if(other.gameObject.tag == "Reticle") {
				return;
			}
			BulletType opposingType = (BulletType)System.Enum.Parse(typeof(BulletType), other.gameObject.tag);


			if(opposingType == type){
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("identicalBulletCancel");
				Destroy(other.gameObject);
				Destroy(gameObject);

			}
            else if ((int)type == System.Enum.GetValues(typeof(BulletType)).Length - 1)
            {
                return;
            }
            else if((int)opposingType == System.Enum.GetValues(typeof(BulletType)).Length - 1 && (int)type == 0) {
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("rpsBulletCancel");
				Destroy(gameObject);
				GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
				sparks.transform.localScale = new Vector3(10f, 10f, 10f);
				sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");	
			}
			else {
				GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
				sparks.transform.localScale = new Vector3(10f, 10f, 10f);
				sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");	
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("rpsBulletCancel");
				GameObject destroyedObject = opposingType > type ? other.gameObject : gameObject;
				Destroy(destroyedObject);
			}
		}
	}

	void IndirectLogic(){
		renderer.transform.Rotate(0, 0, 2);
		if(headingTime < indirectHomingTime) {
			targetPosition = target.position;
		}
		// Might be better to handle this shit as a rotation
		if(headingTime < 1f) {
		Vector3 startVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
		Vector3 temp = Vector3.Lerp(startVector, targetPosition - gameObject.transform.position, 
			headingTime);
		travelVector.x = temp.x;
		travelVector.y = temp.y;
		headingTime += indirectCorrectionSpeed / (indirectCorrectionSpeed * 60);
		}

	}

	void StraightLogic(){
		//travelVector = new Vector2(velocity, 0f);
	}

	void SlowShotLogic(){
		renderer.transform.position += Vector3.one * shieldVelocity.Evaluate(shieldOscillationTime);
		shieldOscillationTime += Time.deltaTime;
		if(shieldOscillationTime > 1) {
			shieldOscillationTime = 0;
			renderer.transform.localPosition = Vector3.zero;
		}
	}
}
