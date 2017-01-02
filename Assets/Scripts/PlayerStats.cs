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
	private BarController bufferBarController;

    public float screenShake;

    
    void Start() {
        bufferBarController = bufferBar.GetComponent<BarController>();
    }

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

	public void UpdateBufferBar(float scale) {
		bufferBarController.LerpChange(scale);
	}

    IEnumerator ShakeCamera(float damage)
    {
        if(damage >= 4.0f) // starting with shaking camera yes/no at 4 damage or higher. later we'l try more dynamism
        {
            float shakeScale = Mathf.Pow(2.0f, damage - 4.0f)/4.0f;
            Vector3 shakeX = new Vector3(screenShake * shakeScale, 0, 0);
           	if(shakeX.x > 4) {
           		shakeX.x = 4;
           	}
            Debug.Log("Screen shake: " + shakeX + " shakeScale :" + shakeScale + "damage: " + damage);
            Camera.main.transform.position += shakeX / 2.0f;
            yield return new WaitForSeconds(0.05f);
            Camera.main.transform.position -= shakeX;
            yield return new WaitForSeconds(0.05f);
            Camera.main.transform.position += shakeX;
            yield return new WaitForSeconds(0.05f);
            Camera.main.transform.position -= shakeX;
            yield return new WaitForSeconds(0.05f);
            Camera.main.transform.position += shakeX / 2.0f;
            yield return null;
            //shake camera
        }
    }

}
