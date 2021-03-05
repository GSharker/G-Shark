using System;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo: TangentAt need the DerivativeAt
    // ToDo: ClosestPoint
    // ToDo: Transform
    // ToDo: IEquatable
    // ToDo: ArcFromTangent
    /// <summary>
    /// Represents the value of a plane, two angles (interval) and a radius (radiance).
    /// </summary>
    public class Arc
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
            if (angle < 0.0) angle += 2.0 * Math.PI;

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
        public double Length => Math.Abs(Angle * this.Radius);

        /// <summary>
        /// Gets true if the arc is a circle, so the angle is describable as 2Pi.
        /// </summary>
        public bool isCircle => Math.Abs(this.Angle - 2.0 * Math.PI) <= GeoSharpMath.EPSILON;

        /// <summary>
        /// Returns the point at the parameter t on the arc.
        /// </summary>
        /// <param name="t">A parameter between 0.0 to 1.0./param>
        /// <returns>Point on the arc.</returns>
        public Vector3 PointAt(double t)
        {
            double tRemap = GeoSharpMath.RemapValue(t, new Interval(0.0, 1.0), AngleDomain);

            Vector3 xDir = this.Plane.XAxis * Math.Cos(tRemap) * this.Radius;
            Vector3 yDir = this.Plane.YAxis * Math.Sin(tRemap) * this.Radius;

            return this.Plane.Origin + xDir + yDir;
        }
        
        /// <summary>
        /// Gets the BoundingBox of this arc.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                if (isCircle)
                {
                    Vector3 xDir = this.Plane.XAxis * this.Radius;
                    Vector3 yDir = this.Plane.YAxis * this.Radius;

                    Vector3 min = this.Center - xDir - yDir;
                    Vector3 max = this.Center + xDir + yDir;

                    return new BoundingBox(min, max);
                }
                Vector3 pt0 = PointAt(0.0);
                Vector3 pt1 = PointAt(0.5);
                Vector3 pt2 = PointAt(1.0);

                Vector3[] pts = new[] {pt0, pt1, pt2};

                return new BoundingBox(pts);
            }
        }

        /// <summary>
        /// Gets the text representation of an arc.
        /// </summary>
        /// <returns>Text value.</returns>
        public override string ToString()
        {
            return $"Arc(R:{this.Radius} - A:{GeoSharpMath.ToDegrees(this.Angle)})";
        }
    }
}
