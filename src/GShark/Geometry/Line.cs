using GShark.Core;
using GShark.Interfaces;
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
    public class Line : NurbsBase, IGeometry<Line>
    {
        /// <summary>
        /// Initializes a line by start point and end point.
        /// </summary>
        /// <param name="startPoint">Start point.</param>
        /// <param name="endPoint">End point.</param>
        public Line(Point3 startPoint, Point3 endPoint)
        {
            if (startPoint == endPoint || !startPoint.IsValid || !endPoint.IsValid)
            {
                throw new Exception("Start or end point is not valid, or they are equal");
            }

            StartPoint = startPoint;
            EndPoint = endPoint;
            Length = StartPoint.DistanceTo(EndPoint);
            Direction = (EndPoint - StartPoint).Unitize();
            ToNurbs();
        }

        /// <summary>
        /// Initializes a line from a starting point, direction and length.
        /// </summary>
        /// <param name="startPoint">Starting point of the line.</param>
        /// <param name="direction">Direction of the line.</param>
        /// <param name="length">Length of the line.</param>
        public Line(Point3 startPoint, Vector3 direction, double length)
        {
            if (length <= GSharkMath.Epsilon)
            {
                throw new Exception("Length must be bigger than zero");
            }

            StartPoint = startPoint;
            EndPoint = startPoint + direction.Amplify(length);
            Length = Math.Abs(length);
            Direction = (EndPoint - StartPoint).Unitize();
            ToNurbs();
        }

        /// <summary>
        /// Gets the start point of the line.
        /// </summary>
        public override Point3 StartPoint { get; }

        /// <summary>
        /// Gets the middle point of the line.
        /// </summary>
        public override Point3 MidPoint => StartPoint + (EndPoint - StartPoint) / 2;

        /// <summary>
        /// Gets the end point of the line.
        /// </summary>
        public override Point3 EndPoint { get; }

        /// <summary>
        /// Length of the line.
        /// </summary>
        public override double Length { get; }

        /// <summary>
        /// Unit vector representing direction of the line.
        /// </summary>
        public Vector3 Direction { get; }

        /// <summary>
        /// Gets the bounding box in ascending fashion.
        /// </summary>
        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bBox = new BoundingBox(StartPoint, EndPoint);
            BoundingBox validBBox = bBox.MakeItValid();
            return validBBox;
        }

        /// <summary>
        /// Gets the closest point on the line to the test point.
        /// </summary>
        /// <param name="point">The closest point to find.</param>
        /// <returns>The point on the line closest to the test point.</returns>
        public override Point3 ClosestPoint(Point3 point)
        {
            Vector3 dir = Direction;
            Vector3 v = point - StartPoint;
            double d = Vector3.DotProduct(v, dir);

            d = Math.Min(Length, d);
            d = Math.Max(d, 0);

            return StartPoint + dir * d;
        }

        /// <summary>
        /// Computes the parameter on the line that is closest to a test point.
        /// </summary>
        /// <param name="point">The test point.</param>
        /// <returns>The parameter on the line closest to the test point.</returns>
        public override double ClosestParameter(Point3 point)
        {
            Vector3 dir = EndPoint - StartPoint;
            double dirLength = dir.SquareLength;

            if (!(dirLength > 0.0)) return 0.0;
            Vector3 ptToStart = point - StartPoint;
            Vector3 ptToEnd = point - EndPoint;

            if (ptToStart.SquareLength <= ptToEnd.SquareLength)
            {
                return Vector3.DotProduct(ptToStart, dir) / dirLength;
            }

            return 1.0 + Vector3.DotProduct(ptToEnd, dir) / dirLength;
        }

        /// <summary>
        /// Evaluates the line at the specified parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate the line. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The point at the specific parameter.</returns>
        public override Point3 PointAt(double t)
        {
            if (t <= 0.0)
            {
                return StartPoint;
            }

            if (t >= 1.0)
            {
                return EndPoint;
            }

            return StartPoint + Direction * (Length * t);
        }

        /// <summary>
        /// Evaluates the point on the curve at a given length.
        /// </summary>
        /// <param name="length">Length, between 0.0 and the length of the curve.</param>
        /// <returns>The point at the given length.</returns>
        public override Point3 PointAtLength(double length)
        {
            if (length <= 0)
            {
                return StartPoint;
            }

            if (length >= Length)
            {
                return EndPoint;
            }

            return StartPoint + Direction * length;
        }

        /// <summary>
        /// Returns the length at a given parameter.
        /// </summary>
        /// <param name="t">Parameter, between 0.0 and 1.0.</param>
        /// <returns>The curve length at parameter.</returns>
        public override double LengthAt(double t)
        {
            if (t <= 0)
            {
                return 0;
            }

            if (t >= 1)
            {
                return Length;
            }

            return Length * t;
        }

        /// <summary>
        /// Flip the endpoint of the line.
        /// </summary>
        /// <returns>The line flipped.</returns>
        public new Line Reverse()
        {
            return new Line(EndPoint, StartPoint);
        }

        /// <summary>
        /// Computes the offset of the line.
        /// </summary>
        /// <param name="distance">The distance of the offset. If negative the offset will be in the opposite side.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset line.</returns>
        public new Line Offset(double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return this;
            }

            Vector3 vecOffset = Vector3.CrossProduct(Direction, pln.ZAxis).Amplify(distance);
            return new Line(StartPoint + vecOffset, EndPoint + vecOffset);
        }

        /// <summary>
        /// Extends the line by lengths on both side.
        /// </summary>
        /// <param name="startLength">Length to extend the line at the start point.</param>
        /// <param name="endLength">Length to extend the line at the end point.</param>
        /// <returns>The extended line.</returns>
        public Line Extend(double startLength, double endLength)
        {
            Point3 start = StartPoint;
            Point3 end = EndPoint;

            if (startLength != 0)
            {
                start = StartPoint - (Direction * startLength);
            }

            if (endLength != 0)
            {
                end = EndPoint + (Direction * endLength);
            }

            return new Line(start, end);
        }

        /// <summary>
        /// Gets the NURBS form of the curve.
        /// </summary>
        /// <returns>A NURBS curve.</returns>
        private void ToNurbs()
        {
            Weights = new List<double> { 1.0, 1.0 };
            Degree = 1;
            Knots = new KnotVector { 0.0, 0.0, 1.0, 1.0 };
            ControlPointLocations = new List<Point3> { StartPoint, EndPoint };
            ControlPoints = new List<Point4> { StartPoint, EndPoint };
        }

        /// <summary>
        /// Transforms the line using the transformation matrix.
        /// </summary>
        /// <param name="t">Transformation matrix to apply.</param>
        /// <returns>A line transformed.</returns>
        public Line Transform(TransformMatrix t)
        {
            Point3 pt1 = StartPoint.Transform(t);
            Point3 pt2 = EndPoint.Transform(t);
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

            return StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint);
        }

        /// <summary>
        /// Checks if the line is equal to the provided line.<br/>
        /// Two lines are equal if the end points are the same.
        /// </summary>
        /// <param name="obj">The curve object.</param>
        /// <returns>Return true if the nurbs curves are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

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
            return new[] { StartPoint, EndPoint }.GetHashCode();
        }

        /// <summary>
        /// Constructs the string representation of the line.
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return $"{StartPoint} - {EndPoint} - L:{Length}";
        }
    }
}
