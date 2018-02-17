using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light), typeof(ConeCollider))]
public class StreetLight : MonoBehaviour {

	Light lt;
	VolumetricLight vl;
	ConeCollider cone;

	[SerializeField, Range(.01f, 179f)]
	private int angle = 30;
	public int Angle
	{ get
		{
			return angle;
		}
		set
		{
			cone.Angle = angle = value / 2;
			lt.spotAngle = value;
		}
	}
	[SerializeField]
	private float range = 10;
	public float Range
	{
		get
		{
			return range;
		}
		set
		{
			cone.Distance = lt.range = range = value;

		}
	}
	// Use this for initialization
	void Start () {
		cone = GetComponent<ConeCollider>();
		if (!cone) cone = gameObject.AddComponent<ConeCollider>();
		lt = GetComponent<Light>();
		if (!lt) lt = gameObject.AddComponent<Light>();
		vl = GetComponent<VolumetricLight> ();
		if (!vl) vl = gameObject.AddComponent<VolumetricLight> ();
		cone.Angle = angle / 2;
		cone.Distance = range;
		cone.IsTrigger = true;
		cone.Init();
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider col) {
		if (LayerMask.LayerToName (col.gameObject.layer) == "Enemy") {
			col.GetComponent<MeshRenderer> ().enabled = true;
		}
	}
}
