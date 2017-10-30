using System;
using UnityEngine.UI;
using UnityEngine;

public interface ICharacterStats
{
    /// <summary>
    /// Returns whether this player character's hp has reached zero
    /// </summary>
    /// <returns></returns>
    bool IsDead();
    /// <summary>
    /// Returns the Movement range of the character
    /// </summary>
    /// <returns>movement range as an Integer</returns>
    int GetMovementRange();

    /// <summary>
    /// Reduces the HP of this player character by the given amount
    /// FACTORING in only personal damage mitigation and not outside factors
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>dmg taken after self-mitigation calculated</returns>
    int TakeDamage(int damage);

    /// <summary>
    /// Permanently changes the damage mitigation value
    /// </summary>
    /// <param name="value"> value to change the damage mitigation to to</param>
    void ChangeMitigationValue(int value);
   
    /// <summary>
    /// Handles all onTurn start effects, such as resetting movement flags and action count
    /// </summary>
    void OnTurnStart();

    /// <summary>
    /// Returns this character's designated base portrait
    /// </summary>
    /// <returns></returns>
    Texture2D getCharacterPortrait();

    /// <summary>
    /// returns character's name
    /// </summary>
    /// <returns></returns>
    string getCharacterName();

}
