﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarController : MonoBehaviour {

	private new Image renderer;
	public Color startColor;
	public Color damageColor;
	public float flashTime;
	public float changeSpeed;
	public int changeDirection;
	private float flashTimer;
	private Vector3 startPosition;

	void Start () {
		startPosition = transform.position;
		flashTimer = 0;
		renderer = GetComponent<Image>();
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

	public void LerpChange(float newScale) {
        if (newScale < transform.localScale.x) {
			StartCoroutine(ChangeValue(newScale));
			Flash(damageColor);
		}
		else if(newScale > transform.localScale.x) {
			StartCoroutine(ChangeValue(newScale));
			}
	}

	private IEnumerator ChangeValue(float newScale) {
		float t = 0;
		Vector3 startingScale = transform.localScale;
		Vector3 endingScale = new Vector3(newScale, startingScale.y, startingScale.z);
		Vector3 startPos = transform.position;
		Vector3 endPos = new Vector3(startPosition.x + (endingScale.x) * changeDirection, startPos.y, startPos.z);
        //Debug.Log("vector info. startingscale:" + startingScale + "endingscale:" + endingScale + "startpos:" + startPos + "endpos:" + endPos);

        while (t < changeSpeed) {
			transform.localScale = Vector3.Lerp(startingScale, endingScale, t / changeSpeed);
			//transform.localPosition = Vector3.Lerp(startPos, endPos, t / changeSpeed);
			t += Time.deltaTime;
			yield return null;
		}
	}


    public void changeBarColor(Color newColor)
    {
        startColor = newColor;
    }
}
