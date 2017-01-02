using UnityEngine;
using System.Collections;

public class MenuBasedPlatformerMovement : MonoBehaviour {

	Rigidbody2D rb;
	Animator animator;
	public float jumpPower;
	public float speed;
	private float movementSpeed;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		movementSpeed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float vert = Input.GetAxis("Jump");
			animator.SetInteger("movement", 1);
			transform.Translate(movementSpeed * speed * Time.deltaTime, 0, 0);

			animator.SetInteger("movement", 0);
		if(!animator.GetBool("grounded")) {
			if(rb.velocity.y == 0) {
				animator.SetBool("grounded", true);
			}
		}
	}

	public void MoveRight() {
		movementSpeed = 1;
	}

	public void MoveLeft() {
		movementSpeed = -1;
	}

	public void Jump() {
		if(animator.GetBool("grounded")) {
			animator.SetBool("grounded", false);
			rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
		}
	}

	public void Stop() {
		movementSpeed = 0;
	}
}
