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
    private UnityEvent[] abilities;

    void Start()
    {
        _sprites = new Sprite[_buttons.Length];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _sprites[i] = _buttons[i].GetComponent<Sprite>();
            // TODO: Change image, change script to call.
        }
    }

	// Use this for initialization
	void OnEnable () {
        PlayerMovementManager.Instance.OnSelect += ChangeAbilityButtons;
    }

    void OnDisable()
    {
        PlayerMovementManager.Instance.OnSelect -= ChangeAbilityButtons;
    }

    // Update is called once per frame
    void ChangeAbilityButtons()
    {
        Debug.Log("Buttons changed");
        abilities = new UnityEvent[PlayerMovementManager.Instance.SelectedCharacterStats.Abilities.Length];
        for (int i = 0; i < _buttons.Length; i++)
        {
            abilities[i] = PlayerMovementManager.Instance.SelectedCharacterStats.Abilities[i];
            // TODO: Change image, change script to call.
            _buttons[i].onClick.RemoveAllListeners();
            var e = abilities[i];
            _buttons[i].onClick.AddListener(delegate
            {
                e.Invoke();
            });
        }
	}
}
