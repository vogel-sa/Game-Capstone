using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DentedPixel;
using System.Linq;

public class EnemyTurn : MonoBehaviour
{

	private static EnemyTurn _instance;
	private static object _lock = new object();
	public static EnemyTurn instance
	{
		get
		{
			lock(_lock)
			{
				if (!_instance)
				{
					var inst = FindObjectOfType<EnemyTurn> ();
					_instance = inst ? inst : new GameObject ().AddComponent<EnemyTurn> ();
				}
			}
			return _instance;
		}
	}

    void OnEnable()
    {
        TurnManager.instance.OnTurnChange += RunTurn;
    }

    private void OnDisable()
    {
        TurnManager.instance.OnTurnChange -= RunTurn;
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
			foreach (var player in players)
			{
				List<GraphNode> neighbors = new List<GraphNode> ();
				// TODO: Find a better way to do this, one that takes enemy and ally blocked nodes into account.
				GraphNode neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.left).node;
				if (neighbor == null || !neighbor.Walkable) neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.right).node;
				if (neighbor == null || !neighbor.Walkable) neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.forward).node;
				if (neighbor == null || !neighbor.Walkable) neighbor = AstarData.active.GetNearest(player.transform.position + Vector3.back).node;

				var newPath = PathManager.Instance.getPath (enemy.transform.position, (Vector3)neighbor.position, PathManager.CharacterFaction.ENEMY);
				if (newPath != null && (path == null || path.vectorPath.Count > newPath.vectorPath.Count)) path = newPath;
			}
			if (path != null)
			{
				// TODO: truncate path to correct movedist for enemy before modifier.
				var modifier = new GameObject ().AddComponent<RaycastModifier> ();
				modifier.raycastOffset = Vector3.up * .2f;
				modifier.mask = LayerMask.GetMask ("Obstacle", "Player");
				modifier.Apply (path);
				bool finished = false;

				var arr = new Vector3[path.vectorPath.Count + 2];
				path.vectorPath.CopyTo (arr, 1);
				arr [0] = arr [1];
				arr [arr.Length - 1] = arr [arr.Length - 2];
				var spline = new LTSpline (arr);
				Destroy (modifier.gameObject);
                if (arr.Length >= 4)
                {
                    LeanTween.moveSpline(enemy.gameObject, spline, spline.distance / moveSpeed).
                        setOnComplete(() => finished = true).// May want to fiddle with animation states here.
                        setEase(LeanTweenType.linear).
                        setOrientToPath(true);
                    yield return new WaitUntil(() => finished);
                }
				yield return new WaitForSeconds (.2f);
			}
		}
		yield return null;
        //TurnManager.instance.SwitchTurn ();
        Debug.Log("This happens");
	}
	
	private float ManhattanDist(Vector3 a, Vector3 b)
	{
		float ret = Mathf.Abs (a.x - b.x) + Mathf.Abs (a.z - b.z);
		Debug.Log (ret);
		return ret;
	}
}