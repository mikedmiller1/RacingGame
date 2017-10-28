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
    public Environment( double XMin, double XMax, double YMin, double YMax )
    {
        // Assign the properties
        this.XMin = XMin;
        this.XMax = XMax;
        this.YMin = YMin;
        this.YMax = YMax;
    }

    #endregion



    #region Properties

    private List<AIDriver> _AIDrivers = new List<AIDriver>();
    /// <summary>
    /// The AI drivers in the environment.
    /// </summary>
    public List<AIDriver> AIDrivers
    {
        get { return _AIDrivers; }
        set
        {
            _AIDrivers = value;
        }
    }



    private HumanDriver _HumanDriver;

    public HumanDriver HumanDriver
    {
        get { return _HumanDriver; }
        set
        {
            _HumanDriver = value;
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



    private double _XMin;
    /// <summary>
    /// The minimum X value of the environment.
    /// </summary>
    public double XMin
    {
        get { return _XMin; }
        set { _XMin = value; }
    }



    private double _XMax;
    /// <summary>
    /// The maximum X value of the environment.
    /// </summary>
    public double XMax
    {
        get { return _XMax; }
        set { _XMax = value; }
    }



    private double _YMin;
    /// <summary>
    /// The minimum Y value of the environment.
    /// </summary>
    public double YMin
    {
        get { return _YMin; }
        set { _YMin = value; }
    }



    private double _YMax;
    /// <summary>
    /// The maximum Y value of the environment.
    /// </summary>
    public double YMax
    {
        get { return _YMax; }
        set { _YMax = value; }
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
            foreach ( AIDriver CurrentDriver in AIDrivers )
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
            foreach ( AIDriver CurrentDriver in AIDrivers )
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
                foreach ( AIDriver CurrentDriver in AIDrivers )
                {
                    CurrentDriver.RunSimulation = value;
                }
            }
        }
    }


    #endregion

}
