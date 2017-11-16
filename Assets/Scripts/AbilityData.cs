using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AbilityData{

    /// <summary>
    /// Name of the Ability
    /// </summary>
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

    /// <summary>
    /// Description for the Ability
    /// </summary>
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

    /// <summary>
    /// The amount of damage for said ability
    /// </summary>
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

    /// <summary>
    /// The max Cooldown for this ability. The cooldown listed when the ability is first used
    /// </summary>
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

    /// <summary>
    /// The current cooldown of this ability,  ability is disabled if it is > 0
    /// </summary>
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
    [SerializeField]
    UnityEvent _ability;
    public UnityEvent Ability
    {
        get
        {
            return _ability;
        }
    }

    [SerializeField]
    Sprite _abilitySprite;
    public Sprite AbilitySprite
    {
        get
        {
            return _abilitySprite;
        }
    }
    
    /// <summary>
    /// READ BEFORE SETTING THIS DICTIONARY
    /// Use "Range" for range of ability
    /// Use "Area of Effect" for radius
    /// </summary>
    [SerializeField]
    StringFloatDictionary _othervalues;
    public StringFloatDictionary OtherValues
    {
        get
        {
            return _othervalues;
        }

        set
        {
            _othervalues = value;
        }
    }
}

[Serializable]
public class StringFloatDictionary : SerializableDictionary<string, float> {

    public StringFloatDictionary(IDictionary<string, float> dict) :base(dict)
    {

    }
}
