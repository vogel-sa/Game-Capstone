using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public enum GAMESTATE
    {
        PLAYERTURN,
        ENEMYTURN
    }

	public delegate void TurnChangeAction(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, GAMESTATE turn);
    public event TurnChangeAction OnTurnChange;

    public GAMESTATE currentTurn { get; private set;}

    //List of all player stats in the game.  remove when player character is defeated


	public List<PlayerCharacterStats> playerList;

	public List<EnemyStats> enemyList;
  	[SerializeField]
  	GameObject VictoryScreen;
	[SerializeField]
	GameObject TurnChangeScreen;

	void Awake()
	{
        playerList = new List<PlayerCharacterStats>();
		enemyList = new List<EnemyStats>();
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
		TurnChangeScreen.SetActive(false);
		print ("Reloaded " + this.GetType().ToString());
	}

    private void OnPlayerTurnStart(){

		GetComponent<PlayerMovementManager>().enabled = true;

        foreach (var player in playerList)
        {
            player.hasMoved = false;
            player.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
	}

    private void OnEnemyTurnStart()
    {
		GetComponent<PlayerMovementManager>().enabled = false;
    }

    /// <summary>
    /// Switches the Current turn to the Other turn
    /// </summary>
	public void SwitchTurn(){
		GetComponent<PlayerMovementManager>().Deselect ();
		foreach (PlayerCharacterStats stats in playerList)
		{
			stats.GetComponentInChildren<MeshRenderer>().material.color = Color.gray;
		}
		StartCoroutine (_switchTurn ());
	}

	private IEnumerator _switchTurn()
	{
		var text = TurnChangeScreen.GetComponentInChildren<Text>();
		switch (currentTurn) {

		case GAMESTATE.ENEMYTURN:
			{
				TurnChangeScreen.SetActive(true);
				text.text = "Player turn start";
				yield return new WaitForSeconds (2f);
				TurnChangeScreen.SetActive(false);
				currentTurn = GAMESTATE.PLAYERTURN;
				OnPlayerTurnStart();
				break;
			}

		case GAMESTATE.PLAYERTURN:
			{
				TurnChangeScreen.SetActive(true);
				text.text = "Enemy turn start";
				yield return new WaitForSeconds (2f);
				TurnChangeScreen.SetActive(false);
				currentTurn = GAMESTATE.ENEMYTURN;
				OnEnemyTurnStart();
				break;
			}

		}
		yield return new WaitForSeconds (.2f);
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
            StartCoroutine(PlayerLoses(2));
        }
        if (enemyList.Count == 0)
        {
			StartCoroutine(GameOver(2));
        }
    }

    private IEnumerator GameOver(int waitval)
    {
        VictoryScreen.SetActive(true);
		GetComponent<PlayerMovementManager>().enabled = false;
		GetComponent<TurnManager>().enabled = false;
        yield return new WaitForSeconds(waitval);

        //GOTO MAIN MENU

		SceneManager.LoadScene("Main Menu");
    }

    private IEnumerator PlayerLoses(int waitval)
    {
        var text = VictoryScreen.GetComponentInChildren<Text>();
        text.text = "You lost! \n:(";
        VictoryScreen.SetActive(true);
        GetComponent<PlayerMovementManager>().enabled = false;
        GetComponent<TurnManager>().enabled = false;
        yield return new WaitForSeconds(waitval);

        //GOTO MAIN MENU

        SceneManager.LoadScene("Main Menu");
    }

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

}
