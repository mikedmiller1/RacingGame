using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Properties

    /// <summary>
    /// The name of the driver.
    /// </summary>
    [HideInInspector]
    public string Name;



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
    /// Flag to control showing the current path.
    /// </summary>
    public bool ShowCurrentPath;



    /// <summary>
    /// Flag to control showing all paths.
    /// </summary>
    public bool ShowAllPaths;



    /// <summary>
    /// Flag to control the planner checking a direct path to the goal.
    /// </summary>
    public bool CheckDirectPath;

    #endregion



    #region Unity Methods

    // Update is called once per frame
    void Update()
    {
        // Check if all paths should be drawn
        if( ShowAllPaths )
        {
            // Draw all the paths in grey
            foreach( Path CurrentPath in AiDriver.Paths )
            {
                foreach( Segment CurrentSegment in CurrentPath.Segments )
                {
                    Vector3 Start = new Vector3( (float)CurrentSegment.X, (float)CurrentSegment.Y, 0 );
                    Vector3 End = new Vector3( (float)CurrentSegment.X2, (float)CurrentSegment.Y2, 0 );
                    Debug.DrawLine( Start, End, Color.gray, Time.deltaTime, false );
                }
            }
        }

        // Check if the current path should be drawn
        if( ShowCurrentPath )
        {
            // Draw the current best path in red
            foreach( Segment CurrentSegment in AiDriver.CurrentBestPath.Segments )
            {
                Vector3 Start = new Vector3( (float)CurrentSegment.X, (float)CurrentSegment.Y, 0 );
                Vector3 End = new Vector3( (float)CurrentSegment.X2, (float)CurrentSegment.Y2, 0 );
                Debug.DrawLine( Start, End, Color.red, Time.deltaTime, false );
            }
        }


        // Perform navigation
        AiDriver.Navigate();

        // Assign the destination
        Destination.x = (float)AiDriver.X;
        Destination.y = (float)AiDriver.Y;


        // Get the distance from the current position to the destination
        Vector3 Towards = Destination - transform.position;
        float RemainingDistance = Towards.magnitude;
        Quaternion TowardsRotation = Quaternion.LookRotation( Towards );

        // Rotate to the destination
        transform.rotation = Quaternion.Lerp( transform.rotation, TowardsRotation, Time.deltaTime * 10 );

        // Move the player
        transform.position = new Vector3( (float)AiDriver.X, (float)AiDriver.Y, 0 );
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Moves the player to the specified destination.
    /// </summary>
    /// <param name="Destination">The location to move to.</param>
    public void MoveTo( Vector3 Destination )
    {
        
    }

    #endregion
}
