using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    GAMESTATE currentTurn;

	int soldierLeft; //soldiers didn't move
	List<GameObject> playerList;
	List<GameObject> enemyList;

	void Awake(){
		theInstance = this;
	}

	void Start () {
		UpdateAllCombatMembers ();
	}
	

	void Update () {
		
	}

	public void OnTurnStart(){
		
		Debug.Log ("Start New Turn");
		Debug.Log ("Now is " + currentTurn + "'s turn");
	}

	public void OnTurnEnd(){
		switch (currentTurn) {

		case GAMESTATE.ENEMYTURN:
			currentTurn = GAMESTATE.PLAYERTURN;
			break;

		case GAMESTATE.PLAYERTURN:
			currentTurn = GAMESTATE.ENEMYTURN;
			break;
		}

		Debug.Log ("End Current Turn");
	}

	void AutoEndTurnCheck(){
		//checking if all soldiers have moved, if so, Auto-end turn
		/*
		if(){
		OnTurnEnd ();
		}
		*/
	}

	public void UpdateAllCombatMembers(){
		//Call this to update the player list and enemy list
		//Call this at:
		//The start of the battle
		//New member has entered the battle
		Debug.Log("TurnManager updated all Combat members");
		playerList = new List<GameObject>();
		playerList.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		enemyList = new List<GameObject>();
		enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
	}
}
