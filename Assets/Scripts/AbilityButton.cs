using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Button))]
public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public AbilityData ability;
    private Button button;
	private Text cooldown;
	// Use this for initialization
	AbilityDescriptionPage page;
	bool stop = false;
	void Start () {
		page = GameObject.Find ("AbilityDescription").GetComponent<AbilityDescriptionPage> ();
        button = GetComponent<Button>();
		cooldown = GetComponentInChildren<Text>();
        if (!button) button = gameObject.AddComponent<Button>();
        HideMe();
	}

    void OnEnable()
    {
		var pmm = FindObjectOfType<PlayerMovementManager> ();
		if (pmm) {
			pmm.OnSelect += ShowMe;
			pmm.OnDeselect += HideMe;
		}
    }

    void OnDisable()
    {
		var pmm = FindObjectOfType<PlayerMovementManager> ();
		if (pmm) {
			pmm.OnSelect -= ShowMe;
			pmm.OnDeselect -= HideMe;
		}
    }

    void Update()
    {
		
        if (ability.Currcooldown == 0)
        {
            button.interactable = true;
			cooldown.text = "";
        }
        else
        {
            button.interactable = false;
			cooldown.text = ability.Currcooldown.ToString();
        }
    }

	public void OnPointerEnter(PointerEventData data){
		
		if (ability.Name != "") {
				page.Show (ability);
			}

	}

	public void OnPointerExit(PointerEventData data){
		page.Hide ();
	}

    private void HideMe()
    {
        GetComponent<Image>().enabled = false;
		GetComponentInChildren<Text>().enabled = false;

    }

    private void ShowMe()
    {
        GetComponent<Image>().enabled = true;
		GetComponentInChildren<Text>().enabled = true;
	
    }
}
