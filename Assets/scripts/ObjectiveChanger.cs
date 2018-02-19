using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveChanger : MonoBehaviour {

	[SerializeField]
	GameObject ObjectiveTextOBJ;
	Text ObjectiveText;
	GameObject[] enemies;


	// Use this for initialization
	void OnEnable () {
		ObjectiveText = ObjectiveTextOBJ.GetComponent<Text> ();
		enemies = GameObject.FindGameObjectsWithTag ("Enemy");

	}
	
	// Update is called once per frame
	void Update () {
		ObjectiveText.text = "- The Objective of this level is to defeat all enemies on the map.\n \n - Enemies Remaining:  " + enemies.Length;
	}
}
