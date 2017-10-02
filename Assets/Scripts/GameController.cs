using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    #region Properties

    /// <summary>
    /// The number of AI players.
    /// </summary>
    public float NumberOfPlayers;



    /// <summary>
    /// List of player objects.
    /// </summary>
    private List<GameObject> Players;



    /// <summary>
    /// The AI environment.
    /// </summary>
    private Environment Environment;



    /// <summary>
    /// Reference to the player prefab.
    /// </summary>
    public GameObject PlayerPrefab;



    /// <summary>
    /// Reference to the waypoint prefab.
    /// </summary>
    public GameObject WaypointPrefab;



    /// <summary>
    /// List of waypoint objects in the environment.
    /// </summary>
    [HideInInspector]
    public List<GameObject> Waypoints;

    #endregion



    #region Unity Methods

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        // Initialize the AI driver environment
        Environment = new Environment( 5, 5 );


        // Define a list of waypoint coordiantes
        List<Vector3> WaypointCoordinatesList = new List<Vector3>();
        WaypointCoordinatesList.Add( new Vector3( 10, 0, 0 ) );
        WaypointCoordinatesList.Add( new Vector3( 0, 0, 10 ) );
        WaypointCoordinatesList.Add( new Vector3( -10, 0, 0 ) );
        WaypointCoordinatesList.Add( new Vector3( 0, 0, -10 ) );

        // Populate the waypoints
        foreach( Vector3 CurrentCoordinate in WaypointCoordinatesList )
        {
            // Create the waypoint in the UI environment
            Waypoints.Add( Instantiate( WaypointPrefab, CurrentCoordinate, Quaternion.identity ) );

            // Add the waypoint as a goal in the AI environment
            Environment.Goals.Add( new Goal( CurrentCoordinate, 0.001 ) );
        }



        // Create the AI drivers
        Players = new List<GameObject>();
        for( int PlayerNum = 0; PlayerNum < NumberOfPlayers; PlayerNum++ )
        {
            // Calculate the initial position
            Vector3 StartPosition = new Vector3( PlayerNum * 2.5f, 0, 0 );

            // Create the player
            Players.Add( Instantiate( PlayerPrefab, StartPosition, Quaternion.identity ) );
        }

        // Initialize the AI drivers
        foreach( GameObject Player in Players )
        {
            Player.GetComponent<PlayerController>().AiDriver = new Driver( Environment, Player.transform.position.x, Player.transform.position.z, 0.5 );
        }


        // Initialize the AI driver
        foreach( GameObject Player in Players )
        {
            // Start the driver
            Player.GetComponent<PlayerController>().AiDriver.PlannerActive = true;
            Player.GetComponent<PlayerController>().AiDriver.NavigationActive = true;
            Player.GetComponent<PlayerController>().AiDriver.RunSimulation = true;
        }
    }



    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Check for a left mouse click
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            
        }
    }

    #endregion
}
