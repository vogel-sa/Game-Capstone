using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterStats : MonoBehaviour, ICharacterStats
{
    #region
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
    private int _maxHP;
    public int MaxHP
    {
        get
        {
            return _maxHP;
        }

        set
        {
            _maxHP = value;
        }
    }

    [SerializeField]
    private int _currHP; public int CurrHP
    {
        get
        {
            return _currHP;
        }

        set
        {
            if (CurrHP + value > _maxHP)
            {
                _currHP = _maxHP;
            }
            else if (CurrHP - value < 0)
            {
                _currHP = 0;
            }
    
            _currHP = value;
        }
    }

    /* Represents a scalar damage reduction value   
    A value of 1 would reduce all incoming damage by 1
    Value of 2 reduces all incoming damage by 2, etc.*/
    [SerializeField]
    private int _mitigationValue = 0;
    public int MitigationValue
    {
        get
        {
            return _mitigationValue;
        }

        set
        {
            _mitigationValue = value;
        }
    }

    [SerializeField]
    private int _movementRange;
    public int MovementRange
    {
        get
        {
            return _movementRange;
        }

        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Movement Range cannot be less than 1");
            }
            _movementRange = value;
        }
    }

    public bool hasMoved;
    /// <summary>
    /// Number of Actions a character has in a turn. Default at turn start is 1,
    /// Every attack on the bar costs 1. Free Actions cost 0.
    /// </summary>
    [SerializeField]
    private int _actionsleft;
    public int Actionsleft
    {
        get
        {
            return _actionsleft;
        }

        set
        {
            _actionsleft = value;
        }
    }

    [SerializeField]
    Texture2D _portrait;
    public Texture2D Portrait
    {
        get
        {
            return _portrait;
        }

        set
        {
            _portrait = value;
        }
    }
    #endregion

    #region
    //[SerializeField]
    //Add UnityEvents
    [SerializeField]
    Sprite[] abilitySprites;
    [SerializeField]
    AbilityData[] abilityData;
    #endregion

    public bool IsDead()
    {
        return CurrHP <= 0;
    }

    public int TakeDamage(int damage)
    {
        if (MitigationValue >= damage)
        {
            return 0;
        }
        var dmgtaken = damage - MitigationValue;

        CurrHP = Math.Max(CurrHP - dmgtaken, 0);
        return dmgtaken;
    }

    public void OnTurnStart()
    {
        hasMoved = false;
        Actionsleft = 1;

    }
}
