using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Flare : MonoBehaviour {
    [SerializeField]
    private int turns = 3;

	// Use this for initialization
	void Awake()
    {
        PlayerMovementManager.Instance.OnSelect += Countdown;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.GetMask("Ground"))
        {
            var body = GetComponent<Rigidbody>();
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
            body.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void Countdown()
    {
        if ((turns -= 1) == 0)
        {
            Destroy(gameObject);
        }
        
    }
}
