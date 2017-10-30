using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitChanger : MonoBehaviour {

    private ICharacterStats selectedCharacter;
    RawImage raw;
    Text name;
   
    

	// Use this for initialization
	void Awake () {
        selectedCharacter = PlayerMovementManager.Instance.selectedCharacterStats;
        raw = GetComponent <RawImage>();
        name = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (selectedCharacter != PlayerMovementManager.Instance.selectedCharacterStats)
        {
            selectedCharacter = PlayerMovementManager.Instance.selectedCharacterStats;
            changeSelected();
        }
	}

    private void changeSelected()
    {
        raw.texture = selectedCharacter.getCharacterPortrait();
        name.text = selectedCharacter.getCharacterName();
    }
}
