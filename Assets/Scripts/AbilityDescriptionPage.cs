using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilityDescriptionPage : MonoBehaviour {

	// Use this for initialization
	Text name;
	Text des;
	void Start () {
		name = transform.Find ("Name").GetComponent<Text> ();
		des = transform.Find ("Description").GetComponent<Text> ();

		Hide ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show(AbilityData ad){
		transform.position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y + 100, 0);
		Debug.Log ("show page");
		name.text = ad.Name;
		des.text = ad.Description;
	}

	public void Hide(){
		
		transform.position = new Vector3 (2000, 2000, 0);
	}
}
