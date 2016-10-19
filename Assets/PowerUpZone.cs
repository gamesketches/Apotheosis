using UnityEngine;
using System.Collections;

public enum PowerZoneTypes {Buffer, Speed, Damage};

public class PowerUpZone : MonoBehaviour {

	public PowerZoneTypes PowerUpType;
	public float SpeedBoostTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
		if(other.tag == "Player") {
			switch(PowerUpType) {
				case PowerZoneTypes.Buffer:
					other.gameObject.GetComponent<InputManager>().poweredUpBuffer = true;
					break;
				case PowerZoneTypes.Damage:
					other.gameObject.GetComponent<InputManager>().poweredUpBullets = true;
					break;
				case PowerZoneTypes.Speed:
					other.gameObject.GetComponent<PlayerMovement>().speed *= 2;
					break;
				}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.tag == "Player") {
			switch(PowerUpType) {
				case PowerZoneTypes.Buffer:
					other.gameObject.GetComponent<InputManager>().poweredUpBuffer = false;
					break;
				case PowerZoneTypes.Damage:
					other.gameObject.GetComponent<InputManager>().poweredUpBullets = false;
					break;
				case PowerZoneTypes.Speed:
					StartCoroutine(SpeedBoost(other.gameObject.GetComponent<PlayerMovement>()));
					break;
				}
		}
	}

	IEnumerator SpeedBoost(PlayerMovement script) {
		float t = 0;
		while(t < SpeedBoostTime) {
			t += Time.deltaTime;
			yield return null;
		}

		script.speed /= 2;
	}
}
