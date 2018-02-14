using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DentedPixel;
using System.Linq;

public class EnemyTurn : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<TurnManager>().OnTurnChange += RunTurn;
    }

    private void OnDisable()
    {
        GetComponent<TurnManager>().OnTurnChange -= RunTurn;
    }


    private void RunTurn(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, TurnManager.GAMESTATE turn)
	{
		if (turn == TurnManager.GAMESTATE.ENEMYTURN)
		{
			StartCoroutine (_runTurn (players, enemies, turn));
		}
	}

	private IEnumerator _runTurn(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, TurnManager.GAMESTATE turn)
	{
		var moveSpeed = 5f;
		foreach (var enemy in enemies)
		{
			ABPath path = null;
			PlayerCharacterStats target = null;
			foreach (PlayerCharacterStats player in players)
			{
                if (Vector3.Distance(player.transform.position, enemy.transform.position) > enemy.DetectionRadius) break;
				List<GraphNode> neighbors = new List<GraphNode> ();
				// TODO: Find a better way to do this, one that takes enemy and ally blocked nodes into account.
				GraphNode neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.left).node;
				if (neighbor == null || !neighbor.Walkable) neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.right).node;
				if (neighbor == null || !neighbor.Walkable) neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.forward).node;
				if (neighbor == null || !neighbor.Walkable) neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.back).node;

				var newPath = GetComponent<PathManager>().getPath (enemy.transform.position, (Vector3)neighbor.position, PathManager.CharacterFaction.ENEMY);
				if (newPath != null && (path == null || path.vectorPath.Count > newPath.vectorPath.Count)) path = newPath;
				target = player;
			}
			if (path != null && !path.error)
			{
				bool finished = false;
				var arr = new Vector3[path.vectorPath.Count + 2];//new Vector3[Mathf.Min(enemy.MovementRange + 2)];
                for (int i = 1; i < arr.Length - 1; i++)
                {
                    arr[i] = path.vectorPath[i-1];
                }

				arr [0] = arr [1];
				arr [arr.Length - 1] = arr [arr.Length - 2];
				var spline = new LTSpline (arr);
				//Destroy (modifier.gameObject);
                if (arr.Length >= 4)
                {
                    LeanTween.moveSpline(enemy.gameObject, spline, spline.distance / moveSpeed).
                        setOnComplete(() => finished = true).// May want to fiddle with animation states here.
						//setEase(LeanTweenType.easeInQuad).
                        setOrientToPath(true);
                    yield return new WaitUntil(() => finished);
                }
				yield return new WaitForSeconds (.2f);
				if (Vector3.Distance (AstarData.active.GetNearest (target.transform.position).position,
					    AstarData.active.GetNearest (enemy.transform.position).position) <= 1f) {
					target.TakeDamage (enemy.Atk);
				}
			}
			RaycastHit hit;
			foreach (PlayerCharacterStats player in players) {
				//Debug.Log (player.name);
				if (player.isCoveringFire) {
					var abilData = (from abil in player.AbilityData where abil.Name == "CoveringFire" select abil).FirstOrDefault();
					if (Vector3.Distance (enemy.transform.position, player.transform.position) <= abilData.OtherValues.Range) {
						if (Physics.Raycast(enemy.transform.position, (player.transform.position - enemy.transform.position), out hit, abilData.OtherValues.Range) && hit.transform.parent.tag == "Player") {
							//Debug.Log (player.name);
							//Debug.Log (hit.transform.parent.name);
							enemy.TakeDamage(abilData.DamageAmount);
							player.isCoveringFire = false;
						}

					}
					
				}
			}
		}
		yield return null;
        GetComponent<TurnManager>().SwitchTurn ();
	}
	
	private float ManhattanDist(Vector3 a, Vector3 b)
	{
		float ret = Mathf.Abs (a.x - b.x) + Mathf.Abs (a.z - b.z);
		Debug.Log (ret);
		return ret;
	}
}