using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AbilityData{

    #region
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
    int _damageAmount;
    public int DamageAmount
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
    #endregion

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
    OtherAbilityValues _othervalues;
    public OtherAbilityValues OtherValues
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
public class OtherAbilityValues
{
    /// <summary>
    /// The Range of the ability.  How far the attack extends out from the player
    /// </summary>
    [SerializeField]
    private float _range;
    public float Range
    {
        get
        {
            return _range;
        }

        set
        {
            _range = value;
        }
    }

    /// <summary>
    /// The width for a lined attack
    /// </summary>
    [SerializeField]
    private float _width;
    public float Width
    {
        get
        {
            return _width;
        }

        set
        {
            _width = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private int _duration;
    public int Duration
    {
        get
        {
            return _duration;
        }

        set
        {
            _duration = value;
        }
    }

    [SerializeField]
    private float _areaofeffect;
    public float AreaOfEffect
    {
        get
        {
            return _areaofeffect;
        }

        set
        {
            _areaofeffect = value;
        }
    }

    [SerializeField]
    private float _angle;
    public float Angle
    {
        get
        {
            return _angle;
        }

        set
        {
            _angle = value;
        }
    }


}
