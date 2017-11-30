using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{

	#region Driver Properties

	/// <summary>
	/// The name of the driver.
	/// </summary>
	[HideInInspector]
	public string Name;



	/// <summary>
	/// Maximum speed of the player.
	/// </summary>
	[HideInInspector]
	public float MaxSpeed;



	/// <summary>
	/// Flag to control extra debugging info.
	/// </summary>
	[HideInInspector]
	public bool Debugging = false;

	#endregion



	#region Path Planning Properties

	/// <summary>
	/// Flag that controls the movement of the AI driver.
	/// </summary>
	[HideInInspector]
	public bool NavigationActive = false;



	/// <summary>
	/// Radius within which the driver has arrived at a waypoint.
	/// </summary>
	[HideInInspector]
	public float ArriveRadius;



	/// <summary>
	/// List of all waypoints.
	/// </summary>
	[HideInInspector]
	public List<Vector2> Waypoints = new List<Vector2> ();


	/// <summary>
	/// The current waypoint that the driver is moving towards.
	/// </summary>
	[HideInInspector]
	public Vector2 CurrentWaypoint;

	#endregion


	#region Local Avoidance Properties

	/// <summary>
	/// The distance to perform the raycast.
	/// </summary>
	public float RaycastDistance;



	/// <summary>
	/// The angle of the left and right raycasts, in degrees.
	/// </summary>
	public float RaycastAngle;



	/// <summary>
	/// Angle to adjust path to avoid collisions.
	/// </summary>
	public float CollisionAvoidanceAngle;



	/// <summary>
	/// Reference to the global random number generator in the GameController.
	/// </summary>
	[HideInInspector]
	public System.Random rand;



	/// <summary>
	/// Direction chosen for current obstacle avoidance.
	/// 1 is left, -1 is right.
	/// </summary>
	private int AvoidanceDirection = 1;



	/// <summary>
	/// Flag indicating if the driver is currently avoiding an obstacle.
	/// </summary>
	private bool Avoiding = false;



    /// <summary>
    /// The previous position of the driver, used to calculate velocity.
    /// </summary>
    private Vector2 PreviousPosition = new Vector2();
    private List<Vector2> AveragePreviousPosition = new List<Vector2>(10);


    /// <summary>
    /// The length of time to be stuck before initiating unstuck mode.
    /// </summary>
    public float StuckTimeout = 2;



    /// <summary>
    /// The amount of time the driver should reverse to get unstuck.
    /// </summary>
    public float GetUnstuckTime = 2;



    /// <summary>
    /// The amount of time the driver has been stuck and not moving.
    /// </summary>
    private float ElapsedStuckTime = 0;



    /// <summary>
    /// The amount of time the driver has spent getting unstuck.
    /// </summary>
    private float ElapsedGettingUnstuckTime = 0;



    /// <summary>
    /// Flag indicating if the driver is in the process of getting unstuck.
    /// </summary>
    private bool GettingUnstuck = false;

	#endregion



	#region Fields

	/// <summary>
	/// The position of the AI player as a Vector2.
	/// </summary>
	private Vector2 Position {
		get { return new Vector2 (transform.position.x, transform.position.y); }
	}

	#endregion



	#region Unity Methods

	// Update is called once per frame
	void FixedUpdate ()
	{
		// If there are waypoints to use and navigation is active
		if (Waypoints.Count > 0 && NavigationActive) {
			// Get the next waypoint from the list of waypoints
			Vector2 NextWaypoint = Waypoints [0];

			// Check if the current waypoint has changed from last time
			if (CurrentWaypoint != NextWaypoint) {
				// Assign the next waypoint as the current waypoint
				CurrentWaypoint = NextWaypoint;
			}

			// Get the vector from the current position to the waypoint
			Vector2 Towards = CurrentWaypoint - Position;


			// Perform local avoidance
			// Define the origin of the raycasts
			Vector2 RaycastOrigin = Position + Towards.normalized * 0.6f;

			// Define the directions for the collision raycasts
			Vector2 CenterDirection = Towards.normalized;
			Vector2 LeftDirection = Quaternion.Euler (0, 0, RaycastAngle) * CenterDirection;
			Vector2 RightDirection = Quaternion.Euler (0, 0, -RaycastAngle) * CenterDirection;

			// Perform a raycast forward and to either side
			bool CenterCollision = Physics2D.Raycast (RaycastOrigin, CenterDirection, RaycastDistance);
			bool LeftCollision = Physics2D.Raycast (RaycastOrigin, LeftDirection, RaycastDistance);
			bool RightCollision = Physics2D.Raycast (RaycastOrigin, RightDirection, RaycastDistance);

			// Draw raycast lines if debugging
			if (Debugging) {
				Vector2 CenterLine = new Vector3 (CenterDirection.x * RaycastDistance, CenterDirection.y * RaycastDistance, 0);
				Vector2 LeftLine = new Vector3 (LeftDirection.x * RaycastDistance, LeftDirection.y * RaycastDistance, 0);
				Vector2 RightLine = new Vector3 (RightDirection.x * RaycastDistance, RightDirection.y * RaycastDistance, 0);
				Debug.DrawRay (RaycastOrigin, CenterLine, Color.blue, Time.deltaTime, false);
				Debug.DrawRay (RaycastOrigin, LeftLine, Color.green, Time.deltaTime, false);
				Debug.DrawRay (RaycastOrigin, RightLine, Color.red, Time.deltaTime, false);
			}


			// Check if any raycasts detected a collision
			// Center
			if (CenterCollision) {
				// If we are not already avoiding an obstacle, randomly pick a direction
				// Otherwise, the driver will keep going in the direction already chosen
				if (!Avoiding) {
					// For a center collision, pick a random direction to turn
					// Get a random number from 0 to 1
					double r = rand.NextDouble ();

					// If the random number is < 0.5, go left
					if (r < 0.5) {
						AvoidanceDirection = 1;
					}

                    // Otherwise, go right
                    else {
						AvoidanceDirection = -1;
					}
				}

				// Set the avoiding flag
				Avoiding = true;
			}

            // Left
            else if (LeftCollision) {
				AvoidanceDirection = -1;
				Avoiding = true;
			}

            // Right
            else if (RightCollision) {
				AvoidanceDirection = 1;
				Avoiding = true;
			}

            // No collisions
            else {
				Avoiding = false;
			}


			// If the driver is avoiding an obstacle
			if (Avoiding && !GettingUnstuck) {
				// Rotate the towards vector by the avoidance angle, in the correct direction
				Towards = Quaternion.Euler (0, 0, CollisionAvoidanceAngle * AvoidanceDirection) * Towards;
            }


            // Calculate the vehicle velocity
            Vector2 Velocity = (Position - PreviousPosition) / Time.fixedDeltaTime;

            // Check if the driver is stuck (not moving, or barely moving)
            if( Velocity.magnitude < 10 )
            {
                // Increment the stuck time
                ElapsedStuckTime += Time.fixedDeltaTime;
            }
            // Otherwise, reduce the stuck time
            else
            {
                ElapsedStuckTime -= Time.fixedDeltaTime * 3;
            }

            // If the stuck timeout has elapsed
            if( ElapsedStuckTime >= StuckTimeout )
            {
                // Enable getting unstuck mode
                GettingUnstuck = true;
            }


			// Initialize the distance to move as the maximum speed
			float DistanceToMove = MaxSpeed;

			// If moving at the max speed is greater than the distance to the waypoint, use the shorter distance
			if (DistanceToMove > Towards.magnitude) {
				DistanceToMove = Towards.magnitude;
			}

			// Multiply the direction unit vector by the speed to get the XY distance
			Vector2 MoveVector = Towards.normalized * DistanceToMove;

            // If the driver is in getting unstuck mode
            if( GettingUnstuck )
            {
                // Reverse the move vector to go backwards
                MoveVector *= -1;

                // Increment the getting unstuck time
                ElapsedGettingUnstuckTime += Time.fixedDeltaTime;

                // If the required getting unstuck time has passed
                if( ElapsedGettingUnstuckTime >= GetUnstuckTime )
                {
                    // Turn off getting unstuck mode
                    GettingUnstuck = false;
                    ElapsedGettingUnstuckTime = 0;
                    ElapsedStuckTime = 0;
                }
            }

			// Calculate the new position
			Vector2 NewPosition = Position + MoveVector;

			// Move to the new position
			transform.position = NewPosition;

			// Rotate to the destination
			Quaternion TowardsRotation = Quaternion.LookRotation (Towards.normalized);
			TowardsRotation = TowardsRotation * Quaternion.Euler (0, 90, 90);  // Have to rotate 90* about Y and Z for some reason...
			transform.rotation = Quaternion.Slerp (transform.rotation, TowardsRotation, Time.deltaTime * 8);


			// Check if we reached a waypoint
			// Get the distance to the current waypoint
			Vector2 DistanceRemaining = CurrentWaypoint - Position;

			// If the distance is less than the arrive radius
			if (DistanceRemaining.magnitude <= ArriveRadius) {
				// Move the current waypoint to the end of the list
				Waypoints.Add (new Vector2 (CurrentWaypoint.x, CurrentWaypoint.y));
				Waypoints.Remove (CurrentWaypoint);
			}


            // Store the current position as the previous position for the next update
            PreviousPosition.x = Position.x;
            PreviousPosition.y = Position.y;
		}

	}

	#endregion
}
