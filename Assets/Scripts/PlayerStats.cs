using UnityEngine;
using System.Collections;


public class PlayerStats : MonoBehaviour {
	public float health;
	public float maxHealth;
	public Color playerColor;
	public int shotCooldown;
	public int meleeCooldown;
	public int number;
	public Character character;
	public GameObject lifeBar;


	public void TakeDamage(float damage) {

		health -= damage;
		if(health > 0) {
			lifeBar.transform.localScale = new Vector3((health / maxHealth) * 0.5f, 1f, 1f);

		}
		else {
			health = 0;
		}

		lifeBar.GetComponent<LifebarFlash>().Flash();
	
	}

}
