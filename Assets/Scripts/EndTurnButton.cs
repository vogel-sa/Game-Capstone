using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour {

    Button myButton;

    private void OnEnable()
    {
        FindObjectOfType<TurnManager>().OnTurnChange += DisableButton;
    }

    private void OnDisable()
    {
        FindObjectOfType<TurnManager>().OnTurnChange -= DisableButton;
    }

    private void Start()
    {
        myButton = GetComponent<Button>();
    }

   void DisableButton(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, TurnManager.GAMESTATE turn) {

        if (turn == TurnManager.GAMESTATE.ENEMYTURN) {
            myButton.interactable = false;
        }
        else
        {
            myButton.interactable = true;
        }
    }
}
