using GeometrySharp.Core;
using GeometrySharp.Geometry.Interfaces;
using GeometrySharp.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// Represents the value of a plane, two angles (interval) and a radius (radiance).
    /// </summary>
    public class Arc : Curve, IEquatable<Arc>, ITransformable<Arc>
    {
        /// <summary>
        /// Initializes an arc from a plane, a radius and an angle domain expressed as an interval.
        /// </summary>
        /// <param name="plane">Base plane.</param>
        /// <param name="radius">Radius value.</param>
        /// <param name="angleDomain">Interval defining the angle of the arc. Interval should be between 0.0 to 2Pi</param>
        public Arc(Plane plane, double radius, Interval angleDomain)
        {
            if (angleDomain.Max < angleDomain.Min)
            {
                throw new Exception("Angle domain must never be decreasing.");
            }

            Plane = plane;
            Radius = radius;
            AngleDomain = (angleDomain.Length > Math.PI * 2.0)
                ? new Interval(AngularDiff(angleDomain.Min, Math.PI * 2.0), AngularDiff(angleDomain.Max, Math.PI * 2.0))
                : angleDomain;

            ToNurbsCurve();
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
        /// </summary>
        /// <param name="pt1">Start point of the arc.</param>
        /// <param name="pt2">Interior point on arc.</param>
        /// <param name="pt3">End point of the arc.</param>
        public Arc(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            Vector3 center = Trigonometry.PointAtEqualDistanceFromThreePoints(pt1, pt2, pt3);
            Vector3 normal = Vector3.ZAxis.PerpendicularTo(pt1, pt2, pt3);
            Vector3 xDir = pt1 - center;
            Vector3 yDir = Vector3.Cross(normal, xDir);
            Plane pl = new Plane(center, xDir, yDir, normal);

            (double u, double v) = pl.ClosestParameters(pt3);
            double angle = Math.Atan2(v, u);

            if (angle < 0.0)
            {
                angle += 2 * Math.PI;
            }

            Plane = pl;
            Radius = xDir.Length();
            AngleDomain = new Interval(0.0, angle);
            ToNurbsCurve();
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
        /// Gets the angle domain of this arc.
        /// The domain is in radians.
        /// </summary>
        public Interval AngleDomain { get; }

        /// <summary>
        /// Calculates the length of the arc.
        /// </summary>
        public double Length => Math.Abs(Angle * Radius);

        /// <summary>
        /// Gets the start point of the arc.
        /// </summary>
        public Vector3 StartPoint => ControlPoints[0];

        /// <summary>
        /// Gets the mid-point of the arc.
        /// </summary>
        public Vector3 MidPoint => PointAt(AngleDomain.Mid);

        /// <summary>
        /// Gets the end point of the arc.
        /// </summary>
        public Vector3 EndPoint => ControlPoints[^1];

        /// <summary>
        /// Gets the BoundingBox of this arc.
        /// https://stackoverflow.com/questions/1336663/2d-bounding-box-of-a-sector
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get
            {
                Plane orientedPlane = Plane.Align(Vector3.XAxis);
                Vector3 pt0 = StartPoint;
                Vector3 pt1 = EndPoint;
                Vector3 ptC = orientedPlane.Origin;

                double theta0 = Math.Atan2(pt0[1] - ptC[1], pt0[0] - ptC[0]);
                double theta1 = Math.Atan2(pt1[1] - ptC[1], pt1[0] - ptC[0]);

                List<Vector3> pts = new List<Vector3>{ pt0, pt1 };

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
        }

        /// <summary>
        /// Creates an arc defined by a start point, end point and a direction at the first point.
        /// </summary>
        /// <param name="ptStart">Start point arc.</param>
        /// <param name="ptEnd">End point arc.</param>
        /// <param name="dir">TangentAt direction at start.</param>
        /// <returns>An arc.</returns>
        public static Arc ByStartEndDirection(Vector3 ptStart, Vector3 ptEnd, Vector3 dir)
        {
            Vector3 vec0 = dir.Unitize();
            Vector3 vec1 = (ptEnd - ptStart).Unitize();
            if (vec1.Length().Equals(0.0))
            {
                throw new Exception("Points must not be coincident.");
            }

            Vector3 vec2 = (vec0 + vec1).Unitize();
            Vector3 vec3 = vec2 * ( 0.5 * ptStart.DistanceTo(ptEnd) / Vector3.Dot(vec2, vec0));
            return new Arc(ptStart, ptStart + vec3, ptEnd);
        }

        /// <summary>
        /// Returns the point at the parameter t on the arc.
        /// </summary>
        /// <param name="t">A parameter between 0.0 to 1.0 or between the angle domain.></param>
        /// <returns>Point on the arc.</returns>
        public override Vector3 PointAt(double t)
        {
            Vector3 xDir = Plane.XAxis * Math.Cos(t) * Radius;
            Vector3 yDir = Plane.YAxis * Math.Sin(t) * Radius;

            return Plane.Origin + xDir + yDir;
        }

        /// <summary>
        /// Returns the tangent at the parameter t on the arc.
        /// </summary>
        /// <param name="t">A parameter between 0.0 to 1.0 or between the angle domain.</param>
        /// <returns>Tangent vector at the t parameter.</returns>
        public Vector3 TangentAt(double t)
        {
            double r1 = Radius;
            double r2 = Radius;

            r1 *= -Math.Sin(t);
            r2 *= Math.Cos(t);

            Vector3 vector = Plane.XAxis * r1 + Plane.YAxis * r2;

            return vector.Unitize();
        }

        /// <summary>
        /// Calculates the point on an arc that is close to a test point.
        /// </summary>
        /// <param name="pt">The test point. Point to get close to.</param>
        /// <returns>The point on the arc that is close to the test point.</returns>
        public override Vector3 ClosestPt(Vector3 pt)
        {
            double twoPi = 2.0 * Math.PI;

            (double u, double v) = Plane.ClosestParameters(pt);
            if (Math.Abs(u) < GeoSharpMath.MAXTOLERANCE && Math.Abs(v) < GeoSharpMath.MAXTOLERANCE)
            {
                return PointAt(0.0);
            }

            double t = Math.Atan2(v, u);
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

            return PointAt(AngleDomain.Min + t);
        }

        /// <summary>
        /// Applies a transformation to the plane where the arc is on.
        /// </summary>
        /// <param name="transformation">Transformation matrix to apply.</param>
        /// <returns>A transformed arc.</returns>
        public Arc Transform(Transform transformation)
        {
            Plane plane = Plane.Transform(transformation);
            Interval angleDomain = new Interval(AngleDomain.Min, AngleDomain.Max);

            return new Arc(plane, Radius, angleDomain);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this arc.
        /// Implementation of Algorithm A7.1 from The NURBS Book by Piegl & Tiller.
        /// </summary>
        /// <returns>A nurbs curve shaped like this arc.</returns>
        private void ToNurbsCurve()
        {
            Vector3 axisX = Plane.XAxis;
            Vector3 axisY = Plane.YAxis;
            int numberOfArc;
            Vector3[] ctrPts;
            double[] weights;

            // Number of arcs.
            double piNum = 0.5 * Math.PI;
            if (Angle <= piNum)
            {
                numberOfArc = 1;
                ctrPts = new Vector3[3];
                weights = new double[3];
            }
            else if (Angle <= piNum * 2)
            {
                numberOfArc = 2;
                ctrPts = new Vector3[5];
                weights = new double[5];
            }
            else if (Angle <= piNum * 3)
            {
                numberOfArc = 3;
                ctrPts = new Vector3[7];
                weights = new double[7];
            }
            else
            {
                numberOfArc = 4;
                ctrPts = new Vector3[9];
                weights = new double[9];
            }

            double detTheta = Angle / numberOfArc;
            double weight1 = Math.Cos(detTheta / 2);
            Vector3 p0 = Center + (axisX * (Radius * Math.Cos(AngleDomain.Min)) + axisY * (Radius * Math.Sin(AngleDomain.Min)));
            Vector3 t0 = axisY * Math.Cos(AngleDomain.Min) - axisX * Math.Sin(AngleDomain.Min);

            Knot knots = new Knot(Sets.RepeatData(0.0, ctrPts.Length + 3));
            int index = 0;
            double angle = AngleDomain.Min;

            ctrPts[0] = p0;
            weights[0] = 1.0;

            for (int i = 1; i < numberOfArc + 1; i++)
            {
                angle += detTheta;
                Vector3 p2 = Center + (axisX * (Radius * Math.Cos(angle)) + axisY * (Radius * Math.Sin(angle)));
                
                weights[index + 2] = 1;
                ctrPts[index + 2] = p2;

                Vector3 t2 = (axisY * Math.Cos(angle)) - (axisX * Math.Sin(angle));
                Line ln0 = new Line(p0, t0.Unitize() + p0);
                Line ln1 = new Line(p2, t2.Unitize() + p2);
                bool intersect = Intersect.LineLine(ln0, ln1, out _, out _, out double u0, out double u1);
                Vector3 p1 = p0 + (t0 * u0);

                weights[index + 1] = weight1;
                ctrPts[index + 1] = p1;
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
                    knots[3] = knots[4] = (double) 1 / 3;
                    knots[5] = knots[6] = (double) 2 / 3;
                    break;
                case 4:
                    knots[3] = knots[4] = 0.25;
                    knots[5] = knots[6] = 0.5;
                    knots[7] = knots[8] = 0.75;
                    break;
            }

            Degree = 2;
            Knots = knots;
            HomogenizedPoints = LinearAlgebra.PointsHomogeniser(ctrPts.ToList(), weights.ToList());
        }

        /// <summary>
        /// Determines whether the arc is equal to another arc.
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

            return Math.Abs(Radius - other.Radius) < GeoSharpMath.MAXTOLERANCE &&
                   Math.Abs(Angle - other.Angle) < GeoSharpMath.MAXTOLERANCE &&
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
            return $"Arc(R:{Radius} - A:{GeoSharpMath.ToDegrees(Angle)})";
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
