#nullable enable
using GeometrySharp.Core;
using GeometrySharp.Geometry.Interfaces;
using GeometrySharp.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.ExtendedMethods;

namespace GeometrySharp.Geometry
{
    //ToDo: BoundingBox has to be implemented properly.
    /// <summary>
    /// A NURBS curve - this class represents the base class of many curve types and provides tools for analysis and evaluation.
    /// This object is deliberately constrained to be immutable. The methods deliberately return copies.
    /// /// </summary>
    public class NurbsCurve : ICurve, IEquatable<NurbsCurve>, ITransformable<NurbsCurve>
    {
        /// <summary>
        /// Creates a Nurbs curve object.
        /// </summary>
        /// <param name="degree">ICurve degree.</param>
        /// <param name="knots">Knot defining the curve.</param>
        /// <param name="controlPoints">Control points, as a collection of Vector3.</param>
        /// <param name="weights">Weight values, as a collection of doubles.</param>
        public NurbsCurve(int degree, Knot knots, List<Vector3> controlPoints, List<double>? weights = null)
        {
            if (controlPoints is null)
            {
                throw new ArgumentNullException(nameof(ControlPoints));
            }

            if (knots is null)
            {
                throw new ArgumentNullException(nameof(Knots));
            }

            if (degree < 1)
            {
                throw new ArgumentException("Degree must be greater than 1!");
            }

            if (knots.Count != controlPoints.Count + degree + 1)
            {
                throw new ArgumentException("Number of points + degree + 1 must equal knots length!");
            }

            if (!knots.AreValidKnots(degree, controlPoints.Count))
            {
                throw new ArgumentException("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            }

            Weights = weights ?? Sets.RepeatData(1.0, controlPoints.Count);
            Degree = degree;
            Knots = knots;
            ControlPoints = controlPoints;
            HomogenizedPoints = LinearAlgebra.PointsHomogeniser(controlPoints, Weights);
            BoundingBox = new BoundingBox(controlPoints);
        }

        /// <summary>
        /// Creates a nurbs curve object.
        /// </summary>
        /// <param name="controlPoints">Control points, as a collection of Vector3.</param>
        /// <param name="degree">Curve degree.</param>
        public NurbsCurve(List<Vector3> controlPoints, int degree)
            : this(degree, new Knot(degree, controlPoints.Count), controlPoints)
        {
        }

        /// <summary>
        /// Construct a NurbsCurve by a NurbsCurve object.
        /// </summary>
        /// <param name="curve">The curve object</param>
        private NurbsCurve(NurbsCurve curve)
        {
            Degree = curve.Degree;
            ControlPoints = new List<Vector3>(curve.ControlPoints);
            HomogenizedPoints = new List<Vector3>(curve.HomogenizedPoints);
            Knots = new Knot(curve.Knots);
            Weights = new List<double>(curve.Weights!);
            BoundingBox = new BoundingBox(ControlPoints);
        }

        /// <summary>
        /// List of weight values.
        /// </summary>
        public List<double> Weights { get; }

        public int Degree { get; }

        public List<Vector3> ControlPoints { get; }

        public List<Vector3> HomogenizedPoints { get; }

        public Knot Knots { get; }

        public Interval Domain => new Interval(Knots.First(), Knots.Last());

        public BoundingBox BoundingBox { get; }

        /// <summary>
        /// Obtain a copy of the NurbsCurve.
        /// </summary>
        /// <returns>The copied curve.</returns>
        public NurbsCurve Clone()
        {
            return new NurbsCurve(this);
        }

        /// ToDo implement the async method.
        /// <summary>
        /// Transform a curve with the given matrix.
        /// </summary>
        /// <param name="transformation">The transformation matrix.</param>
        /// <returns>A new curve transformed.</returns>
        public NurbsCurve Transform(Transform transformation)
        {
            var pts = new List<Vector3>(ControlPoints);

            for (int i = 0; i < pts.Count; i++)
            {
                var pt = pts[i];
                pt.Add(1.0);
                pts[i] = (pt * transformation).Take(pt.Count - 1).ToVector();
            }

            return new NurbsCurve(Degree, Knots, pts, Weights!);
        }

        /// ToDo implement the async method.
        /// <summary>
        /// Sample a point at the given parameter.
        /// </summary>
        /// <param name="t">The parameter to sample the curve.</param>
        /// <returns>A point at the given parameter.</returns>
        public Vector3 PointAt(double t)
        {
            return LinearAlgebra.PointDehomogenizer(Evaluation.CurvePointAt(this, t));
        }

        /// <summary>
        /// Obtain the curve tangent at the given parameter.
        /// </summary>
        /// <param name="t">The parameter to sample the curve.</param>
        /// <returns>The vector at the given parameter.</returns>
        /// ToDo implement the async method.
        public Vector3 TangentAt(double t)
        {
            return Evaluation.RationalCurveTangent(this, t);
        }

        /// ToDo implement the async method.
        /// <summary>
        /// Determine the arc length of the curve.
        /// </summary>
        /// <returns>The length of the curve.</returns>
        public double Length()
        {
            return Analyze.CurveLength(this);
        }

        /// ToDo implement the async method.
        /// <summary>
        /// Reverse the parametrization of the curve.
        /// </summary>
        /// <returns>A reversed curve.</returns>
        public NurbsCurve Reverse()
        {
            return (NurbsCurve) Modify.ReverseCurve(this);
        }

        /// <summary>
        /// Compute the closest point on the curve to the given point.
        /// </summary>
        /// <param name="point">Point to analyze.</param>
        /// <param name="t">Parameter of local closest point.</param>
        /// <returns>The closest point on the curve.</returns>
        /// ToDo implement the async method.
        public Vector3 ClosestPt(Vector3 point)
        {
            return Analyze.CurveClosestPoint(this, point, out _);
        }

        /// <summary>
        /// Gets the parameter along the curve which coincides with a given length along the curve.
        /// </summary>
        /// <param name="segmentLength">Length of segment to measure. Must be less than or equal to the length of the curve.</param>
        /// <param name="tolerance">If set less or equal 0.0, the tolerance used is 1e-10.</param>
        /// <returns></returns>
        public double ParameterAtLength(double segmentLength, double tolerance = -1.0)
        {
            return Analyze.CurveParameterAtLength(this, segmentLength, tolerance);
        }

        /// <summary>
        /// Get the length curve which coincides with a given parameter t.
        /// </summary>
        /// <param name="t">The parameter at which to evaluate.</param>
        /// <returns>The length of the curve at the give parameter t.</returns>
        public double LengthParameter(double t)
        {
            return Analyze.CurveLength(this, t);
        }

        /// <summary>
        /// Compare if two NurbsCurves are the same.
        /// Two NurbsCurve are equal when the have same degree, same control points order and dimension, and same knots.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Return true if the NurbsCurves are equal.</returns>
        public bool Equals(NurbsCurve? other)
        {
            List<Vector3>? pts = ControlPoints;
            List<Vector3>? otherPts = other?.ControlPoints;

            if (other == null)
            {
                return false;
            }

            if (pts.Count != otherPts.Count)
            {
                return false;
            }

            if (Knots.Count != other.Knots.Count)
            {
                return false;
            }

            if (pts.Where((t, i) => !t.Equals(otherPts[i])).Any())
            {
                return false;
            }

            return Degree == other.Degree && Knots.All(other.Knots.Contains) && Weights.All(other.Weights.Contains);
        }

        /// <summary>
        /// Implement the override method to string.
        /// </summary>
        /// <returns>The representation of a NurbsCurve in string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string controlPts = string.Join("\n", ControlPoints.Select(first => $"({string.Join(",", first)})"));
            string knots = $"Knots = ({string.Join(",", Knots)})";
            string degree = $"CurveDegree = {Degree}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(knots);
            stringBuilder.AppendLine(degree);

            return stringBuilder.ToString();
        }
    }
}
