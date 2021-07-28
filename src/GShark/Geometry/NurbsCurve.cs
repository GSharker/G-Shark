#nullable enable
using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using GShark.Operation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public NurbsCurve() { }

        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="degree">The curve degree.</param>
        /// <param name="knots">The knots defining the curve.</param>
        /// <param name="controlPoints">The control points of the curve.</param>
        /// <param name="weights">The weight values.</param>
        public NurbsCurve(int degree, KnotVector knots, List<Point3> points, List<double>? weights = null)
        {
            if (points is null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            if (knots is null)
            {
                throw new ArgumentNullException(nameof(knots));
            }

            if (degree < 1)
            {
                throw new ArgumentException("Degree must be greater than 1!");
            }

            if (knots.Count != points.Count + degree + 1)
            {
                throw new ArgumentException("Number of points + degree + 1 must equal knots length!");
            }

            if (!knots.IsValid(degree, points.Count))
            {
                throw new ArgumentException("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            }

            Weights = weights ?? Sets.RepeatData(1.0, points.Count);
            Degree = degree;
            Knots = knots;
            LocationPoints = points;
            ControlPoints = LinearAlgebra.PointsHomogeniser(points, Weights);
        }

        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="points">The control points of the curve.</param>
        /// <param name="degree">The curve degree.</param>
        public NurbsCurve(List<Point3> points, int degree)
            : this(degree, new KnotVector(degree, points.Count), points)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        private NurbsCurve(NurbsCurve curve)
        {
            Degree = curve.Degree;
            LocationPoints = new List<Point3>(curve.LocationPoints);
            ControlPoints = new List<Point4>(curve.ControlPoints);
            Knots = new KnotVector(curve.Knots);
            Weights = new List<double>(curve.Weights!);
        }

        /// <summary>
        /// List of weight values.
        /// </summary>
        public List<double> Weights { get; }

        public int Degree { get; }

        public List<Point3> LocationPoints { get; }

        public List<Point4> ControlPoints { get; }

        public KnotVector Knots { get; }

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

        public BoundingBox BoundingBox
        {
            get
            {
                NurbsCurve curve = this;

                if (IsPeriodic())
                {
                    curve = ClampEnds();
                }

                List<Point3> pts = new List<Point3> { curve.LocationPoints[0] };
                List<ICurve> beziers = Modify.DecomposeCurveIntoBeziers(curve, true);
                foreach (ICurve crv in beziers)
                {
                    Extrema e = Evaluation.ComputeExtrema(crv);
                    pts.AddRange(e.Values.Select(eValue => crv.PointAt(eValue)));
                }

                pts.Add(curve.LocationPoints[curve.LocationPoints.Count - 1]);
                Point3[] removedDuplicate = Point3.CullDuplicates(pts, GeoSharkMath.MaxTolerance);
                return new BoundingBox(removedDuplicate);
            }
        }

        /// <summary>
        /// Checks if a NURBS curve is closed.<br/>
        /// A curve is closed if the first point and the last are the same.
        /// </summary>
        /// <returns>True if the curve is closed.</returns>
        public bool IsClosed()
        {
            return !(LocationPoints[0].DistanceTo(LocationPoints[LocationPoints.Count - 1]) > 0);
        }

        /// <summary>
        /// Checks if a NURBS curve is periodic.<br/>
        /// A curve is periodic, where the number of overlapping points is equal the curve degree.
        /// </summary>
        /// <returns>True if the curve is periodic.</returns>
        public bool IsPeriodic()
        {
            if (!Knots.IsKnotVectorPeriodic(Degree)) return false;
            int i, j = 0;
            for (i = 0, j = LocationPoints.Count - Degree; i < Degree; i++, j++)
            {
                if (LocationPoints[i].DistanceTo(LocationPoints[j]) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a copy of the curve.
        /// </summary>
        /// <returns>The copied curve.</returns>
        public NurbsCurve Clone()
        {
            return new NurbsCurve(this);
        }

        /// <summary>
        /// Creates a periodic NURBS curve.<br/>
        /// This method uses the control point wrapping solution.
        /// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-curve-closed.html
        /// </summary>
        /// <returns>A periodic NURBS curve.</returns>
        public NurbsCurve Close()
        {
            // Wrapping control points
            List<Point3> pts = new List<Point3>(LocationPoints);
            for (int i = 0; i < Degree; i++)
            {
                pts.Add(pts[i]);
            }

            KnotVector knots = KnotVector.UniformPeriodic(Degree, pts.Count);
            return new NurbsCurve(Degree, knots, pts);
        }

        /// <summary>
        /// Transforms a NURBS curve with the given transformation matrix.
        /// </summary>
        /// <param name="transformation">The transformation matrix.</param>
        /// <returns>A new NURBS curve transformed.</returns>
        public NurbsCurve Transform(Transform transformation)
        {
            List<Point3> pts = LocationPoints.Select(pt => pt.Transform(transformation)).ToList();
            return new NurbsCurve(Degree, Knots, pts, Weights!);
        }

        /// <summary>
        /// Computes a point at the given parameter.
        /// </summary>
        /// <param name="t">The parameter to sample the curve.</param>
        /// <returns>A point at the given parameter.</returns>
        public Point3 PointAt(double t)
        {
            return Evaluation.CurvePointAt(this, t);
        }

        /// <summary>
        /// Computes the curve tangent at the given parameter.
        /// </summary>
        /// <param name="t">The parameter to sample the curve.</param>
        /// <returns>The vector at the given parameter.</returns>
        public Vector3 TangentAt(double t)
        {
            return Evaluation.RationalCurveTangent(this, t);
        }

        /// <summary>
        /// Calculates the length of the curve.
        /// </summary>
        /// <returns>The length of the curve.</returns>
        public double Length()
        {
            return Analyze.CurveLength(this);
        }

        /// <summary>
        /// Reverses the parametrization of the curve.
        /// </summary>
        /// <returns>A reversed curve.</returns>
        public NurbsCurve Reverse()
        {
            return (NurbsCurve)Modify.ReverseCurve(this);
        }

        /// <summary>
        /// Computes the closest point on the curve to the given point.
        /// </summary>
        /// <param name="point">Point to analyze.</param>
        /// <returns>The closest point on the curve.</returns>
        public Point3 ClosestPoint(Point3 point)
        {
            return LinearAlgebra.PointDehomogenizer(Analyze.CurveClosestPoint(this, point, out _));
        }

        /// <summary>
        /// Computes the parameter along the curve which coincides with a given length.
        /// </summary>
        /// <param name="segmentLength">Length of segment to measure. Must be less than or equal to the length of the curve.</param>
        /// <param name="tolerance">If set less or equal 0.0, the tolerance used is 1e-10.</param>
        /// <returns></returns>
        public double ParameterAtLength(double segmentLength, double tolerance = -1.0)
        {
            return Analyze.CurveParameterAtLength(this, segmentLength, tolerance);
        }

        /// <summary>
        /// Computes the length curve which coincides with a given parameter t.
        /// </summary>
        /// <param name="t">The parameter at which to evaluate.</param>
        /// <returns>The length of the curve at the give parameter.</returns>
        public double LengthParameter(double t)
        {
            return Analyze.CurveLength(this, t);
        }

        /// <summary>
        /// Converts a NURBS curve where the knotVector is clamped.
        /// </summary>
        /// <returns>A NURBS curve with clamped knots.</returns>
        public NurbsCurve ClampEnds()
        {
            List<Point3> evalPts = new List<Point3>(LocationPoints);
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
        /// Compares if two NURBS curves are the same.<br/>
        /// Two NURBS curves are equal when the have same degree, same control points order and dimension, and same knots.
        /// </summary>
        /// <param name="other">The NURBS curve.</param>
        /// <returns>Return true if the NURBS curves are equal.</returns>
        public bool Equals(NurbsCurve? other)
        {
            List<Point3>? otherPts = other?.LocationPoints;

            if (other == null)
            {
                return false;
            }

            if (LocationPoints.Count != otherPts?.Count)
            {
                return false;
            }

            if (Knots.Count != other.Knots.Count)
            {
                return false;
            }

            if (LocationPoints.Where((t, i) => !t.Equals(otherPts[i])).Any())
            {
                return false;
            }

            return Degree == other.Degree && Knots.All(other.Knots.Contains) && Weights.All(other.Weights.Contains);
        }

        /// <summary>
        /// Compares if two NURBS curves are the same.<br/>
        /// Two NURBS curves are equal when the have same degree, same control points order and dimension, and same knots.
        /// </summary>
        /// <param name="obj">The curve object.</param>
        /// <returns>Return true if the NURBS curves are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is NurbsCurve curve)
                return Equals(curve);
            return false;
        }

        /// <summary>
        /// Implements the override method to string.
        /// </summary>
        /// <returns>The representation of a NURBS curve in string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string controlPts = string.Join("\n", LocationPoints.Select(first => $"({string.Join(",", first)})"));
            string knots = $"Knots = ({string.Join(",", Knots)})";
            string degree = $"CurveDegree = {Degree}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(knots);
            stringBuilder.AppendLine(degree);

            return stringBuilder.ToString();
        }
    }
}
