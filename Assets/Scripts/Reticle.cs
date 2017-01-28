using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	public float jabDamage;
	public float spinDamage;
	public float jabCooldown;
	public float spinCooldown;
	public float jabHitStun = 0.1f;
	public float spinHitStun = 0.2f;

	public Color color;

	public bool melee;
	public bool spinning;

	private Rigidbody2D rb2D;

    new AudioSource audio;

	void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
		audio = GetComponent<AudioSource>();
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if(melee) {
			if(collider.gameObject.layer != gameObject.layer) {
				if(collider.gameObject.tag == "Player") {
					if(spinning) {
						StartCoroutine(Hitstop(spinHitStun));
						collider.gameObject.GetComponent<PlayerMovement>().StartKnockback(transform, spinDamage);
						collider.gameObject.GetComponent<InputInterpretter>().SetExponentCooldownTimer(spinCooldown);
					} else {
						StartCoroutine(Hitstop(jabHitStun));
						collider.gameObject.GetComponent<PlayerMovement>().StartKnockback(transform, jabDamage);
						collider.gameObject.GetComponent<InputInterpretter>().SetExponentCooldownTimer(jabCooldown);
					}
					string hitSparkSpritePath = string.Concat("sprites/hitSparks/hit", color == Color.blue ? "BR" : "RB");
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
                    return;
				} else if(collider.gameObject.tag != "Reticle"){
                    //ski turn off sword dominance 10/20/2016
                    /*
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
                    if (gameObject.layer == 8) {
                        sparks.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.1f, 0.2f);
                    }
                    else
                    {
                        sparks.GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.2f, 0.9f);
                    }
                    sparks.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
                    Destroy(collider.gameObject);
                    */
				}
				audio.Play();
			}
		}
	}

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}

	IEnumerator Hitstop(float hitStun) {
		float startTime = Time.realtimeSinceStartup;
		Time.timeScale = 0;
		while(Time.realtimeSinceStartup < startTime + hitStun) {
			Debug.Log(Time.timeSinceLevelLoad);
			Debug.Log(startTime + hitStun);
			yield return null;
		}
		Time.timeScale = 1;
	}
}
