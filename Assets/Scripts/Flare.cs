using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Flare : MonoBehaviour {
    [SerializeField]
    public int turns = 3;
	[SerializeField]
	private AudioClip light, sizzle;

	private AudioSource source;

    void OnEnable()
    {
		source = GetComponent<AudioSource> ();
        FindObjectOfType<TurnManager>().OnTurnChange += Countdown;
		StartCoroutine (playSounds ());
    }

	private IEnumerator playSounds()
	{
		if (light) {
			source.clip = light;
			source.Play ();
		}
		yield return new WaitForSeconds (light.length);
		if (sizzle) {
			source.Stop ();
			source.clip = sizzle;
			source.Play();
		}
	}

    void OnDisable()
    {
        FindObjectOfType<TurnManager>().OnTurnChange -= Countdown;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.GetMask("Ground"))
        {
            var body = GetComponent<Rigidbody>();
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
            body.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

	void Countdown(IList<PlayerCharacterStats> stats, IList<EnemyStats> enemies, TurnManager.GAMESTATE turn)
    {
        if (turn == TurnManager.GAMESTATE.PLAYERTURN)
        {
            Debug.Log("Reduced");
            if ((--turns) == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
