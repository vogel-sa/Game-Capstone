using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitChanger : MonoBehaviour {

    private ICharacterStats selectedCharacter;
    RawImage raw;
   
    

	// Use this for initialization
	void Awake () {
        selectedCharacter = PlayerMovementManager.Instance.selectedCharacterStats;
        raw = GetComponent <RawImage>();
	}
	
	// Update is called once per frame
	void Update () {
		if (selectedCharacter != PlayerMovementManager.Instance.selectedCharacterStats)
        {
            selectedCharacter = PlayerMovementManager.Instance.selectedCharacterStats;
            changeTexture();
        }
	}

    private void changeTexture()
    {
        raw.texture = selectedCharacter.getCharacterPortrait();
    }
}
