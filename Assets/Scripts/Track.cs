using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        var waypoints = GetComponentsInChildren<Waypoint>();
        foreach (var waypoint in waypoints)
        {
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
