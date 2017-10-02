/// <summary>
/// Defines mutation operators that may be performed on a path.
/// </summary>
public enum Operator
{
    /// <summary>
    /// Recombines two parent paths into two new paths.
    /// Divides the parent paths randomly into two parts
    /// Forms two child paths by recombining the first part of the first path with the second part of the second path, and vice versa.
    /// </summary>
    Crossover,

    /// <summary>
    /// Randomly adjusts a knot point coordinates within some local clearance of the path such that it remains feasible.
    /// </summary>
    MutationSmall,

    /// <summary>
    /// Randomly adjusts a knot point coordinates a large distance.
    /// The resulting path may be feasible or infeasible.
    /// </summary>
    MutationLarge,

    /// <summary>
    /// Inserts a new random knot location into an infeasible segment of a path.
    /// Also removes any knots that are inside obstacles.
    /// </summary>
    Insert,

    /// <summary>
    /// Deletes knots from a path.
    /// For infeasible paths this is done randomly.
    /// For feasible paths a heuristic is used to determine which node should be deleted.
    /// </summary>
    Delete,

    /// <summary>
    /// Swaps the coordinates of two randomly selected adjacent nodes.
    /// </summary>
    Swap,

    /// <summary>
    /// Smooths out a feasible path by "cutting corners".
    /// For a randomly selected knot, two knots are inserted around it which serve to flatten out the curve.
    /// The selected knot is then deleted.
    /// Knots with sharper turns are more likely to be selected.
    /// </summary>
    Smooth,

    /// <summary>
    /// Repairs a randomly selected infeasible segment by "pulling" the segment out around any interfering obstacles.
    /// </summary>
    Repair
}
