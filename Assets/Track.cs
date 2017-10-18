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
            print(waypoint.GetPosition());
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
