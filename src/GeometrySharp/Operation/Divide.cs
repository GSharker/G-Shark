using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometrySharp.Operation
{
    /// <summary>
    /// Divide provides various tools for dividing and splitting NURBS geometry.
    /// </summary>
    public class Divide
    {
        /// ToDo implement the async method.
		/// <summary>
		/// Split a curve into two parts at a given parameter.
		/// </summary>
		/// <param name="curve">The curve object.</param>
		/// <param name="u">The parameter where to split the curve.</param>
		/// <returns>Two new curves, defined by degree, knots, and control points.</returns>
		public static List<ICurve> SplitCurve(ICurve curve, double u)
        {
            int degree = curve.Degree;

            List<double> knotsToInsert = Sets.RepeatData(u, degree + 1);

            ICurve refinedCurve = Modify.CurveKnotRefine(curve, knotsToInsert);

            int s = curve.Knots.Span(degree, u);

            Knot knots0 = refinedCurve.Knots.ToList().GetRange(0, s + degree + 2).ToKnot();
            Knot knots1 = refinedCurve.Knots.GetRange(s + 1, refinedCurve.Knots.Count - (s + 1)).ToKnot();

            List<Vector3> controlPoints0 = refinedCurve.ControlPoints.GetRange(0, s + 1);
            List<Vector3> controlPoints1 = refinedCurve.ControlPoints.GetRange(s + 1, refinedCurve.ControlPoints.Count - (s + 1));

            return new List<ICurve> { new NurbsCurve(degree, knots0, controlPoints0), new NurbsCurve(degree, knots1, controlPoints1) };
        }

        /// ToDo implement the async method.
		/// <summary>
		/// Divides a curve for a given number of time, including the end points.
		/// The result is not split curves but a collection of t values and lengths that can be used for splitting.
		/// As with all arc length methods, the result is an approximation.
		/// </summary>
		/// <param name="curve">The curve object to divide.</param>
		/// <param name="divisions">The number of parts to split the curve into.</param>
		/// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
		public static (List<double> tValues, List<double> lengths) CurveByCount(ICurve curve, int divisions)
        {
            double approximatedLength = Analyze.CurveLength(curve);
            double arcLengthSeparation = approximatedLength / divisions;

            return CurveByLength(curve, arcLengthSeparation);
        }

        /// ToDo implement the async method.
		/// <summary>
		/// Divides a curve for a given length, including the end points.
		/// The result is not split curves but a collection of t values and lengths that can be used for splitting.
		/// As with all arc length methods, the result is an approximation.
		/// </summary>
		/// <param name="curve">The curve object to divide.</param>
		/// <param name="length">The length separating the resultant samples.</param>
		/// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
		public static (List<double> tValues, List<double> lengths) CurveByLength(ICurve curve, double length)
        {
            List<ICurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
            List<double> curveLengths = curves.Select(nurbsCurve => Analyze.BezierCurveLength(nurbsCurve)).ToList();
            double totalLength = curveLengths.Sum();

            List<double> tValues = new List<double> { curve.Knots[0] };
            List<double> divisionLengths = new List<double> { 0.0 };

            if (length > totalLength) return (tValues, divisionLengths);

            int i = 0;
            double sum = 0.0;
            double sum2 = 0.0;
            double segmentLength = length;


            while (i < curves.Count)
            {
                sum += curveLengths[i];

                while (segmentLength < sum + GeoSharpMath.EPSILON)
                {
                    double t = Analyze.BezierCurveParamAtLength(curves[i], segmentLength - sum2,
                        GeoSharpMath.MAXTOLERANCE, curveLengths[i]);

                    tValues.Add(t);
                    divisionLengths.Add(segmentLength);

                    segmentLength += length;
                }

                sum2 += curveLengths[i];
                i++;
            }

            return (tValues, divisionLengths);
        }

        /// <summary>
        /// Samples a curve along its domain.
        /// Following this algorithm, http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf.
        /// </summary>
        /// <param name="curve">The curve to sampling.</param>
        /// <param name="tol">The tolerance for the adaptive division.</param>
        /// <returns>A tuple collecting the parameter where it was sampled and the points.</returns>
        public static (List<double> tValues, List<Vector3> pts) CurveAdaptiveSample(ICurve curve, double tol = 1e-6)
        {
            if (curve.Degree != 1) return CurveAdaptiveSampleRange(curve, curve.Knots[0], curve.Knots[^1], tol);
            Knot copyKnot = new Knot(curve.Knots);
            copyKnot.RemoveAt(0);
            copyKnot.RemoveAt(copyKnot.Count - 1);
            return (copyKnot, curve.ControlPoints);
        }

        /// <summary>
        /// Samples a curve at 3 points, facilitating adaptive sampling.
        /// </summary>
        /// <param name="curve">The curve to sampling.</param>
        /// <param name="start">The start parameter for sampling.</param>
        /// <param name="end">The end parameter for sampling.</param>
        /// <param name="tol">The tolerance for the adaptive division.</param>
        /// <returns>A tuple collecting the parameter where it was sampled and the points.</returns>
        public static (List<double> tValues, List<Vector3> pts) CurveAdaptiveSampleRange(ICurve curve, double start, double end, double tol)
        {
            Random random = new Random();
            double tMiddle = start + (end - start) * (0.45 + 0.1 * random.NextDouble());
            Vector3 pt1 = Evaluation.CurvePointAt(curve, start);
            Vector3 pt2 = Evaluation.CurvePointAt(curve, tMiddle);
            Vector3 pt3 = Evaluation.CurvePointAt(curve, end);

            Vector3 diff0 = pt1 - pt3;
            Vector3 diff1 = pt1 - pt2;

            if ((Vector3.Dot(diff0, diff0) < tol && Vector3.Dot(diff1, diff1) > tol) ||
                !Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, tol))
            {

                tMiddle = start + (end - start) * (0.45 + 0.1 * random.NextDouble());

                (List<double> tValues, List<Vector3> pts) leftHalves = CurveAdaptiveSampleRange(curve, start, tMiddle, tol);
                (List<double> tValues, List<Vector3> pts) rightHalves = CurveAdaptiveSampleRange(curve, tMiddle, end, tol);

                int count = leftHalves.tValues.Count - 1;
                List<double> tMerged = leftHalves.tValues.GetRange(0, count).Concat(rightHalves.tValues).ToList();
                List<Vector3> ptsMerged = leftHalves.pts.GetRange(0, count).Concat(rightHalves.pts).ToList();
                return (tMerged, ptsMerged);
            }
            else
            {
                return (new List<double> { start, end }, new List<Vector3> { pt1, pt3 });
            }
        }
    }
}
