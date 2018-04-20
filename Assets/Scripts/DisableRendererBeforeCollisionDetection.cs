using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DisableRendererBeforeCollisionDetection : MonoBehaviour {

	/// <summary>
    /// Using fixed update to disable mesh renderer each frame before the physics events are called which may re-enable.
    /// </summary>
	void FixedUpdate()
    {
        GetComponent<SkinnedMeshRenderer>().enabled = false;
        GetComponent<cakeslice.Outline>().enabled = false;
	}
}
