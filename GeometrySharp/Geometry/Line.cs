using System;
using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo: Checks if all the methods have the associated test.
    /// <summary>
    /// A curve representing a straight line.
    /// </summary>
    public class Line : IEquatable<Line>
    {
        /// <summary>
        /// Line by start point and end point.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        public Line(Vector3 start, Vector3 end)
        {
            if(start == end || !start.IsValid() || !end.IsValid())
                throw new Exception("Inputs are not valid, or are equal");
            this.Start = start;
            this.End = end;
            this.Length = Start.DistanceTo(End);
            this.Direction = (End - Start).Unitize();
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
                throw new Exception("Length must not be 0.0");
            this.Start = start;
            this.End = start + direction.Amplify(length);
            this.Length = Math.Abs(length);
            this.Direction = (End - Start).Unitize();
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
        public BoundingBox BoundingBox
        {
            get
            {
                var bBox = new BoundingBox(Start, End);
                var validBBox = bBox.MakeItValid();
                return validBBox;
            }
        }

        /// <summary>
        /// Get the closest point on the line from this point.
        /// </summary>
        /// <param name="line">The line on which to find the closest point.</param>
        /// <returns>The closest point on the line from this point.</returns>
        public Vector3 ClosestPoint(Vector3 vec)
        {
            var dir = this.Direction;
            var v = vec - this.Start;
            var d = Vector3.Dot(v, dir);

            d = Math.Min(this.Length, d);
            d = Math.Max(d, 0);

            return this.Start + dir * d;
        }

        /// <summary>
        /// Evaluate the line at the specified parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate the line. Parameter should be between 0.0 and 1.0</param>
        /// <returns>The point at the specific parameter.</returns>
        public Vector3 PointAt(double t)
        {
            if (t > 1.0 || t < 0.0)
                throw new ArgumentOutOfRangeException(nameof(t), "Parameter is outside the domain 0.0 to 1.0");

            return this.Start + this.Direction * (this.Length * t);
        }

        /// <summary>
        /// Flip the endpoint of the line.
        /// </summary>
        /// <returns>The line flipped.</returns>
        public Line Flip()
        {
            return new Line(this.End, this.Start);
        }

        /// <summary>
        /// Extends the line by lengths on both side.
        /// </summary>
        /// <param name="startLength">Length to extend the line at the start point.</param>
        /// <param name="endLength">Length to extend the line at the end point.</param>
        /// <returns>The extended line.</returns>
        public Line Extend(double startLength, double endLength)
        {
            var start = this.Start;
            var end = this.End;

            if (startLength >= -GeoSharpMath.EPSILON || startLength <= GeoSharpMath.EPSILON)
                start = this.Start - (this.Direction * startLength);
            if (endLength >= -GeoSharpMath.EPSILON || endLength <= GeoSharpMath.EPSILON)
                end = this.End + (this.Direction * endLength);

            return new Line(start, end);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this line.
        /// </summary>
        /// <returns>A Nurbs curve shaped like this line.</returns>
        public NurbsCurve ToNurbsCurve()
        {
            List<Vector3> pts = new List<Vector3>{Start, End};
            Knot knots = new Knot{0,0,1,1};

            return new NurbsCurve(degree: 1, knots, pts);
        }

        /// <summary>
        /// Transform the line using the transformation matrix.
        /// </summary>
        /// <param name="xForm">Transform matrix to apply.</param>
        /// <returns>A line transformed.</returns>
        public Line Transform(Transform xForm)
        {
            var pt1 = Start * xForm;
            var pt2 = End * xForm;
            return new Line(pt1, pt2);
        }

        /// <summary>
        /// Constructs the string representation of the line.
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return $"{Start} - {End} - L:{Length}";
        }

        /// <summary>
        /// Check if the line is equal to the provided line.
        /// Two lines are equal if the end points are the same.
        /// </summary>
        /// <param name="other">The line to compare.</param>
        /// <returns>True if the end points are equal, otherwise false.</returns>
        public bool Equals(Line other)
        {
            if (other is null) return false;
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        /// <summary>
        /// Get the hash code for the line.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return new[] { Start, End }.GetHashCode();
        }
    }
}
