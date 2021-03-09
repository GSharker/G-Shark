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
        /// Initializes an circle from three points.
        /// https://github.com/sergarrido/random/tree/master/circle3d
        /// </summary>
        /// <param name="pt1">Start point of the arc.</param>
        /// <param name="pt2">Interior point on arc.</param>
        /// <param name="pt3">End point of the arc.</param>
        public Circle(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            if(LinearAlgebra.Orientation(pt1, pt2, pt3) == 0)
                throw new Exception("Points must not be collinear.");

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

            double radius = xDir.Length();

            Plane = new Plane(center, pt1, center + yDir.Amplify(radius));
            Radius = radius;
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
                double val1 = this.Radius * length(this.Plane.ZAxis[1], this.Plane.ZAxis[2]);
                double val2 = this.Radius * length(this.Plane.ZAxis[2], this.Plane.ZAxis[0]);
                double val3 = this.Radius * length(this.Plane.ZAxis[0], this.Plane.ZAxis[1]);

                double minX = this.Plane.Origin[0] - val1;
                double maxX = this.Plane.Origin[0] + val1;
                double minY = this.Plane.Origin[1] - val2;
                double maxY = this.Plane.Origin[1] + val2;
                double minZ = this.Plane.Origin[2] - val3;
                double maxZ = this.Plane.Origin[2] + val3;

                Vector3 min = new Vector3 {minX, minY, minZ};
                Vector3 max = new Vector3 { maxX, maxY, maxZ };

                return new BoundingBox(min, max);
            }
        }

        private double length(double x, double y)
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
                num1 = x <= 0.0 || double.IsInfinity(x) ? 0.0 : x;
            return num1;
        }

        /// <summary>
        /// Calculates the point on a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of point to evaluate.</param>
        /// <param name="parametrize">True per default using parametrize value between 0.0 to 1.0.</param>
        /// <returns>The point on the circle at the given parameter.</returns>
        public Vector3 PointAt(double t, bool parametrize = true)
        {
            double tRemap = (parametrize) ? GeoSharpMath.RemapValue(t, new Interval(0.0, 1.0), new Interval(0.0, 2 * Math.PI)) : t;
            return Plane.PointAt(Math.Cos(tRemap) * this.Radius, Math.Sin(tRemap) * this.Radius);
        }

        /// <summary>
        /// Calculates the vector tangent of a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of tangent ot evaluate.</param>
        /// <returns></returns>
        public Vector3 TangentAt(double t, bool parametrize = true)
        {
            double tRemap = (parametrize) ? GeoSharpMath.RemapValue(t, new Interval(0.0, 1.0), new Interval(0.0, 2 * Math.PI)) : t;
            double r1 = this.Radius * (-Math.Sin(tRemap));
            double r2 = this.Radius * (Math.Cos(tRemap));

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
            (double u, double v) = Plane.ClosestParameters(pt);
            if (Math.Abs(u) < GeoSharpMath.MAXTOLERANCE && Math.Abs(v) < GeoSharpMath.MAXTOLERANCE)
            {
                return PointAt(0.0);
            }

            double t = Math.Atan2(v, u);
            if (t < 0.0)
            {
                t += 2.0 * Math.PI;
            }

            return PointAt(t, false);
        }
    }
}
