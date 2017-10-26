using cakeslice;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour {

    private static object _lock = new object();
    private static PlayerMovementManager _instance;
    public static PlayerMovementManager Instance
    {
        get
        {
            lock(_lock)
            {
                if (!_instance)
                {
                    var inst = (PlayerMovementManager)FindObjectOfType(typeof(PlayerMovementManager));
                    _instance = inst ? inst : new GameObject().AddComponent<PlayerMovementManager>();
                }
            }
            return _instance;
        }
    }

    // Static pool of quads from which to pull for outline purposes.
    private static GameObject[] quads = new GameObject[100];
    private static int quadsInUse = 0;

    private Transform selected { get; set; }

    [SerializeField]
    private int range = 5;
    [SerializeField]
    private float cursorHeight = 3f;
    [SerializeField]
    private Material mat;
    [SerializeField]
    private int highlightedColor = 1; // This should correspond to 0, 1, or 2 in the camera's outlineEffect component.

    private List<GraphNode> nodes = null;
    
    private Mesh cursor;

    private Vector3 cursorPosition;

    private int[] cursorCoordinates = { 0, 0 };

    private Outline lastUpdateOutline;
    private int notHighlightedColor;

    void Awake()
    {
        cursor = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
        Vector3 quadRotation = new Vector3(90, 0, 0);
        for (int i = 0; i < 100; i++)
        {
            var quad = PrimitiveHelper.CreatePrimitive(PrimitiveType.Quad, false);
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
            Vector3 newPos;
            if (Input.GetKeyDown(KeyCode.W))
            {
                newPos = cursorPosition + new Vector3(0, 0, 1);

                if (Mathf.Abs(cursorCoordinates[0]) + Mathf.Abs(cursorCoordinates[1] + 1) <= range && AstarData.active.GetNearest(newPos).node.Walkable)
                {
                    cursorPosition = newPos;
                    cursorCoordinates[1]++;
                }
            }
            // Region is just near-duplicate code of the above if block.
            // TODO: clean this up later.
            #region 
            if (Input.GetKeyDown(KeyCode.A))
            {
                newPos = cursorPosition + new Vector3(-1, 0, 0);
                if (Mathf.Abs(cursorCoordinates[0] - 1) + Mathf.Abs(cursorCoordinates[1]) <= range && AstarData.active.GetNearest(newPos).node.Walkable)
                {
                    cursorCoordinates[0]--;
                    cursorPosition = newPos;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                newPos = cursorPosition + new Vector3(0, 0, -1);
                if (Mathf.Abs(cursorCoordinates[0]) + Mathf.Abs(cursorCoordinates[1] - 1) <= range && AstarData.active.GetNearest(newPos).node.Walkable)
                {
                    cursorPosition = newPos;
                    cursorCoordinates[1]--;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                newPos = cursorPosition + new Vector3(1, 0, 0);
                if (Mathf.Abs(cursorCoordinates[0] + 1) + Mathf.Abs(cursorCoordinates[1]) <= range && AstarData.active.GetNearest(newPos).node.Walkable)
                {
                    cursorPosition = newPos;
                    cursorCoordinates[0]++;
                }
            }
            #endregion

            // TODO: Draw the cursor.
            if (selected)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (lastUpdateOutline) lastUpdateOutline.color = notHighlightedColor;
                if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, 1 << LayerMask.NameToLayer("Outline")))
                {
                    Outline outline = hit.transform.GetComponent<Outline>();
                    if (lastUpdateOutline) lastUpdateOutline.color = notHighlightedColor;
                    outline.color = highlightedColor;
                    lastUpdateOutline = outline;
                }
            }

            // TODO: Add character movement/pathfinding
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var node = AstarData.active.GetNearest(cursorPosition).node;
                var seeker = selected.GetComponent<Seeker>();//.StartPath(transform.position, cursorPosition - new Vector3(0, 3, 0));
                Path path = seeker.StartPath(transform.position, cursorPosition - new Vector3(0, 3, 0));
            }
        }
	}

    public void Select(Transform t)
    {
        for (int i = 0; i < quads.Length; i++)
        {
            quads[i].SetActive(false);
        }
        cursorPosition = new Vector3(t.position.x, 3, t.position.z);
        cursorCoordinates = new int[] { 0, 0 };
        Debug.Log("select");
        // Get list of traversable nodes within range.
        nodes = PathUtilities.BFS(AstarData.active.GetNearest(t.position).node, range);
        // Shouldn't ever need too many quads.
        int count = 0;
        if (nodes.Count > quads.Length)
        {
            throw new System.Exception("Too many quads are required for the current range");
        }
        foreach (var node in nodes)
        {
            quads[count].SetActive(true);
            quads[count].transform.position = (Vector3)node.position + new Vector3(0, .01f, 0);
            //Graphics.DrawMesh(quad, (Vector3)node.position + new Vector3(0, .1f, 0), Quaternion.Euler(90, 0, 0), mat, LayerMask.NameToLayer("Outline"));
            count++;
        }
        selected = t;
    }
}
