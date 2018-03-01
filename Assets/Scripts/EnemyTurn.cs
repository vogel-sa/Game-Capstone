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
        var manager = FindObjectOfType<TurnManager>();
		foreach (var enemy in enemies)
		{
			ABPath path = null;
			PlayerCharacterStats target = null;
			var blocked = (from blocker in GetComponent<PathManager> ().enemies
			               select blocker.lastBlocked).Concat (from blocker in GetComponent<PathManager> ().allies
			                                                   select blocker.lastBlocked).ToList ();
            var pm = GetComponent<PathManager>();
            GridGraph gg = AstarPath.active.data.gridGraph;
            GridNode node = gg.GetNearest(enemy.transform.position).node as GridNode;
            int x = node.NodeInGridIndex % gg.width;
            int y = node.NodeInGridIndex / gg.width;
            GraphNode dest = null;
            GraphNode[] neighbors = new GraphNode[4];
            neighbors[0] = gg.GetNode(x - 1, y);
            neighbors[1] = gg.GetNode(x + 1, y);
            neighbors[2] = gg.GetNode(x, y - 1);
            neighbors[3] = gg.GetNode(x, y + 1);

            foreach (var n in neighbors)
            {
                var ally = pm.allyOnNode(n);
                if (n != null && ally != null && (!target || ally.CurrHP < target.CurrHP))
                {
                    target = ally;
                }
            }
            if (target)
            {
                target.TakeDamage(enemy.Atk);
                continue;
            }
            // Find closest player
            foreach (PlayerCharacterStats player in players)
			{
                if (Vector3.Distance(player.transform.position, enemy.transform.position) > enemy.DetectionRadius) continue;
                neighbors[0] = null;
                neighbors[1] = null;
                neighbors[2] = null;
                neighbors[3] = null;
                GridNode playerNode = gg.GetNearest(player.transform.position).node as GridNode;
                x = playerNode.NodeInGridIndex % gg.width;
                y = playerNode.NodeInGridIndex / gg.width;
                neighbors[0] = gg.GetNode(x - 1, y);
                neighbors[1] = gg.GetNode(x + 1, y);
                neighbors[2] = gg.GetNode(x, y - 1);
                neighbors[3] = gg.GetNode(x, y + 1);
                target = null;
                //var nodesInRange = new HashSet<GraphNode>(PathUtilities.BFS(node, enemy.MovementRange));
                
                foreach (GraphNode n in neighbors)
                {
                    
                    if (n != null && !pm.enemyOnNode(n) && !pm.allyOnNode(n) &&
                                  (dest == null || Vector3.Distance((Vector3)node.position,(Vector3)n.position) <
                                                   Vector3.Distance((Vector3)node.position, (Vector3)dest.position)))
                    {
                        dest = n;
                        target = pm.allyOnNode(playerNode);
                    }
                }

                if (target && dest != null)
                {
                    path = pm.getPath(enemy.transform.position, (Vector3)dest.position, PathManager.CharacterFaction.ENEMY);
                    break;
                }


                //var newPath = GetComponent<PathManager>().getPath (enemy.transform.position, (Vector3)neighbor.position, PathManager.CharacterFaction.ENEMY);
				//if (newPath != null && (path == null || path.vectorPath.Count > newPath.vectorPath.Count)) path = newPath;
				//target = player;
			}
			if (path != null && !path.error)
			{
                print("Enemy found valid path");
				bool finished = false;
				var arr = new Vector3[Mathf.Min(path.vectorPath.Count + 2, enemy.MovementRange + 2)];
                for (int i = 1; i < arr.Length - 1; i++)
                {
                    arr[i] = path.vectorPath[i-1];
                }
				if (blocked.Contains(AstarData.active.GetNearest( arr[arr.Length - 1]).node)) {
					continue;
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
				enemy.GetComponent<SingleNodeBlocker> ().BlockAtCurrentPosition ();
				yield return new WaitForSeconds (.2f);
				if (Vector3.Distance (AstarData.active.GetNearest (target.transform.position).position,
					    AstarData.active.GetNearest (enemy.transform.position).position) <= 1f) {
					target.TakeDamage (enemy.Atk);
					Debug.Log (target.name);
				}
                manager.CheckGameOver();
			}
			checkCoveringFire(enemy, players);
		}
		yield return new WaitForSeconds(1f);
        GetComponent<TurnManager>().SwitchTurn ();
	}
	
	private float ManhattanDist(Vector3 a, Vector3 b)
	{
		float ret = Mathf.Abs (a.x - b.x) + Mathf.Abs (a.z - b.z);
		Debug.Log (ret);
		return ret;
	}

	private void checkCoveringFire(EnemyStats enemy, IList<PlayerCharacterStats> players) {
		RaycastHit hit;
		foreach (PlayerCharacterStats player in players) {
			//Debug.Log (player.name);
			if (player.isCoveringFire) {
				var abilData = (from abil in player.AbilityData where abil.Name == "CoveringFire" select abil).FirstOrDefault();
				if (Vector3.Distance (enemy.transform.position, player.transform.position) <= abilData.OtherValues.Range && enemy.GetComponentInChildren<MeshRenderer>().enabled) {
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

}