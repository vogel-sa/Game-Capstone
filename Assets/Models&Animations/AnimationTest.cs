using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

	Animator animator;

	private string animState;
	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKey(KeyCode.W)){
				animator.SetFloat ("Speed", 1);
			}
		if(Input.GetKey(KeyCode.S)){
			animator.SetFloat ("Speed", 0);
		}
		if(Input.GetKey(KeyCode.D)){
			transform.Rotate(Vector3.up * Time.deltaTime*100);
		}
		if(Input.GetKey(KeyCode.A)){
			transform.Rotate(Vector3.down * Time.deltaTime*100);
		}

		if (Input.GetMouseButtonDown (0)) {
			animator.SetTrigger ("Fire");
		}

	}

}
