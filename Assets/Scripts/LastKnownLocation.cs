using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastKnownLocation : MonoBehaviour {
    private EnemyStats _stats = null;
    public EnemyStats stats {
        get
        {
            return _stats;
        }
        set
        {
            _stats = value;
            rend = stats.GetComponentInChildren<MeshRenderer>();
        }
    }
    private MeshRenderer rend;
    private MeshRenderer myRend;
    Vector3 pos;
	// Use this for initialization
	void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        myRend.enabled = false;
    }
	// Update is called once per frame
	void Update () {
		if (stats && rend.enabled)
        {
            transform.position = AstarPath.active.data.gridGraph.GetNearest(stats.transform.position).clampedPosition;
            myRend.enabled = true;
        }
	}
}
