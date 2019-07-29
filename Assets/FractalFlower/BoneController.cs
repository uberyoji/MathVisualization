using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneController : MonoBehaviour
{
    public float InfluenceRadius = 1f;
    public float InfluenceScale = 1f;

    public bool ShowRadius = false;

    private void OnDrawGizmos()
    {
        if(ShowRadius)
        {
            Gizmos.DrawWireSphere(transform.position, InfluenceRadius);
        }
    }
}
