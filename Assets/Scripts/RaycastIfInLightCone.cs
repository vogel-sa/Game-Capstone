using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConeCollider), typeof(Light))]
public class RaycastIfInLightCone : MonoBehaviour
{
    private ConeCollider cone;
    private Light lt;
    [SerializeField, Range(.01f, 179f)]
    private int angle = 30;
    public int Angle
    { get
        {
            return angle;
        }
        set
        {
            cone.Angle = angle = value / 2;
            lt.spotAngle = value;
        }
    }
    [SerializeField]
    private float range = 10;
    public float Range
    {
        get
        {
            return range;
        }
        set
        {
            cone.Distance = lt.range = range = value;

        }
    }
    [SerializeField]
    private float lightIntensity = 8;
    public float LightIntensity
    {
        get
        {
            return lightIntensity;
        }
        set
        {
            lt.intensity = lightIntensity = value;
        }
    }
    [SerializeField]
    LayerMask raycastIgnore;// = LayerMask.GetMask("Player");
    [SerializeField]
    public int turns = 2;

    void Start()
    {
        raycastIgnore = LayerMask.GetMask("Player", "Raycast Ignore");
        cone = GetComponent<ConeCollider>();
        if (!cone) cone = gameObject.AddComponent<ConeCollider>();
        lt = GetComponent<Light>();
        if (!lt) lt = gameObject.AddComponent<Light>();
        cone.Angle = angle / 2;
        cone.Distance = range;
        cone.IsTrigger = true;
        cone.Init();
        lt.type = LightType.Spot;
        lt.range = range;
        lt.spotAngle = angle;
        lt.intensity = lightIntensity;
    }

    void Awake()
    {
        TurnManager.instance.OnTurnChange += Countdown;
    }

    void OnDestroy()
    {
        TurnManager.instance.OnTurnChange -= Countdown;
    }

#if DEBUG
    // For now, this doesn't even need to exist in the final version
    void Update()
    {
        for (int i = -angle; i <= angle; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis(i, Vector3.up) * transform.forward).normalized;
			Debug.DrawRay(transform.position, direction * range, Color.blue);
        }
    } 
#endif

    void OnTriggerStay(Collider col)
    {
        if (col.transform.tag =="Enemy" && !col.GetComponent<MeshRenderer>().enabled)
        {
            if (RaySweep(col))
            {
                col.GetComponent<MeshRenderer>().enabled = true;
                col.GetComponent<cakeslice.Outline>().enabled = true;
            }
        }
    }

    private bool RaySweep(Collider col)
    {
        RaycastHit hit;

        for (int i = -angle; i < angle; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis(i, Vector3.up) * transform.forward).normalized;
            Physics.Raycast(transform.position, direction, out hit, range, ~(raycastIgnore | (1 << 2)));
            if (hit.collider && hit.collider.Equals(col)) return true;
        }
        return false;
    }

    private void Countdown(IList<PlayerCharacterStats> players, IList<EnemyStats> enemies, TurnManager.GAMESTATE turn)
    {
        if ((--turns) == 0)
        {
            Destroy(gameObject);
        }
    }
}
