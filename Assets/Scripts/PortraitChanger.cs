using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitChanger : MonoBehaviour {

    private ICharacterStats selectedCharacter;
    RawImage raw;
    Text Name { get; set; }
   


	// Use this for initialization
	void OnEnable () {
        PlayerMovementManager.Instance.OnSelect += Select;
        raw = GetComponent <RawImage>();
        Name = GetComponentInChildren<Text>();
	}

    private void OnDisable()
    {
        PlayerMovementManager.Instance.OnSelect -= Select;
    }

    // Update is called once per frame
    void Select () {
        selectedCharacter = PlayerMovementManager.Instance.SelectedCharacterStats;
        changeSelected();
	}

    private void changeSelected()
    {
        raw.texture = selectedCharacter.Portrait;
        Name.text = selectedCharacter.Name;
    }
}
