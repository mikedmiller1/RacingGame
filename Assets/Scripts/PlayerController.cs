﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Properties

    /// <summary>
    /// Reference to the rigid body.
    /// </summary>
    [SerializeField]
    private Rigidbody rb;



    /// <summary>
    /// Maximum speed of the player.
    /// </summary>
    public float MaxSpeed;



    /// <summary>
    /// Target location for movement.
    /// </summary>
    [HideInInspector]
    public Vector3 Destination = new Vector3();



    /// <summary>
    /// Radius within which the player has arrived at the destination.
    /// </summary>
    [HideInInspector]
    public float ArriveRadius;



    /// <summary>
    /// AI driver.
    /// </summary>
    public Driver AiDriver;



    /// <summary>
    /// Flag to control extra debug information.
    /// </summary>
    public bool Debugging;



    /// <summary>
    /// Flag to control the planner checking a direct path to the goal.
    /// </summary>
    public bool CheckDirectPath;

    #endregion



    #region Unity Methods

    // Update is called once per frame
    void Update()
    {
        // Check for debugging
        if( Debugging )
        {
            // Draw all the paths in grey
            foreach( Path CurrentPath in AiDriver.Paths )
            {
                foreach( Segment CurrentSegment in CurrentPath.Segments )
                {
                    Vector3 Start = new Vector3( (float)CurrentSegment.X, 0, (float)CurrentSegment.Y);
                    Vector3 End = new Vector3( (float)CurrentSegment.X2, 0, (float)CurrentSegment.Y2 );
                    Debug.DrawLine( Start, End, Color.gray, Time.deltaTime, false );
                }
            }

            // Draw the current best path in red
            foreach( Segment CurrentSegment in AiDriver.CurrentBestPath.Segments )
            {
                Vector3 Start = new Vector3( (float)CurrentSegment.X, 0, (float)CurrentSegment.Y);
                Vector3 End = new Vector3( (float)CurrentSegment.X2, 0, (float)CurrentSegment.Y2 );
                Debug.DrawLine( Start, End, Color.red, Time.deltaTime, false );
            }
        }


        // Get the distance from the current position to the destination
        Vector3 Towards = Destination - transform.position;
        float RemainingDistance = Towards.magnitude;

        // If we are not at the destination
        if( RemainingDistance > ArriveRadius )
        {
            // Move towards the destination
            MoveTo( Destination );
        }

        else
        {
            // Perform navigation
            AiDriver.Navigate();

            // Assign the destination
            Destination.x = (float)AiDriver.X;
            Destination.z = (float)AiDriver.Y;
        }

        
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Moves the player to the specified destination.
    /// </summary>
    /// <param name="Destination">The location to move to.</param>
    public void MoveTo( Vector3 Destination )
    {
        // Get a vector from the current position to the destination
        Vector3 Towards = Destination - transform.position;
        Quaternion TowardsRotation = Quaternion.LookRotation( Towards );

        // Rotate to the destination
        transform.rotation = Quaternion.Lerp( transform.rotation, TowardsRotation, Time.deltaTime * 8 );

        /*
        // If the movement would exceed the maximum speed allowed
        if ( Towards.magnitude > MaxSpeed )
        {
            // Normalize to a unit vector
            Towards.Normalize();

            // Move at the maximum speed
            Towards *= MaxSpeed;
        }
        */

        // Normalize to a unit vector
        Towards.Normalize();

        // Move at the maximum speed
        Towards *= MaxSpeed;

        // Move the player
        rb.velocity = Towards;
    }

    #endregion
}