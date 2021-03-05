using System;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo: TangentAt need the DerivativeAt
    // ToDo: ClosestPoint
    // ToDo: Transform
    // ToDo: IEquatable
    // ToDo: ArcFrom3Pts
    // ToDo: ToString - Arc(R: A:)
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
    }
}
