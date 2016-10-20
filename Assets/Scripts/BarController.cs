﻿using UnityEngine;
using System.Collections;

public class BarController : MonoBehaviour {

	private new SpriteRenderer renderer;
	public Color startColor;
	public Color damageColor;
	public float flashTime;
	public float changeSpeed;
	private float flashTimer;

	void Start () {
		flashTimer = 0;
		renderer = GetComponent<SpriteRenderer>();
		startColor = renderer.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(flashTimer > 0) {
			flashTimer -= Time.deltaTime;
		}
		else {
			renderer.color = startColor;
		}
	}

	public void Flash(Color color) {
		renderer.color = color;
		flashTimer = flashTime;
	}

	public void LerpLifeChange(float newScale) {
        if (newScale < transform.localScale.x) {
			StartCoroutine(ChangeLifeValue(newScale));
			Flash(damageColor);
		}
	}

	private IEnumerator ChangeLifeValue(float newScale) {
		float t = 0;
		Vector3 startingScale = transform.localScale;
		Vector3 endingScale = new Vector3(newScale, startingScale.y, startingScale.z);
		while(t < changeSpeed) {
			transform.localScale = Vector3.Lerp(startingScale, endingScale, t / changeSpeed);
			t += Time.deltaTime;
			yield return null;
		}
	}


    public void changeLifebarColor(Color newColor)
    {
        startColor = newColor;
    }
}
