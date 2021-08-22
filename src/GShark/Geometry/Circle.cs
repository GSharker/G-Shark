using GShark.Core;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// Represents a circle.<br/>
    /// The base values of the circle are the radius and a plane, with origin at the center of the circle.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Geometry/CircleTests.cs?name=example)]
    /// </example>
    public class Circle : IEquatable<Circle>, ITransformable<Circle>
    {
        internal Interval Domain = new Interval(0.0, 2.0 * Math.PI);

        /// <summary>
        /// Initializes a circle on a plane with a given radius.
        /// </summary>
        /// <param name="plane">Plane of the circle. Plane origin defines the center of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        public Circle(Plane plane, double radius)
        {
            Plane = plane;
            Radius = Math.Abs(radius);
        }

        /// <summary>
        /// Initializes a circle on planeXY with center in 0,0,0 by a given radius.
        /// </summary>
        /// <param name="radius">Radius of the circle.</param>
        public Circle(double radius)
            : this(Plane.PlaneXY, radius)
        {
        }

        /// <summary>
        /// Initializes an circle from three points.
        /// https://github.com/sergarrido/random/tree/master/circle3d
        /// </summary>
        /// <param name="pt1">Start point of the arc.</param>
        /// <param name="pt2">Interior point on arc.</param>
        /// <param name="pt3">End point of the arc.</param>
        public Circle(Point3 pt1, Point3 pt2, Point3 pt3)
        {
            if (!pt1.IsValid)
            {
                throw new Exception("The first point is not valid.");
            }
            if (!pt2.IsValid)
            {
                throw new Exception("The second point is not valid.");
            }
            if (!pt3.IsValid)
            {
                throw new Exception("The third point is not valid.");
            }

            Point3 center = Trigonometry.PointAtEqualDistanceFromThreePoints(pt1, pt2, pt3);
            Vector3 normal = Vector3.ZAxis.PerpendicularTo(pt1, pt2, pt3);
            Vector3 xDir = pt1 - center;
            Vector3 yDir = Vector3.CrossProduct(normal, xDir);

            Plane = new Plane(center, xDir, yDir);
            Radius = xDir.Length;
        }

        /// <summary>
        /// Gets the plane where the circle lays.
        /// </summary>
        public Plane Plane { get; }

        /// <summary>
        /// Gets the radius of the circle.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets the center of the circle.
        /// </summary>
        public Point3 Center => Plane.Origin;

        /// <summary>
        /// Gets the circumference of the circle.
        /// </summary>
        public double Length => Math.Abs(2.0 * Math.PI * Radius);

        /// <summary>
        /// Gets the start point of the circle.
        /// </summary>
        public Point3 StartPoint => PointAt(0.0);

        /// <summary>
        /// Gets the mid-point of the arc.
        /// </summary>
        public Point3 MidPoint => PointAt(Domain.Mid);

        /// <summary>
        /// Gets the end point of the arc.
        /// </summary>
        public Point3 EndPoint => PointAt(1.0);

        /// <summary>
        /// Gets the BoundingBox of this Circle.
        /// </summary>
        public virtual BoundingBox GetBoundingBox()
        {
            double val1 = Radius * SelectionLength(Plane.ZAxis[1], Plane.ZAxis[2]);
            double val2 = Radius * SelectionLength(Plane.ZAxis[2], Plane.ZAxis[0]);
            double val3 = Radius * SelectionLength(Plane.ZAxis[0], Plane.ZAxis[1]);

            double minX = Plane.Origin[0] - val1;
            double maxX = Plane.Origin[0] + val1;
            double minY = Plane.Origin[1] - val2;
            double maxY = Plane.Origin[1] + val2;
            double minZ = Plane.Origin[2] - val3;
            double maxZ = Plane.Origin[2] + val3;

            Point3 min = new Point3(minX, minY, minZ);
            Point3 max = new Point3(maxX, maxY, maxZ);
            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Evaluates the point at the parameter t on the circle.
        /// </summary>
        /// <param name="t">A parameter between 0.0 to 1.0 or between the angle domain.></param>
        /// <returns>Point on the circle.</returns>
        public Point3 PointAt(double t)
        {
            if (t < 0.0)
            {
                return StartPoint;
            }

            if (t > 1.0)
            {
                return EndPoint;
            }

            double theta = Domain.T0 + (Domain.T1 - Domain.T0) * t;
            return Plane.PointAt(Math.Cos(theta) * Radius, Math.Sin(theta) * Radius);
        }

        /// <summary>
        /// Evaluates the point at the specific length.
        /// </summary>
        /// <param name="length">The length where to evaluate the point.</param>
        /// <returns>The point at the length.</returns>
        public Point3 PointAtLength(double length)
        {
            if (length < 0)
            {
                return StartPoint;
            }

            if (length > Length)
            {
                return EndPoint;
            }

            double angleLength = GSharkMath.ToRadians((length * 360) / (Math.PI * 2 * Radius));

            Vector3 xDir = Plane.XAxis * Math.Cos(angleLength) * Radius;
            Vector3 yDir = Plane.YAxis * Math.Sin(angleLength) * Radius;

            return Plane.Origin + xDir + yDir;
        }

        /// <summary>
        /// Calculates the tangent at the parameter t on the circle curve.
        /// </summary>
        /// <param name="t">A parameter between 0.0 to 1.0.</param>
        /// <returns>Unitized tangent vector at the t parameter.</returns>
        public Vector3 TangentAt(double t)
        {
            if (t < 0.0)
            {
                t = 0.0;
            }

            if (t > 1.0)
            {
                t = 1.0;
            }

            double theta = Domain.T0 + (Domain.T1 - Domain.T0) * t;

            double r1 = Radius * (-Math.Sin(theta));
            double r2 = Radius * (Math.Cos(theta));

            Vector3 vector = Plane.XAxis * r1 + Plane.YAxis * r2;

            return vector.Unitize();
        }

        /// <summary>
        /// Evaluates the tangent at the specific length.
        /// </summary>
        /// <param name="length">The length where to evaluate the tangent.</param>
        /// <returns>The unitize tangent at the length.</returns>
        public Vector3 TangentAtLength(double length)
        {
            Point3 pt = PointAtLength(length);
            (double u, double v) = Plane.ClosestParameters(pt);
            double t = EvaluateParameter(u, v, true);
            return TangentAt(t);
        }

        /// <summary>
        /// Returns the length at a given parameter.
        /// </summary>
        /// <param name="t">Parameter, between 0 and 1.</param>
        /// <returns>The curve length at t.</returns>
        public double LengthAt(double t)
        {
            if (t < 0)
            {
                return 0;
            }

            if (t > 1)
            {
                return Length;
            }

            return Length * t;
        }

        /// <summary>
        /// Gets the point on the circular curve which is closest to the test point.
        /// </summary>
        /// <param name="pt">The test point to project onto the circular curve.</param>
        /// <returns>The point on the circular curve that is close to the test point.</returns>
        public Point3 ClosestPoint(Point3 pt)
        {
            (double u, double v) = Plane.ClosestParameters(pt);
            if (Math.Abs(u) < GSharkMath.MinTolerance && Math.Abs(v) < GSharkMath.MinTolerance)
            {
                return PointAt(0.0);
            }

            double t = EvaluateParameter(u, v, true);

            return PointAt(t);
        }

        /// <summary>
        /// Gets the NURBS form of the circle.
        /// </summary>
        /// <returns>A NURBS curve.</returns>
        public virtual NurbsCurve ToNurbs()
        {
            Point4[] ctrPts = new Point4[9];
            ctrPts[0] = new Point4(Plane.PointAt(Radius, 0.0));
            ctrPts[1] = new Point4(Plane.PointAt(Radius, Radius), 1.0 / Math.Sqrt(2.0));
            ctrPts[2] = new Point4(Plane.PointAt(0.0, Radius));
            ctrPts[3] = new Point4(Plane.PointAt(-Radius, Radius), 1.0 / Math.Sqrt(2.0));
            ctrPts[4] = new Point4(Plane.PointAt(-Radius, 0.0));
            ctrPts[5] = new Point4(Plane.PointAt(-Radius, -Radius), 1.0 / Math.Sqrt(2.0));
            ctrPts[6] = new Point4(Plane.PointAt(0.0, -Radius));
            ctrPts[7] = new Point4(Plane.PointAt(Radius, -Radius), 1.0 / Math.Sqrt(2.0));
            ctrPts[8] = ctrPts[0];

            KnotVector knots = new KnotVector
            {
                0, 0, 0,
                0.5 * Math.PI, 0.5 * Math.PI,
                Math.PI, Math.PI,
                1.5 * Math.PI, 1.5 * Math.PI,
                2.0 * Math.PI, 2.0 * Math.PI, 2.0 * Math.PI
            };

            return new NurbsCurve(2, knots, ctrPts.ToList());
        }

        /// <summary>
        /// Applies a transformation to the plane where the circle is on.
        /// </summary>
        /// <param name="transformation">Transformation matrix to apply.</param>
        /// <returns>A transformed arc.</returns>
        public Circle Transform(Transform transformation)
        {
            Plane plane = Plane.Transform(transformation);
            return new Circle(plane, Radius);
        }

        /// <summary>
        /// Determines whether the circle is equal to another circle.
        /// The circles are equal if have the same plane and radius.
        /// </summary>
        /// <param name="other">The circle to compare to.</param>
        /// <returns>True if the circle are equal, otherwise false.</returns>
        public bool Equals(Circle other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return false;
            }

            return Math.Abs(Radius - other.Radius) < GSharkMath.MinTolerance && Plane == other.Plane;
        }

        /// <summary>
        /// Computes a hash code for the circle.
        /// </summary>
        /// <returns>A unique hashCode of an circle.</returns>
        public override int GetHashCode()
        {
            return Radius.GetHashCode() ^ Plane.GetHashCode();
        }

        /// <summary>
        /// Gets the text representation of an circle.
        /// </summary>
        /// <returns>Text value.</returns>
        public override string ToString()
        {
            return $"Circle(R:{Radius})";
        }

        private static double SelectionLength(double x, double y)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            if (y > x)
            {
                double num = x;
                x = y;
                y = num;
            }
            double num1;
            if (x > double.Epsilon)
            {
                double num2 = 1.0 / x;
                y *= num2;
                num1 = x * Math.Sqrt(1.0 + y * y);
            }
            else
            {
                num1 = x <= 0.0 || double.IsInfinity(x) ? 0.0 : x;
            }

            return num1;
        }

        private double EvaluateParameter(double u, double v, bool parametrize = false)
        {
            double twoPi = 2.0 * Math.PI;

            double t = Math.Atan2(v, u);
            if (t < 0.0)
            {
                t += twoPi;
            }

            t -= Domain.T0;

            while (t < 0.0)
            {
                t += twoPi;
            }

            while (t >= twoPi)
            {
                t -= twoPi;
            }

            double t1 = Domain.Length;
            if (t > t1)
            {
                t = t > 0.5 * t1 + Math.PI ? 0.0 : t1;
            }

            return (parametrize) ? (t - Domain.T0) / (Domain.T1 - Domain.T0) : t;
        }
    }
}
