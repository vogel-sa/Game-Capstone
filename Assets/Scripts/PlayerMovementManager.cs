using cakeslice;
using Pathfinding;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DentedPixel;

public class PlayerMovementManager : MonoBehaviour
{
    private struct BoolWrapper
    {
        public bool val { get; set; }
    }

    // Static pool of quads from which to pull for outline purposes.
    private GameObject quadParent;
    private readonly GameObject[] quads = new GameObject[300];
    private int quadsInUse = 0;


    public delegate void SelectAction(PlayerCharacterStats stats);
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
    private float moveSpeed = 2f;

    private GridGraph gg;

	void Awake() {
        quadParent = new GameObject();
		//quadParent.transform.parent = gameObject.transform;
        Vector3 quadRotation = new Vector3(90, 0, 0);
		for (int i = 0; i < quads.Length; i++)
        {
            
            var quad = PrimitiveHelper.CreatePrimitive(PrimitiveType.Quad, false);
            quad.transform.parent = quadParent.transform;
            var col = quad.AddComponent<BoxCollider>();
            col.size = new Vector3(1, .4f, 1);
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

    void Start()
    {
        gg = AstarData.active.data.gridGraph;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) Deselect();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (lastUpdateOutline) lastUpdateOutline.color = notHighlightedColor;
			if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast (ray, out hit, Camera.main.farClipPlane, LayerMask.GetMask ("Outline", "UI"))) {
				Outline outline = hit.transform.GetComponent<Outline> ();
				if (lastUpdateOutline)
					lastUpdateOutline.color = notHighlightedColor;
				outline.color = highlightedColor;
				lastUpdateOutline = outline;

				if (controlsEnabled && Input.GetMouseButtonDown (0) && !SelectedCharacterStats.hasMoved) {
					Vector3 hitPos = AstarData.active.GetNearest (hit.point).position;
					if (!Physics.Raycast (new Ray (hitPos, Vector3.up), 1, LayerMask.GetMask ("Player", "UI"))) { // Check if occupied.

                        var node = gg.GetNearest(hitPos).node as GridNode;
                        var pm = GetComponent<PathManager>();

						var path = pm.getPath (selected.transform.position, hitPos, PathManager.CharacterFaction.ALLY);

                        this.path = path;
						StartCoroutine (MoveCharacter (path));
					} else {
						Debug.Log ("Space occupied.");
					}
				}
			}
        }
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
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
        this.enabled = false;
		var audio = GetComponent<AudioManager>();
		if (audio) {
			audio.playSoundEffect(SelectedCharacterStats.MovementSound);
		}

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
        //var modifier = new GameObject().AddComponent<RaycastModifier>();
        //    modifier.raycastOffset = Vector3.up * .2f;
        //modifier.mask = LayerMask.GetMask("Obstacle", "Enemy");
        //modifier.Apply(path);
        var finished = false;
        var positionEnumeration = (from node in path.vectorPath
                                   orderby path.vectorPath.IndexOf(node)
                                   select (Vector3)node).ToList();
        for (int i = positionEnumeration.Count - 1; i >= 0; i--)
        {
            var node = gg.GetNearest(positionEnumeration[i]).node;
            if (GetComponent<PathManager>().enemyOnNode(node))
            {
                positionEnumeration.RemoveAt(i);
            }
            else
            {
                break;
            }
        }
        var arr = new Vector3[positionEnumeration.Count() + 2];
        positionEnumeration.CopyTo(arr, 1);
        arr[0] = arr[1];
        arr[arr.Length - 1] = arr[arr.Length - 2];
        var spline = new LTSpline(arr);
		//Destroy (modifier.gameObject);

        LeanTween.moveSpline(selected.gameObject, spline, spline.distance / moveSpeed).
            setOnStart(() => {
			var anim = selected.GetComponentInChildren<Animator>();
			if (anim) anim.SetTrigger("Walk");
            }).
            setOnComplete(() =>
            {
                finished = true;
				var anim = selected.GetComponentInChildren<Animator>();
				if (anim) anim.SetTrigger("Cancel");
            }). // May want to fiddle with animation states here.
            setEase(LeanTweenType.easeInOutQuad).
            setOrientToPath(true);
        //.setOnStart()

        yield return new WaitUntil(() => finished);
        controlsEnabled = true;
        // TODO: Fix the heirarchy for stats.
        SelectedCharacterStats.hasMoved = true;
		SelectedCharacterStats.CheckCharacterCannotMove();
		GetComponent<TurnManager>().AutoEndTurnCheck();
        selected.GetComponent<SingleNodeBlocker>().BlockAtCurrentPosition();
        this.enabled = true;
    }

    public void Select(PlayerCharacterStats stats)
    {
        SelectedCharacterStats = stats;
		if (!stats.hasMoved) {
			for (int i = 0; i < quads.Length; i++) {
				quads [i].SetActive (false);
			}
			// Get list of traversable nodes within range.
			var blocked = from blocker in GetComponent<PathManager>().enemies
			                       select blocker.lastBlocked;

            nodes = PathUtilities.BFS(AstarData.active.GetNearest(stats.transform.position).node,
                stats.MovementRange);
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
        OnSelect(stats);
    }

    public void Deselect()
    {
        SelectedCharacterStats = null;
        SetQuadsEnabled(false);
		if (OnDeselect != null) OnDeselect();
    }

    public void SetQuadsEnabled(bool enabled)
    {
        quadParent.SetActive(enabled);
    }

}
