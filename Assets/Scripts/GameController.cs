using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    #region Properties

    /// <summary>
    /// The number of AI drivers.
    /// </summary>
    public float NumberOfAIDrivers;



    /// <summary>
    /// List of AI driver objects.
    /// </summary>
    [HideInInspector]
    public List<GameObject> AIDrivers;



    /// <summary>
    /// Reference to the track object.
    /// </summary>
    public GameObject Track;



    /// <summary>
    /// Reference to the opponent prefab.
    /// </summary>
    public GameObject PlayerPrefab;



    /// <summary>
    /// List of waypoint locations around the track.
    /// </summary>
    [HideInInspector]
    public List<Vector2> Waypoints = new List<Vector2> ();



    /// <summary>
    /// The Waypoint game object to use when debugging.
    /// </summary>
    public GameObject WaypointPrefab;



    /// <summary>
    /// Radius within which the player has arrived at the waypoint.
    /// </summary>
    public float WaypointArriveRadius;



    /// <summary>
    /// The initial spacing of the drivers for the start.
    /// </summary>
    public float DriverStartSpacing = 2.5f;



    /// <summary>
    /// The normal max speed of the AI drivers, before random adjustment.
    /// </summary>
    public float NormalMaxSpeed = 0.1f;



    /// <summary>
    /// The max speed random adjustment range to apply.
    /// </summary>
    public float MaxSpeedAdjustmentRange = 0.015f;



    /// <summary>
    /// The speed below which the driver is considered stuck.
    /// </summary>
    public float StuckSpeed = 7;



    /// <summary>
    /// The length of time a driver should be stuck before initiating unstuck mode.
    /// </summary>
    public float StuckTimeout = 2;



    /// <summary>
    /// The amount of time the driver should reverse to get unstuck.
    /// </summary>
    public float GetUnstuckTime = 1;



    /// <summary>
    /// Global random number generator.
    /// </summary>
    private System.Random rand = new System.Random ();



    /// <summary>
    /// Flag to control extra debugging info.
    /// </summary>
    public bool Debugging;

    private bool isPaused;

    #endregion



    #region Unity Methods

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        // Get the waypoints from the game objects around the track
        Waypoint[] WaypointsFromTrack = Track.GetComponentsInChildren<Waypoint> ();
        foreach( Waypoint CurrentWaypoint in WaypointsFromTrack )
        {
            // Add the waypoint to the list
            Waypoints.Add( new Vector2( CurrentWaypoint.GetPosition().x, CurrentWaypoint.GetPosition().y ) );

            if( Debugging )
            {
                Instantiate( WaypointPrefab, CurrentWaypoint.GetPosition(), Quaternion.identity );
            }
        }


        // Create the driver game objects
        AIDrivers = new List<GameObject>();
        for( int PlayerNum = 0; PlayerNum < NumberOfAIDrivers; PlayerNum++ )
        {
            // Calculate the initial position
            Vector2 StartPosition = new Vector2 (Mathf.Floor(((PlayerNum+1)/2)) * DriverStartSpacing, 0);

            // Stagger the drivers
            if( PlayerNum % 2 == 0 )
            {
                StartPosition.y += -0.5f;
            }
            else
            {
                StartPosition.y += 0.5f;
            }


            // Create the driver in the UI environment
            GameObject NewDriver = Instantiate( PlayerPrefab, StartPosition, Quaternion.Euler(0,0,90) );
            AIDrivers.Add( NewDriver );

            // Create a new driver AI
            string DriverName = "Player " + PlayerNum.ToString ();
            NewDriver.GetComponent<AIPlayerController>().Name = DriverName;
            NewDriver.GetComponent<AIPlayerController>().ArriveRadius = WaypointArriveRadius;
            NewDriver.GetComponent<AIPlayerController>().Waypoints = new List<Vector2>( Waypoints );
            NewDriver.GetComponent<AIPlayerController>().rand = rand;
            NewDriver.GetComponent<AIPlayerController>().Debugging = Debugging;
            NewDriver.GetComponent<AIPlayerController>().MaxSpeed = NormalMaxSpeed;
            NewDriver.GetComponent<AIPlayerController>().StuckSpeed = StuckSpeed;
            NewDriver.GetComponent<AIPlayerController>().StuckTimeout = StuckTimeout;
            NewDriver.GetComponent<AIPlayerController>().GetUnstuckTime = GetUnstuckTime;
            NewDriver.GetComponent<SpriteRenderer>().material.SetColor( "_Color", Random.ColorHSV( 0f, 1f, 1f, 1f, 0.5f, 1f ) );

            // Get a random speed adjustment
            double SpeedAdjustment = (rand.NextDouble () - 0.5) * MaxSpeedAdjustmentRange;
            NewDriver.GetComponent<AIPlayerController>().MaxSpeed += (float)SpeedAdjustment;

            // Activate the driver navigation
            NewDriver.GetComponent<AIPlayerController>().NavigationActive = true;
        }
        isPaused = false;
    }



    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Check for esc pressed
        //		if (Input.GetKeyDown (KeyCode.Escape) && !isPaused) {
        //			for (int PlayerNum = 0; PlayerNum < NumberOfAIDrivers; PlayerNum++)
        //				AIDrivers [PlayerNum].GetComponent<AIPlayerController> ().NavigationActive = false;
        //		} else if (Input.GetKeyDown (KeyCode.Escape) && isPaused) {
        //			for (int PlayerNum = 0; PlayerNum < NumberOfAIDrivers; PlayerNum++)
        //				AIDrivers [PlayerNum].GetComponent<AIPlayerController> ().NavigationActive = true;
        //		}
    }

    #endregion
}
