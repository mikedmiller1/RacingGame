/// <summary>
/// Represents a human driver, to track the position.
/// </summary>
public class HumanDriver : ObjectBase
{
    /// <summary>
    /// Creates a new human driver.
    /// </summary>
    /// <param name="PositionX">The X position of the driver.</param>
    /// <param name="PositionY">The Y position of the driver.</param>
    /// <param name="Radius">The radius of the driver.</param>
    public HumanDriver( double PositionX, double PositionY, double Radius )
        : base(  PositionX, PositionY, Radius )
    {
    }
}
