using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

	private static TurnManager _instance;
	private static object _lock = new object();
    private static bool applicationIsQuitting = false;
    public static TurnManager instance
	{
		get
		{
            if (applicationIsQuitting) return null;
			lock(_lock)
			{
				if (!_instance)
				{
					var inst = FindObjectOfType<TurnManager> ();
					_instance = inst ? inst : new GameObject ().AddComponent<TurnManager> ();
				}
			}
			return _instance;
		}
	}

    public enum GAMESTATE
    {
        PLAYERTURN,
        ENEMYTURN
    }

	public delegate void TurnChangeAction(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, GAMESTATE turn);
    public event TurnChangeAction OnTurnChange;

    public GAMESTATE currentTurn { get; private set;}

    //List of all player stats in the game.  remove when player character is defeated

	public IList<PlayerCharacterStats> playerList { get; private set; }
	public IList<EnemyStats> enemyList { get; private set; }
  	[SerializeField]
  	GameObject VictoryScreen;

	void Awake(){
        playerList = new List<PlayerCharacterStats>();
		enemyList = new List<EnemyStats>();
		_instance = this;
        PlayerCharacterStats player;
		currentTurn = GAMESTATE.PLAYERTURN;
		EnemyStats enemy;
        foreach (var x in GameObject.FindGameObjectsWithTag("Player")) {
            if (player = x.GetComponent<PlayerCharacterStats>())
            {
                playerList.Add(player);
            }
        }
		foreach (var x in GameObject.FindGameObjectsWithTag("Enemy")) {
			if (enemy = x.GetComponent<EnemyStats>())
			{
				enemyList.Add(enemy);
			}
		}

        VictoryScreen.SetActive(false);
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
		PlayerMovementManager.Instance.Deselect ();
		StartCoroutine (_switchTurn ());
	}

	private IEnumerator _switchTurn()
	{
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
		yield return new WaitForSeconds (.5f);
		OnTurnChange(playerList, enemyList, currentTurn);
	}

    /// <summary>
    /// Checks if a gameOver state has been reached
    /// </summary>
    public void CheckGameOver()
    {
        //if all players have been 
        if (playerList.Count == 0)
        {
            PlayerLoses();
        }
        if (enemyList.Count == 0)
        {
			StartCoroutine(GameOver(2));
        }
    }

    private IEnumerator GameOver(int waitval)
    {
        VictoryScreen.SetActive(true);
		PlayerMovementManager.Instance.enabled = false;
		TurnManager.instance.enabled = false;
        yield return new WaitForSeconds(waitval);

        //GOTO MAIN MENU

		SceneManager.LoadScene("Main Menu");
    }

    private void PlayerLoses()
    {

    }

    /// <summary>
    /// Checks if all players have moved, then auto ends the players turn if ALL player characters have moved
    /// </summary>
    public void AutoEndTurnCheck()
    {
        if (HaveAllPlayersTakenAction())
        {
            SwitchTurn();
        }
    }

	private bool HaveAllPlayersTakenAction(){

        foreach (var playerchars in playerList)
        {
            if (!playerchars.hasMoved  || playerchars.Actionsleft > 0)
            {
                return false;
            }
        }
        return true;
	}


	void OnDestroy()
	{
		applicationIsQuitting = true;
	}
}
