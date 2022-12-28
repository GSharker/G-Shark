using GShark.Core;
using System;
using System.Collections.Generic;
using GShark.Interfaces;

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
    public class Arc : Circle, ICurve<Arc>
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
            ToNurbs();
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
            ToNurbs();
        }

        /// <summary>
        /// Gets the angle of this arc.<br/>
        /// Angle value in radians.
        /// </summary>
        public double Angle => _domain.Length;

        /// <summary>
        /// Gets the bounding box of this arc.<br/>
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
        /// <param name="t">Transformation matrix to apply.</param>
        /// <returns>A transformed arc.</returns>
        public new Arc Transform(TransformMatrix t)
        {
            Plane plane = Plane.Transform(t);
            Interval angleDomain = new Interval(_domain.T0, _domain.T1);

            return new Arc(plane, Radius, angleDomain);
        }

        /// <summary>
        /// Computes the offset of the arc.
        /// </summary>
        /// <param name="distance">The distance of the offset.</param>
        /// <returns>The offset arc.</returns>
        public new Arc Offset(double distance)
        {
            if (distance == 0.0)
            {
                return this;
            }

            return new Arc(Plane, Radius + distance, AngleDomain);
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
