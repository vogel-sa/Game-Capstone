using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class AbilityData{

    [SerializeField]
    string _name;
    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }
    [SerializeField]
    string _description;
    public string Description
    {
        get
        {
            return _description;
        }

        set
        {
            _description = value;
        }
    }

    [SerializeField]
    string _damageAmount;
    public string DamageAmount
    {
        get
        {
            return _damageAmount;
        }

        set
        {
            _damageAmount = value;
        }
    }

    [SerializeField]
    int _maxcooldown;
    public int Maxcooldown
    {
        get
        {
            return _maxcooldown;
        }

        set
        {
            _maxcooldown = value;
        }
    }

    [SerializeField]
    int _currcooldown;
    public int Currcooldown
    {
        get
        {
            return _currcooldown;
        }

        set
        {
            _currcooldown = value;
        }
    }







    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
