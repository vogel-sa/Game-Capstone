using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChangeTurnText : MonoBehaviour {


    [SerializeField]
    private Color yourturnColor;
    [SerializeField]
    private Color enemyturnColor;

    [SerializeField]
    private string yourturn = "Your turn";
    [SerializeField]
    private string enemyturn = "Enemy turn";

    private Text text;
    TurnManager.GAMESTATE state;
    TurnManager manager;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        text.text = yourturn;
        text.color = yourturnColor;

        manager = TurnManager.instance;
        state = manager.currentTurn;

	}
	
	void OnEnable () {
        TurnManager.instance.OnTurnChange += switchText;
	}

    void OnDisable()
    {
        TurnManager.instance.OnTurnChange -= switchText;
    }

    public void switchText(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, TurnManager.GAMESTATE turn)
    {
        if (text.text == yourturn)
        {
            text.text = enemyturn;
            text.color = enemyturnColor;
            state = TurnManager.GAMESTATE.ENEMYTURN;
        }
        else
        {
            text.text = yourturn;
            text.color = yourturnColor;
            state = TurnManager.GAMESTATE.PLAYERTURN;
        }
    }
}
