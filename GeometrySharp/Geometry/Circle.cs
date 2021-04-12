using System;
using System.Linq;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo: ToNurbs.
    // ToDo: Transform.
    public class Circle : Curve, ITransformable<Circle>
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
            Vector3 center = Trigonometry.PointAtEqualDistanceFromThreePoints(pt1, pt2, pt3);
            Vector3 normal = Vector3.ZAxis.PerpendicularTo(pt1, pt2, pt3);
            Vector3 xDir = pt1 - center;
            Vector3 yDir = Vector3.Cross(normal, xDir);

            Plane = new Plane(center, xDir, yDir, normal);
            Radius = xDir.Length();
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
        public override BoundingBox BoundingBox
        {
            get
            {
                double val1 = this.Radius * Length(this.Plane.ZAxis[1], this.Plane.ZAxis[2]);
                double val2 = this.Radius * Length(this.Plane.ZAxis[2], this.Plane.ZAxis[0]);
                double val3 = this.Radius * Length(this.Plane.ZAxis[0], this.Plane.ZAxis[1]);

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

        //ToDo: describe this better.
        private double Length(double x, double y)
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
        public override Vector3 PointAt(double t, bool parametrize = true)
        {
            double tRemap = (parametrize) ? GeoSharpMath.RemapValue(t, new Interval(0.0, 1.0), new Interval(0.0, 2 * Math.PI)) : t;
            return Plane.PointAt(Math.Cos(tRemap) * this.Radius, Math.Sin(tRemap) * this.Radius);
        }

        /// <summary>
        /// Calculates the vector tangent of a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of tangent ot evaluate.</param>
        /// <returns></returns>
        public override Vector3 TangentAt(double t, bool parametrize = true)
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
        public override Vector3 ClosestPt(Vector3 pt)
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

        public Circle Transform(Transform transform)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this circle.
        /// </summary>
        /// <returns>A nurbs curve shaped like this circle.</returns>
        private void ToNurbsCurve()
        {
            Knot knot = new Knot
            {
                0, 0, 0.5 * Math.PI, 0.5 * Math.PI, Math.PI, Math.PI, 1.5 * Math.PI, 1.5 * Math.PI, 2.0 * Math.PI,
                2.0 * Math.PI
            };

            Vector3[] pts = new Vector3[9];
            pts[0] = Plane.PointAt(Radius, 0.0);
            pts[1] = Plane.PointAt(Radius, Radius);
            pts[2] = Plane.PointAt(0.0, Radius);
            pts[3] = Plane.PointAt(-Radius, Radius);
            pts[4] = Plane.PointAt(-Radius, 0.0);
            pts[5] = Plane.PointAt(-Radius, -Radius);
            pts[6] = Plane.PointAt(0.0, -Radius);
            pts[7] = Plane.PointAt(Radius, -Radius);
            pts[8] = pts[0];

            

            NurbsCurve n = new NurbsCurve(2, knot, pts.ToList());
        }
    }
}
