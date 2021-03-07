using System;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo: Circle by 3 points.
    public class Circle
    {
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
        public Vector3 Center => Plane.Origin;

        /// <summary>
        /// Gets the circumference of the circle.
        /// </summary>
        public double Circumference => Math.Abs(2.0 * Math.PI * Radius);

        /// <summary>
        /// Gets the BoundingBox of this Circle.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                Vector3 xDir = Plane.XAxis * Radius;
                Vector3 yDir = Plane.YAxis * Radius;

                Vector3 min = Center - xDir - yDir;
                Vector3 max = Center + xDir + yDir;

                return new BoundingBox(min, max);
            }
        }

        /// <summary>
        /// Calculates the point on a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of point to evaluate.</param>
        /// <returns>The point on the circle at the given parameter.</returns>
        public Vector3 PointAt(double t)
        {
            return Plane.PointAt(Math.Cos(t) * this.Radius, Math.Sin(t) * this.Radius);
        }

        /// <summary>
        /// Calculates the vector tangent of a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of tangent ot evaluate.</param>
        /// <returns></returns>
        public Vector3 TangentAt(double t)
        {
            double r1 = this.Radius * (-Math.Sin(t));
            double r2 = this.Radius * (Math.Cos(t));

            Vector3 vector = this.Plane.XAxis * r1 + this.Plane.YAxis * r2;

            return vector.Unitize();
        }

        /// <summary>
        /// Gets the point on the circle which is closest to the test point.
        /// </summary>
        /// <param name="pt">The test point to project onto the circle.</param>
        /// <returns>The point on the circle that is close to the test point.</returns>
        public Vector3 ClosestPt(Vector3 pt)
        {
            Vector3 closestPt = Vector3.Unset;
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
                t += 2.0 * Math.PI;
            }

            return PointAt(t);
        }
    }
}
