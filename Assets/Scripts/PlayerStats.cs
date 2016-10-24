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
	public GameObject bufferBar;

    

	public void TakeDamage(float damage) {
		health -= damage;

        if (health > 0) {
			lifeBar.GetComponent<BarController>().LerpChange(health / maxHealth);
            StartCoroutine(ShakeCamera(damage));

		}
		else {
            lifeBar.GetComponent<BarController>().LerpChange(0);
            health = 0;
        }
	}

    IEnumerator ShakeCamera(float damage)
    {
        if(damage >= 4.0) // starting with shaking camera yes/no at 4 damage or higher. later we'l try more dynamism
        {
            Camera.main.transform += 4.0f;
            yield return WaitForSeconds(0.25f);
            Camera.main.transform -= 8.0f;
            yield return WaitForSeconds(0.25f);
            Camera.main.transform -= 8.0f;
            yield return WaitForSeconds(0.25f);
            Camera.main.transform += 8.0f;
            yield return WaitForSeconds(0.25f);
            Camera.main.transform -= 4.0f;
            yield return null;
            //shake camera
        }
    }

}
