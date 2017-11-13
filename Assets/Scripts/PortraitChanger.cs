﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitChanger : MonoBehaviour {

    private ICharacterStats selectedCharacter;
    RawImage raw;
    Text Name { get; set; }
   
    

	// Use this for initialization
	void Awake () {
        selectedCharacter = PlayerMovementManager.Instance.selectedCharacterStats;
        raw = GetComponent <RawImage>();
        Name = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (selectedCharacter.Name != PlayerMovementManager.Instance.selectedCharacterStats.Name)
        {
            selectedCharacter = PlayerMovementManager.Instance.selectedCharacterStats;
            changeSelected();
        }
	}

    private void changeSelected()
    {
        raw.texture = selectedCharacter.Portrait;
        Name.text = selectedCharacter.Name;
    }
}
