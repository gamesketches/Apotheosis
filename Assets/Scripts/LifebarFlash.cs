using UnityEngine;
using System.Collections;

public class LifebarFlash : MonoBehaviour {

	private new SpriteRenderer renderer;
	private Color startColor;
	public Color flashColor;
	public float flashTime;
	private float flashTimer;

	// Use this for initialization
	void Start () {
		flashTimer = 0;
		renderer = GetComponent<SpriteRenderer>();
		startColor = renderer.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(flashTimer > 0) {
			renderer.color = flashColor;
			flashTimer -= Time.deltaTime;
		}
		else {
			renderer.color = startColor;
		}
	}

	public void Flash() {
		flashTimer = flashTime;
	}

}
