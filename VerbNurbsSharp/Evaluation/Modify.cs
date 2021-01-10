using System;
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
		/// Refine curve knot vector
		/// corresponds to Algorithm A5.4 (Piegl & Tiller)
		/// </summary>
		/// <param name="curve"></param>
		/// <param name="knotsToInsert"></param>
		/// <returns></returns>
		public static NurbsCurve CurveKnotRefine(NurbsCurve curve, List<double> knotsToInsert)
		{
			if (knotsToInsert.Count == 0)
				return curve;

			int degree = curve.Degree;
			List<Vector> controlPoints = curve.ControlPoints;
			KnotArray knots = curve.Knots;

			int n = controlPoints.Count - 1; 
			int m = n + degree + 1; //the number of curve segment
			int r = knotsToInsert.Count - 1; //number of knots
			int a = Eval.KnotSpan(degree, knotsToInsert[0], knots);
			int b = Eval.KnotSpan(degree, knotsToInsert[r], knots) + 1;

			int g = b + degree - 1;
			int k = b + degree + r;

			Vector[] controlPoints_post = new Vector[g];
			double[] knots_post = new double[knots.Count + knotsToInsert.Count];

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

			//int j = r;

			for (int j = r; j >= 0; j--)
			{
				while (knotsToInsert[j] <= knots[g] && g > a)
				{
					controlPoints_post[k - degree - 1] = controlPoints[g - degree - 1];
					knots_post[k] = knots[g];
					k--;
					g--;
				}

				controlPoints_post[k - degree - 1] = controlPoints_post[k - degree];

				for (int i = 1; i <= degree; i++)
				{
					int ind = k - degree + 1;
					double alfa = knots_post[k + i] - knotsToInsert[j];

					if (Math.Abs(alfa) < Constants.EPSILON)
						controlPoints_post[ind - 1] = controlPoints_post[ind];
					else
					{
						alfa = alfa / (knots_post[k + i] - knots[g - degree + i]);
						var cont_vect = Constants.Addition(Constants.Multiplication(controlPoints_post[ind - 1], alfa), Constants.Multiplication(controlPoints_post[ind], 1.0 - alfa));
						controlPoints_post[ind - 1] = new Vector() { cont_vect[0], cont_vect[1], cont_vect[2] };
					}
				}

				knots_post[k] = knotsToInsert[j];
				k--;
			}

			////while (j >= 0)
			////{
			////	while (knotsToInsert[j] <= knots[g] && g > a)
			////	{
			////		controlPoints_post[k - degree - 1] = controlPoints[g - degree - 1];
			////		knots_post[k] = knots[g];
			////		k--;
			////		g--;
			////	}

			////	controlPoints_post[k - degree - 1] = controlPoints_post[k - degree];

			////	for (int i = 1; i <= degree; i++)
			////	{
			////		int ind = k - degree + 1;
			////		double alfa = knots_post[k + 1] - knotsToInsert[j];

			////		if (Math.Abs(alfa) < Constants.EPSILON)
			////			controlPoints_post[ind - 1] = controlPoints_post[ind];
			////		else
			////		{
			////			alfa = alfa / (knots_post[k + 1] - knots[g - degree + 1]);
			////			controlPoints_post[ind - 1] =(Vector)Constants.Addition(Constants.Multiplication(controlPoints_post[ind - 1], alfa), Constants.Multiplication(controlPoints_post[ind], 1.0-alfa));
			////		}
			////	}

			////	knots_post[k] = knotsToInsert[j];
			////	k = k - 1;
			////	j--;
			////}

			KnotArray finalKnot = new KnotArray();
			foreach (double d in knots_post)
				finalKnot.Add(d);

			return new NurbsCurve(degree, finalKnot, controlPoints_post.ToList());
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
		public static KnotArray KnotsReverse(KnotArray knots)
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

		/// <summary>
		/// Transform a NURBS curve using a matrix
		/// </summary>
		/// <param name="curve">The curve to transform</param>
		/// <param name="mat">The matrix to use for the transform - the dimensions should be the dimension of the curve + 1 in both directions</param>
		/// <returns>A new NURBS surface after transformation</returns>
		public static NurbsCurve RationalCurveTransform(NurbsCurve curve, Matrix mat)
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
