using GeometrySharp.Core;
using GeometrySharp.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometrySharp.Geometry
{
    public class Circle : Curve, IEquatable<Circle>, ITransformable<Circle>
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
            ToNurbsCurve();
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
            ToNurbsCurve();
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
                double val1 = Radius * SelectionLength(Plane.ZAxis[1], Plane.ZAxis[2]);
                double val2 = Radius * SelectionLength(Plane.ZAxis[2], Plane.ZAxis[0]);
                double val3 = Radius * SelectionLength(Plane.ZAxis[0], Plane.ZAxis[1]);

                double minX = Plane.Origin[0] - val1;
                double maxX = Plane.Origin[0] + val1;
                double minY = Plane.Origin[1] - val2;
                double maxY = Plane.Origin[1] + val2;
                double minZ = Plane.Origin[2] - val3;
                double maxZ = Plane.Origin[2] + val3;

                Vector3 min = new Vector3 {minX, minY, minZ};
                Vector3 max = new Vector3 { maxX, maxY, maxZ };

                return new BoundingBox(min, max);
            }
        }

        /// <summary>
        /// Calculates the point on a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of point to evaluate.</param>
        /// <returns>The point on the circle at the given parameter.</returns>
        public override Vector3 PointAt(double t)
        {
            return Plane.PointAt(Math.Cos(t) * Radius, Math.Sin(t) * Radius);
        }

        /// <summary>
        /// Calculates the vector tangent of a circle at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of tangent ot evaluate.</param>
        /// <returns></returns>
        public Vector3 TangentAt(double t)
        {
            double r1 = Radius * (-Math.Sin(t));
            double r2 = Radius * (Math.Cos(t));

            Vector3 vector = Plane.XAxis * r1 + Plane.YAxis * r2;

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

            return PointAt(t);
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
        /// Constructs a nurbs curve representation of this circle.
        /// </summary>
        /// <returns>A nurbs curve shaped like this circle.</returns>
        private void ToNurbsCurve()
        {
            Knot knots = new Knot
            {
                0, 0, 0, 
                0.5 * Math.PI, 0.5 * Math.PI, 
                Math.PI, Math.PI, 
                1.5 * Math.PI, 1.5 * Math.PI, 
                2.0 * Math.PI, 2.0 * Math.PI, 2.0 * Math.PI
            };

            Vector3[] ctrPts = new Vector3[9];
            ctrPts[0] = Plane.PointAt(Radius, 0.0);
            ctrPts[1] = Plane.PointAt(Radius, Radius);
            ctrPts[2] = Plane.PointAt(0.0, Radius);
            ctrPts[3] = Plane.PointAt(-Radius, Radius);
            ctrPts[4] = Plane.PointAt(-Radius, 0.0);
            ctrPts[5] = Plane.PointAt(-Radius, -Radius);
            ctrPts[6] = Plane.PointAt(0.0, -Radius);
            ctrPts[7] = Plane.PointAt(Radius, -Radius);
            ctrPts[8] = ctrPts[0];

            List<double> weights = Sets.RepeatData(1.0, ctrPts.Length);
            weights[1] = weights[3] = weights[5] = weights[7] = 1.0 / Math.Sqrt(2.0);

            NurbsCurve n = new NurbsCurve(2, knots, ctrPts.ToList(), weights);

            Degree = 2;
            Knots = knots;
            HomogenizedPoints = LinearAlgebra.PointsHomogeniser(ctrPts.ToList(), weights.ToList());
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

            return Math.Abs(Radius - other.Radius) < GeoSharpMath.MAXTOLERANCE && Plane == other.Plane;
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
    }
}
