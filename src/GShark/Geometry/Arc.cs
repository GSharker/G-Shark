using GShark.Core;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// An arc is any portion (other than the entire curve) of the circumference of a circle.<br/>
    /// Represents the value of a plane, two angles (interval in radians) and a radius (radians).<br/>
    /// The arc run ccw rotation where Xaxis and Yaxis form a orthonormal frame.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Geometry/ArcTests.cs?name=example)]
    /// </example>
    public class Arc : Circle, IEquatable<Arc>, ITransformable<Arc>
    {
        /// <summary>
        /// Initializes an arc from a plane, a radius and an angle domain expressed as an interval in radians.
        /// </summary>
        /// <param name="plane">Base plane.</param>
        /// <param name="radius">Radius value in radians.</param>
        /// <param name="angleDomainRadians">Interval defining the angle in radians of the arc. Interval should be between 0.0 to 2Pi</param>
        public Arc(Plane plane, double radius, Interval angleDomainRadians) : base(plane, radius)
        {
            if (angleDomainRadians.T1 < angleDomainRadians.T0)
            {
                throw new Exception("Angle domain must never be decreasing.");
            }

            _domain = (angleDomainRadians.Length > Math.PI * 2.0)
                ? new Interval(AngularDiff(angleDomainRadians.T0, Math.PI * 2.0),
                    AngularDiff(angleDomainRadians.T1, Math.PI * 2.0))
                : angleDomainRadians;
            _length = Math.Abs(Angle * Radius);
        }

        /// <summary>
        /// Initializes an arc from a plane, a radius and an angle in radians.
        /// </summary>
        /// <param name="plane">Base plane.</param>
        /// <param name="radius">Radius value.</param>
        /// <param name="angleRadians">Angle of the arc in radians.</param>
        public Arc(Plane plane, double radius, double angleRadians)
            : this(plane, radius, new Interval(0.0, angleRadians))
        {
        }

        /// <summary>
        /// Initializes an arc from three points.
        /// </summary>
        /// <param name="pt1">Start point of the arc.</param>
        /// <param name="pt2">Interior point on arc.</param>
        /// <param name="pt3">End point of the arc.</param>
        public Arc(Point3 pt1, Point3 pt2, Point3 pt3) : base(pt1, pt2, pt3)
        {
            (double u, double v) = Plane.ClosestParameters(pt3);
            double angle = Math.Atan2(v, u);

            if (angle < 0.0)
            {
                angle += 2 * Math.PI;
            }

            _domain = new Interval(0.0, angle);
            _length = Math.Abs(Angle * Radius);
        }

        /// <summary>
        /// Gets the angle of this arc.<br/>
        /// Angle value in radians.
        /// </summary>
        public double Angle => Domain.Length;

        /// <summary>
        /// Gets the BoundingBox of this arc.<br/>
        /// https://stackoverflow.com/questions/1336663/2d-bounding-box-of-a-sector
        /// </summary>
        public BoundingBox BoundingBox()
        {
            Plane orientedPlane = Plane.Align(Vector3.XAxis);
            Point3 pt0 = StartPoint;
            Point3 pt1 = EndPoint;
            Point3 ptC = orientedPlane.Origin;

            double theta0 = Math.Atan2(pt0[1] - ptC[1], pt0[0] - ptC[0]);
            double theta1 = Math.Atan2(pt1[1] - ptC[1], pt1[0] - ptC[0]);

            List<Point3> pts = new List<Point3> { pt0, pt1 };

            if (AnglesSequence(theta0, 0, theta1))
            {
                pts.Add(ptC + orientedPlane.XAxis * Radius);
            }
            if (AnglesSequence(theta0, Math.PI / 2, theta1))
            {
                pts.Add(ptC + orientedPlane.YAxis * Radius);
            }
            if (AnglesSequence(theta0, Math.PI, theta1))
            {
                pts.Add(ptC - orientedPlane.XAxis * Radius);
            }
            if (AnglesSequence(theta0, Math.PI * 3 / 2, theta1))
            {
                pts.Add(ptC - orientedPlane.YAxis * Radius);
            }

            return new BoundingBox(pts);
        }

        /// <summary>
        /// Creates an arc defined by a start point, end point and a direction at the first point.
        /// </summary>
        /// <param name="ptStart">Start point arc.</param>
        /// <param name="ptEnd">End point arc.</param>
        /// <param name="dir">TangentAt direction at start.</param>
        /// <returns>An arc.</returns>
        public static Arc ByStartEndDirection(Point3 ptStart, Point3 ptEnd, Vector3 dir)
        {
            if (!ptStart.IsValid)
            {
                throw new Exception("The first point is not valid.");
            }
            if (!ptEnd.IsValid)
            {
                throw new Exception("The second point is not valid.");
            }
            if (!dir.IsValid)
            {
                throw new Exception("The tangent is not valid.");
            }

            Vector3 vec0 = dir.Unitize();
            Vector3 vec1 = (ptEnd - ptStart).Unitize();
            if (vec1.Length.Equals(0.0))
            {
                throw new Exception("Start and End point of the arc are coincident. Enable to create an arc");
            }

            if (vec0.IsParallelTo(vec1) != 0)
            {
                throw new Exception("Tangent is parallel with the endpoints. Enable to create an arc");
            }

            Vector3 vec2 = (vec0 + vec1).Unitize();
            Vector3 vec3 = vec2 * (0.5 * ptStart.DistanceTo(ptEnd) / Vector3.DotProduct(vec2, vec0));
            return new Arc(ptStart, ptStart + vec3, ptEnd);
        }

        /// <summary>
        /// Applies a transformation to the plane where the arc is on.
        /// </summary>
        /// <param name="transformation">Transformation matrix to apply.</param>
        /// <returns>A transformed arc.</returns>
        public new Arc Transform(Transform transformation)
        {
            Plane plane = Plane.Transform(transformation);
            Interval angleDomain = new Interval(Domain.T0, Domain.T1);

            return new Arc(plane, Radius, angleDomain);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this arc.<br/>
        /// Implementation of Algorithm A7.1 from The NURBS Book by Piegl and Tiller.
        /// </summary>
        /// <returns>A nurbs curve shaped like this arc.</returns>
        public new NurbsCurve ToNurbs()
        {
            Vector3 axisX = Plane.XAxis;
            Vector3 axisY = Plane.YAxis;
            int numberOfArc;
            Point4[] ctrPts;

            // Number of arcs.
            double piNum = 0.5 * Math.PI;
            if ((Angle - piNum) <= GSharkMath.Epsilon)
            {
                numberOfArc = 1;
                ctrPts = new Point4[3];
            }
            else if ((Angle - piNum * 2) <= GSharkMath.Epsilon)
            {
                numberOfArc = 2;
                ctrPts = new Point4[5];
            }
            else if ((Angle - piNum * 3) <= GSharkMath.Epsilon)
            {
                numberOfArc = 3;
                ctrPts = new Point4[7];
            }
            else
            {
                numberOfArc = 4;
                ctrPts = new Point4[9];
            }

            double detTheta = Angle / numberOfArc;
            double weight = Math.Cos(detTheta / 2);
            Point3 p0 = Center + (axisX * (Radius * Math.Cos(Domain.T0)) + axisY * (Radius * Math.Sin(Domain.T0)));
            Vector3 t0 = axisY * Math.Cos(Domain.T0) - axisX * Math.Sin(Domain.T0);

            KnotVector knots = new KnotVector(CollectionHelpers.RepeatData(0.0, ctrPts.Length + 3));
            int index = 0;
            double angle = Domain.T0;

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
        /// Determines whether the arc is equal to another.<br/>
        /// The arcs are equal if have the same plane, radius and angle.
        /// </summary>
        /// <param name="other">The arc to compare to.</param>
        /// <returns>True if the arc are equal, otherwise false.</returns>
        public bool Equals(Arc other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return false;
            }

            return Math.Abs(Radius - other.Radius) < GSharkMath.MinTolerance &&
                   Math.Abs(Angle - other.Angle) < GSharkMath.MinTolerance &&
                   Plane == other.Plane;
        }

        /// <summary>
        /// Computes a hash code for the arc.
        /// </summary>
        /// <returns>A unique hashCode of an arc.</returns>
        public override int GetHashCode()
        {
            return Radius.GetHashCode() ^ Angle.GetHashCode() ^ Plane.GetHashCode();
        }

        /// <summary>
        /// Gets the text representation of an arc.
        /// </summary>
        /// <returns>Text value.</returns>
        public override string ToString()
        {
            return $"Arc(R:{Radius} - A:{GSharkMath.ToDegrees(Angle)})";
        }


        private static bool AnglesSequence(double angle1, double angle2, double angle3)
        {
            return AngularDiff(angle1, angle2) + AngularDiff(angle2, angle3) < 2 * Math.PI;
        }

        private static double AngularDiff(double theta1, double theta2)
        {
            double dif = theta2 - theta1;
            while (dif >= 2 * Math.PI)
            {
                dif -= 2 * Math.PI;
            }

            while (dif <= 0)
            {
                dif += 2 * Math.PI;
            }

            return dif;
        }
    }
}
