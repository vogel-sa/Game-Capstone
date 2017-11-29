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

    //List of all player stats in the game.  remove when player character is defeated
    [SerializeField]
	List<PlayerCharacterStats> playerList;
    //List of all Enemy stats currently in game, remove when enemy is defeated
    [SerializeField]
	List<EnemyStats> enemyList;

	void Awake(){
        playerList = new List<PlayerCharacterStats>();
        enemyList = new List<EnemyStats>();
		_instance = this;
        PlayerCharacterStats player;
        EnemyStats enemy;
        foreach (var x in GameObject.FindGameObjectsWithTag("Player")) {
            if (player = x.GetComponent<PlayerCharacterStats>())
            {
                playerList.Add(player);
            }
        }
        foreach (var x in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy = x.GetComponent<EnemyStats>())
            {
                enemyList.Add(enemy);
            }
        }
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

    /// <summary>
    /// checks for victory condition
    /// </summary>
    /// <returns></returns>
    public void isVictory()
    {
       if (enemyList.Count == 0)
        {
            //TODO: End state
        }
    }

    public void isDefeat()
    {
        if (playerList.Count == 0)
        {
            //TODO:  Game Over
        }
    }
}
