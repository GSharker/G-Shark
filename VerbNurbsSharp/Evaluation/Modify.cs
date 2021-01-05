﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Evaluation
{
	public class Modify
	{
		/// <summary>
		/// Insert a collectioin of knots on a curve
		/// corresponds to Algorithm A5.4 (Piegl & Tiller)
		/// </summary>
		/// <param name="curve"></param>
		/// <param name="knotsToInsert"></param>
		/// <returns></returns>
		public static NurbsCurveData CurveKnotRefine(NurbsCurveData curve, List<double> knotsToInsert)
		{
			if (knotsToInsert.Count == 0)
				return Make.ClonedCurve(curve);

			int degree = curve.Degree;
			List<Point> controlPoints = curve.ControlPoints;
			KnotArray knots = curve.Knots;

			int n = controlPoints.Count - 1;
			int m = n + degree + 1;
			int r = knotsToInsert.Count - 1;
			int a = Eval.KnotSpan(degree, knotsToInsert[0], knots);
			int b = Eval.KnotSpan(degree, knotsToInsert[r], knots);
			List<Point> controlPoints_post = new List<Point>();
			KnotArray knots_post = new KnotArray();

			//new control points
			for (int i = 0; i <= a - degree; i++)
				controlPoints_post[i] = controlPoints[i];

			for (int i = b - 1; i <= n; i++)
				controlPoints_post[i + r + 1] = controlPoints[i];

			//new knot vector
			for (int i = 0; i <= a; i++)
				knots_post[i] = knots[i];

			for (int i = b + degree; i <= m; i++)
				knots_post[i + r + 1] = knots[i];

			int ii = b + degree + 1;
			int k = b + degree + r;
			int j = r;
			while (j >= 0)
			{
				while (knotsToInsert[j] <= knots[ii] && ii > a)
				{
					controlPoints_post[k - degree - 1] = controlPoints[ii - degree - 1];
					knots_post[k] = knots[ii];
					k = k - 1;
					ii = ii - 1;
				}

				controlPoints_post[k - degree - 1] = controlPoints_post[k - degree];

				for (int i = 1; i <= degree; i++)
				{
					int ind = k - degree + 1;
					double alfa = knots_post[k + 1] - knotsToInsert[j];

					if (Math.Abs(alfa) < Constants.EPSILON)
						controlPoints_post[ind - 1] = controlPoints_post[ind];
					else
					{
						alfa = alfa / (knots_post[k + 1] - knots[ii - degree + 1]);
						controlPoints_post[ind - 1] = Vector.Add(Vector.Mul(alfa, controlPoints_post[ind - 1]), Vector.Mul((1.0-alfa), controlPoints_post[ind]));
					}
				}

				knots_post[k] = knotsToInsert[j];
				k = k - 1;
				j--;
			}


			return new NurbsCurveData(degree, knots_post, controlPoints_post);
		}

		private static int Imin(int a, int b)
		{
			return a < b ? a : b;
		}

		private static int Imax(int a, int b)
		{  
			return a > b? a : b;
		}

		//////////////////////////// =================================== not implemented yet ================================== ///////////////////

		/// <summary>
		/// Reverses the parameterization of a NURBS curve. The domain is unaffected.
		/// </summary>
		/// <param name="curve">A NURBS curve to be reversed</param>
		/// <returns>A new NURBS curve with a reversed parameterization</returns>
		public static NurbsCurveData CurveReverse(NurbsCurveData curve)
		{
			throw new NotImplementedException();
		}

		public static NurbsSurfaceData SurfaceReverse(NurbsSurfaceData surface, bool useV = false)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reverse a knot vector
		/// </summary>
		/// <param name="knots">An array of knots</param>
		/// <returns>The reversed array of knots</returns>
		public static KnotArray KnotsReverse(KnotArray knots)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Unify the knot vectors of a collection of NURBS curves. This can be used, for example, is used for lofting between curves.
		/// </summary>
		/// <param name="curves">An array of NURBS curves</param>
		/// <returns>A collection of NURBS curves, all with the same knot vector</returns>
		public static List<NurbsCurveData> UnifyCurveKnotVectors(List<NurbsCurveData> curves)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Elevate the degree of a NURBS curve
		/// </summary>
		/// <param name="curve">The curve to elevate</param>
		/// <param name="finalDegree">The expected final degree</param>
		/// <returns>The NURBS curve after degree elevation - if the supplied degree is <= the curve is returned unmodified</returns>
		public static NurbsCurveData CurveElevateDegree(NurbsCurveData curve, int finalDegree)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Transform a NURBS surface using a matrix
		/// </summary>
		/// <param name="surfaceData">The surface to transform</param>
		/// <param name="mat">The matrix to use for the transform - the dimensions should be the dimension of the surface + 1 in both directions</param>
		/// <returns>A new NURBS surface after transformation</returns>
		public static NurbsSurfaceData RationalSurfaceTransform(NurbsSurfaceData surfaceData, Matrix mat)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Transform a NURBS curve using a matrix
		/// </summary>
		/// <param name="curve">The curve to transform</param>
		/// <param name="mat">The matrix to use for the transform - the dimensions should be the dimension of the curve + 1 in both directions</param>
		/// <returns>A new NURBS surface after transformation</returns>
		public static NurbsCurveData RationalCurveTransform(NurbsCurveData curve, Matrix mat)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Perform knot refinement on a NURBS surface by inserting knots at various parameters
		/// </summary>
		/// <param name="surface">The surface to insert the knots into</param>
		/// <param name="knotsToInsert">The knots to insert - an array of parameter positions within the surface domain</param>
		/// <param name="useV">Whether to insert in the U direction or V direction of the surface</param>
		/// <returns>A new NURBS surface with the knots inserted</returns>
		public static NurbsSurfaceData SurfaceKnotRefine(NurbsSurfaceData surface, List<double> knotsToInsert, bool useV)
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
		public static List<NurbsCurveData> DecomposeCurveIntoBeziers(NurbsCurveData curve)
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
		public static NurbsCurveData CurveKnotInsert(NurbsCurveData curve, double u, int r)
		{
			throw new NotImplementedException();
		}
	}
}
