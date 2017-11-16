using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

	private static TurnManager _instance;

	public static TurnManager instance{
		get{
			if (!_instance) {
				var inst = FindObjectOfType<TurnManager> ();
				_instance = inst ? inst : new GameObject ().AddComponent<TurnManager> ();
			}
			return _instance;
		}
	}

    public enum GAMESTATE
    {
        PLAYERTURN,
        ENEMYTURN
    }

    public delegate void TurnChangeAction();
    public event TurnChangeAction OnTurnChange;

    public GAMESTATE currentTurn { get; private set;}

    [SerializeField]
	List<PlayerCharacterStats> playerList;
	List<GameObject> enemyList;

	void Awake(){
        playerList = new List<PlayerCharacterStats>();
        enemyList = new List<GameObject>();
		_instance = this;
        PlayerCharacterStats player;
        foreach (var x in GameObject.FindGameObjectsWithTag("Player")) {
            if (player = x.GetComponent<PlayerCharacterStats>())
            {
                playerList.Add(player);
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
