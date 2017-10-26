using UnityEngine;
using System.Collections;
public class BlockerTest : MonoBehaviour
{
    public void Start()
    {
        var blocker = GetComponent<SingleNodeBlocker>();
        blocker.BlockAtCurrentPosition();
    }
}