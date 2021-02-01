using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
	/// <summary>
	/// Modify contains many fundamental algorithms for working with NURBS. These include algorithms for:
	/// knot insertion, knot refinement, degree elevation, reparameterization.
    /// Many of these algorithms owe their implementation to Piegl & Tiller's, "The NURBS Book".
	/// </summary>
	public class Modify
	{
        /// <summary>
		/// Insert a collection of knots on a curve.
		/// Implementation of Algorithm A5.4 of The NURBS Book by Piegl & Tiller, 2nd Edition.
		/// </summary>
		/// <param name="curve">The NurbsCurve.</param>
		/// <param name="knotsToInsert">The set of Knots.</param>
		/// <returns>NurbsCurve.</returns>
		public static NurbsCurve CurveKnotRefine(NurbsCurve curve, List<double> knotsToInsert)
		{
			if (knotsToInsert.Count == 0)
				return new NurbsCurve(curve);

			int degree = curve.Degree;
			List<Vector3> controlPoints = curve.ControlPoints;
			Knot knots = curve.Knots;

			// Initialize common variables.
			int n = controlPoints.Count - 1;
			int m = n + degree + 1;
			int r = knotsToInsert.Count - 1;
			int a = knots.Span(degree, knotsToInsert[0]);
			int b = knots.Span(degree, knotsToInsert[r]);
			Vector3[] controlPointsPost = new Vector3[n+r+2];
			double[] knotsPost = new double[m+r+2];

			// New control points.
            for (int i = 0; i < a - degree + 1; i++)
                controlPointsPost[i] = controlPoints[i];
            for (int i = b - 1; i < n + 1; i++)
				controlPointsPost[i + r + 1] = controlPoints[i];

			// New knot vector.
			for (int i = 0; i < a + 1; i++)
                knotsPost[i] = knots[i];
            for (int i = b + degree; i < m + 1; i++)
                knotsPost[i + r + 1] = knots[i];

			// Initialize variables for knot refinement.
			int g = b + degree - 1;
            int k = b + degree + r;
            int j = r;

			// Apply knot refinement.
			while (j >= 0)
            {
                while (knotsToInsert[j] <= knots[g] && g > a)
                {
                    controlPointsPost[k - degree - 1] = controlPoints[g - degree - 1];
                    knotsPost[k] = knots[g];
                    --k;
                    --g;
                }

                controlPointsPost[k - degree - 1] = controlPointsPost[k - degree];

                for (int l = 1; l < degree+1; l++)
                {
                    int ind = k - degree + l;
                    double alfa = knotsPost[k + l] - knotsToInsert[j];

                    if (Math.Abs(alfa) < GeoSharpMath.EPSILON)
                        controlPointsPost[ind - 1] = controlPointsPost[ind];
                    else
                    {
                        alfa /= (knotsPost[k + l] - knots[g - degree + l]);
                        controlPointsPost[ind - 1] = (controlPointsPost[ind - 1] * alfa) + (controlPointsPost[ind] * (1.0 - alfa));
                    }
                }
                knotsPost[k] = knotsToInsert[j];
                --k;
                --j;
            }
            return new NurbsCurve(degree, knotsPost.ToKnot(), controlPointsPost.ToList());
        }

		/// <summary>
		/// Decompose a NURBS curve into a collection of bezier's.  Useful
		/// as each bezier fits into it's convex hull.  This is a useful starting
		/// point for intersection, closest point, divide & conquer algorithms
		/// </summary>
		/// <param name="curve">NurbsCurveData object representing the curve</param>
		/// <returns>List of NurbsCurveData objects, defined by degree, knots, and control points</returns>
		public static List<NurbsCurve> DecomposeCurveIntoBeziers(NurbsCurve curve)
		{
			var degree = curve.Degree;
			var controlPoints = curve.ControlPoints;
			var knots = curve.Knots;

			// Find all of the unique knot values and their multiplicity.
			// For each, increase their multiplicity to degree + 1.
			var knotMultiplicities = knots.Multiplicities();
			var reqMultiplicity = degree + 1;

			// Insert the knots.
			foreach (var (key, value) in knotMultiplicities)
			{
                if (value < reqMultiplicity)
                {
                    var knotsToInsert = Sets.RepeatData(key, reqMultiplicity - value);
                    var curveTemp = new NurbsCurve(degree, knots, controlPoints);
                    var curveResult = CurveKnotRefine(curveTemp, knotsToInsert);
                    knots = curveResult.Knots;
                    controlPoints = curveResult.ControlPoints;
				}
            }

			var crvKnotLength = reqMultiplicity * 2;
			var curves = new List<NurbsCurve>();
			var i = 0;

			while (i < controlPoints.Count)
			{
				var knotsRange = knots.GetRange(i, crvKnotLength).ToKnot();
				var ptsRange = controlPoints.GetRange(i, reqMultiplicity);

				var tempCrv = new NurbsCurve(degree, knotsRange, ptsRange);
				curves.Add(tempCrv);
				i += reqMultiplicity;
			}

			return curves;
		}

        /// <summary>
        /// Transform a NURBS curve using a matrix.
        /// </summary>
        /// <param name="curve">The curve to transform.</param>
        /// <param name="mat">The matrix to use for the transform - the dimensions should be the dimension of the curve + 1 in both directions.</param>
        /// <returns>A new NURBS surface after transformation.</returns>
        public static NurbsCurve RationalCurveTransform(NurbsCurve curve, Matrix mat)
        {
            var pts = curve.ControlPoints;
            for (int i = 0; i < pts.Count; i++)
            {
                var pt = pts[i];
				pt.Add(1.0);
                pts[i] = Matrix.Dot(mat, pt).Take(pt.Count - 1).ToVector();
            }

            return new NurbsCurve(curve.Degree, curve.Knots, pts, curve.Weights!);
        }
    }
}
