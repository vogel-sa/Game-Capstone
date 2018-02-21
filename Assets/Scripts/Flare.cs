using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Flare : MonoBehaviour {
    [SerializeField]
    public int turns = 3;

    void OnEnable()
    {
        FindObjectOfType<TurnManager>().OnTurnChange += Countdown;
    }

    void OnDisable()
    {
        FindObjectOfType<TurnManager>().OnTurnChange -= Countdown;
    }

    // Update is called once per frame
    void Update () {
		
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
