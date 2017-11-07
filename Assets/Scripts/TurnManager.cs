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

        if (Input.GetKeyDown(KeyCode.Space))
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
                    PlayerMovementManager.Instance.enabled = false;
                    break;
                }
		}

		Debug.Log ("End Current Turn");
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
