/// <summary>
/// The base class for objects within the environment.
/// </summary>
public class ObjectBase
{
    #region Constructor

    /// <summary>
    /// Creates a new object.
    /// </summary>
    public ObjectBase()
    {
    }



    /// <summary>
    /// Creates a new object.
    /// </summary>
    /// <param name="X">The X position of the object.</param>
    /// <param name="Y">The Y position of the object.</param>
    /// <param name="Radius">The radius of the object.</param>
    public ObjectBase( double X, double Y, double Radius )
    {
        // Assign the properties
        _X = X;
        _Y = Y;
        _Radius = Radius;
    }



    /// <summary>
    /// Copy constructor.
    /// Creates a new object as a copy of an object.
    /// </summary>
    /// <param name="Original">The object to copy.</param>
    public ObjectBase( ObjectBase Original )
    {
        // Assign the properties
        this._X = Original._X;
        this._Y = Original._Y;
        this._Radius = Original._Radius;
    }



    /// <summary>
    /// Deep clone.
    /// Returns a copy of this object.
    /// </summary>
    /// <returns>A copy of the object.</returns>
    public ObjectBase DeepClone()
    {
        return new ObjectBase( this );
    }


    #endregion



    #region Properties

    protected double _X;
    /// <summary>
    /// The X position of the obstacle.
    /// </summary>
    public double X
    {
        get { return _X; }
        set
        {
            _X = value;
        }
    }



    protected double _Y;
    /// <summary>
    /// The Y position of the obstacle.
    /// </summary>
    public double Y
    {
        get { return _Y; }
        set
        {
            _Y = value;
        }
    }



    protected double _Radius;
    /// <summary>
    /// The radius of the object.
    /// </summary>
    public double Radius
    {
        get { return _Radius; }
        set { _Radius = value; }
    }

    #endregion
}
