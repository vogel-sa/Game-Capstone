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

    

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        text.text = yourturn;
        text.color = yourturnColor;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void switchText()
    {
        if (text.text == yourturn)
        {
            text.text = enemyturn;
            text.color = enemyturnColor;
        }
        else
        {
            text.text = yourturn;
            text.color = yourturnColor;
        }
    }
}
