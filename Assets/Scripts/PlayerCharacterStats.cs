using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterStats : MonoBehaviour, ICharacterStats
{
    [SerializeField]
    //Name of Player Character
    string name;
    [SerializeField]
    //Integer value of player character's max hit points
    private int maxHP;
    [SerializeField]
    //Integer value of player character's current hit points
    private int currHP;
    [SerializeField]
    /// <summary>
    /// Represents a scalar damage reduction value   
    /// A value of 1 would reduce all incoming damage by 1
    /// Value of 2 reduces all incoming damage by 2, etc.
    /// </summary>
    private int dmgMitigation = 0;
    [SerializeField]
    ///Number of squares a player is able to move in a single turn
    private int movementRange;
    ///Indicator whether or not this character has moved or not
    public bool hasMoved;
    /// <summary>
    /// Number of Actions a character has in a turn. Default at turn start is 1,
    /// Every attack on the bar costs 1. Free Actions cost 0.
    /// </summary>
    [SerializeField]
    private int actionsleft;

    [SerializeField]
    Texture2D portrait;



    public int GetMovementRange()
    {
        return movementRange;
    }

    public bool IsDead()
    {
        return currHP <= 0;
    }

    public void ChangeMitigationValue(int value)
    {
        dmgMitigation = value;
    }

    public int TakeDamage(int damage)
    {
        if (dmgMitigation >= damage)
        {
            return 0;
        }
        var dmgtaken = damage - dmgMitigation;

        currHP = Math.Max(currHP - dmgtaken, 0);
        return dmgtaken;
    }

    public void OnTurnStart()
    {
        hasMoved = false;
        actionsleft = 1;

    }

    public Texture2D getCharacterPortrait()
    {
        return portrait;
    }

    public string getCharacterName()
    {
        return name;
    }
}
