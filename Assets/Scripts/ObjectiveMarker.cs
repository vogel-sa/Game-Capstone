using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class ObjectiveMarker : MonoBehaviour {

	private int numCharacters
    {
        get
        {
            return FindObjectsOfType<PlayerCharacterStats>()
                .ToList()
                .Where(x => x.CurrHP > 0)
                .Count();
        }
    }
	[SerializeField]
	HashSet<PlayerCharacterStats> charactersInObjective = new HashSet<PlayerCharacterStats>();
	[SerializeField]
	private GameObject VictoryScreen;


    void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player")
			charactersInObjective.Add (col.GetComponentInParent<PlayerCharacterStats>());
		if (charactersInObjective.Count >= numCharacters) {
			Debug.Log (charactersInObjective.Count);
			StartCoroutine (GameOver(2));
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Player")
			charactersInObjective.Remove(col.GetComponentInParent<PlayerCharacterStats>());
	}

	private IEnumerator GameOver(int waitval)
	{
		VictoryScreen.SetActive(true);
		FindObjectOfType<PlayerMovementManager>().enabled = false;
		FindObjectOfType<TurnManager>().enabled = false;
		yield return new WaitForSeconds(waitval);

		//GOTO MAIN MENU

		SceneManager.LoadScene("Main Menu");
	}
}
