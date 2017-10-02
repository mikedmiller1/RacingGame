using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// A path for the robot to follow.
/// </summary>
public class Path : IComparable
{
    #region Constructor

    /// <summary>
    /// Creates a new path.
    /// </summary>
    public Path()
    {
    }



    /// <summary>
    /// Creates a new path as a copy of an existing path.
    /// </summary>
    /// <param name="Original">The path to copy.</param>
    public Path( Path Original )
    {
        // Check for null
        if ( Original == null )
        { return; }

        // Assign the properties
        this._Cost = Original._Cost;
        this._IsFeasible = Original._IsFeasible;

        // Loop through the knots
        foreach ( Knot OriginalKnot in Original._Knots )
        {
            // Copy the knot
            this._Knots.Add( new Knot( OriginalKnot ) );
        }

        // Regenerate segments
        GenerateSegments();
    }

    #endregion



    #region Properties

    private List<Knot> _Knots = new List<Knot>();
    /// <summary>
    /// The knot points of the path.
    /// </summary>
    public List<Knot> Knots
    {
        get { return _Knots; }
        set { _Knots = value; }
    }



    private List<Segment> _Segments = new List<Segment>();
    /// <summary>
    /// The segments of the path.
    /// </summary>
    public List<Segment> Segments
    {
        get { return _Segments; }
        set { _Segments = value; }
    }



    /// <summary>
    /// The number of knots in the path.
    /// </summary>
    public int NumberOfKnots
    { get { return _Knots.Count; } }



    /// <summary>
    /// The number of segments in the path.
    /// </summary>
    public int NumberOfSegments
    { get { return _Segments.Count; } }



    private double _Cost;
    /// <summary>
    /// The path cost, as determined by the evaluation function.
    /// </summary>
    public double Cost
    {
        get { return _Cost; }
        set { _Cost = value; }
    }



    private bool _IsFeasible = true;
    /// <summary>
    /// Flag indicating if the path is feasible.
    /// </summary>
    public bool IsFeasible
    {
        get { return _IsFeasible; }
        set { _IsFeasible = value; }
    }

    #endregion



    #region Methods

    /// <summary>
    /// Compares another path to this path, returns the path with the lowest cost.
    /// </summary>
    /// <param name="o">The other path to compare.</param>
    /// <returns>The CompareTo() value of the comparison.</returns>
    public int CompareTo( object o )
    {
        // Cast the input object to a path
        Path other = (Path)o;

        // Return the path with the smallest cost
        return this.Cost.CompareTo( other.Cost );
    }



    /// <summary>
    /// Generates the segments of the path.
    /// </summary>
    public void GenerateSegments()
    {
        // Clear the segments list
        Segments = new List<Segment>();

        // If there are less than two knots in the path, there are no segments
        if ( NumberOfKnots <= 1 )
        { return; }

        // Loop through the points
        // Skip the last point
        for ( int KnotNum = 0; KnotNum < NumberOfKnots - 1; KnotNum++ )
        {
            // Add the segment from the current knot to the next knot
            Segments.Add( new Segment( Knots[ KnotNum ], Knots[ KnotNum + 1 ] ) );
        }
    }



    /// <summary>
    /// Adds a knot to the end of the path.
    /// </summary>
    /// <param name="NewKnot">The knot to add</param>
    public void AddKnot( Knot NewKnot )
    {
        // Add the knot to the end of the Knots list
        Knots.Add( NewKnot );

        // Regenerate the segments
        GenerateSegments();
    }



    /// <summary>
    /// Inserts a knot at the specified index in the path.
    /// </summary>
    /// <param name="Index">The index to insert the knot at.</param>
    /// <param name="NewKnot">The knot to insert.</param>
    public void InsertKnot( int Index, Knot NewKnot )
    {
        // Insert the new knot at the specified index
        Knots.Insert( Index, NewKnot );

        // Regenerate the segments
        GenerateSegments();
    }



    /// <summary>
    /// Replaces the knot at the specified index in the path.
    /// </summary>
    /// <param name="Index">The index of the knot to replace.</param>
    /// <param name="NewKnot">The knot to insert.</param>
    public void ReplaceKnot( int Index, Knot NewKnot )
    {
        // Replace the knot
        Knots[ Index ] = NewKnot;

        // Regenerate the segments
        GenerateSegments();
    }



    /// <summary>
    /// Removes the knot at the specified index.
    /// </summary>
    /// <param name="Index">The index of the knot to remove.</param>
    public void RemoveKnot( int Index )
    {
        // Remove the knot at the specified index
        Knots.RemoveAt( Index );

        // Regenerate the segments
        GenerateSegments();
    }

    #endregion

}
