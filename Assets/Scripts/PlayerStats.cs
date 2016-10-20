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

        if (health > 0) {
			lifeBar.GetComponent<BarController>().LerpLifeChange(health / maxHealth);

		}
		else {
            lifeBar.GetComponent<BarController>().LerpLifeChange(0);
            health = 0;
        }
	}

}
