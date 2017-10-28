using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    /// <summary>
    /// Flag to control rotating the camera view as the player car rotates around the track
    /// </summary>
    public bool RotateWithPlayer;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Target.transform.position;

        if( RotateWithPlayer )
        {
            transform.rotation = Target.transform.rotation;
        }
    }

    [SerializeField]
    private GameObject Target;
}
