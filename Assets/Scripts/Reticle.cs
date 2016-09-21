using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	public float jabDamage;
	public float spinDamage;
	public float jabCooldown;
	public float spinCooldown;

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
						collider.gameObject.GetComponent<PlayerMovement>().StartKnockback(transform, spinDamage);
						collider.gameObject.GetComponent<InputManager>().SetExponentCooldownTimer(spinCooldown);
					} else {
						collider.gameObject.GetComponent<PlayerMovement>().StartKnockback(transform, jabDamage);
						collider.gameObject.GetComponent<InputManager>().SetExponentCooldownTimer(jabCooldown);
                        Debug.Log("cooldown now");
					}
					string hitSparkSpritePath = string.Concat("sprites/hitSparks/hit", color == Color.blue ? "BR" : "RB");
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
                    return;
				} else if(collider.gameObject.tag != "Reticle"){
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
				}
				audio.Play();
			}
		}
	}

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}
}
