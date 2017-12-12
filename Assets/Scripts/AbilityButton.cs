using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Button))]
public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public AbilityData ability;
    private Button button;
	// Use this for initialization
	AbilityDescriptionPage page;
	bool stop = false;
	void Start () {
		page = GameObject.Find ("AbilityDescription").GetComponent<AbilityDescriptionPage> ();
        button = GetComponent<Button>();
        if (!button) button = gameObject.AddComponent<Button>();
        HideMe();
	}

    void OnEnable()
    {
        PlayerMovementManager.Instance.OnSelect += ShowMe;
        PlayerMovementManager.Instance.OnDeselect += HideMe;
    }

    void OnDisable()
    {
        PlayerMovementManager.Instance.OnSelect -= ShowMe;
        PlayerMovementManager.Instance.OnDeselect -= HideMe;
    }

    void Update()
    {
        if (ability.Currcooldown == 0)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
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
    }

    private void ShowMe()
    {
        GetComponent<Image>().enabled = true;
    }
}
