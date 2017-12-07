using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonAbilitySelector : MonoBehaviour {

    [SerializeField]
    private Button[] _buttons;
    private Sprite[] _sprites;
    [SerializeField]
    private AbilityData[] abilities;

    void Start()
    {
        _sprites = new Sprite[_buttons.Length];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _sprites[i] = _buttons[i].GetComponent<Sprite>();
        }
    }

	// Use this for initialization
	void OnEnable () {
        PlayerMovementManager.Instance.OnSelect += ChangeAbilityButtons;
    }

    void OnDisable()
    {
        if (PlayerMovementManager.Instance)
            PlayerMovementManager.Instance.OnSelect -= ChangeAbilityButtons;
    }

    // Update is called once per frame
    void ChangeAbilityButtons()
    {
        Debug.Log("Buttons changed");
        abilities = new AbilityData[PlayerMovementManager.Instance.SelectedCharacterStats.AbilityData.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i] = PlayerMovementManager.Instance.SelectedCharacterStats.AbilityData[i];

            _buttons[i].onClick.RemoveAllListeners();
            var e = abilities[i];
            _buttons[i].onClick.AddListener(delegate
            {
                e.Ability.Invoke();
            });
            // TODO: Change button sprite, change ability data.

			//store abilitydata on buttons
			_buttons[i].gameObject.GetComponent<AbilityButton>().ability = new AbilityData();
			_buttons[i].gameObject.GetComponent<AbilityButton>().ability = abilities[i];
        }
		UpdateAbilityIcon (abilities);
	}

	void UpdateAbilityIcon(AbilityData[] ad){
		
		for (int i = 0; i < abilities.Length; i++) {
			
			transform.GetChild(i).GetComponent<Image>().sprite = ad [i].AbilitySprite;
		}
	}
}
