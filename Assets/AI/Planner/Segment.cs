/// <summary>
/// A segment connecting two knot points
/// </summary>
public class Segment : ObjectBase
{
    #region Constructor

    /// <summary>
    /// Creates a new segment.
    /// </summary>
    /// <param name="Start">The start point of the segment.</param>
    /// <param name="End">The end point of the segment.</param>
    public Segment( ObjectBase Start, ObjectBase End )
    {
        this._X = Start.X;
        this._Y = Start.Y;
        this._X2 = End.X;
        this._Y2 = End.Y;
    }



    /// <summary>
    /// Creates a new path as a copy of an existing segment.
    /// </summary>
    /// <param name="Original">The segment to copy.</param>
    public Segment( Segment Original )
    {
        _X = Original._X;
        _Y = Original._Y;
        _X2 = Original._X2;
        _Y2 = Original._Y2;
    }

    #endregion



    #region Properties

    private double _X2;

    public double X2
    {
        get { return _X2; }
        set { _X2 = value; }
    }



    private double _Y2;

    public double Y2
    {
        get { return _Y2; }
        set { _Y2 = value; }
    }



    private bool _IsFeasible = true;
    /// <summary>
    /// Flag indicating if the segment is feasible.
    /// </summary>
    public bool IsFeasible
    {
        get { return _IsFeasible; }
        set { _IsFeasible = value; }
    }

    #endregion
}
