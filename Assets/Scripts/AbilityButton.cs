using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public AbilityData ability;
	// Use this for initialization
	AbilityDescriptionPage page;
	bool stop = false;
	void Start () {
		page = GameObject.Find ("AbilityDescription").GetComponent<AbilityDescriptionPage> ();
	}

	public void OnPointerEnter(PointerEventData data){
		
		if (ability.Name != "") {
				page.Show (ability);
			}

	}

	public void OnPointerExit(PointerEventData data){
		page.Hide ();
	}
}
