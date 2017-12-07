using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilityDescriptionPage : MonoBehaviour {

	// Use this for initialization
	Text name;
	Text des;
    Image background;
	void Start () {
		name = transform.Find ("Name").GetComponent<Text> ();
		des = transform.Find ("Description").GetComponent<Text> ();
        background = transform.Find("Background").GetComponent<Image>();
		Hide ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show(AbilityData ad){
        name.enabled = true;
        des.enabled = true;
        background.enabled = true;

        name.text = ad.Name;
		des.text = ad.Description;
	}

	public void Hide(){

        name.enabled = false;
        des.enabled = false;
        background.enabled = false;
	}
}
