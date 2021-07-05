using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Contains many fundamental algorithms for working with NURBS.<br/>
    /// These include algorithms for:<br/>
    /// knot insertion, knot refinement, degree elevation, re-parametrization.<br/>
    /// Many of these algorithms owe their implementation to The NURBS Book by Piegl and Tiller.
    /// </summary>
    public class Modify
    {
        /// <summary>
		/// Inserts a collection of knots on a curve.<br/>
		/// <em>Implementation of Algorithm A5.4 of The NURBS Book by Piegl and Tiller.</em>
		/// </summary>
		/// <param name="curve">The curve object.</param>
		/// <param name="knotsToInsert">The set of knots.</param>
		/// <returns>A curve with refined knots.</returns>
		public static ICurve CurveKnotRefine(ICurve curve, List<double> knotsToInsert)
        {
            if (knotsToInsert.Count == 0)
                return curve;

            int degree = curve.Degree;
            List<Point3> controlPoints = curve.ControlPoints;
            KnotVector knots = curve.Knots;

            // Initialize common variables.
            int n = controlPoints.Count - 1;
            int m = n + degree + 1;
            int r = knotsToInsert.Count - 1;
            int a = knots.Span(degree, knotsToInsert[0]);
            int b = knots.Span(degree, knotsToInsert[r]);
            Point3[] controlPointsPost = new Point3[n + r + 2];
            double[] knotsPost = new double[m + r + 2];

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

                for (int l = 1; l < degree + 1; l++)
                {
                    int ind = k - degree + l;
                    double alfa = knotsPost[k + l] - knotsToInsert[j];

                    if (Math.Abs(alfa) < GeoSharkMath.Epsilon)
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
        /// Decompose a curve into a collection of bezier's.<br/>
        /// Useful as each Bezier curve fits into it's convex hull.<br/>
        /// This is a useful starting point for intersection, closest point, divide and conquer algorithms.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="normalize">Set as per default false, true normalize the knots between 0 to 1.</param>
        /// <returns>Collection of curve objects, defined by degree, knots, and control points.</returns>
        public static List<ICurve> DecomposeCurveIntoBeziers(ICurve curve, bool normalize = false)
        {
            int degree = curve.Degree;
            List<Point3> controlPoints = curve.ControlPoints;
            KnotVector knots = curve.Knots;

            // Find all of the unique knot values and their multiplicity.
            // For each, increase their multiplicity to degree + 1.
            Dictionary<double, int> knotMultiplicities = knots.Multiplicities();
            int reqMultiplicity = degree + 1;

            // Insert the knots.
            foreach ((double key, int value) in knotMultiplicities)
            {
                if (value < reqMultiplicity)
                {
                    List<double> knotsToInsert = Sets.RepeatData(key, reqMultiplicity - value);
                    NurbsCurve curveTemp = new NurbsCurve(degree, knots, controlPoints);
                    ICurve curveResult = CurveKnotRefine(curveTemp, knotsToInsert);
                    knots = curveResult.Knots;
                    controlPoints = curveResult.ControlPoints;
                }
            }

            int crvKnotLength = reqMultiplicity * 2;
            List<ICurve> curves = new List<ICurve>();
            int i = 0;

            while (i < controlPoints.Count)
            {

                KnotVector knotsRange = (normalize)
                    ? KnotVector.Normalize(knots.GetRange(i, crvKnotLength).ToKnot())
                    : knots.GetRange(i, crvKnotLength).ToKnot();
                List<Point3> ptsRange = controlPoints.GetRange(i, reqMultiplicity);

                NurbsCurve tempCrv = new NurbsCurve(degree, knotsRange, ptsRange);
                curves.Add(tempCrv);
                i += reqMultiplicity;
            }

            return curves;
        }

        /// <summary>
        /// Reverses the parametrization of a curve.<br/>
        /// The domain is unaffected.
        /// </summary>
        /// <param name="curve">The curve has to be reversed.</param>
        /// <returns>A curve with a reversed parametrization.</returns>
        public static ICurve ReverseCurve(ICurve curve)
        {
            List<Point3> pts = new List<Point3>(curve.ControlPoints);
            pts.Reverse();

            List<double> weights = LinearAlgebra.GetWeights(curve.HomogenizedPoints);
            weights.Reverse();

            KnotVector knots = KnotVector.Reverse(curve.Knots);

            return new NurbsCurve(curve.Degree, knots, pts, weights);
        }

        ///// <summary>
        ///// Performs knot refinement on a NURBS surface by inserting knots at various parameters.
        ///// </summary>
        ///// <param name="nurbsSurface">The surface to insert the knots into.</param>
        ///// <param name="knots">The knots to insert - an array of parameter positions within the surface domain.</param>
        ///// <param name="useU">Whether to insert in the U direction or V direction of the surface. U is default.</param>
        ///// <returns>A NURBS surface with the knot refined.</returns>
        //public static NurbsSurface SurfaceKnotRefine(NurbsSurface nurbsSurface, KnotVector knotsToInsert, bool useU = true)
        //{
        //    List<List<Point3d>> ctrlPts;
        //    List<List<Point3d>> refinedPts = new List<List<Point3d>>();
        //    KnotVector knots;
        //    int degree;

        //    //u dir
        //    if (useU)
        //    {
        //        ctrlPts = nurbsSurface.ControlPoints;
        //        degree = nurbsSurface.DegreeU;
        //        knots = nurbsSurface.KnotsU;
        //    }
        //    //v dir
        //    else
        //    {
        //        //Reverse the points matrix
        //        ctrlPts = Sets.Reverse2DMatrixPoints(nurbsSurface.ControlPoints);
        //        degree = nurbsSurface.DegreeV;
        //        knots = nurbsSurface.KnotsV;
        //    }

        //    //Do knot refinement on every row
        //    ICurve crv = new NurbsCurve();
        //    foreach (List<Point3d> cptRow in ctrlPts)
        //    {
        //        crv = CurveKnotRefine(new NurbsCurve(degree, knots, cptRow), knotsToInsert);
        //        refinedPts.Add(crv.ControlPoints);
        //    }

        //    KnotVector newKnots = crv.Knots;
        //    if (useU)
        //        return new NurbsSurface(nurbsSurface.DegreeU, nurbsSurface.DegreeV, newKnots, nurbsSurface.KnotsV, Sets.Reverse2DMatrixPoints(refinedPts));
        //    else
        //        return new NurbsSurface(nurbsSurface.DegreeU, nurbsSurface.DegreeV, nurbsSurface.KnotsU, newKnots, refinedPts);

        //}
    }
}
