using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Light))]
public class RaycastIfInLightSphere : MonoBehaviour
{

    [SerializeField]
    private float range = 2f;
    [SerializeField]
    private int segments = 90;
    [SerializeField]
    private float lightIntensity = 2;

    void Start()
    {
        var col = GetComponent<SphereCollider>();
        col.radius = range;
        var light = GetComponent<Light>();
        light.type = LightType.Point;
        light.range = range;
        light.intensity = lightIntensity;
    }

#if DEBUG
    // For now, this doesn't even need to exist in the final version
    void Update()
    {
        for (int i = 0; i <= segments; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis(i * 360 / segments, Vector3.up) * transform.forward).normalized;
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
                col.GetComponent<cakeslice.Outline>().enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag.Equals("Enemy"))
        {
            col.GetComponent<MeshRenderer>().enabled = false;
            col.GetComponent<cakeslice.Outline>().enabled = false;
        }
    }

    private bool RaySweep(Collider col)
    {
        RaycastHit hit;

        for (int i = 0; i < segments; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis(i * 360 / segments, Vector3.up) * transform.forward).normalized;
            Physics.Raycast(transform.position, direction, out hit, range);
            if (hit.collider && hit.collider.Equals(col)) return true;
        }
        return false;
    }
}
