using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
    public class Modify
	{
		// ToDo make the test.
		/// <summary>
		/// Insert a collection of knots on a curve.
		/// corresponds to Algorithm A5.4 (Piegl & Tiller)
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

			int n = controlPoints.Count - 1;
			int m = n + degree + 1;
			int r = knotsToInsert.Count - 1;
			int a = Eval.KnotSpan(degree, knotsToInsert[0], knots);
			int b = Eval.KnotSpan(degree, knotsToInsert[r], knots);
			Vector3[] controlPointsPost = new Vector3[n+r+2];
			double[] knotsPost = new double[m+r+2];
			//new control points
            for (int i = 0; i < a - degree + 1; i++)
                controlPointsPost[i] = controlPoints[i];
            for (int i = b - 1; i < n + 1; i++)
				controlPointsPost[i + r + 1] = controlPoints[i];

			//new knot vector
			for (int i = 0; i < a + 1; i++)
                knotsPost[i] = knots[i];

			for (int i = b + degree; i < m + 1; i++)
                knotsPost[i + r + 1] = knots[i];

            int g = b + degree - 1;
            int k = b + degree + r;
            int j = r;
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

                    if (System.Math.Abs(alfa) < GeoSharpMath.EPSILON)
                        controlPointsPost[ind - 1] = controlPointsPost[ind];
                    else
                    {
                        alfa = alfa / (knotsPost[k + l] - knots[g - degree + l]);
                        controlPointsPost[ind - 1] = (controlPointsPost[ind - 1] * alfa) + (controlPointsPost[ind] * (1.0 - alfa));
                    }
                }
                knotsPost[k] = knotsToInsert[j];
                --k;
                --j;
            }
            return new NurbsCurve(degree, knotsPost.ToKnot(), controlPointsPost.ToList());
        }

		private static int Imin(int a, int b)
		{
			return a < b ? a : b;
		}

		private static int Imax(int a, int b)
		{
			return a > b ? a : b;
		}


		//////////////////////////// =================================== not implemented yet ================================== ///////////////////

		/// <summary>
		/// Reverses the parameterization of a NURBS curve. The domain is unaffected.
		/// </summary>
		/// <param name="curve">A NURBS curve to be reversed</param>
		/// <returns>A new NURBS curve with a reversed parameterization</returns>
		public static NurbsCurve CurveReverse(NurbsCurve curve)
		{
			throw new NotImplementedException();
		}

		public static NurbsSurface SurfaceReverse(NurbsSurface surface, bool useV = false)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reverse a knot vector
		/// </summary>
		/// <param name="knots">An array of knots</param>
		/// <returns>The reversed array of knots</returns>
		public static Knot KnotsReverse(Knot knots)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Unify the knot vectors of a collection of NURBS curves. This can be used, for example, is used for lofting between curves.
		/// </summary>
		/// <param name="curves">An array of NURBS curves</param>
		/// <returns>A collection of NURBS curves, all with the same knot vector</returns>
		public static List<NurbsCurve> UnifyCurveKnotVectors(List<NurbsCurve> curves)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Elevate the degree of a NURBS curve
		/// </summary>
		/// <param name="curve">The curve to elevate</param>
		/// <param name="finalDegree">The expected final degree</param>
		/// <returns>The NURBS curve after degree elevation - if the supplied degree is <= the curve is returned unmodified</returns>
		public static NurbsCurve CurveElevateDegree(NurbsCurve curve, int finalDegree)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Transform a NURBS surface using a matrix
		/// </summary>
		/// <param name="surfaceData">The surface to transform</param>
		/// <param name="mat">The matrix to use for the transform - the dimensions should be the dimension of the surface + 1 in both directions</param>
		/// <returns>A new NURBS surface after transformation</returns>
		public static NurbsSurface RationalSurfaceTransform(NurbsSurface surfaceData, Matrix mat)
		{
			throw new NotImplementedException();
		}

		//ToDo this method has to be tested.
        /// <summary>
        /// Transform a NURBS curve using a matrix.
        /// </summary>
        /// <param name="curve">The curve to transform.</param>
        /// <param name="mat">The matrix to use for the transform - the dimensions should be the dimension of the curve + 1 in both directions.</param>
        /// <returns>A new NURBS surface after transformation.</returns>
        public static NurbsCurve RationalCurveTransform(NurbsCurve curve, Matrix mat)
        {
            var pts = curve.ControlPoints;
			var weights = Eval.Weight1d(curve.ControlPoints);

            if (!curve.AreControlPointsHomogenized())
            {
                pts = Eval.Homogenize1d(curve.ControlPoints);
                weights = Sets.RepeatData(1.0, curve.ControlPoints.Count);
            }

            var controlPtsTransformed = pts.Select(pt => Matrix.Dot(mat, pt).Take(pt.Count - 1).ToVector()).ToList();
            var homogenizePts = Eval.Homogenize1d(controlPtsTransformed, weights);
            return new NurbsCurve(curve.Degree, curve.Knots, homogenizePts);
        }

        /// <summary>
        /// Perform knot refinement on a NURBS surface by inserting knots at various parameters
        /// </summary>
        /// <param name="surface">The surface to insert the knots into</param>
        /// <param name="knotsToInsert">The knots to insert - an array of parameter positions within the surface domain</param>
        /// <param name="useV">Whether to insert in the U direction or V direction of the surface</param>
        /// <returns>A new NURBS surface with the knots inserted</returns>
        public static NurbsSurface SurfaceKnotRefine(NurbsSurface surface, List<double> knotsToInsert, bool useV)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Insert a knot along a rational curve.  Note that this algorithm only works
		/// for r + s <= degree, where s is the initial multiplicity (number of duplicates) of the knot.
		/// </summary>
		/// <param name="curve"></param>
		/// <param name="u"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static NurbsCurve CurveKnotInsert(NurbsCurve curve, double u, int r)
		{
			throw new NotImplementedException();
		}
	}
}
