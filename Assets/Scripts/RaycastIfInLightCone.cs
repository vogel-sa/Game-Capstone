using UnityEngine;

[RequireComponent(typeof(ConeCollider))]
public class RaycastIfInLightCone : MonoBehaviour
{

    [SerializeField, Range(.01f, 179f)]
    private int angle = 30;
    [SerializeField]
    private float range = 10;
    [SerializeField]
    private int segments = 30;
    [SerializeField]
    private float lightIntensity = 8;

    void Start()
    {
        ConeCollider col = GetComponent<ConeCollider>();
        col.Angle = angle / 2;
        col.Distance = range;
        var light = GetComponent<Light>();
        light.type = LightType.Spot;
        light.range = range;
        light.spotAngle = angle;
        light.intensity = lightIntensity;
    }

#if DEBUG
    // For now, this doesn't even need to exist in the final version
    void Update()
    {
        for (int i = 0; i <= segments; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis((i * angle / segments) - angle / 2, Vector3.up) * transform.forward).normalized;
			Debug.DrawRay(transform.position, direction * range, Color.blue);
        }
    } 
#endif

    void OnTriggerStay(Collider col)
    {
        if (col.transform.tag.Equals("Enemy") && !col.GetComponent<MeshRenderer>().enabled)
        {
            if (RaySweep(col))
            {
                col.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag.Equals("Enemy"))
        {
            col.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private bool RaySweep(Collider col)
    {
        RaycastHit hit;

        for (int i = 0; i < segments; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis(i - angle / 2, Vector3.up) * transform.forward).normalized;
            Physics.Raycast(transform.position, direction, out hit, range);
            if (hit.collider && hit.collider.Equals(col)) return true;
        }
        return false;
    }
}
