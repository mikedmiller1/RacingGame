using System;
using System.Collections.Generic;

/// <summary>
/// The environment containing drivers, goals, obstacles, and trajectories.
/// </summary>
public class Environment
{
    #region Constructor

    /// <summary>
    /// Creates a new environment.
    /// </summary>
    /// <param name="Height">The height of the environment.</param>
    /// <param name="Width">The width of the environment.</param>
    public Environment( int Height, int Width )
    {
        // Assign the properties
        this._Height = Height;
        this._Width = Width;
    }

    #endregion



    #region Properties

    private List<Driver> _Drivers = new List<Driver>();
    /// <summary>
    /// The drivers in the environment.
    /// </summary>
    public List<Driver> Drivers
    {
        get { return _Drivers; }
        set
        {
            _Drivers = value;
        }
    }



    private List<Goal> _Goals = new List<Goal>();
    /// <summary>
    /// The goals in the environment.
    /// </summary>
    public List<Goal> Goals
    {
        get { return _Goals; }
        set
        {
            _Goals = value;
        }
    }



    private List<Obstacle> _Obstacles = new List<Obstacle>();
    /// <summary>
    /// The obstacles in the environment.
    /// </summary>
    public List<Obstacle> Obstacles
    {
        get { return _Obstacles; }
        set
        {
            _Obstacles = value;
        }
    }



    private List<Path> _AllPaths = new List<Path>();
    /// <summary>
    /// The paths of all the drivers in the environment.
    /// </summary>
    public List<Path> AllPaths
    {
        get { return _AllPaths; }
        set
        {
            _AllPaths = value;
        }
    }




    private Path _BestPath;
    /// <summary>
    /// The best path of the driver.
    /// </summary>
    public Path BestPath
    {
        get { return _BestPath; }
        set
        {
            _BestPath = value;
        }
    }



    private int _Height;
    /// <summary>
    /// The height of the environment
    /// </summary>
    public int Height
    {
        get { return _Height; }
    }



    private int _Width;
    /// <summary>
    /// The width of the environment
    /// </summary>
    public int Width
    {
        get { return _Width; }
    }



    private Random _Random = new Random();
    /// <summary>
    /// Random number generator.
    /// </summary>
    public Random Random
    {
        get { return _Random; }
    }



    private bool _NavigationActive = false;
    /// <summary>
    /// Flag indicating if the driver navigation should run.
    /// </summary>
    public bool NavigationActive
    {
        get { return _NavigationActive; }
        set
        {
            _NavigationActive = value;

            // Set the value in all the drivers
            foreach ( Driver CurrentDriver in Drivers )
            {
                CurrentDriver.NavigationActive = value;
            }
        }
    }



    private bool _PlannerActive = false;
    /// <summary>
    /// Flag indicating if the driver planners should run.
    /// </summary>
    public bool PlannerActive
    {
        get { return _PlannerActive; }
        set
        {
            _PlannerActive = value;

            // Set the value in all the drivers
            foreach ( Driver CurrentDriver in Drivers )
            {
                CurrentDriver.PlannerActive = value;
            }
        }
    }



    private bool _RunSimulation = false;
    /// <summary>
    /// Flag indicating that the simulation should run.
    /// </summary>
    public bool RunSimulation
    {
        get { return _RunSimulation; }
        set
        {
            if ( _RunSimulation != value )
            {
                _RunSimulation = value;

                // Set the value in all the robots
                foreach ( Driver CurrentDriver in Drivers )
                {
                    CurrentDriver.RunSimulation = value;
                }
            }
        }
    }


    #endregion

}
