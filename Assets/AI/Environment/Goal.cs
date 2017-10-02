using UnityEngine;


/// <summary>
/// A goal for the robot to travel towards.
/// </summary>
public class Goal : ObjectBase
{
    #region Constructor

    public Goal()
        : base()
    {
    }



    /// <summary>
    /// Creates a new goal.
    /// </summary>
    /// <param name="PositionX">The X position of the goal.</param>
    /// <param name="PositionY">The Y position of the goal.</param>
    /// <param name="Radius">The radius of the goal.</param>
    public Goal( double PositionX, double PositionY, double Radius )
        : base( PositionX, PositionY, Radius )
    {
    }



    /// <summary>
    /// Creates a new goal.
    /// </summary>
    /// <param name="Position">The position of hte goal.</param>
    /// <param name="Radius">The radius of the goal.</param>
    public Goal( Vector3 Position, double Radius )
        : base( Position.x, Position.z, Radius )
    {
    }



    /// <summary>
    /// Creates a new goal as a copy of the existing goal.
    /// </summary>
    /// <param name="original">The goal to copy.</param>
    public Goal( Goal original )
        : base( original )
    {
    }



    /// <summary>
    /// Returns a copy of this goal.
    /// </summary>
    /// <returns></returns>
    public new Goal DeepClone()
    {
        return new Goal( this );
    }

    #endregion
}
