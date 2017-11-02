using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class DisableRendererBeforeCollisionDetection : MonoBehaviour {

	/// <summary>
    /// Using fixed update to disable mesh renderer each frame before the physics events are called which may re-enable.
    /// </summary>
	void FixedUpdate()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<cakeslice.Outline>().enabled = false;
	}
}
