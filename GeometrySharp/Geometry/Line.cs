using GeometrySharp.Core;
using GeometrySharp.Geometry.Interfaces;
using System;
using System.Collections.Generic;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A curve representing a straight line.
    /// </summary>
    public class Line : Curve, IEquatable<Line>, ITransformable<Line>
    {
        /// <summary>
        /// Line by start point and end point.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        public Line(Vector3 start, Vector3 end)
        {
            if(start == end || !start.IsValid() || !end.IsValid())
            {
                throw new Exception("Inputs are not valid, or are equal");
            }

            Start = start;
            End = end;
            Length = Start.DistanceTo(End);
            Direction = (End - Start).Unitize();
            ToNurbsCurve();
        }

        /// <summary>
        /// Line from a starting point, direction and length.
        /// </summary>
        /// <param name="start">Starting point of the line.</param>
        /// <param name="direction">Direction of the line.</param>
        /// <param name="length">Length of the line.</param>
        public Line(Vector3 start, Vector3 direction, double length)
        {
            if(length >= -GeoSharpMath.EPSILON && length <= GeoSharpMath.EPSILON)
            {
                throw new Exception("Length must not be 0.0");
            }

            Start = start;
            End = start + direction.Amplify(length);
            Length = Math.Abs(length);
            Direction = (End - Start).Unitize();
            ToNurbsCurve();
        }

        /// <summary>
        /// Start point of the line.
        /// </summary>
        public Vector3 Start { get; }

        /// <summary>
        /// End point of the line.
        /// </summary>
        public Vector3 End { get; }

        /// <summary>
        /// Length of the line.
        /// </summary>
        public double Length { get; }

        /// <summary>
        /// Direction of the line.
        /// </summary>
        public Vector3 Direction { get; }

        /// <summary>
        /// Gets the BoundingBox in ascending fashion.
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get
            {
                BoundingBox bBox = new BoundingBox(Start, End);
                BoundingBox validBBox = bBox.MakeItValid();
                return validBBox;
            }
        }

        /// <summary>
        /// Get the closest point on the line from this point.
        /// </summary>
        /// <param name="pt">The closest point to find.</param>
        /// <returns>The closest point on the line from this point.</returns>
        public override Vector3 ClosestPt(Vector3 pt)
        {
            Vector3 dir = Direction;
            Vector3 v = pt - Start;
            double d = Vector3.Dot(v, dir);

            d = Math.Min(Length, d);
            d = Math.Max(d, 0);

            return Start + dir * d;
        }

        /// <summary>
        /// Evaluate the line at the specified parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate the line. Parameter should be between 0.0 and 1.0</param>
        /// <returns>The point at the specific parameter.</returns>
        public override Vector3 PointAt(double t)
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
            Vector3 start = Start;
            Vector3 end = End;

            if (startLength >= -GeoSharpMath.EPSILON || startLength <= GeoSharpMath.EPSILON)
            {
                start = Start - (Direction * startLength);
            }

            if (endLength >= -GeoSharpMath.EPSILON || endLength <= GeoSharpMath.EPSILON)
            {
                end = End + (Direction * endLength);
            }

            return new Line(start, end);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this line.
        /// </summary>
        /// <returns>A nurbs curve shaped like this line.</returns>
        private void ToNurbsCurve()
        {
            List<Vector3> pts = new List<Vector3>{Start, End};
            Knot knots = new Knot{0,0,1,1};

            Degree = 1;
            Knots = knots;
            HomogenizedPoints = LinearAlgebra.PointsHomogeniser(pts, 1.0);
        }

        /// <summary>
        /// Transforms the line using the transformation matrix.
        /// </summary>
        /// <param name="transformation">Transform matrix to apply.</param>
        /// <returns>A line transformed.</returns>
        public Line Transform(Transform transformation)
        {
            Vector3 pt1 = Start * transformation;
            Vector3 pt2 = End * transformation;
            return new Line(pt1, pt2);
        }

        /// <summary>
        /// Check if the line is equal to the provided line.
        /// Two lines are equal if the end points are the same.
        /// </summary>
        /// <param name="other">The line to compare.</param>
        /// <returns>True if the end points are equal, otherwise false.</returns>
        public bool Equals(Line other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (!ReferenceEquals(this, other))
            {
                return false;
            }

            return Start.Equals(other.Start) && End.Equals(other.End);
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
