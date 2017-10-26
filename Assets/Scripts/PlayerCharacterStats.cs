using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCharacterStats : MonoBehaviour
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
    //Number of squares a player is able to move in a single turn
    private int movementRange;
    //Indicator whether or not this character has moved or not
    public bool hasMoved;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public int getMovementRange()
    {
        return movementRange;
    }

    /// <summary>
    /// Returns whether this player character's hp has reached zero
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {
        return currHP <= 0;
    }

    /// <summary>
    /// Permanently changes the damage mitigation value
    /// </summary>
    /// <param name="value"> value to change the damage mitigation to to</param>
    public void ChangeMitigationValue(int value)
    {
        dmgMitigation = value;
    }

    /// <summary>
    /// Reduces the HP of this player character by the given amount
    /// FACTORING in only personal damage mitigation and not outside factors
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>dmg taken after self-mitigation calculated</returns>
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
}
