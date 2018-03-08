using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour {

    [SerializeField]
    Text damageText;

	// Use this for initialization
	void Awake () {
        damageText = GetComponentInChildren<Text>();
        damageText.enabled = false;
	}

    public void displayText(int damage, float time)
    {
        StartCoroutine(displayCoRoutine(damage, time));
    }

     IEnumerator displayCoRoutine(int damage, float timetoDisplay)
    {
        damageText.text = damage.ToString();
        damageText.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
        damageText.enabled = true;

        yield return new WaitForSeconds(timetoDisplay);
        damageText.enabled = false;
    }
}
