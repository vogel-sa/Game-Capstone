using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventBroadcast : MonoBehaviour {

	public delegate void CollisionEvent(Collision col);
	public event CollisionEvent onCollideEnter;

	public delegate void TriggerEvent(Collider col);
	public event TriggerEvent onTriggerEnter;

	public event CollisionEvent onCollideExit;
	public event TriggerEvent onTriggerExit;

	void OnCollisionEnter(Collision col)
	{
		if (onCollideEnter != null)
			onCollideEnter (col);
	}

	void OnTriggerEnter(Collider col)
	{
		Debug.Log ("triggered");
		if (onTriggerEnter != null)
			onTriggerEnter (col);
	}

	void OnCollisionExit(Collision col)
	{
		if (onCollideExit != null)
			onCollideExit (col);
	}

	void OnTriggerExit(Collider col)
	{
		if (onTriggerExit != null)
			onTriggerExit (col);
	}
}
