using System;
using UnityEngine;

/// <summary>
/// Contains utilities to perform various math functions.
/// </summary>
public static class MathUtilities
{
    /// <summary>
    /// Calculates the squared Euclidean distance between two points.
    /// This is faster than the regular Euclidean distance because it skips the square root.
    /// It is useful for comparing relative distances, but does not provide an absolute distance.
    /// </summary>
    /// <param name="X1">The X coordinate of point 1.</param>
    /// <param name="Y1">The Y coordinate of point 1.</param>
    /// <param name="X2">The X coordinate of point 2.</param>
    /// <param name="Y2">The Y coordinate of point 2.</param>
    /// <returns>The squared Euclidean distance from point 1 to point 2.</returns>
    public static double SquaredDistance( double X1, double Y1, double X2, double Y2 )
    {
        return Math.Pow( X2 - X1, 2 ) + Math.Pow( Y2 - Y1, 2 );
    }



    /// <summary>
    /// Calculates the Euclidean distance between two points.
    /// </summary>
    /// <param name="X1">The X coordinate of point 1.</param>
    /// <param name="Y1">The Y coordinate of point 1.</param>
    /// <param name="X2">The X coordinate of point 2.</param>
    /// <param name="Y2">The Y coordinate of point 2.</param>
    /// <returns>The Euclidean distance from point 1 to point 2.</returns>
    public static double Distance( double X1, double Y1, double X2, double Y2 )
    {
        return Math.Sqrt( SquaredDistance( X1, Y1, X2, Y2 ) );
    }



    /// <summary>
    /// Calculates the Euclidean distance between two Knot points.
    /// </summary>
    /// <param name="Object1">The first Knot point.</param>
    /// <param name="Object2">The second Knot point.</param>
    /// <returns>The Euclidean distance from Knot 1 to Knot 2.</returns>
    public static double Distance( ObjectBase Object1, ObjectBase Object2 )
    {
        return Distance( Object1.X, Object1.Y, Object2.X, Object2.Y );
    }



    /// <summary>
    /// Calculates the length of a vector.
    /// </summary>
    /// <param name="Vector">The vector.</param>
    /// <returns>The length of the vector.</returns>
    public static double Distance( Vector Vector )
    {
        return Distance( 0, 0, Vector.I, Vector.J );
    }



    /// <summary>
    /// Calculates the slope of a line connecting two knot points.
    /// Returns NaN if the X values of the knot points are the same (vertical line).
    /// </summary>
    /// <param name="Knot1">The first Knot point.</param>
    /// <param name="Knot2">The second Knot point.</param>
    /// <returns>The slope from Knot 1 to Knot 2.</returns>
    public static double Slope( ObjectBase Knot1, ObjectBase Knot2 )
    {
        // Check if the X values of both knot points are the same.
        // Divide by zero protection
        if ( Knot1.X == Knot2.X )
        {
            return Double.NaN;
        }

        return (Knot2.Y - Knot1.Y) / (Knot2.X - Knot1.X);
    }



    /// <summary>
    /// Calculates the acute angle (in radians) between two lines.
    /// This is the angle from slope 1 to slope 2, positive is counter-clockwise.
    /// </summary>
    /// <param name="Slope1">The first line slope.</param>
    /// <param name="Slope2">The second line slope.</param>
    /// <returns>The acute angle between the lines.</returns>
    public static double Angle( double Slope1, double Slope2 )
    {
        return Math.Atan( (Slope1 - Slope2) / (1 + (Slope1 * Slope2)) );
    }



    /// <summary>
    /// Calculates the smoothness along a series of 3 knot points.
    /// </summary>
    /// <param name="Knot1">The first Knot point.</param>
    /// <param name="Knot2">The second Knot point.</param>
    /// <param name="Knot3">The third Knot point.</param>
    /// <returns>The smoothness of the knot series.</returns>
    public static double Smoothness( ObjectBase Knot1, ObjectBase Knot2, ObjectBase Knot3 )
    {
        // Get the slope of each segment
        double Slope1 = Slope( Knot1, Knot2 );
        double Slope2 = Slope( Knot2, Knot3 );

        // Get the angle between the slopes
        double Theta = Angle( Slope1, Slope2 );

        // Get the distance of each segment
        double Dist1 = Distance( Knot1, Knot2 );
        double Dist2 = Distance( Knot2, Knot3 );

        // Get the minimum of the two segment distances
        double MinDist = Math.Min( Dist1, Dist2 );

        // Calculate the smoothness
        double Smooth = Theta / MinDist;

        // Return the smoothness
        return Smooth;
    }



    /// <summary>
    /// Calculates the minimum distance between a line segment and a point.
    /// Takes the object radius into account, such that the returned distance is to the outer surface of the object.
    /// See http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
    /// </summary>
    /// <param name="Point1">The first point of the line.</param>
    /// <param name="Point2">The second point of the line.</param>
    /// <param name="Point3">The object to find the minimum distance to.</param>
    /// <returns>The minimum distance from the line to the outer surface of the object.</returns>
    public static double MinDistance( ObjectBase Point1, ObjectBase Point2, ObjectBase Point3 )
    {
        // Get the distance between the knots
        double LineDistance = Distance( Point1, Point2 );

        // Divide by zero protection
        if ( LineDistance == 0 )
        { return 0; }


        // Initialize the min distance to the center of the object
        double MinDistanceCenter;

        // Check if the point is within the line segment or not
        // If the point is past Point1
        if ( (Point2.X - Point1.X) * (Point3.X - Point1.X) + (Point2.Y - Point1.Y) * (Point3.Y - Point1.Y) >= 0 )
        {
            // If it is within the line segment
            if ( (Point2.X - Point1.X) * (Point3.X - Point2.X) + (Point2.Y - Point1.Y) * (Point3.Y - Point2.Y) <= 0 )
            {
                // Get the closest point on the line
                ObjectBase Closest = ClosestPoint( Point1, Point2, Point3 );

                // Get the distance from the point on the line to the original point
                MinDistanceCenter = Distance( Point3, Closest );
            }

            // Otherwise, it is past Point2
            else
            {
                MinDistanceCenter = Distance( Point3, Point2 );
            }
        }

        // Otherwise, it is before Point1
        else
        {
            MinDistanceCenter = Distance( Point3, Point1 );
        }


        // Calculate the min distance to the object surface
        double MinDistanceSurface = MinDistanceCenter - Point3.Radius;

        // Return the min distance
        return MinDistanceSurface;
    }



    /// <summary>
    /// Calculates the point on a line that is closest to another point.
    /// Assumes that the third point is in between the two points that form the line.
    /// </summary>
    /// <param name="Point1">The first point on the line.</param>
    /// <param name="Point2">The second point on the line.</param>
    /// <param name="Point3">The point to find the closest point on the line to.</param>
    /// <returns></returns>
    public static ObjectBase ClosestPoint( ObjectBase Point1, ObjectBase Point2, ObjectBase Point3 )
    {
        // Get a vector from the first point to the second and third points
        Vector V12 = GetVector( Point1, Point2 );
        Vector V13 = GetVector( Point1, Point3 );

        // Calculate the dot products
        double V12_V13 = DotProduct( V12, V13 );
        double V12_V12 = DotProduct( V12, V12 );

        // Divide by zero protection
        if ( V12_V12 == 0 )
        {
            V12_V12 = Double.MinValue;
        }

        // Calculate the slope
        double Slope = V12_V13 / V12_V12;

        // Calculate the closest point
        double X = Point1.X + V12.I * Slope;
        double Y = Point1.Y + V12.J * Slope;

        return new ObjectBase( X, Y, 0 );
    }



    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="V1">The first vector.</param>
    /// <param name="V2">The second vector.</param>
    /// <returns></returns>
    public static double DotProduct( Vector V1, Vector V2 )
    {
        return (V1.I * V2.I) + (V1.J * V2.J);
    }



    /// <summary>
    /// Determines if an object is within a second object's radius.
    /// </summary>
    /// <param name="Object1">The first object, will be checked as being within the radius of the second object.</param>
    /// <param name="Object2">The second object, to check if the first object is within it's radius.</param>
    /// <returns>True if the knot is within the radius of the object center.</returns>
    public static bool IsWithin( ObjectBase Object1, ObjectBase Object2 )
    {
        // Check if the distance between the object centers is less than the radius of Object2
        if ( Distance( Object1, Object2 ) < Object2.Radius )
        {
            return true;
        }

        else
        {
            return false;
        }
    }



    /// <summary>
    /// Calculates a vector from point 1 to point 2.
    /// </summary>
    /// <param name="X1">The X coordinate of the first point.</param>
    /// <param name="Y1">The Y coordinate of the first point.</param>
    /// <param name="X2">The X coordinate of the second point.</param>
    /// <param name="Y2">The Y coordinate of the second point.</param>
    /// <returns>A vector from point 1 to point 2.</returns>
    public static Vector GetVector( double X1, double Y1, double X2, double Y2 )
    {
        // Calculate the I and J components
        double I = X2 - X1;
        double J = Y2 - Y1;

        // Return the vector
        return new Vector( I, J );
    }



    /// <summary>
    /// Calculates a vector from object 1 to object 2.
    /// </summary>
    /// <param name="Object1">The first object.</param>
    /// <param name="Object2">The second object.</param>
    /// <returns>A vector from object 1 to object 2.</returns>
    public static Vector GetVector( ObjectBase Object1, ObjectBase Object2 )
    {
        return GetVector( Object1.X, Object1.Y, Object2.X, Object2.Y );
    }



    /// <summary>
    /// Calculates a unit vector from point 1 to point 2.
    /// </summary>
    /// <param name="X1">The X coordinate of the first point.</param>
    /// <param name="Y1">The Y coordinate of the first point.</param>
    /// <param name="X2">The X coordinate of the second point.</param>
    /// <param name="Y2">The Y coordinate of the second point.</param>
    /// <returns>A unit vector from point 1 to point 2.</returns>
    public static Vector GetUnitVector( double X1, double Y1, double X2, double Y2 )
    {
        // Get the distance between the points
        double Dist = Distance( X1, Y1, X2, Y2 );

        // Divide by zero protection
        if ( Dist == 0 )
        { return new Vector( 0, 0 ); }

        // Get the vector from point 1 to point 2
        Vector Vect = GetVector( X1, Y1, X2, Y2 );

        // Return the normalized unit vector
        return new Vector( Vect.I / Dist, Vect.J / Dist );
    }



    /// <summary>
    /// Calculates a unit vector from object 1 to object 2.
    /// </summary>
    /// <param name="Object1">The first object.</param>
    /// <param name="Object2">The second object.</param>
    /// <returns>A unit vector from object 1 to object 2.</returns>
    public static Vector GetUnitVector( ObjectBase Object1, ObjectBase Object2 )
    {
        return GetUnitVector( Object1.X, Object1.Y, Object2.X, Object2.Y );
    }



    /// <summary>
    /// Returns a random number with a probability weighting corresponding to the input weights.
    /// The returned value is the index of the weighted value that was randomly selected.
    /// </summary>
    /// <param name="Weights">An array of weights to apply to the random distribution.</param>
    /// <returns>The index of the weight.</returns>
    public static int GetWeightedRandom( double[] Weights )
    {
        // Get a random value between 0 and 1
        System.Random Random = new System.Random();
        double r = Random.NextDouble();


        // Initialize the total weight
        double TotalWeight = 0;

        // Loop through the weights to get the total weight to normalize by
        for ( int WeightNum = 0; WeightNum < Weights.Length; WeightNum++ )
        {
            // Add the current weight to the cumulative weight
            TotalWeight += Weights[ WeightNum ];
        }


        // Initialize the cumulative weight
        double CumulativeWeight = 0;

        // Loop through the weights to select the random value
        for ( int WeightNum = 0; WeightNum < Weights.Length; WeightNum++ )
        {
            // Increment the cumulative weight
            CumulativeWeight += Weights[ WeightNum ] / TotalWeight;

            // If the random number is within the current cumulative weight
            if ( r < CumulativeWeight )
            {
                // Return the operator
                return WeightNum;
            }
        }


        // If we get here, something went wrong
        // Return -1
        return -1;
    }
}




/// <summary>
/// Represents a 2D vector.
/// </summary>
public class Vector
{
    /// <summary>
    /// Creates a new vector.
    /// </summary>
    /// <param name="I">The I component of the vector.</param>
    /// <param name="J">the J component of the vector.</param>
    public Vector( double I, double J )
    {
        this.I = I;
        this.J = J;
    }



    public double I;
    public double J;
}
