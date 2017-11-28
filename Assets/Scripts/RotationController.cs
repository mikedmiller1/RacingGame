using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    /// <summary>
    /// The object to follow and point at.
    /// </summary>
    public GameObject ObjectToFollow;


    /// <summary>
    /// Direction of rotation.
    /// 1 is counterclockwise, -1 is clockwise.
    /// </summary>
    public int RotationDirection = 1;


    /// <summary>
    /// The rotation speed.
    /// </summary>
    public float RotationSpeed = 1;


    /// <summary>
    /// The axis to rotate about.
    /// </summary>
    private Vector3 RotationAxis = new Vector3( 0f, 0f, 1f );



	// Use this for initialization
	void Start()
    {
        // Point at the object to follow
        transform.LookAt( ObjectToFollow.transform.position );
    }
    

	// Update is called once per frame
	void Update()
    {
        // Rotate about the object
        transform.RotateAround( ObjectToFollow.transform.position, RotationAxis, Time.deltaTime * 10 * RotationSpeed * RotationDirection );
	}
}
