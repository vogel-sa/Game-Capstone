using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class StreetLight : MonoBehaviour {

	Light lt;
	VolumetricLight vl;
	ConeCollider cone;

    [SerializeField, Range(.01f, 179f)]
    private int angle;
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
    private float range;
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
        //cone = GetComponent<ConeCollider>();
        //if (!cone) cone = gameObject.AddComponent<ConeCollider>();
        //lt = GetComponent<Light>();
        //if (!lt) lt = gameObject.AddComponent<Light>();
        //vl = GetComponent<VolumetricLight>();
        //if (!vl) vl = gameObject.AddComponent<VolumetricLight>();
        //cone.Angle = angle;
        //cone.Distance = range / 2;
        //cone.IsTrigger = true;
        //cone.Init();

        //transform.rotation = Quaternion.Euler(90, 0, 0);
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider col) {
		if (LayerMask.LayerToName (col.gameObject.layer) == "Enemy") {
            col.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => x.enabled = true);
            col.GetComponentsInChildren<cakeslice.Outline>().ToList().ForEach(x => x.enabled = true);
        }
	}
}
