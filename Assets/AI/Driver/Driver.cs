using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

/// <summary>
/// A circular holonomic robot driver.
/// Runs a real-time adaptive planning (RAMP) algorithm to reach goals.
/// Adapts in real-time to avoid obstacles and other drivers.
/// </summary>
public class Driver : ObjectBase
{
    #region Constructor

    /// <summary>
    /// Creates a new driver.
    /// </summary>
    /// <param name="Name">The name of the driver.</param>
    /// <param name="Environment">The environment of the driver.</param>
    /// <param name="PositionX">The X position of the driver.</param>
    /// <param name="PositionY">The Y position of the driver.</param>
    /// <param name="Radius">The radius of the driver.</param>
    public Driver( string Name, Environment Environment, double PositionX, double PositionY, double Radius )
        : base( PositionX, PositionY, Radius )
    {
        // Set the name
        this._Name = Name;

        // Set the environment reference
        this.Environment = Environment;

        // Get the goals
        LookForGoals();

        // Set up the planner timer
        PlanTimer = new System.Timers.Timer( PlanInterval );
        PlanTimer.Elapsed += new ElapsedEventHandler( Plan );

        // Start the timer
        PlanTimer.Enabled = true;
    }

    #endregion



    #region Driver Properties

    private string _Name;
    /// <summary>
    /// The name of the driver.
    /// </summary>
    public string Name
    {
        get { return _Name; }
    }



    /// <summary>
    /// Reference to the environment.
    /// </summary>
    private Environment Environment;



    /// <summary>
    /// Reference to the main random number generator.
    /// </summary>
    public Random Random
    {
        get { return Environment.Random; }
    }



    private int _PlanInterval = 1;
    /// <summary>
    /// The interval between planning iterations, in milliseconds.
    /// </summary>
    public int PlanInterval
    {
        get { return _PlanInterval; }
        set
        {
            _PlanInterval = value;
        }
    }



    private double _Speed = 0.1;
    /// <summary>
    /// The speed of the driver, in units per time-step.
    /// </summary>
    public double Speed
    {
        get { return _Speed; }
        set { _Speed = value; }
    }



    private List<Goal> _Goals = new List<Goal>();
    /// <summary>
    /// The driver's goals.
    /// </summary>
    public List<Goal> Goals
    {
        get { return _Goals; }
        set
        {
            _Goals = value;
        }
    }



    private Goal _CurrentGoal;
    /// <summary>
    /// The current goal that the driver is moving towards.
    /// This should be the closest goal to the driver.
    /// </summary>
    public Goal CurrentGoal
    {
        get { return _CurrentGoal; }
        set { _CurrentGoal = value; }
    }



    private List<ObjectBase> _KnownObstacles = new List<ObjectBase>();
    /// <summary>
    /// List of obstacles that the driver has observed.
    /// </summary>
    public List<ObjectBase> KnownObstacles
    {
        get { return _KnownObstacles; }
        set { _KnownObstacles = value; }
    }



    private bool _NavigationReady = false;
    /// <summary>
    /// Flag indicating if the driver is ready to navigate along a path.
    /// </summary>
    public bool NavigationReady
    {
        get { return _NavigationReady; }
        set { _NavigationReady = value; }
    }



    private bool _NavigationActive = false;
    /// <summary>
    /// Flag indicating if the navigation should run.
    /// </summary>
    public bool NavigationActive
    {
        get { return _NavigationActive; }
        set { _NavigationActive = value; }
    }



    private bool _PlannerActive = false;
    /// <summary>
    /// Flag indicating if the planner should run.
    /// </summary>
    public bool PlannerActive
    {
        get { return _PlannerActive; }
        set { _PlannerActive = value; }
    }



    private bool _RunSimulation = false;
    /// <summary>
    /// Flag indicating if the simulation should run.
    /// </summary>
    public bool RunSimulation
    {
        get { return _RunSimulation; }
        set { _RunSimulation = value; }
    }



    /// <summary>
    /// Lock to control access to Paths.
    /// </summary>
    public Mutex PathMutex = new Mutex();



    /// <summary>
    /// Lock to control access to Obstacles.
    /// </summary>
    public Mutex ObstacleMutex = new Mutex();



    /// <summary>
    /// Lock to control access to Goals.
    /// </summary>
    public Mutex GoalMutex = new Mutex();



    /// <summary>
    /// Timer controlling the planner.
    /// </summary>
    private System.Timers.Timer PlanTimer;

    #endregion



    #region Path Planning Properties

    private List<Path> _Paths = new List<Path>();
    /// <summary>
    /// A list of possible paths from the current location to the current goal.
    /// </summary>
    public List<Path> Paths
    {
        get { return _Paths; }
        set { _Paths = value; }
    }



    /// <summary>
    /// The current best path, out of the collection of possible paths.
    /// </summary>
    public Path CurrentBestPath
    {
        get
        {
            // Null check
            if ( Paths == null ||
                    Paths.Count < 1 ||
                    Paths[ 0 ] == null )
            { return null; }

            return Paths[ 0 ];
        }
    }



    /// <summary>
    /// The current knot to travel towards.
    /// </summary>
    public Knot CurrentKnot
    {
        get
        {
            // Null check
            if ( CurrentBestPath.Knots == null ||
                CurrentBestPath.Knots.Count < 2 ||
                CurrentBestPath.Knots[ 1 ] == null )
            { return null; }

            return CurrentBestPath.Knots[ 1 ];
        }
    }



    private int _NumberOfPaths = 5;
    /// <summary>
    /// The number of possible paths to maintain at all times.
    /// </summary>
    public int NumberOfPaths
    {
        get { return _NumberOfPaths; }
        set { _NumberOfPaths = value; }
    }



    private int _MinKnots = 3;
    /// <summary>
    /// The minimum number of knots when generating a new trajectory.
    /// This includes the driver and goal, so it should be a minimum of 3.
    /// </summary>
    public int MinKnots
    {
        get { return _MinKnots; }
        set { _MinKnots = value; }
    }



    private int _MaxKnots = 20;
    /// <summary>
    /// The maximum number of knots when generating a new trajectory.
    /// </summary>
    public int MaxKnots
    {
        get { return _MaxKnots; }
        set { _MaxKnots = value; }
    }



    private double _SafeDistance = 1;
    /// <summary>
    /// A "safe" distance around obstacles when evaluating the clear metric.
    /// </summary>
    public double SafeDistance
    {
        get { return _SafeDistance; }
        set { _SafeDistance = value; }
    }



    private double _InterferenceWeight = 5;
    /// <summary>
    /// The weight of the interference exponent in the evaluation function.
    /// </summary>
    public double InterferenceWeight
    {
        get { return _InterferenceWeight; }
        set { _InterferenceWeight = value; }
    }



    private double _EvalWeightDistance = 1;
    /// <summary>
    /// The weight of the distance term in the evaluation function.
    /// </summary>
    public double EvalWeightDistance
    {
        get { return _EvalWeightDistance; }
        set { _EvalWeightDistance = value; }
    }



    private double _EvalWeightSmoothness = 1;
    /// <summary>
    /// The weight of the smoothness term in the evaluation function.
    /// </summary>
    public double EvalWeightSmoothness
    {
        get { return _EvalWeightSmoothness; }
        set { _EvalWeightSmoothness = value; }
    }



    private double _EvalWeightClearance = 1;
    /// <summary>
    /// The weight of the clear term in the evaluation function.
    /// </summary>
    public double EvalWeightClearance
    {
        get { return _EvalWeightClearance; }
        set { _EvalWeightClearance = value; }
    }



    /// <summary>
    /// The weighted probabilities of selecting each of the mutation operators.
    /// Values obtained from G3.11 - Evolutionary planner/navigator in a mobile driver environment.
    ///     { 0.6, 0.8, 0.5, 0.5, 0.5, 0.5, 0.9, 0.8 }
    /// Operators:
    ///     Crossover
    ///     MutationSmall
    ///     MutationLarge
    ///     Insert
    ///     Delete
    ///     Swap
    ///     Smooth
    ///     Repair
    /// </summary>
    private double[] OperatorProbabilities = new double[ 8 ] { 0, 0.8, 0.5, 0.5, 0.5, 0.5, 0, 0.8 };


    /// <summary>
    /// The tolerance to allow when checking if the driver has reached a knot or goal.
    /// </summary>
    private double DistanceTolerance = 0.1;



    private bool _ShouldCheckDirectPath = true;
    /// <summary>
    /// Flag indicating if the planner should check for a direct path to the goal.
    /// </summary>
    public bool ShouldCheckDirectPath
    {
        get { return _ShouldCheckDirectPath; }
        set { _ShouldCheckDirectPath = value; }
    }

    #endregion



    #region Methods

    /// <summary>
    /// Attempts to move the driver from it's current position to the nearest goal using the current best path.
    /// Avoids obstacles and other drivers.
    /// </summary>
    public void Navigate()
    {
        // If the navigation is ready and activated
        if( RunSimulation && NavigationReady && NavigationActive )
        {
            // Move the driver
            Move();

            // Check if a knot or goal has been reached
            CheckPosition();
        }
    }



    /// <summary>
    /// Path planner that runs continuously to develop the paths for the driver navigation to follow.
    /// </summary>
    public void Plan( object sender, ElapsedEventArgs e )
    {
        // If the planner should run
        if ( RunSimulation && PlannerActive )
        {
            // Get the current closest goal
            GoalMutex.WaitOne();
            Goal CurrentClosestGoal = GetNextGoal();
            GoalMutex.ReleaseMutex();

            // If there is no goal (null) then stop the driver
            if ( CurrentClosestGoal == null )
            {
                NavigationReady = false;
                return;
            }

            // Check if the closest goal has changed from last time
            if ( CurrentGoal != CurrentClosestGoal )
            {
                // Assign the closest goal as the current goal
                GoalMutex.WaitOne();
                CurrentGoal = CurrentClosestGoal;
                GoalMutex.ReleaseMutex();

                // Generate a set of possible paths
                GeneratePaths();

                // Allow the driver to move
                NavigationReady = true;
            }

            // Look around for obstacles
            LookForObstacles();

            // Evaluate paths
            EvaluatePaths();

            // Check for a direct path
            if( ShouldCheckDirectPath )
            {
                CheckDirectPath();
            }

            // Mutate paths
            MutatePaths();
        }
    }



    /// <summary>
    /// Generates a set of possible paths from the current location to the current goal.
    /// </summary>
    public void GeneratePaths()
    {
        // Get the mutex
        PathMutex.WaitOne();

        // Initialize the paths
        Paths = new List<Path>();

        // Loop through the number of paths to generate
        for ( int PathNum = 0; PathNum < NumberOfPaths; PathNum++ )
        {
            // Create a new path
            Path NewPath = new Path();

            // Get a random number of knots for the path
            // This should be between the allowed min and max
            int NumberOfKnots = Random.Next( MinKnots, MaxKnots );

            // Set the first knot to the driver
            NewPath.AddKnot( new Knot( X, Y ) );

            // Loop through the remaining number of knots to add
            // Subtract two for the first (driver) and last (goal) knots
            for ( int KnotNum = 0; KnotNum < NumberOfKnots - 1; KnotNum++ )
            {
                NewPath.AddKnot( new Knot( MathUtilities.GetRandomInRange( Environment.XMin, Environment.XMax, Random ), MathUtilities.GetRandomInRange( Environment.YMin, Environment.YMax, Random ) ) );
            }

            // Set the last knot to the goal
            NewPath.AddKnot( new Knot( CurrentGoal.X, CurrentGoal.Y ) );

            // Add the new path to the paths list
            Paths.Add( NewPath );
        }


        // Evaluate paths
        EvaluatePaths();

        // Release the mutex
        PathMutex.ReleaseMutex();
    }



    /// <summary>
    /// Evaluates the possible paths.
    /// </summary>
    public void EvaluatePaths()
    {
        // Get the mutex
        PathMutex.WaitOne();

        // Loop through the paths
        foreach ( Path CurrentPath in Paths )
        {
            // Calculate the current path cost
            EvaluatePath( CurrentPath );
        }

        // Sort the paths by cost, from lowest to highest
        Paths.Sort();

        // If the best path cost is 0, there is something wrong
        if( Paths[0].Cost == 0 )
        {
            // Regenerate the paths
            PathMutex.ReleaseMutex();
            GeneratePaths();
        }

        // Release the mutex
        PathMutex.ReleaseMutex();
    }



    /// <summary>
    /// Calculates the cost of the specified path.
    /// </summary>
    /// <param name="CurrentPath">The path to evaluate.</param>
    public void EvaluatePath( Path CurrentPath )
    {
        // Get the mutex
        PathMutex.WaitOne();

        // Loop through each knot
        foreach ( Knot CurrentKnot in CurrentPath.Knots )
        {
            // Check if the current knot is feasible
            // Loop through each obstacle
            ObstacleMutex.WaitOne();
            foreach ( ObjectBase CurrentObstacle in KnownObstacles )
            {
                // If the knot is within the current object, it is not feasible
                if ( MathUtilities.IsWithin( CurrentKnot, CurrentObstacle ) )
                {
                    CurrentKnot.IsFeasible = false;
                    CurrentPath.IsFeasible = false;
                    break;
                }
            }
            ObstacleMutex.ReleaseMutex();
        }


        // Initialize the terms to 0
        double Distance = 0;
        double Smoothness = 0;
        double Clearance = 0;

        // Calculate the segment cost terms
        // Loop through the knots in the path
        // Subtract 1 because we consider the segment between the current knot and the next knot
        for ( int KnotNum = 0; KnotNum < CurrentPath.NumberOfKnots - 1; KnotNum++ )
        {
            // Get the current and next knots
            Knot CurrentKnot = CurrentPath.Knots[ KnotNum ];
            Knot NextKnot = CurrentPath.Knots[ KnotNum + 1 ];


            // Calculate the distance term of the current segment
            Distance += MathUtilities.Distance( CurrentKnot, NextKnot );


            // Calculate the smoothness term of the current segment
            // If this is not the first knot
            if ( KnotNum > 0 )
            {
                // Get the previous knot
                Knot PreviousKnot = CurrentPath.Knots[ KnotNum - 1 ];

                // Calculate the smoothness of the three knot points.
                double SmoothnessTemp = MathUtilities.Smoothness( PreviousKnot, CurrentKnot, NextKnot );

                // If the current temp smoothness is greater than the current smoothness, use it
                if ( SmoothnessTemp > Smoothness )
                {
                    Smoothness = SmoothnessTemp;
                }
            }


            // Calculate the clearance term of the current segment
            // Loop through the obstacles
            ObstacleMutex.WaitOne();
            foreach ( ObjectBase CurrentObstacle in KnownObstacles )
            {
                // Initialize the clearance temp
                double ClearanceTemp;

                // Get the closest distance from the current segment to the current object
                double CurrentMinDistance = MathUtilities.MinDistance( CurrentKnot, NextKnot, CurrentObstacle );

                // If the min distance is greater than or equal to the safe distance
                if ( CurrentMinDistance >= SafeDistance )
                {
                    ClearanceTemp = CurrentMinDistance - SafeDistance;
                }

                // Otherwise, calculate the interference weight
                else
                {
                    ClearanceTemp = Math.Pow( Math.E, InterferenceWeight * (SafeDistance - CurrentMinDistance) ) - 1;

                    // If the min distance is within the object
                    if ( CurrentMinDistance < CurrentObstacle.Radius )
                    {
                        // The path is infeasible
                        CurrentPath.IsFeasible = false;
                    }
                }

                // If the current temp clearance is greater than the current clearance, use it
                if ( ClearanceTemp > Clearance )
                {
                    Clearance = ClearanceTemp;
                }
            }
            ObstacleMutex.ReleaseMutex();

        }


        // Calculate the total cost, applying the weights to each term
        CurrentPath.Cost = EvalWeightDistance * Distance + EvalWeightSmoothness * Smoothness + EvalWeightClearance * Clearance;

        // Release the mutex
        PathMutex.ReleaseMutex();
    }



    /// <summary>
    /// Mutates the possible paths.
    /// </summary>
    public void MutatePaths()
    {
        // Get a copy of the paths
        PathMutex.WaitOne();
        List<Path> PathsCopy = new List<Path>( Paths );
        PathMutex.ReleaseMutex();

        // Check the copy
        if ( Double.IsNaN( PathsCopy[ 0 ].Cost ) )
        { return; }

        // Randomly choose a mutation operation
        Operator Mutation = (Operator)MathUtilities.GetWeightedRandom( OperatorProbabilities, Random );

        // Get weights for each path based on the cost
        // Lower cost paths have a higher weight so they are more likely to be selected
        // Initialize the path weight array
        double[] PathWeights = new double[ NumberOfPaths ];

        // Loop through the paths
        for ( int PathNum = 0; PathNum < NumberOfPaths; PathNum++ )
        {
            // Calculate the path weight
            // Add 1 for divide by zero protection
            PathWeights[ PathNum ] = 1 / (PathsCopy[ PathNum ].Cost + 1);
        }

        // Randomly choose a path
        int PathIndex = MathUtilities.GetWeightedRandom( PathWeights, Random );
        Path CurrentPath = PathsCopy[ PathIndex ];

        // If the path has 0 or 1 knots, there is nothing to mutate
        if ( CurrentPath.NumberOfKnots <= 1 )
        {
            return;
        }

        // Initialize the new path
        Path NewPath = new Path( CurrentPath );

        // Initialize the knot index
        int KnotIndex;
        int KnotIndex2;

        // Switch based on the mutation
        switch ( Mutation )
        {
            case Operator.Crossover:

                // If there are 2 or less knots, cannot crossover
                if ( CurrentPath.NumberOfKnots <= 2 )
                { return; }


                // Initialize the second path
                int PathIndex2;
                Path CurrentPath2;

                // Loop until we pick a second path that is not the first path and has sufficient knots
                do
                {
                    PathIndex2 = MathUtilities.GetWeightedRandom( PathWeights, Random );
                    CurrentPath2 = PathsCopy[ PathIndex2 ];
                }
                while ( PathIndex != PathIndex2 && CurrentPath2.NumberOfKnots > 2 );


                // Pick a random knot for each path cut
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 2 );
                KnotIndex2 = Random.Next( 1, CurrentPath2.NumberOfKnots - 2 );

                // Initialize new PathsCopy
                Path NewPath1 = new Path();
                Path NewPath2 = new Path();

                // Loop through the knots in the first path
                for ( int KnotNum = 0; KnotNum < CurrentPath.NumberOfKnots; KnotNum++ )
                {
                    // If the current knot is before the cut knot, add it to the first new path
                    if ( KnotNum < KnotIndex )
                    {
                        NewPath1.AddKnot( new Knot( CurrentPath.Knots[ KnotNum ] ) );
                    }

                    // Otherwise, add it to the second new path
                    else
                    {
                        NewPath2.AddKnot( new Knot( CurrentPath.Knots[ KnotNum ] ) );
                    }
                }

                // Loop through the knots in the second path
                for ( int KnotNum = 0; KnotNum < CurrentPath2.NumberOfKnots; KnotNum++ )
                {
                    // If the current knot is before the cut knot, add it to the second new path
                    if ( KnotNum < KnotIndex2 )
                    {
                        NewPath2.AddKnot( new Knot( CurrentPath2.Knots[ KnotNum ] ) );
                    }

                    // Otherwise, add it to the first new path
                    else
                    {
                        NewPath1.AddKnot( new Knot( CurrentPath2.Knots[ KnotNum ] ) );
                    }
                }


                // Evaluate the new paths
                EvaluatePath( NewPath1 );
                EvaluatePath( NewPath2 );

                // If the new path cost is better than the current path, use it
                PathMutex.WaitOne();
                if ( NewPath1.Cost < CurrentPath.Cost )
                {
                    Paths[ PathIndex ] = NewPath1;
                }

                if ( NewPath1.Cost < CurrentPath2.Cost )
                {
                    Paths[ PathIndex ] = NewPath1;
                }

                if ( NewPath2.Cost < CurrentPath.Cost )
                {
                    Paths[ PathIndex2 ] = NewPath2;
                }

                if ( NewPath2.Cost < CurrentPath2.Cost )
                {
                    Paths[ PathIndex2 ] = NewPath2;
                }
                PathMutex.ReleaseMutex();
                break;


            case Operator.Delete:

                // If there are 2 or less knots, cannot delete
                // These are the current driver position and the goal
                if ( CurrentPath.NumberOfKnots <= 2 )
                { return; }

                // Pick a random knot
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 1 );

                // Delete the knot
                NewPath.RemoveKnot( KnotIndex );
                break;


            case Operator.Insert:

                // Pick a random knot
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 1 );

                // Insert a new knot at a random coordinate
                NewPath.InsertKnot( KnotIndex, new Knot( MathUtilities.GetRandomInRange( Environment.XMin, Environment.XMax, Random ), MathUtilities.GetRandomInRange( Environment.YMin, Environment.YMax, Random ) ) );
                break;


            case Operator.MutationLarge:

                // If there are 2 or less knots, cannot mutate
                // These are the current driver position and the goal
                if ( CurrentPath.NumberOfKnots <= 2 )
                { return; }

                // Pick a random knot
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 1 );

                // Replace the knot with a random coordinate
                NewPath.ReplaceKnot( KnotIndex, new Knot( MathUtilities.GetRandomInRange( Environment.XMin, Environment.XMax, Random ), MathUtilities.GetRandomInRange( Environment.YMin, Environment.YMax, Random ) ) );
                break;


            case Operator.MutationSmall:

                // If there are 2 or less knots, cannot mutate
                // These are the current driver position and the goal
                if ( CurrentPath.NumberOfKnots <= 2 )
                { return; }

                // Pick a random knot
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 1 );

                // Define the radius of the small mutation
                double SmallRadius = 1;

                // Get random numbers between -1 and 1
                double XScalar = (Random.NextDouble() * 2) - 1;
                double YScalar = (Random.NextDouble() * 2) - 1;

                // Normalize to the small radius
                double XDistance = SmallRadius * XScalar;
                double YDistance = SmallRadius * YScalar;

                // Translate to the chosen knot coordinates
                double XPosition = NewPath.Knots[ KnotIndex ].X + XDistance;
                double YPosition = NewPath.Knots[ KnotIndex ].Y + YDistance;

                // Replace the knot with the random coordinate
                NewPath.ReplaceKnot( KnotIndex, new Knot( XPosition, YPosition ) );
                break;


            case Operator.Repair:

                // If there are 1 or less knots, cannot repair
                if ( CurrentPath.NumberOfKnots <= 1 )
                { return; }

                // Pick a random knot
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 0, CurrentPath.NumberOfKnots - 1 );

                // Get a reference to the current knot and the next knot
                Knot CurrentKnot = NewPath.Knots[ KnotIndex ];
                Knot NextKnot    = NewPath.Knots[ KnotIndex + 1 ];


                // Loop through the obstacles
                ObstacleMutex.WaitOne();
                foreach ( ObjectBase CurrentObstacle in KnownObstacles )
                {
                    // Get the closest distance from the current segment to the current object
                    double CurrentMinDistance = MathUtilities.MinDistance( CurrentKnot, NextKnot, CurrentObstacle );

                    // If the min distance is greater than or equal to the safe distance
                    if ( CurrentMinDistance >= SafeDistance )
                    {
                        // Nothing to repair
                        ObstacleMutex.ReleaseMutex();
                        return;
                    }

                    // Otherwise, repair the segment
                    else
                    {
                        // Find the point in the segment that is closest to the obstacle
                        ObjectBase ClosestPoint = MathUtilities.ClosestPoint( CurrentKnot, NextKnot, CurrentObstacle );

                        // Find the vector from the obstacle to the closest point
                        Vector ClosestVector = MathUtilities.GetUnitVector( CurrentObstacle, ClosestPoint );

                        // Calculate the distance to insert the point
                        double Distance = CurrentObstacle.Radius + SafeDistance;

                        // Create a new point outside the obstacle
                        Knot NewKnot = new Knot( CurrentObstacle.X + (ClosestVector.I * Distance ), CurrentObstacle.Y + (ClosestVector.J * Distance ) );

                        // Add the knot to the path
                        NewPath.InsertKnot( KnotIndex + 1, NewKnot );
                    }
                }
                ObstacleMutex.ReleaseMutex();
                break;


            case Operator.Smooth:

                // If there are 2 or less knots, cannot smooth
                // These are the current driver position and the goal
                if ( CurrentPath.NumberOfKnots <= 2 )
                { return; }

                // Pick a random knot
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 1 );

                // Get a reference to the current knot
                CurrentKnot = NewPath.Knots[ KnotIndex ];

                // Get the previous and next nodes
                int KnotIndexPrevious = KnotIndex - 1;
                int KnotIndexNext = KnotIndex + 1;

                // Get a unit vector from the middle knot to the previous and next knots
                Vector MiddleToPrevious = MathUtilities.GetUnitVector( CurrentKnot, NewPath.Knots[ KnotIndexPrevious ] );
                Vector MiddleToNext     = MathUtilities.GetUnitVector( CurrentKnot, NewPath.Knots[ KnotIndexNext ] );

                // Get the distance between each set of knots
                double MiddleToPreviousDistance = MathUtilities.Distance( CurrentKnot, NewPath.Knots[ KnotIndexPrevious ] );
                double MiddleToNextDistance     = MathUtilities.Distance( CurrentKnot, NewPath.Knots[ KnotIndexNext ] );

                // Use the shortest vector distance so we don't go past a knot
                double DistanceToUse = Math.Min( MiddleToPreviousDistance, MiddleToNextDistance );

                // Get a random amount to move along each vector
                double RandomAmount = Random.NextDouble();

                // Create two new knots at the random distance around the middle knot
                Knot NewPrevious = new Knot( CurrentKnot.X + ( DistanceToUse * MiddleToPrevious.I * RandomAmount ), CurrentKnot.Y + ( DistanceToUse * MiddleToPrevious.J * RandomAmount ) );
                Knot NewNext = new Knot( CurrentKnot.X + ( DistanceToUse * MiddleToNext.I * RandomAmount ), CurrentKnot.Y + ( DistanceToUse * MiddleToNext.J * RandomAmount ) );

                // Remove the middle knot
                NewPath.RemoveKnot( KnotIndex );

                // Insert the new knots
                NewPath.InsertKnot( KnotIndex, NewNext );
                NewPath.InsertKnot( KnotIndex, NewPrevious );
                break;


            case Operator.Swap:

                // If there are 3 or less knots, cannot mutate
                // These are the current driver position, one knot, and the goal
                if ( CurrentPath.NumberOfKnots <= 3 )
                { return; }

                // Pick a random knot
                // Don't include the first knot, this is the current driver position
                // Don't include the last knot, this is the goal knot
                // Don't include the second to last knot, because we use the next knot to swap
                KnotIndex = Random.Next( 1, CurrentPath.NumberOfKnots - 2 );

                // Swap with the next knot
                KnotIndex2 = KnotIndex + 1;

                // Swap the knots
                Knot KnotTemp = new Knot( NewPath.Knots[ KnotIndex ] );
                NewPath.ReplaceKnot( KnotIndex, new Knot( NewPath.Knots[ KnotIndex2 ] ) );
                NewPath.ReplaceKnot( KnotIndex2, KnotTemp );
                break;
        }


        // Evaluate the new path
        EvaluatePath( NewPath );

        // If the new path cost is better than the current path, use it
        PathMutex.WaitOne();
        if ( NewPath.Cost < CurrentPath.Cost )
        {
            Paths[ PathIndex ] = NewPath;
        }

        // Replace the worst path with the new path
        //Paths[ NumberOfPaths - 1 ] = NewPath;

        // Sort the paths by cost, from lowest to highest
        Paths.Sort();
        PathMutex.ReleaseMutex();
    }



    /// <summary>
    /// Checks for a direct path from the driver to the goal.
    /// </summary>
    public void CheckDirectPath()
    {
        // Create a new path from the driver to the goal
        Path DirectPath = new Path();
        DirectPath.AddKnot( new Knot( X, Y ) );
        DirectPath.AddKnot( new Knot( CurrentGoal.X, CurrentGoal.Y ) );

        // Evaluate the path
        EvaluatePath( DirectPath );

        // If it is feasible and lower cost than the current path
        PathMutex.WaitOne();
        if ( DirectPath.IsFeasible &&
            DirectPath.Cost < CurrentBestPath.Cost )
        {
            Paths.Insert( 0, DirectPath );
        }
        PathMutex.ReleaseMutex();
    }



    /// <summary>
    /// Moves the driver along the current best path.
    /// </summary>
    public void Move()
    {
        // Initialize the current knot copy
        Knot CurrentKnotCopy = null;

        // If the current knot is not null
        PathMutex.WaitOne();
        if ( CurrentKnot != null )
        {
            // Create a copy of the current knot
            CurrentKnotCopy = new Knot( CurrentKnot );
        }
        PathMutex.ReleaseMutex();


        // If the current knot is null or NaN, exit
        if ( CurrentKnotCopy == null ||
            Double.IsNaN( CurrentKnotCopy.X ) ||
            Double.IsNaN( CurrentKnotCopy.Y ) )
        { return; }

        // Get the unit vector from the current position to the next knot in the current best path
        Vector Direction = MathUtilities.GetUnitVector( X, Y, CurrentKnotCopy.X, CurrentKnotCopy.Y );

        // Initialize the distance to move as the speed
        double DistanceToMove = Speed;

        // Get the distance to the next knot point
        double DistanceToKnot = MathUtilities.Distance( X, Y, CurrentKnotCopy.X, CurrentKnotCopy.Y );

        // If the distance to move is greater than the distance to the knot point, use the shorter distance
        if ( DistanceToMove > DistanceToKnot )
        {
            DistanceToMove = DistanceToKnot;
        }

        // Multiply the direction unit vector by the speed to get the XY distance
        Vector MoveVector = new Vector( Direction.I * DistanceToMove, Direction.J * DistanceToMove );

        // Calculate the new position
        double NewX = X + MoveVector.I;
        double NewY = Y + MoveVector.J;

        // Check if the new position is infeasible
        // Loop through each obstacle
        ObstacleMutex.WaitOne();
        foreach ( ObjectBase CurrentObstacle in KnownObstacles )
        {
            // If the new position is within the current object, it is not feasible
            if ( MathUtilities.IsWithin( new ObjectBase( NewX, NewY, 0 ), CurrentObstacle ) )
            {
                // Abort the move
                ObstacleMutex.ReleaseMutex();
                return;
            }
        }
        ObstacleMutex.ReleaseMutex();


        // Move the driver
        X = NewX;
        Y = NewY;

        // Loop through the paths
        PathMutex.WaitOne();
        foreach ( Path CurrentPath in Paths )
        {
            // Replace the knot 0 position with the current position
            CurrentPath.ReplaceKnot( 0, new Knot( X, Y ) );
        }
        PathMutex.ReleaseMutex();
    }



    /// <summary>
    /// Checks the driver position to see if a knot or goal point has been reached.
    /// </summary>
    public void CheckPosition()
    {
        // If the driver has reached the current knot
        PathMutex.WaitOne();
        if ( (CurrentKnot != null) && (Math.Abs( X - CurrentKnot.X ) < DistanceTolerance) && (Math.Abs( Y - CurrentKnot.Y ) < DistanceTolerance) )
        {
            // Remove it from the path
            CurrentBestPath.RemoveKnot( 1 );
        }
        PathMutex.ReleaseMutex();


        // If the driver has reached the current goal
        GoalMutex.WaitOne();
        if ( (Math.Abs( X - CurrentGoal.X ) < DistanceTolerance) && (Math.Abs( Y - CurrentGoal.Y ) < DistanceTolerance) )
        {
            // Move it to the end of the goals list
            Goals.Add( CurrentGoal.DeepClone() );
            Goals.Remove( CurrentGoal );
        }
        GoalMutex.ReleaseMutex();

    }



    /// <summary>
    /// Gets the nexdt goal.
    /// </summary>
    /// <returns>The next goal in the environment.</returns>
    public Goal GetNextGoal()
    {
        // If there are no goals, return null
        if ( Goals.Count == 0 )
        { return null; }

        // Return the next goal
        return Goals[ 0 ];
    }



    /// <summary>
    /// Looks around the environment for goals.
    /// </summary>
    public void LookForGoals()
    {
        // Get the mutex
        GoalMutex.WaitOne();

        // Intialize the goals list
        Goals = new List<Goal>();

        // Loop through all the goals in the environment
        foreach( Goal CurrentGoal in Environment.Goals )
        {
            // Add the goal to the list of goals
            Goals.Add( new Goal( CurrentGoal ) );
        }

        // Release the mutex
        GoalMutex.ReleaseMutex();
    }



    /// <summary>
    /// Looks around the environment for obstacles and other robots.
    /// </summary>
    public void LookForObstacles()
    {
        // Get the mutex
        ObstacleMutex.WaitOne();

        // Initialize the obstacles list
        KnownObstacles = new List<ObjectBase>();

        // Loop through all the obstacles in the environment
        foreach ( ObjectBase CurrentObstacle in Environment.Obstacles )
        {
            // Add the obstacle to the list of known obstacles
            KnownObstacles.Add( new ObjectBase( CurrentObstacle ) );

            // Add the driver radius to the obstacle
            KnownObstacles[ KnownObstacles.Count - 1 ].Radius += Radius;
        }


        // Loop through all the robots in the environment
        foreach ( ObjectBase CurrentRobot in Environment.Drivers )
        {
            // If the current driver is this driver, skip it
            if ( CurrentRobot == this )
            { continue; }

            // Add the driver to the list of known obstacles
            KnownObstacles.Add( new ObjectBase( CurrentRobot ) );

            // Add the driver radius to the obstacle
            KnownObstacles[ KnownObstacles.Count - 1 ].Radius += Radius;
        }

        // Release the mutex
        ObstacleMutex.ReleaseMutex();
    }



    /// <summary>
    /// Returns a copy of the paths.
    /// </summary>
    /// <returns></returns>
    public List<Path> GetPathsCopy()
    {
        return new List<Path>( Paths );
    }



    /// <summary>
    /// Returns a copy of the best path.
    /// </summary>
    /// <returns></returns>
    public Path GetCurrentBestPathCopy()
    {
        return new Path( CurrentBestPath );
    }

    #endregion


}
