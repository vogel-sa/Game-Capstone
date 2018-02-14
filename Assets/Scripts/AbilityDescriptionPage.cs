using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilityDescriptionPage : MonoBehaviour {

	// Use this for initialization
	Text name;
	Text des;
    Image background;
    Text cooldowntext;
	void Start () {
		name = transform.Find ("Name").GetComponent<Text> ();
		des = transform.Find ("Description").GetComponent<Text> ();
        background = transform.Find("Background").GetComponent<Image>();
        cooldowntext = transform.Find("Cooldown Text").GetComponent<Text>();
		Hide ();
	}

	public void Show(AbilityData ad){
        name.enabled = true;
        des.enabled = true;
        background.enabled = true;
        cooldowntext.enabled = true;

        name.text = ad.Name;
		des.text = ad.Description;

        if (ad.Maxcooldown > 0)
        {
            if (ad.Maxcooldown == 1)
            {
                cooldowntext.text = "Cooldown:  " + ad.Maxcooldown + " Turn";
            }else
            {
                cooldowntext.text = "Cooldown:  " + ad.Maxcooldown + " Turns";
            }
        }
        else
        {
            cooldowntext.text = "No Cooldown";
        }
	}

	public void Hide(){

        name.enabled = false;
        des.enabled = false;
        background.enabled = false;
        cooldowntext.enabled = false;
	}
}
