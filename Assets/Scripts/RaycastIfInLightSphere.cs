using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SphereCollider), typeof(Light))]
public class RaycastIfInLightSphere : MonoBehaviour
{

    [SerializeField]
    private float range = 2f;
    [SerializeField]
    private int segments = 90;
    [SerializeField]
    private float lightIntensity = 2;
    [SerializeField]
    LayerMask raycastIgnore;
    [SerializeField]

    void Start()
    {
        var col = GetComponent<SphereCollider>();
        col.radius = range;
        col.isTrigger = true;
        var light = GetComponent<Light>();
        light.type = LightType.Point;
        light.range = range;
        light.intensity = lightIntensity;
        light.cullingMask = ~raycastIgnore;
        light.shadows = LightShadows.Hard;
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
		if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy" && !col.GetComponentInChildren<SkinnedMeshRenderer>().enabled)
        {
            if (RaySweep(col))
            {
                col.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => x.enabled = true);
                col.GetComponentsInChildren<cakeslice.Outline>().ToList().ForEach(x => x.enabled = true);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
		if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy")
        {
            col.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => x.enabled = false);
            col.GetComponentsInChildren<cakeslice.Outline>().ToList().ForEach(x => x.enabled = false);
        }
    }

    private bool RaySweep(Collider col)
    {
        RaycastHit hit;

        for (int i = 0; i < segments; i++)
        {
            Vector3 direction = (Quaternion.AngleAxis(i * 360 / segments, Vector3.up) * transform.forward).normalized;
            Physics.Raycast(transform.position, direction, out hit, range, ~(raycastIgnore | (1 << 2)));
            if (hit.collider && hit.collider.Equals(col)) return true;
        }
        return false;
    }
}
