using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSelectCharacter : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    var obj = hit.transform;
                    while (obj.parent && obj.parent.tag == "Player")
                    {
                        obj = obj.parent;
                    }
                    var stats = obj.GetComponentInParent<PlayerCharacterStats>();
                    PlayerMovementManager.Instance.Select(stats);
                }
            }
        }
	}
}
