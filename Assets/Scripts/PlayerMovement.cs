using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float speed;
	public float reticleRadius;
	public bool locked;
	public int bufferIter;

	public Reticle reticle;

	private int direction = 8;

	private float radians;
	private float degrees;

	private string horizontalAxis;
	private string verticalAxis;

	private Rigidbody2D rb2D;

	private Animator anim;

	private PlayerStats playerStats;
	private new SpriteRenderer renderer;

	void Awake() {
		locked = false;
	}

	void Start() {
		playerStats = GetComponent<PlayerStats>();
		gameObject.layer = playerStats.number + 8;
		bufferIter = 0;

		reticle.gameObject.layer = gameObject.layer;
		rb2D = GetComponent<Rigidbody2D>();
		anim.speed = 0.6f;
		if(playerStats.number == 0) {
			radians = 0.0f;
		} else if(playerStats.number == 1) {
			radians = Mathf.PI;
		}
		degrees = radians * Mathf.Rad2Deg;
		SetReticle();
		renderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		HandleMovement();
	}

	void HandleMovement() {
		if(!locked) {
			float computedSpeed =  speed * (float)(1 - 0.1 * bufferIter);
			rb2D.velocity = (new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis))).normalized * (float)computedSpeed;
				anim.SetInteger("xAxis", (int) rb2D.velocity.x);
				anim.SetInteger("yAxis", (int) rb2D.velocity.y);
				if(playerStats.character == Character.Hiruko) {
					renderer.flipX = rb2D.velocity.x < 0 ? true : false;
				}
				if(rb2D.velocity.x != 0.0f || rb2D.velocity.y != 0.0f) {
					radians = Mathf.Atan2(rb2D.velocity.y, rb2D.velocity.x);
					degrees = radians * Mathf.Rad2Deg;
					if(degrees < 0.0f) {
						degrees += 360.0f;
					}
					SetReticle();
				}
			}
		else {
			rb2D.velocity = Vector2.zero;
		}
	}

	public float CurrentShotAngle() {
			return radians * Mathf.Rad2Deg;
	}

	public void InitializeAxes(string[] controls) {
		horizontalAxis = controls[0];
		verticalAxis = controls[1];
	}

	public void SetReticle() {
		reticle.GetRigidbody2D().MovePosition((Vector2)transform.position + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * reticleRadius);
		reticle.GetRigidbody2D().MoveRotation(degrees - 90.0f);
	}

	public void SetAnimator(AnimatorOverrideController value) {
		Debug.Log(value.animationClips[0]);
		anim = GetComponent<Animator>();
		anim.runtimeAnimatorController = value;
	}

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}
}
