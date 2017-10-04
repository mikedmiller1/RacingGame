/// <summary>
/// A knot point in a path.
/// </summary>
public class Knot : ObjectBase
{
    #region Constructor

    /// <summary>
    /// Creates a new knot.
    /// </summary>
    public Knot()
        : base()
    {
    }



    /// <summary>
    /// Creates a new knot.
    /// </summary>
    /// <param name="PositionX">The X position of the knot.</param>
    /// <param name="PositionY">The Y position of the knot.</param>
    public Knot( double PositionX, double PositionY )
        : base( PositionX, PositionY, TheRadius )
    {
    }



    /// <summary>
    /// Creates a new knot as a copy of an existing knot.
    /// </summary>
    /// <param name="Original">The knot to copy.</param>
    public Knot( Knot Original )
        : base( Original )
    {

    }



    /// <summary>
    /// Returns a copy of this knot.
    /// </summary>
    /// <returns></returns>
    public new Knot DeepClone()
    {
        return new Knot( this );
    }

    #endregion



    #region Properties

    /// <summary>
    /// The radius of a knot point.
    /// </summary>
    private static double TheRadius = 0.01;



    private bool _IsFeasible = true;
    /// <summary>
    /// Flag indicating if the knot is feasible.
    /// </summary>
    public bool IsFeasible
    {
        get { return _IsFeasible; }
        set { _IsFeasible = value; }
    }

    #endregion
}
