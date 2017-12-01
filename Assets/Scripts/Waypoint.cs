using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(GetPosition(), "Waypoint.png", false);
    }

    public Vector3 GetPosition()
    {
        return transform.position + Offset;
    }

    public Vector3 Offset;
}
