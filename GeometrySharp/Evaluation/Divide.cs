using System;
using System.Collections.Generic;
using System.Linq;
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

	}
}
