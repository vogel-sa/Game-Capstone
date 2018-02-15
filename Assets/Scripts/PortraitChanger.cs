using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitChanger : MonoBehaviour {

	private PlayerCharacterStats selectedCharacter;
    RawImage raw;
    Slider HPbar;
    Text Name { get; set; }
    [SerializeField]
    GameObject HPTextOBJ;
	[SerializeField]
	GameObject ActionTextOBJ;
    Text HPText;
	Text ActionText;

   


	// Use this for initialization
	void OnEnable () {
		var pmm = FindObjectOfType<PlayerMovementManager> ();
		if (pmm) pmm.OnSelect += Select;
        raw = GetComponent <RawImage>();
        Name = GetComponentInChildren<Text>();
        HPbar = GetComponentInChildren<Slider>();
        HPText = HPTextOBJ.GetComponent<Text>();
		ActionText = ActionTextOBJ.GetComponent<Text>();

	}

    private void OnDisable()
    {
		var pmm = FindObjectOfType<PlayerMovementManager> ();
		if (pmm) pmm.OnSelect -= Select;
    }

    // Update is called once per frame
    void Select () {
		selectedCharacter = FindObjectOfType<PlayerMovementManager>().SelectedCharacterStats;
	}

    private void Update()
    {
		if (selectedCharacter != null) {
			raw.texture = selectedCharacter.Portrait;
			Name.text = selectedCharacter.Name;
			HPbar.maxValue = selectedCharacter.MaxHP;
			HPbar.value = selectedCharacter.CurrHP;
			HPText.text = "HP: " + selectedCharacter.CurrHP;
			ActionText.text = "Actions Left: " + selectedCharacter.Actionsleft;

		}
    }
}
