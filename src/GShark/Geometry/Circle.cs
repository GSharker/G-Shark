using GShark.Core;
using System;
using System.Linq;
using GShark.Interfaces;
using GShark.Operation;

namespace GShark.Geometry
{
    /// <summary>
    /// Represents a circle.<br/>
    /// The base values of the circle are the radius and a plane, with origin at the center of the circle.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Geometry/CircleTests.cs?name=example)]
    /// </example>
    public class Circle : ICurve, IEquatable<Circle>, ITransformable<Circle>
    {
        internal Interval _domain = new Interval(0.0, 2.0 * Math.PI);
        internal double _length;

        /// <summary>
        /// Initializes a circle on a plane with a given radius.
        /// </summary>
        /// <param name="plane">Plane of the circle. Plane origin defines the center of the circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        public Circle(Plane plane, double radius)
        {
            Plane = plane;
            Radius = Math.Abs(radius);
            _length = Math.Abs(2.0 * Math.PI * radius);
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

            Point3 center = Trigonometry.EquidistantPoint(pt1, pt2, pt3);
            Vector3 normal = Vector3.ZAxis.PerpendicularTo(pt1, pt2, pt3);
            Vector3 xDir = pt1 - center;
            Vector3 yDir = Vector3.CrossProduct(normal, xDir);

            Plane = new Plane(center, xDir, yDir);
            Radius = xDir.Length;
            _length = Math.Abs(2.0 * Math.PI * Radius);
        }

        /// <summary>
        /// Gets the plane where the circular curve lays.
        /// </summary>
        public Plane Plane { get; }

        /// <summary>
        /// Gets the radius of the circular curve.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets the angle domain (in radians) of this circular curve.
        /// </summary>
        public Interval AngleDomain => _domain;

        /// <summary>
        /// Gets the center of the circular curve.
        /// </summary>
        public Point3 Center => Plane.Origin;

        /// <summary>
        /// Gets the circumference of the circular curve.
        /// </summary>
        public double Length => _length;

        /// <summary>
        /// Gets the start point of the circular curve.
        /// </summary>
        public Point3 StartPoint => PointAt(0.0);

        /// <summary>
        /// Gets the mid-point of the circular curve.
        /// </summary>
        public Point3 MidPoint => PointAt(_domain.Mid);

        /// <summary>
        /// Gets the end point of the circular curve.
        /// </summary>
        public Point3 EndPoint => PointAt(_domain.T1);

        /// <summary>
        /// Gets the bounding box of this circular curve.
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
        /// Determines the value of the Nth derivative at a parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate derivative. A parameter between 0.0 and angle domain in radians.</param>
        /// <param name="derivative">Which order of derivative is wanted. Valid values are 0,1,2,3.</param>
        /// <returns>The derivative of the circle at the given parameter.</returns>
        public Vector3 DerivativeAt(double t, int derivative = 0)
        {
            if (t < 0.0)
            {
                t = 0.0;
            }

            if (t > _domain.Max)
            {
                t = _domain.Max;
            }

            double r0 = 0;
            double r1 = 0;
            switch (derivative % 4)
            {
                case 0:
                    r0 = Radius * Math.Cos(t);
                    r1 = Radius * Math.Sin(t);
                    break;
                case 1:
                    r0 = Radius * -Math.Sin(t);
                    r1 = Radius * Math.Cos(t);
                    break;
                case 2:
                    r0 = Radius * -Math.Cos(t);
                    r1 = Radius * -Math.Sin(t);
                    break;
                case 3:
                    r0 = Radius * Math.Sin(t);
                    r1 = Radius * -Math.Cos(t);
                    break;
            }

            return r0 * Plane.XAxis + r1 * Plane.YAxis;
        }

        /// <summary>
        /// Evaluates the point at the parameter t on the circular curve.
        /// </summary>
        /// <param name="t">A parameter between 0.0 and angle domain in radians.></param>
        /// <returns>Point on the circular curve.</returns>
        public Point3 PointAt(double t)
        {
            if (t < 0.0)
            {
                t = 0.0;
            }

            if (t > _domain.Max)
            {
                t = _domain.Max;
            }

            return Plane.PointAt(Math.Cos(t) * Radius, Math.Sin(t) * Radius);
        }

        /// <summary>
        /// Evaluates a point at the specif length.
        /// </summary>
        /// <param name="length">The length where to evaluate the point.</param>
        /// <param name="normalized">If false, the length is between 0.0 and length of the curve. If true, the length factor is normalized between 0.0 and 1.0.</param>
        /// <returns>The point at the length.</returns>
        public Point3 PointAtLength(double length, bool normalized = false)
        {
            if (length <= 0)
            {
                return StartPoint;
            }

            if (normalized)
            {
                if (length >= 1)
                {
                    return EndPoint;
                }
            }
            else
            {
                if (length > Length)
                {
                    return EndPoint;
                }
            }

            double theta = (normalized)
                ? _domain.T0 + (_domain.T1 - _domain.T0) * length
                : GSharkMath.ToRadians((length * 360) / (Math.PI * 2 * Radius));

            Vector3 xDir = Plane.XAxis * Math.Cos(theta) * Radius;
            Vector3 yDir = Plane.YAxis * Math.Sin(theta) * Radius;

            return Plane.Origin + xDir + yDir;
        }

        /// <summary>
        /// Calculates the tangent at the parameter on the circular curve.
        /// </summary>
        /// <param name="t">A parameter between 0.0 and angle domain in radians.</param>
        /// <returns>Unitized tangent vector at the parameter.</returns>
        public Vector3 TangentAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            Vector3 derivative = DerivativeAt(t, 1);
            return derivative.Unitize();
        }

        /// <summary>
        /// Evaluates the tangent at the specific length.
        /// </summary>
        /// <param name="length">The length where to evaluate the tangent.</param>
        /// <param name="normalized">If false, the length is between 0.0 and length of the curve. If true, the length factor is normalized between 0.0 and 1.0.</param>
        /// <returns>The unitize tangent at the length.</returns>
        public Vector3 TangentAtLength(double length, bool normalized = false)
        {
            Point3 pt = PointAtLength(length, normalized);
            (double u, double v) = Plane.ClosestParameters(pt);
            double t = EvaluateParameter(u, v, false);
            return DerivativeAt(t, 1).Unitize();
        }

        /// <summary>
        /// Returns the length at a given parameter.
        /// </summary>
        /// <param name="t">A parameter between 0.0 and angle domain in radians.</param>
        /// <returns>The curve length at t.</returns>
        public double LengthAt(double t)
        {
            if (t <= 0)
            {
                return 0;
            }

            if (t >= _domain.T1)
            {
                return _length;
            }

            return Radius * t;
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

            double t = EvaluateParameter(u, v, false);

            return PointAt(t);
        }

        /// <summary>
        /// Gets the parameter on the circular curve which is closest to the test point.
        /// </summary>
        /// <param name="pt">The test point to project onto the circular curve.</param>
        /// <returns>The parameter on the circular curve that is close to the test point.</returns>
        public double ClosestParameter(Point3 pt)
        {
            (double u, double v) = Plane.ClosestParameters(pt);
            if (Math.Abs(u) < GSharkMath.MinTolerance && Math.Abs(v) < GSharkMath.MinTolerance)
            {
                return 0.0;
            }

            return EvaluateParameter(u, v, false);
        }

        /// <summary>
        /// Computes the offset of a circle.
        /// </summary>
        /// <param name="distance">The distance of the offset. If negative the offset will be in the opposite side.</param>
        /// <returns>The offset circle.</returns>
        public Circle Offset(double distance)
        {
            if (distance == 0.0)
            {
                return this;
            }

            return new Circle(Plane, Radius + distance);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this arc.<br/>
        /// <em>Implementation of Algorithm A7.1 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <returns>A nurbs curve shaped like this arc.</returns>
        public NurbsCurve ToNurbs()
        {
            Vector3 axisX = Plane.XAxis;
            Vector3 axisY = Plane.YAxis;
            double curveAngle = _domain.Length;
            int numberOfArc;
            Point4[] ctrPts;

            // Number of arcs.
            double piNum = 0.5 * Math.PI;
            if ((curveAngle - piNum) <= GSharkMath.Epsilon)
            {
                numberOfArc = 1;
                ctrPts = new Point4[3];
            }
            else if ((curveAngle - piNum * 2) <= GSharkMath.Epsilon)
            {
                numberOfArc = 2;
                ctrPts = new Point4[5];
            }
            else if ((curveAngle - piNum * 3) <= GSharkMath.Epsilon)
            {
                numberOfArc = 3;
                ctrPts = new Point4[7];
            }
            else
            {
                numberOfArc = 4;
                ctrPts = new Point4[9];
            }

            double detTheta = curveAngle / numberOfArc;
            double weight = Math.Cos(detTheta / 2);
            Point3 p0 = Center + (axisX * (Radius * Math.Cos(_domain.T0)) + axisY * (Radius * Math.Sin(_domain.T0)));
            Vector3 t0 = axisY * Math.Cos(_domain.T0) - axisX * Math.Sin(_domain.T0);

            KnotVector knots = new KnotVector(CollectionHelpers.RepeatData(0.0, ctrPts.Length + 3));
            int index = 0;
            double angle = _domain.T0;

            ctrPts[0] = new Point4(p0);

            for (int i = 1; i < numberOfArc + 1; i++)
            {
                angle += detTheta;
                Point3 p2 = Center + (axisX * (Radius * Math.Cos(angle)) + axisY * (Radius * Math.Sin(angle)));

                ctrPts[index + 2] = new Point4(p2);

                Vector3 t2 = (axisY * Math.Cos(angle)) - (axisX * Math.Sin(angle));
                Line ln0 = new Line(p0, t0.Unitize() + p0);
                Line ln1 = new Line(p2, t2.Unitize() + p2);
                Intersect.LineLine(ln0, ln1, out _, out _, out double u0, out _);
                Point3 p1 = p0 + (t0 * u0);

                ctrPts[index + 1] = new Point4(p1, weight);
                index += 2;

                if (i >= numberOfArc)
                {
                    continue;
                }

                p0 = p2;
                t0 = t2;
            }

            int j = 2 * numberOfArc + 1;
            for (int i = 0; i < 3; i++)
            {
                knots[i] = 0.0;
                knots[i + j] = 1.0;
            }

            switch (numberOfArc)
            {
                case 2:
                    knots[3] = knots[4] = 0.5;
                    break;
                case 3:
                    knots[3] = knots[4] = (double)1 / 3;
                    knots[5] = knots[6] = (double)2 / 3;
                    break;
                case 4:
                    knots[3] = knots[4] = 0.25;
                    knots[5] = knots[6] = 0.5;
                    knots[7] = knots[8] = 0.75;
                    break;
            }

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
        /// Determines whether the circle is equal to another.
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

            t -= _domain.T0;

            while (t < 0.0)
            {
                t += twoPi;
            }

            while (t >= twoPi)
            {
                t -= twoPi;
            }

            double t1 = _domain.Length;
            if (t > t1)
            {
                t = t > 0.5 * t1 + Math.PI ? 0.0 : t1;
            }

            return (parametrize) ? (t - _domain.T0) / (_domain.T1 - _domain.T0) : t;
        }
    }
}
