using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowBezier : MonoBehaviour {

	public BezierCurve curve = null;

	private float currT = 1f;
	public float scrollSpeed = 1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		currT += Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed, -1, 1);
		var newPos = curve.GetPointAt (currT);
		transform.position = newPos;
		// Fix lookat
		var lookat = curve.GetPointAt (currT - .1f);
		transform.LookAt (curve[0].position);
		transform.localRotation = Quaternion.Euler( new Vector3 (transform.localRotation.eulerAngles.x, 0, 0));
	}
}
