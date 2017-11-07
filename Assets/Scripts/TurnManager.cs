﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

	private static TurnManager theInstance;

	public static TurnManager instance{
		get{
			if (!theInstance) {
				var inst = FindObjectOfType<TurnManager> ();
				theInstance = inst ? inst : new GameObject ().AddComponent<TurnManager> ();
			}
			return theInstance;
		}
	}

    public enum GAMESTATE
    {
        PLAYERTURN,
        ENEMYTURN
    }

    public GAMESTATE currentTurn;

	int soldierLeft; //soldiers didn't move
    [SerializeField]
	List<PlayerCharacterStats> playerList;
	List<GameObject> enemyList;

	void Awake(){
		theInstance = this;

        foreach (var x in GameObject.FindGameObjectsWithTag("Player")) {
            if (x.GetComponent<PlayerCharacterStats>() != null)
            {
                playerList.Add(x.GetComponent<PlayerCharacterStats>());
            }
        }
	}

	void Start () {
		//UpdateAllCombatMembers ();
	}
	

	void Update () {

        if (Input.GetKeyDown(KeyCode.Space) && currentTurn == GAMESTATE.ENEMYTURN)
        {
            SwitchTurn();
        }
    }

	private void OnPlayerTurnStart(){

        PlayerMovementManager.Instance.enabled = true;

        foreach (var players in playerList)
        {
            players.hasMoved = false;
        }
	}

    private void OnEnemyTurnStart()
    {
        PlayerMovementManager.Instance.enabled = false;
    }

    /// <summary>
    /// Switches the Current turn to the Other turn
    /// </summary>
	public void SwitchTurn(){

        switch (currentTurn) {

		case GAMESTATE.ENEMYTURN:
                {
                    currentTurn = GAMESTATE.PLAYERTURN;
                    OnPlayerTurnStart();
                    break;
                }

		case GAMESTATE.PLAYERTURN:
                {
                    currentTurn = GAMESTATE.ENEMYTURN;
                    OnEnemyTurnStart();
                    break;
                }
                
		}
	}

    public void EndPlayerTurn()
    {
        if (currentTurn == GAMESTATE.PLAYERTURN)
        {
            SwitchTurn();
        }
    }

    /// <summary>
    /// Checks if all players have moved, then auto ends the players turn if ALL player characters have moved
    /// </summary>
    public void AutoEndTurnCheck()
    {
        if (HaveAllPlayersMoved())
        {
            SwitchTurn();
        }
    }

	private bool HaveAllPlayersMoved(){

        foreach (var playerchars in playerList)
        {
            if (!playerchars.hasMoved)
            {
                return false;
            }
        }
        return true;
	}

}
