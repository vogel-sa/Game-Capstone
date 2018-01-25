using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitChanger : MonoBehaviour {

    private ICharacterStats selectedCharacter;
    RawImage raw;
    Slider HPbar;
    Text Name { get; set; }
    [SerializeField]
    GameObject HPTextOBJ;
    Text HPText;

   


	// Use this for initialization
	void OnEnable () {
        PlayerMovementManager.Instance.OnSelect += Select;
        raw = GetComponent <RawImage>();
        Name = GetComponentInChildren<Text>();
        HPbar = GetComponentInChildren<Slider>();
        HPText = HPTextOBJ.GetComponent<Text>();

	}

    private void OnDisable()
    {
        PlayerMovementManager.Instance.OnSelect -= Select;
    }

    // Update is called once per frame
    void Select () {
        selectedCharacter = PlayerMovementManager.Instance.SelectedCharacterStats;
	}

    private void Update()
    {
		if (selectedCharacter != null) {
			raw.texture = selectedCharacter.Portrait;
			Name.text = selectedCharacter.Name;
			HPbar.maxValue = selectedCharacter.MaxHP;
			HPbar.value = selectedCharacter.CurrHP;
			HPText.text = "HP: " + selectedCharacter.CurrHP;
		}
    }
}
