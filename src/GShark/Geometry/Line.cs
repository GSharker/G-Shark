using GShark.Core;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;

namespace GShark.Geometry
{
    /// <summary>
    /// A curve representing a straight line.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Geometry/LineTests.cs?name=example)]
    /// </example>
    public class Line : ICurve, IEquatable<Line>, ITransformable<Line>
    {
        /// <summary>
        /// Initializes a line by start point and end point.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        public Line(Point3 start, Point3 end)
        {
            if(start == end || !start.IsValid || !end.IsValid)
            {
                throw new Exception("Start or end point is not valid, or they are equal");
            }

            Start = start;
            End = end;
            Length = Start.DistanceTo(End);
            Direction = (End - Start).Unitize();
        }

        /// <summary>
        /// Initializes a line from a starting point, direction and length.
        /// </summary>
        /// <param name="start">Starting point of the line.</param>
        /// <param name="direction">Direction of the line.</param>
        /// <param name="length">Length of the line.</param>
        public Line(Point3 start, Vector3d direction, double length)
        {
            if(length >= -GeoSharkMath.Epsilon && length <= GeoSharkMath.Epsilon)
            {
                throw new Exception("Length must not be 0.0");
            }

            Start = start;
            End = start + direction.Amplify(length);
            Length = Math.Abs(length);
            Direction = (End - Start).Unitize();
        }

        /// <summary>
        /// Start point of the line.
        /// </summary>
        public Point3 Start { get; }

        /// <summary>
        /// End point of the line.
        /// </summary>
        public Point3 End { get; }

        /// <summary>
        /// Length of the line.
        /// </summary>
        public double Length { get; }

        /// <summary>
        /// Direction of the line.
        /// </summary>
        public Vector3d Direction { get; }

        public int Degree => 1;

        public List<Point3> ControlPoints => new List<Point3>{Start, End};

        public List<Point4> HomogenizedPoints => LinearAlgebra.PointsHomogeniser(ControlPoints, 1.0);

        public KnotVector Knots => new KnotVector {0, 0, 1, 1};

        public Interval Domain => new Interval(0.0, 1.0);

        /// <summary>
        /// Gets the BoundingBox in ascending fashion.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                BoundingBox bBox = new BoundingBox(Start, End);
                BoundingBox validBBox = bBox.MakeItValid();
                return validBBox;
            }
        }

        /// <summary>
        /// Gets the closest point on the line from this point.
        /// </summary>
        /// <param name="pt">The closest point to find.</param>
        /// <returns>The closest point on the line from this point.</returns>
        public Point3 ClosestPoint(Point3 pt)
        {
            Vector3d dir = Direction;
            Vector3d v = pt - Start;
            double d = Vector3d.DotProduct(v, dir);

            d = Math.Min(Length, d);
            d = Math.Max(d, 0);

            return Start + dir * d;
        }

        /// <summary>
        /// Computes the parameter on the line the is closest to a test point.
        /// </summary>
        /// <param name="pt">The test point.</param>
        /// <returns>The parameter on the line closest to the test point.</returns>
        public double ClosestParameter(Point3 pt)
        {
            Vector3d dir = End - Start;
            double dirLength = dir.SquareLength;

            if (!(dirLength > 0.0)) return 0.0;
            Vector3d ptToStart = pt - Start;
            Vector3d ptToEnd = pt - End;

            if (ptToStart.SquareLength <= ptToEnd.SquareLength)
            {
                return Vector3d.DotProduct(ptToStart, dir) / dirLength;
            }

            return 1.0 + Vector3d.DotProduct(ptToEnd, dir) / dirLength;
        }

        /// <summary>
        /// Evaluates the line at the specified parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate the line. Parameter should be between 0.0 and 1.0</param>
        /// <returns>The point at the specific parameter.</returns>
        public Point3 PointAt(double t)
        {
            if (t > 1.0 || t < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(t), "Parameter is outside the domain 0.0 to 1.0");
            }

            return Start + Direction * (Length * t);
        }

        /// <summary>
        /// Flip the endpoint of the line.
        /// </summary>
        /// <returns>The line flipped.</returns>
        public Line Flip()
        {
            return new Line(End, Start);
        }

        /// <summary>
        /// Extends the line by lengths on both side.
        /// </summary>
        /// <param name="startLength">Length to extend the line at the start point.</param>
        /// <param name="endLength">Length to extend the line at the end point.</param>
        /// <returns>The extended line.</returns>
        public Line Extend(double startLength, double endLength)
        {
            Point3 start = Start;
            Point3 end = End;

            if (startLength >= -GeoSharkMath.Epsilon || startLength <= GeoSharkMath.Epsilon)
            {
                start = Start - (Direction * startLength);
            }

            if (endLength >= -GeoSharkMath.Epsilon || endLength <= GeoSharkMath.Epsilon)
            {
                end = End + (Direction * endLength);
            }

            return new Line(start, end);
        }


        /// <summary>
        /// Transforms the line using the transformation matrix.
        /// </summary>
        /// <param name="transform">Transformation matrix to apply.</param>
        /// <returns>A line transformed.</returns>
        public Line Transform(Transform transform)
        {
            Point3 pt1 = Start.Transform(transform);
            Point3 pt2 = End.Transform(transform);
            return new Line(pt1, pt2);
        }

        /// <summary>
        /// Checks if the line is equal to the provided line.<br/>
        /// Two lines are equal if the end points are the same.
        /// </summary>
        /// <param name="other">The line to compare.</param>
        /// <returns>True if the end points are equal, otherwise false.</returns>
        public bool Equals(Line other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return false;
            }

            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        /// <summary>
        /// Checks if the line is equal to the provided line.<br/>
        /// Two lines are equal if the end points are the same.
        /// </summary>
        /// <param name="obj">The curve object.</param>
        /// <returns>Return true if the nurbs curves are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Line curve)
                return Equals(curve);
            return false;
        }

        /// <summary>
        /// Gets the hash code for the line.
        /// </summary>
        /// <returns>A unique hashCode of an line.</returns>
        public override int GetHashCode()
        {
            return new[] { Start, End }.GetHashCode();
        }

        /// <summary>
        /// Constructs the string representation of the line.
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return $"{Start} - {End} - L:{Length}";
        }
    }
}
