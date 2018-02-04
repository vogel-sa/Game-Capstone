using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {

    public enum CharacterFaction
    {
        ALLY,
        ENEMY
    };

    public BlockManager blockManager;
    public List<SingleNodeBlocker> enemies { get; private set; }
    public List<SingleNodeBlocker> allies { get; private set; }

    public BlockManager.TraversalProvider allyTraversalProvider { get; private set; }
    public BlockManager.TraversalProvider enemyTraversalProvider { get; private set; }

    // Use this for initialization
    void Awake () {
        var bm = GetComponent<BlockManager>();
        blockManager = bm ? bm : gameObject.AddComponent<BlockManager>();
        enemies = new List<SingleNodeBlocker>();
        allies = new List<SingleNodeBlocker>();
        foreach (var blocker in FindObjectsOfType<SingleNodeBlocker>())
        {

            blocker.manager = blockManager;
            blocker.BlockAtCurrentPosition();
            switch (blocker.tag)
            {
                case "Player": allies.Add(blocker); break;
                case "Enemy": enemies.Add(blocker); break;
                default: break;
            }
        }

        allyTraversalProvider = new BlockManager.TraversalProvider(blockManager, BlockManager.BlockMode.OnlySelector, enemies);
        enemyTraversalProvider = new BlockManager.TraversalProvider(blockManager, BlockManager.BlockMode.OnlySelector, allies);
    }

    public ABPath getPath(Vector3 start, Vector3 end, CharacterFaction team)
    {
        var path = ABPath.Construct(start, end, null);
        path.traversalProvider = team == CharacterFaction.ALLY ? allyTraversalProvider : enemyTraversalProvider;
        AstarPath.StartPath(path);
        path.BlockUntilCalculated();

        if (path.error)
        {
            Debug.Log("No path was found.");
        }
        else
        {
            Debug.Log("A path was found with " + path.vectorPath.Count + " nodes");
        }
        return path;
    }
}
