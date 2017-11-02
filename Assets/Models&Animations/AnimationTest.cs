using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

	Animator animator;
	[SerializeField]
	float speed = 0;
	private string animState;
	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetFloat ("Speed", speed);

		//variables for walk-run animation transition: speed
			if (Input.GetKey (KeyCode.W)) {
			if(speed  < 1)
				speed += 3 * Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.S)) {
			if(speed > 0)
				speed-= 3 * Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.D)) {
				transform.Rotate (Vector3.up * Time.deltaTime * 100);
			}
			if (Input.GetKey (KeyCode.A)) {
				transform.Rotate (Vector3.down * Time.deltaTime * 100);
			}


		//fire animation state trigger
		if (Input.GetKeyDown(KeyCode.F)) {
			animator.SetTrigger ("Fire");
		}

		//cover animation state trigger
		if (Input.GetKeyDown(KeyCode.C)) {
			if (animator.GetBool ("Cover")) {
				animator.SetBool ("Cover", false);
			} else {
				animator.SetBool ("Cover", true);
			}
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			animator.SetTrigger ("Grenade");
		}



	}

}
