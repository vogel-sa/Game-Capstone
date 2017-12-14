using cakeslice;
using Pathfinding;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class PlayerMovementManager : MonoBehaviour
{
	#region SINGLETON
    private static object _lock = new object();
    private static bool applicationIsQuitting = false;
    private static PlayerMovementManager _instance;
    public static PlayerMovementManager Instance
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
                    var inst = FindObjectOfType(typeof(PlayerMovementManager)) as PlayerMovementManager;
                    _instance = inst ? inst : new GameObject().AddComponent<PlayerMovementManager>();
                }
            }
            return _instance;
        }
    }
	#endregion

    private struct BoolWrapper
    {
        public bool val { get; set; }
    }

    // Static pool of quads from which to pull for outline purposes.
    private static GameObject quadParent;
    private static readonly GameObject[] quads = new GameObject[300];
    private static int quadsInUse = 0;

    public delegate void SelectAction();
    public event SelectAction OnSelect;

    public delegate void DeselectAction();
    public event DeselectAction OnDeselect;

    private Transform selected;

    public PlayerCharacterStats SelectedCharacterStats { get; private set; }

    [SerializeField]
    private Material mat;
    [SerializeField]
    private int highlightedColor = 1; // This should correspond to 0, 1, or 2 in the camera's outlineEffect component.

    private List<GraphNode> nodes = null;

    private Outline lastUpdateOutline;

    private int notHighlightedColor;

    public bool controlsEnabled = true; // Is the character moving? If so, lock controls and turn off quads.

    private ABPath path;
    private float moveSpeed = 5f;

    void Awake()
    {
        quadParent = new GameObject();
        Vector3 quadRotation = new Vector3(90, 0, 0);
		for (int i = 0; i < quads.Length; i++)
        {

            var quad = PrimitiveHelper.CreatePrimitive(PrimitiveType.Quad, false);
            quad.transform.parent = quadParent.transform;
            var col = quad.AddComponent<BoxCollider>();
            col.size = new Vector3(1, .01f, 1);
            col.isTrigger = true;
            quad.layer = LayerMask.NameToLayer("Outline");
            var renderer = quad.GetComponent<Renderer>();
            renderer.enabled = false;
            renderer.material = mat ? mat : Resources.Load<Material>("Default");
            quad.AddComponent<Outline>().Renderer.sharedMaterials[0] = mat;
            quad.transform.localScale *= .9f;

            quad.SetActive(false);
            quad.transform.Rotate(quadRotation);
            quads[i] = quad;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Deselect();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (lastUpdateOutline) lastUpdateOutline.color = notHighlightedColor;
            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, LayerMask.GetMask("Outline")))
            {
                //if (hit.transform.gameObject.layer == LayerMask.GetMask("Outline"))
                {
                    Outline outline = hit.transform.GetComponent<Outline>();
                    if (lastUpdateOutline) lastUpdateOutline.color = notHighlightedColor;
                    outline.color = highlightedColor;
                    lastUpdateOutline = outline;

                    if (controlsEnabled && Input.GetMouseButtonDown(0) && !SelectedCharacterStats.hasMoved)
                    {
                        Vector3 hitPos = AstarData.active.GetNearest(hit.point).position;
                        if (!Physics.Raycast(new Ray(hitPos, Vector3.up), 1, LayerMask.GetMask("Player"))) // Check if occupied.
                        {
                            var path = PathManager.Instance.getPath(selected.transform.position, hitPos, PathManager.CharacterFaction.ALLY);
                            this.path = path;
                            StartCoroutine(MoveCharacter(path));
                        }
                        else
                        {
                            Debug.Log("Space occupied.");
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Player")
                    {
                        var obj = hit.transform;
                        while (obj.parent && obj.parent.tag == "Player")
                        {
                            obj = obj.parent;
                        }
                    var stats = obj.GetComponentInParent<PlayerCharacterStats>();
                    Select(stats);
                }
            }
        }
        #if DEBUG
        if (this.path != null)
        {
            for (int i = 0; i < path.vectorPath.Count - 1; i++)
            {
                Debug.DrawLine(path.vectorPath[i], path.vectorPath[i + 1], Color.red);
            }
        }
        #endif
    }

    private IEnumerator MoveCharacter(ABPath path)
    {
        #if DEBUG
        for (int i = 0; i < path.vectorPath.Count - 1; i++)
        {
            Debug.DrawLine(path.vectorPath[i], path.vectorPath[i + 1], Color.red);
        }
        #endif
        if (path.path.Count < 2)
        {
            throw new System.ArgumentException("Path must be at least length 2");
        }
        controlsEnabled = false;
        for (int i = 0; i < quads.Length; i++)
        {
            quads[i].SetActive(false);
        }
        var modifier = new GameObject().AddComponent<RaycastModifier>();
            modifier.raycastOffset = Vector3.up * .2f;
        modifier.mask = LayerMask.GetMask("Obstacle", "Enemy");
        modifier.Apply(path);
        var finished = false;
        var positionEnumeration = (from node in path.vectorPath
                                   orderby path.vectorPath.IndexOf(node)
                                   select (Vector3)node).ToArray();
        var arr = new Vector3[positionEnumeration.Count() + 2];
        positionEnumeration.CopyTo(arr, 1);
        arr[0] = arr[1];
        arr[arr.Length - 1] = arr[arr.Length - 2];
        var spline = new LTSpline(arr);
		Destroy (modifier.gameObject);

        LeanTween.moveSpline(selected.gameObject, spline, spline.distance / moveSpeed).
            setOnComplete(() => finished = true). // May want to fiddle with animation states here.
            setEase(LeanTweenType.linear).
            setOrientToPath(true);
        //.setOnStart()

        yield return new WaitUntil(() => finished);
        controlsEnabled = true;
        // TODO: Fix the heirarchy for stats.
        SelectedCharacterStats.hasMoved = true;
        TurnManager.instance.AutoEndTurnCheck();
        selected.GetComponent<SingleNodeBlocker>().BlockAtCurrentPosition();
    }

    void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public void Select(PlayerCharacterStats stats)
    {
        if (stats.Actionsleft <= 0) return;
        SelectedCharacterStats = stats;
		if (!stats.hasMoved) {
			for (int i = 0; i < quads.Length; i++) {
				quads [i].SetActive (false);
			}
			// Get list of traversable nodes within range.
			var blocked = from blocker in PathManager.Instance.enemies
			                       select blocker.lastBlocked;

			nodes = PathUtilities.BFS (AstarData.active.GetNearest (stats.transform.position).node,
				stats.MovementRange,
				walkableDefinition: (n) => !blocked.Contains (n));
			// Shouldn't ever need too many quads.
			int count = 0;
			if (nodes.Count > quads.Length) {
				throw new System.Exception ("Too many quads are required for the current range");
			}
			foreach (var node in nodes) {
				quads [count].SetActive (true);
				quads [count].transform.position = (Vector3)node.position + new Vector3 (0, .01f, 0);
				count++;
			}
			SetQuadsEnabled (true);
		} else {
			SetQuadsEnabled (false);
		}
		selected = stats.transform;
        OnSelect();
    }

    public void Deselect()
    {
        SelectedCharacterStats = null;
        SetQuadsEnabled(false);
        OnDeselect();
    }

    public void SetQuadsEnabled(bool enabled)
    {
        quadParent.SetActive(enabled);
    }

}
