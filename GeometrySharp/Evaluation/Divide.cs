using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeometrySharp.Geometry;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;

namespace GeometrySharp.Evaluation
{
	/// <summary>
	/// Divide provides various tools for dividing and splitting NURBS geometry.
	/// </summary>
	public class Divide
	{
		/// <summary>
		/// Split a NURBS curve into two parts at a given parameter.
		/// </summary>
		/// <param name="curve">NurbsCurveData object representing the curve.</param>
		/// <param name="u">The parameter where to split the curve.</param>
		/// <returns>Two new curves, defined by degree, knots, and control points.</returns>
		public static List<NurbsCurve> CurveSplit(NurbsCurve curve, double u)
		{
			int degree = curve.Degree;
            Knot knots = curve.Knots;

            var knotsToInsert = Sets.RepeatData(u, degree + 1);

			NurbsCurve refinedCurve = Modify.CurveKnotRefine(curve, knotsToInsert);

		 	int s = knots.Span(degree, u);

			var knots0 = refinedCurve.Knots.ToList().GetRange(0, s + degree + 2).ToKnot();
            var knots1 = refinedCurve.Knots.GetRange(s + 1, refinedCurve.Knots.Count - (s + 1)).ToKnot();

			var controlPoints0 = refinedCurve.ControlPoints.GetRange(0, s + 1);
			var controlPoints1 = refinedCurve.ControlPoints.GetRange(s + 1, refinedCurve.ControlPoints.Count - (s + 1));

			return new List<NurbsCurve>() { new NurbsCurve(degree, knots0, controlPoints0), new NurbsCurve(degree, knots1, controlPoints1) };
		}

		/// <summary>
		/// Divide a NURBS curve given a given number of times, including the end points.
		/// The result is not split curves but a collection of t values and lengths that can be used for splitting.
		/// As with all arc length methods, the result is an approximation.
		/// </summary>
		/// <param name="curve">NurbsCurve object to divide.</param>
		/// <param name="divisions">The number of parts to split the curve into.</param>
		/// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
		public static (List<double> tValues, List<double> lengths) RationalCurveByEqualLength(NurbsCurve curve, int divisions)
        {
            var curves = Modify.DecomposeCurveIntoBeziers(curve);
            var curveLengths = curves.Select(nurbsCurve => Analyze.RationalBezierCurveLength(nurbsCurve)).ToList();
            var totalLength = curveLengths.Sum();

			var tValues = new List<double> {curve.Knots[0]};
			var divisionLengths = new List<double> {0.0};

            var approximatedLength = Analyze.RationalCurveArcLength(curve);
            var arcLengthSeparation = approximatedLength / divisions;

            if (arcLengthSeparation > totalLength) return (tValues, divisionLengths);

            var i = 0;
            var sum = 0.0;
            var sum2 = 0.0;
            var segmentLength = arcLengthSeparation;


			while (i < curves.Count)
            {
                sum += curveLengths[i];

                while (segmentLength < sum + GeoSharpMath.EPSILON)
                {
                    var t = Analyze.RationalBezierCurveParamAtLength(curves[i], segmentLength - sum2,
                        GeoSharpMath.MAXTOLERANCE, curveLengths[i]);

					tValues.Add(t);
					divisionLengths.Add(segmentLength);

                    segmentLength += arcLengthSeparation;
                }

                sum2 += curveLengths[i];
                i++;
            }

			return (tValues, divisionLengths);
		}
    }
}
