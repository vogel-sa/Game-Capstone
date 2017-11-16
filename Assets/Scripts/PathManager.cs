using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {

    public enum CharacterFaction
    {
        ALLY,
        ENEMY
    };

    private static object _lock = new object();
    private static bool applicationIsQuitting = false;
    private static PathManager _instance;
    public static PathManager Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (!_instance)
                {
                    var inst = FindObjectOfType(typeof(PathManager)) as PathManager;
                    _instance = inst ? inst : new GameObject().AddComponent<PathManager>();
                    _instance.Init();
                }
            }
            return _instance;
        }
    }

    public BlockManager blockManager;
    public List<SingleNodeBlocker> enemies { get; private set; }
    public List<SingleNodeBlocker> allies { get; private set; }

    public BlockManager.TraversalProvider allyTraversalProvider { get; private set; }
    public BlockManager.TraversalProvider enemyTraversalProvider { get; private set; }

    // Use this for initialization
    void Init () {
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

    void OnDestroy()
    {
        applicationIsQuitting = true;
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
