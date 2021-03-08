using GeometrySharp.Core;
using System;

namespace GeometrySharp.Geometry
{
    // ToDo: TangentAt need the DerivativeAt
    // ToDo: ArcFromTangent
    /// <summary>
    /// Represents the value of a plane, two angles (interval) and a radius (radiance).
    /// </summary>
    public class Arc : IEquatable<Arc>
    {
        internal Interval AngleDomain;

        /// <summary>
        /// Initializes an arc from a plane, a radius and an angle domain expressed as an interval.
        /// </summary>
        /// <param name="plane">Base plane.</param>
        /// <param name="radius">Radius value.</param>
        /// <param name="angleDomain">Interval defining the angle of the arc. Interval should be between 0.0 to 2Pi</param>
        public Arc(Plane plane, double radius, Interval angleDomain)
        {
            Plane = plane;
            Radius = radius;
            AngleDomain = angleDomain;
        }

        /// <summary>
        /// Initializes an arc from a plane, a radius and an angle.
        /// </summary>
        /// <param name="plane">Base plane.</param>
        /// <param name="radius">Radius value.</param>
        /// <param name="angle">Angle of the arc.</param>
        public Arc(Plane plane, double radius, double angle)
            : this(plane, radius, new Interval(0.0, angle))
        {
        }

        /// <summary>
        /// Initializes an arc from three points.
        /// https://github.com/sergarrido/random/tree/master/circle3d
        /// </summary>
        /// <param name="pt1">Start point of the arc.</param>
        /// <param name="pt2">Interior point on arc.</param>
        /// <param name="pt3">End point of the arc.</param>
        public Arc(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            // ToDo: check if the point are collinear return
            Vector3 v1 = pt2 - pt1;
            Vector3 v2 = pt3 - pt1;

            double v1v1 = Vector3.Dot(v1, v1);
            double v2v2 = Vector3.Dot(v2, v2);
            double v1v2 = Vector3.Dot(v1, v2);

            double a = 0.5 / (v1v1 * v2v2 - v1v2 * v1v2);
            double k1 = a * v2v2 * (v1v1 - v1v2);
            double k2 = a * v1v1 * (v2v2 - v1v2);

            Vector3 center = pt1 + v1 * k1 + v2 * k2;
            Vector3 xDir = pt1 - center;
            Vector3 v3 = pt3 - center;
            Vector3 v4 = Vector3.Cross(xDir, v3);
            Vector3 yDir = Vector3.Cross(xDir, v4);

            double u = Vector3.Dot(v3, xDir.Unitize());
            double v = Vector3.Dot(v3, yDir.Unitize());

            double angle = Math.Atan2(v, u);
            if (angle < 0.0)
            {
                angle += 2.0 * Math.PI;
            }

            double radius = xDir.Length();

            Plane = new Plane(center, pt1, center + yDir.Amplify(radius));
            Radius = radius;
            AngleDomain = new Interval(0.0, angle);
        }

        /// <summary>
        /// Gets the plane on which the arc lies.
        /// </summary>
        public Plane Plane { get; }

        /// <summary>
        /// Gets the radius of the arc.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets the center point of this arc.
        /// </summary>
        public Vector3 Center => Plane.Origin;

        /// <summary>
        /// Gets the angle of this arc.
        /// Angle value in radians.
        /// </summary>
        public double Angle => AngleDomain.Length;

        /// <summary>
        /// Calculates the length of the arc.
        /// </summary>
        public double Length => Math.Abs(Angle * Radius);

        /// <summary>
        /// Gets true if the arc is a circle, so the angle is describable as 2Pi.
        /// </summary>
        public bool IsCircle => Math.Abs(Angle - 2.0 * Math.PI) <= GeoSharpMath.EPSILON;

        /// <summary>
        /// Gets the BoundingBox of this arc.
        /// </summary>
        public BoundingBox BoundingBox
        {
            // ToDo this way to do the BoundingBox doesn't provide accuracy if the arc is oriented in the space or close to a circle.
            get
            {
                Vector3 pt0 = PointAt(0.0);
                Vector3 pt1 = PointAt(0.5);
                Vector3 pt2 = PointAt(1.0);

                Vector3[] pts = new[] { pt0, pt1, pt2 };

                return new BoundingBox(pts);
            }
        }

        /// <summary>
        /// Returns the point at the parameter t on the arc.
        /// </summary>
        /// <param name="t">A parameter between 0.0 to 1.0 or between the angle domain.></param>
        /// <param name="parametrize">True per default using parametrize value between 0.0 to 1.0.</param>
        /// <returns>Point on the arc.</returns>
        public Vector3 PointAt(double t, bool parametrize = true)
        {

            double tRemap = (parametrize) ? GeoSharpMath.RemapValue(t, new Interval(0.0, 1.0), AngleDomain) : t;

            Vector3 xDir = Plane.XAxis * Math.Cos(tRemap) * Radius;
            Vector3 yDir = Plane.YAxis * Math.Sin(tRemap) * Radius;

            return Plane.Origin + xDir + yDir;
        }

        /// <summary>
        /// Calculates the point on an arc that is close to a test point.
        /// </summary>
        /// <param name="pt">The test point. Point to get close to.</param>
        /// <returns>The point on the arc that is close to the test point.</returns>
        public Vector3 ClosestPt(Vector3 pt)
        {
            double twoPi = 2.0 * Math.PI;
            double t = 0.0;

            (double u, double v) = Plane.ClosestParameters(pt);
            if (Math.Abs(u) < GeoSharpMath.MAXTOLERANCE && Math.Abs(v) < GeoSharpMath.MAXTOLERANCE)
            {
                t = 0.0;
                return PointAt(t);
            }

            t = Math.Atan2(v, u);
            if (t < 0.0)
            {
                t += twoPi;
            }

            t -= AngleDomain.Min;

            while (t < 0.0)
            {
                t += twoPi;
            }

            while (t >= twoPi)
            {
                t -= twoPi;
            }

            double t1 = AngleDomain.Length;
            if (t > t1)
            {
                t = t > 0.5 * t1 + Math.PI ? 0.0 : t1;
            }

            return PointAt(AngleDomain.Min + t, false);
        }

        /// <summary>
        /// Applies a transformation to the plane where the arc is on.
        /// </summary>
        /// <param name="transformation">Transformation matrix to apply.</param>
        /// <returns>A transformed arc.</returns>
        public Arc Transform(Transform transformation)
        {
            Plane plane = this.Plane.Transform(transformation);
            Interval angleDomain = new Interval(this.AngleDomain.Min, this.AngleDomain.Max);

            return new Arc(plane, this.Radius, angleDomain);
        }

        /// <summary>
        /// Determines whether the arc is equal to another arc.
        /// The arcs are equal if have the same plane, radius and angle.
        /// </summary>
        /// <param name="other">The arc to compare to.</param>
        /// <returns>True if the arc are equal, otherwise false.</returns>
        public bool Equals(Arc other)
        {
            return Math.Abs(this.Radius - other.Radius) < GeoSharpMath.MAXTOLERANCE &&
                   Math.Abs(this.Angle - other.Angle) < GeoSharpMath.MAXTOLERANCE &&
                   this.Plane == other.Plane;
        }

        /// <summary>
        /// Computes a hash code for the arc.
        /// </summary>
        /// <returns>A unique hashCode of an arc.</returns>
        public override int GetHashCode()
        {
            return this.Radius.GetHashCode() ^ this.Angle.GetHashCode() ^ this.Plane.GetHashCode();
        }

        /// <summary>
        /// Determines whether two arcs have same values.
        /// </summary>
        /// <param name="a">The first arc.</param>
        /// <param name="b">The second arc.</param>
        /// <returns>True if all the value are equal, otherwise false.</returns>
        public static bool operator ==(Arc a, Arc b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Determines whether two arcs have different values.
        /// </summary>
        /// <param name="a">The first arc.</param>
        /// <param name="b">The second arc.</param>
        /// <returns>True if all the value are different, otherwise false.</returns>
        public static bool operator !=(Arc a, Arc b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Gets the text representation of an arc.
        /// </summary>
        /// <returns>Text value.</returns>
        public override string ToString()
        {
            return $"Arc(R:{Radius} - A:{GeoSharpMath.ToDegrees(Angle)})";
        }
    }
}
