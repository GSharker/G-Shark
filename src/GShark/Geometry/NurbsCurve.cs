#nullable enable
using GShark.Core;
using GShark.Operation;
using GShark.Operation.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using GShark.Interfaces;

namespace GShark.Geometry
{
    /// <summary>
    /// This class represents a NURBS curve.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Data/NurbsCurveCollection.cs?name=example)]
    /// </example>
    public class NurbsCurve : ICurve, IEquatable<NurbsCurve>, ITransformable<NurbsCurve>
    {
        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="degree">The curve degree.</param>
        /// <param name="knots">The knots defining the curve.</param>
        /// <param name="controlPoints">The controlPoints of the curve.</param>
        internal NurbsCurve(int degree, KnotVector knots, List<Point4> controlPoints)
        {
            if (controlPoints is null)
            {
                throw new ArgumentNullException(nameof(controlPoints));
            }

            if (knots is null)
            {
                throw new ArgumentNullException(nameof(knots));
            }

            if (degree < 1)
            {
                throw new ArgumentException("Degree must be greater than 1!");
            }

            if (knots.Count != controlPoints.Count + degree + 1)
            {
                throw new ArgumentException("Number of controlPoints + degree + 1 must equal knots length!");
            }

            if (!knots.IsValid(degree, controlPoints.Count))
            {
                throw new ArgumentException("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            }

            Weights = Point4.GetWeights(controlPoints);
            Degree = degree;
            Knots = knots;
            ControlPointLocations = Point4.PointDehomogenizer1d(controlPoints);
            ControlPoints = controlPoints;
        }

        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="points">The points of the curve.</param>
        /// <param name="degree">The curve degree.</param>
        public NurbsCurve(List<Point3>? points, int degree)
            : this(degree, new KnotVector(degree, points!.Count), points.Select(p => new Point4(p)).ToList())
        {
        }

        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="points">The points of the curve.</param>
        /// <param name="weights">The weights of each point.</param>
        /// <param name="degree">The curve degree.</param>
        public NurbsCurve(List<Point3>? points, List<double> weights, int degree)
            : this(degree, new KnotVector(degree, points!.Count), points.Select((p, i) => new Point4(p, weights[i])).ToList())
        {
        }

        /// <summary>
        /// Gets the list of weight values.
        /// </summary>
        public List<double> Weights { get; }

        /// <summary>
        /// Gets the degree of the curve.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// Gets the control points in their rational form. 
        /// </summary>
        public List<Point3> ControlPointLocations { get; }

        /// <summary>
        /// Gets the control points in their homogenized form.
        /// </summary>
        public List<Point4> ControlPoints { get; }

        /// <summary>
        /// Gets the knots vectors of the curve.
        /// </summary>
        public KnotVector Knots { get; }

        public double Length => Analyze.CurveLength(this);

        public Interval Domain
        {
            get
            {
                if (IsPeriodic())
                {
                    return new Interval(Knots[Degree], Knots[Knots.Count - Degree - 1]);
                }
                return new Interval(Knots[0], Knots[Knots.Count - 1]);
            }
        }

        public Point3 StartPoint => PointAt(0.0);

        public Point3 MidPoint => PointAt(0.5);

        public Point3 EndPoint => PointAt(1.0);

        public BoundingBox GetBoundingBox()
        {
            NurbsCurve curve = this;

            if (IsPeriodic())
            {
                curve = ClampEnds();
            }

            List<Point3> pts = new List<Point3> { curve.ControlPointLocations[0] };
            List<NurbsCurve> beziers = Modify.DecomposeCurveIntoBeziers(curve, true);
            foreach (NurbsCurve crv in beziers)
            {
                Extrema e = Evaluation.ComputeExtrema(crv);
                pts.AddRange(e.Values.Select(eValue => crv.PointAt(eValue)));
            }

            pts.Add(curve.ControlPointLocations[curve.ControlPointLocations.Count - 1]);
            Point3[] removedDuplicate = Point3.CullDuplicates(pts, GSharkMath.MinTolerance);
            return new BoundingBox(removedDuplicate);
        }

        /// <summary>
        /// Checks if a curve is closed.<br/>
        /// A curve is closed if the first point and the last are the same.
        /// </summary>
        /// <returns>True if the curve is closed.</returns>
        public bool IsClosed()
        {
            Point3 pt0 = Evaluation.CurvePointAt(this,Domain.T0);
            Point3 pt1 = Evaluation.CurvePointAt(this, Domain.T1);
            return pt0.EpsilonEquals(pt1, GSharkMath.Epsilon);
        }

        /// <summary>
        /// Checks if a curve is periodic.<br/>
        /// A curve is periodic, where the number of overlapping points is equal the curve degree.
        /// </summary>
        /// <returns>True if the curve is periodic.</returns>
        public bool IsPeriodic()
        {
            if (!Knots.IsPeriodic(Degree)) return false;
            int i, j;
            for (i = 0, j = ControlPointLocations.Count - Degree; i < Degree; i++, j++)
            {
                if (ControlPointLocations[i].DistanceTo(ControlPointLocations[j]) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a periodic curve.<br/>
        /// This method uses the control point wrapping solution.
        /// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-curve-closed.html
        /// </summary>
        /// <returns>A periodic NURBS curve.</returns>
        public NurbsCurve Close()
        {
            // Wrapping control points
            List<Point4> pts = new List<Point4>(ControlPoints);
            for (int i = 0; i < Degree; i++)
            {
                pts.Add(pts[i]);
            }

            KnotVector knots = KnotVector.UniformPeriodic(Degree, pts.Count);
            return new NurbsCurve(Degree, knots, pts);
        }

        /// <summary>
        /// Transforms a curve with the given transformation matrix.
        /// </summary>
        /// <param name="transformation">The transformation matrix.</param>
        /// <returns>A new NURBS curve transformed.</returns>
        public NurbsCurve Transform(Transform transformation)
        {
            List<Point4> pts = ControlPoints.Select(pt => pt.Transform(transformation)).ToList();
            return new NurbsCurve(Degree, Knots, pts);
        }

        public Point3 PointAt(double t)
        {
            return Evaluation.CurvePointAt(this, t);
        }

        public Point3 PointAtLength(double length, bool normalized = true)
        {
            double parameter = Analyze.CurveParameterAtLength(this, length);
            return Evaluation.CurvePointAt(this, parameter);
        }

        /// <summary>
        /// Computes the curve tangent at the given parameter.
        /// </summary>
        /// <param name="t">The parameter to sample the curve. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The vector at the given parameter.</returns>
        public Vector3 TangentAt(double t)
        {
            return Evaluation.RationalCurveTangent(this, t);
        }

        /// <summary>
        /// Determines the derivatives of a curve at a given parameter.<br/>
        /// </summary>
        /// <param name="t">Parameter on the curve at which the point is to be evaluated. Parameter should be between 0.0 and 1.0.</param>
        /// <param name="numberOfDerivatives">The number of derivatives required.</param>
        /// <returns>The derivatives.</returns>
        public List<Vector3> DerivativeAt(double t, int numberOfDerivatives = 1)
        {
            return Evaluation.RationalCurveDerivatives(this, t, numberOfDerivatives);
        }

        /// <summary>
        /// Computes the curvature vector of the curve at the parameter t.
        /// The vector has length equal to the radius of the curvature circle and with direction to the center of the circle.
        /// </summary>
        /// <param name="t">Evaluation parameter. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The curvature vector.</returns>
        public Vector3 CurvatureAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            List<Vector3> derivatives = Evaluation.RationalCurveDerivatives(this, t, 2);
            return Analyze.Curvature(derivatives[1], derivatives[2]);
        }

        /// <summary>
        /// Calculates the 3D plane at the given parameter.
        /// Defined as the Frenet frame, is constructed from the velocity and the acceleration of the curve.
        /// https://janakiev.com/blog/framing-parametric-curves/
        /// </summary>
        /// <param name="t">Evaluation parameter. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The perpendicular frame.</returns>
        public Plane FrameAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            List<Vector3> derivatives = Evaluation.RationalCurveDerivatives(this, t, 2);

            Vector3 normal = (derivatives[2].Length == 0.0) 
                ? Vector3.PerpendicularTo(derivatives[1])
                : Analyze.Curvature(derivatives[1], derivatives[2]);

            Vector3 yDir = Vector3.CrossProduct(derivatives[1], normal);
            return new Plane(derivatives[0], normal, yDir);
        }

        /// <summary>
        /// Reverses the parametrization of the curve.
        /// </summary>
        /// <returns>A reversed curve.</returns>
        public NurbsCurve Reverse()
        {
            return (NurbsCurve)Modify.ReverseCurve(this);
        }

        public Point3 ClosestPoint(Point3 point)
        {
            return Point4.PointDehomogenizer(Analyze.CurveClosestPoint(this, point, out _));
        }

        public double ClosestParameter(Point3 pt)
        {
            return Analyze.CurveClosestParameter(this, pt);
        }

        /// <summary>
        /// Computes the parameter along the curve which coincides with a given length.
        /// </summary>
        /// <param name="segmentLength">Length of segment to measure. Must be less than or equal to the length of the curve.</param>
        /// <param name="tolerance">If set less or equal 0.0, the tolerance used is 1e-10.</param>
        /// <returns>The parameter on the curve at the given length.</returns>
        public double ParameterAtLength(double segmentLength, double tolerance = -1.0)
        {
            return Analyze.CurveParameterAtLength(this, segmentLength, tolerance);
        }

        public double LengthAt(double t)
        {
            return Analyze.CurveLength(this, t);
        }

        /// <summary>
        /// Converts a curve where the knotVector is clamped.
        /// </summary>
        /// <returns>A curve with clamped knots.</returns>
        public NurbsCurve ClampEnds()
        {
            List<Point4> evalPts = new List<Point4>(ControlPoints);
            KnotVector clampedKnots = new KnotVector(Knots);
            int j = 2;

            while (j-- > 0)
            {
                Evaluation.DeBoor(ref evalPts, clampedKnots, Degree, clampedKnots[Degree]);
                for (int i = 0; i < Degree; i++)
                {
                    clampedKnots[i] = clampedKnots[Degree];
                }
                evalPts.Reverse();
                clampedKnots.Reverse();
            }

            return new NurbsCurve(Degree, clampedKnots, evalPts);
        }

        /// <summary>
        /// Compares two curves for equality.<br/>
        /// Two NURBS curves are equal when the have same control points, weights, knots and degree.
        /// </summary>
        /// <param name="other">The other curve.</param>
        /// <returns>Return true if the curves are equal.</returns>
        public bool Equals(NurbsCurve? other)
        {
            if (other == null)
            {
                return false;
            }

            if (!ControlPointLocations.SequenceEqual(other.ControlPointLocations))
            {
                return false;
            }

            if (!Knots.SequenceEqual(other.Knots))
            {
                return false;
            }

            if (Degree != other.Degree)
            {
                return false;
            }

            return Weights.SequenceEqual(other.Weights);
        }

        /// <summary>
        /// Compares if two curves are the same.<br/>
        /// Two curves are equal when the have same degree, same control points order and dimension, and same knots.
        /// </summary>
        /// <param name="obj">The curve object.</param>
        /// <returns>Return true if the curves are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is NurbsCurve curve)
                return Equals(curve);
            return false;
        }

        /// <summary>
        /// Implements the override method to string.
        /// </summary>
        /// <returns>The representation of a curve in string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string controlPts = string.Join("\n", ControlPointLocations.Select(first => $"({string.Join(",", first)})"));
            string knots = $"Knots = ({string.Join(",", Knots)})";
            string degree = $"CurveDegree = {Degree}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(knots);
            stringBuilder.AppendLine(degree);

            return stringBuilder.ToString();
        }

        public override int GetHashCode()
        {
            var sBldr = new StringBuilder();
            sBldr.Append(Degree);
            sBldr.Append(Knots);

            foreach (var ptStr in ControlPointLocations.Select(p => p.ToString().ToList()))
            {
                sBldr.Append(ptStr);
            }

            foreach (var wtStr in Weights.Select(w => w.ToString(CultureInfo.InvariantCulture).ToList()))
            {
                sBldr.Append(wtStr);
            }

            return sBldr.ToString().GetHashCode();
        }
    }
}
