using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    /// <summary>
    /// Offset of the obstacle relative to the GameObject.
    /// </summary>
    public Vector3 Offset;



    public Vector3 GetPosition()
    {
        return transform.position + Offset;
    }
}
