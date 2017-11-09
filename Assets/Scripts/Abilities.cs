using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour {

    private static object _lock = new object();
    private static Abilities _instance;
    public static Abilities Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    var inst = FindObjectOfType(typeof(Abilities)) as Abilities;
                    _instance = inst ? inst : new GameObject().AddComponent<Abilities>();
                }
            }
            return _instance;
        }
    }

    public void BasicShootAbility(PlayerCharacterStats stats) { StartCoroutine(_basicShootAbility(stats)); }
    private IEnumerator _basicShootAbility(PlayerCharacterStats stats)
    {
        LineRenderer aimLine = new GameObject().AddComponent<LineRenderer>();

        

        yield return null; //Wait 1 frame.
        
		// TODO: Control Ability (exit using escape)
		while(!Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimLine.SetPositions(new Vector3[] { stats.transform.position, mousePos/* TODO: Change to be just a line in the correct direction. */ });
        }
        // TODO: Animation
        Debug.Log("Bang");
        yield return new WaitForSeconds(.5f);
        yield return null;
    }
}
