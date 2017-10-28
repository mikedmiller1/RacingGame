﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    #region Properties

    /// <summary>
    /// The number of AI players.
    /// </summary>
    public float NumberOfAIPlayers;



    /// <summary>
    /// List of player objects.
    /// </summary>
    private List<GameObject> Players;



    /// <summary>
    /// The AI environment.
    /// </summary>
    private Environment Environment;



    /// <summary>
    /// Reference to the track object.
    /// </summary>
    public GameObject Track;



    /// <summary>
    /// Reference to the human player object.
    /// </summary>
    public GameObject HumanPlayer;



    /// <summary>
    /// Reference to the player prefab.
    /// </summary>
    public GameObject PlayerPrefab;



    /// <summary>
    /// Reference to the waypoint prefab.
    /// </summary>
    public GameObject WaypointPrefab;



    /// <summary>
    /// Reference to the obstacle prefab.
    /// </summary>
    public GameObject ObstaclePrefab;



    /// <summary>
    /// List of waypoint objects in the environment.
    /// </summary>
    [HideInInspector]
    public List<GameObject> Waypoints = new List<GameObject>();



    /// <summary>
    /// List of obstacle objects in the environment.
    /// </summary>
    [HideInInspector]
    public List<GameObject> Obstacles = new List<GameObject>();



    /// <summary>
    /// Radius within which the player has arrived at the destination.
    /// </summary>
    public float WaypointRadius;



    /// <summary>
    /// Radius of the obstacles.
    /// </summary>
    public float ObstacleRadius;

    #endregion



    #region Unity Methods

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {

        // Initialize the AI driver environment
        Environment = new Environment( -200, 200, -200, 200 );

        // Get the waypoints from the game objects around the track
        Waypoint[] WaypointsFromTrack = Track.GetComponentsInChildren<Waypoint>();
        foreach( Waypoint CurrentWaypoint in WaypointsFromTrack )
        {
            // Create the waypoint in the UI environment
            GameObject NewWaypoint = Instantiate( WaypointPrefab, CurrentWaypoint.transform.position, Quaternion.identity );
            NewWaypoint.transform.localScale = new Vector3( WaypointRadius, WaypointRadius, 0 );
            Waypoints.Add( NewWaypoint );

            // Add the waypoint as a goal in the AI environment
            Environment.Goals.Add( new Goal( CurrentWaypoint.transform.position, WaypointRadius ) );
        }


        // Add the human player object in the AI driver environment
        Environment.HumanDriver = new HumanDriver( HumanPlayer.transform.position.x, HumanPlayer.transform.position.z, ObstacleRadius );



        /*  Hard coded waypoints and obstacles for testing
        // Define a list of waypoint coordiantes
        List<Vector3> WaypointCoordinatesList = new List<Vector3>();
        WaypointCoordinatesList.Add( new Vector3( 10, 10, 0 ) );
        WaypointCoordinatesList.Add( new Vector3( -10, 10, 0 ) );
        WaypointCoordinatesList.Add( new Vector3( -10, -10, 0 ) );
        WaypointCoordinatesList.Add( new Vector3( 10, -10, 0 ) );

        // Define a list of obstacles
        List<Vector3> ObstacleCoordinatesList = new List<Vector3>();
        ObstacleCoordinatesList.Add( new Vector3( 4, 11, 0 ) );
        ObstacleCoordinatesList.Add( new Vector3( -4, 9, 0 ) );
        ObstacleCoordinatesList.Add( new Vector3( -10, -5, 0 ) );
        ObstacleCoordinatesList.Add( new Vector3( -5, -10, 0 ) );

        // Populate the waypoints
        foreach( Vector3 CurrentWaypoint in WaypointCoordinatesList )
        {
            // Create the waypoint in the UI environment
            GameObject NewWaypoint = Instantiate( WaypointPrefab, CurrentWaypoint, Quaternion.identity );
            NewWaypoint.transform.localScale = new Vector3( WaypointRadius, WaypointRadius, 0 );
            Waypoints.Add( NewWaypoint );

            // Add the waypoint as a goal in the AI environment
            Environment.Goals.Add( new Goal( CurrentWaypoint, WaypointRadius ) );
        }
        
        
        // Populate the obstacles
        foreach( Vector3 CurrentObstacle in ObstacleCoordinatesList )
        {
            // Create the obstacle in the UI environment
            GameObject NewObstacle = Instantiate( ObstaclePrefab, CurrentObstacle, Quaternion.identity );
            NewObstacle.transform.localScale = new Vector3( ObstacleRadius, ObstacleRadius, 0 );
            Obstacles.Add( NewObstacle );

            // Add the obstacle in the AI environment
            Environment.Obstacles.Add( new Obstacle( CurrentObstacle, ObstacleRadius ) );
        }
        */



        // Create the driver game objects
        Players = new List<GameObject>();
        for( int PlayerNum = 0; PlayerNum < NumberOfAIPlayers; PlayerNum++ )
        {
            // Calculate the initial position
            Vector3 StartPosition = new Vector3( PlayerNum * 2.5f, 0, 0 );

            // Create the driver in the UI environment
            GameObject NewDriver = Instantiate( PlayerPrefab, StartPosition, Quaternion.identity );
            Players.Add( NewDriver );

            // Create a new driver AI
            string DriverName = "Player " + PlayerNum.ToString();
            AIDriver NewDriverAI = new AIDriver( DriverName, Environment, NewDriver.transform.position.x, NewDriver.transform.position.y, ObstacleRadius )
            {
                ShouldCheckDirectPath = NewDriver.GetComponent<AIPlayerController>().CheckDirectPath,
                Speed = NewDriver.GetComponent<AIPlayerController>().MaxSpeed
            };
            NewDriver.GetComponent<AIPlayerController>().AiDriver = NewDriverAI;
            NewDriver.GetComponent<AIPlayerController>().ArriveRadius = WaypointRadius;

            // Add the driver in the AI environment
            Environment.AIDrivers.Add( NewDriverAI );
        }


        // Start the AI drivers
        foreach( GameObject Player in Players )
        {
            // Start the driver
            Player.GetComponent<AIPlayerController>().AiDriver.PlannerActive = true;
            Player.GetComponent<AIPlayerController>().AiDriver.NavigationActive = true;
            Player.GetComponent<AIPlayerController>().AiDriver.RunSimulation = true;
        }
    }



    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Update the human driver position
        Environment.HumanDriver.X = HumanPlayer.transform.position.x;
        Environment.HumanDriver.Y = HumanPlayer.transform.position.y;


        // Check for a left mouse click
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            
        }
    }

    #endregion
}
