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
        Debug.Log("Bang");
        yield return null;
    }
}
