using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using GShark.Geometry.Enum;

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
            List<Point4> controlPoints = curve.ControlPoints;
            KnotVector knots = curve.Knots;

            // Initialize common variables.
            int n = controlPoints.Count - 1;
            int m = n + degree + 1;
            int r = knotsToInsert.Count - 1;
            int a = knots.Span(degree, knotsToInsert[0]);
            int b = knots.Span(degree, knotsToInsert[r]);
            Point4[] controlPointsPost = new Point4[n + r + 2];
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
        /// Decompose a curve into a collection of bezier curves.<br/>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="normalize">Set as per default false, true normalize the knots between 0 to 1.</param>
        /// <returns>Collection of curve objects, defined by degree, knots, and control points.</returns>
        public static List<ICurve> DecomposeCurveIntoBeziers(ICurve curve, bool normalize = false)
        {
            int degree = curve.Degree;
            List<Point4> controlPoints = curve.ControlPoints;
            KnotVector knots = curve.Knots;

            // Find all of the unique knot values and their multiplicity.
            // For each, increase their multiplicity to degree + 1.
            Dictionary<double, int> knotMultiplicities = knots.Multiplicities();
            int reqMultiplicity = degree + 1;

            // Insert the knots.
            foreach (KeyValuePair<double, int> kvp in knotMultiplicities)
            {
                if (kvp.Value >= reqMultiplicity) continue;
                List<double> knotsToInsert = Sets.RepeatData(kvp.Key, reqMultiplicity - kvp.Value);
                NurbsCurve curveTemp = new NurbsCurve(degree, knots, controlPoints);
                ICurve curveResult = CurveKnotRefine(curveTemp, knotsToInsert);
                knots = curveResult.Knots;
                controlPoints = curveResult.ControlPoints;
            }

            int crvKnotLength = reqMultiplicity * 2;
            List<ICurve> curves = new List<ICurve>();
            int i = 0;

            while (i < controlPoints.Count)
            {

                KnotVector knotsRange = (normalize)
                    ? knots.GetRange(i, crvKnotLength).ToKnot().Normalize()
                    : knots.GetRange(i, crvKnotLength).ToKnot();
                List<Point4> ptsRange = controlPoints.GetRange(i, reqMultiplicity);

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
            List<Point4> controlPts = new List<Point4>(curve.ControlPoints);
            controlPts.Reverse();

            KnotVector knots = KnotVector.Reverse(curve.Knots);

            return new NurbsCurve(curve.Degree, knots, controlPts);
        }

        /// <summary>
        /// Performs a knot refinement on a NURBS surface by inserting knots at various parameters.<br/>
        /// <em>Implementation of Algorithm A5.5 of The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface object to insert the knots.</param>
        /// <param name="knotsToInsert">The set of knots to insert.</param>
        /// <param name="direction">Whether to insert in the U or V direction of the surface.</param>
        /// <returns>A NURBS surface with the knots inserted.</returns>
        public static NurbsSurface SurfaceKnotRefine(NurbsSurface surface, List<double> knotsToInsert, SurfaceDirection direction)
        {
            List<List<Point4>> modifiedControlPts = new List<List<Point4>>();
            List<List<Point4>> controlPts = surface.ControlPoints;
            KnotVector knots = surface.KnotsV;
            int degree = surface.DegreeV;

            if (direction != SurfaceDirection.V)
            {
                controlPts = Sets.Reverse2DMatrixData(surface.ControlPoints);
                knots = surface.KnotsU;
                degree = surface.DegreeU;
            }

            ICurve curve = null;
            foreach (List<Point4> pts in controlPts)
            {
                curve = CurveKnotRefine(new NurbsCurve(degree, knots, pts), knotsToInsert);
                modifiedControlPts.Add(curve.ControlPoints);
            }

            if (curve == null)
            {
                throw new Exception("The refinement was not be able to be completed. A problem occur refining the internal curves.");
            }

            if (direction != SurfaceDirection.V)
            {
                var reversedControlPts = Sets.Reverse2DMatrixData(modifiedControlPts);
                return new NurbsSurface(surface.DegreeU, surface.DegreeV, curve.Knots, surface.KnotsV.Copy(), reversedControlPts);
            }

            return new NurbsSurface(surface.DegreeU, surface.DegreeV, surface.KnotsU.Copy(), curve.Knots, modifiedControlPts);
        }

        /// <summary>
        /// Elevates the degree of a NURBS curve.
        /// <em>Implementation of Algorithm A5.9 of The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The object curve to elevate.</param>
        /// <param name="finalDegree">The expected final degree. If the supplied degree is less or equal the curve is returned unmodified.</param>
        /// <returns>The curve after degree elevation.</returns>
        public static ICurve ElevateDegree(ICurve curve, int finalDegree)
        {
            if (finalDegree <= curve.Degree)
            {
                return curve;
            }

            int n = curve.Knots.Count - curve.Degree - 2;
            int p = curve.Degree;
            KnotVector U = curve.Knots;
            List<Point4> Pw = curve.ControlPoints;
            int t = finalDegree - curve.Degree; // Degree elevate a curve t times.


            // local arrays.
            double[,] bezalfs = new double[p + t + 1, p + 1];
            Point4[] bpts = new Point4[p + 1];
            Point4[] ebpts = new Point4[p + t + 1];
            Point4[] nextbpts = new Point4[p - 1];
            double[] alphas = new double[p - 1];

            int m = n + p + 1;
            int ph = finalDegree;
            int ph2 = (int) Math.Floor((double) (ph / 2));

            // Output values;
            List<Point4> Qw = new List<Point4>();
            KnotVector Uh = new KnotVector();

            // Compute Bezier degree elevation coefficients.
            bezalfs[0, 0] = bezalfs[ph, p] = 1.0;
            for (int i = 1; i <= ph2; i++)
            {
                double inv = 1.0 / LinearAlgebra.GetBinomial(ph, i);
                int mpi = Math.Min(p, i);

                for (int j = Math.Max(0, i - t); j <= mpi; j++)
                {
                    bezalfs[i, j] = inv * LinearAlgebra.GetBinomial(p, j) * LinearAlgebra.GetBinomial(t, i - j);
                }
            }

            for (int i = ph2 + 1; i <= ph-1; i++)
            {
                int mpi = Math.Min(p, i);
                for (int j = Math.Max(0, i - t); j <= mpi; j++)
                {
                    bezalfs[i, j] = bezalfs[ph - i, p - j];
                }
            }

            int mh = ph;
            int kind = ph + 1;
            int r = -1;
            int a = p;
            int b = p + 1;
            int cind = 1;
            double ua = U[0];
            Qw.Add(Pw[0]);

            for (int i = 0; i <=ph; i++)
            {
                Uh.Add(ua);
            }

            // Initialize first Bezier segment.
            for (int i = 0; i <= p; i++)
            {
                bpts[i] = Pw[i];
            }

            // Big loop thru knot vector.
            while (b < m)
            {
                int i = b;
                while (b < m && Math.Abs(U[b] - U[b+1]) < GeoSharkMath.Epsilon)
                {
                    b += 1;
                }

                int mul = b - i + 1;
                mh = mh + mul + t;
                double ub = U[b];
                int oldr = r;
                r = p - mul;

                // Insert knot U[b] r times.
                // Checks for integer arithmetic.
                int lbz = (oldr > 0) ? (int) Math.Floor((double) ((2 + oldr) / 2)) : 1;
                int rbz = (r > 0) ? (int)Math.Floor((double)(ph - (r + 1) / 2)) : ph;

                if (r > 0)
                {
                    // Inserts knot to get Bezier segment.
                    double numer = ub - ua;
                    for (int k = p; k > mul; k--)
                    {
                        alphas[k - mul - 1] = (numer / (U[a + k] - ua));
                    }
                    for (int j = 1; j <= r; j++)
                    {
                        int save = r - j;
                        int s = mul + j;
                        for (int k = p; k >= s; k--)
                        {
                            bpts[k] = Point4.Interpolate(bpts[k], bpts[k - 1], alphas[k - s]);
                        }
                        nextbpts[save] = bpts[p];
                    }
                }

                // End of insert knot.
                // Degree elevate Bezier.
                for (int j = lbz; j <= ph; j++)
                {
                    ebpts[j] = Point4.Zero;
                    int mpi = Math.Min(p, j);
                    for (int k = Math.Max(0, j - t); k <= mpi; k++)
                    {
                        ebpts[j] += bpts[k] * bezalfs[j, k];
                    }
                }

                if (oldr > 1)
                {
                    // Must remove knot u=U[a] oldr times.
                    int first = kind - 2;
                    int last = kind;
                    double den = ub - ua;
                    double bet = (ub - Uh[kind - 1]) / den;
                    for (int tr = 1; tr < oldr; tr++)
                    {
                        // Knot removal loop.
                        int ii = first;
                        int jj = last;
                        int kj = jj - kind + 1;

                        while (jj - ii > tr)
                        {
                            // Loop and compute the new control points for one removal step.
                            if (ii < cind)
                            {
                                double alf = (ub - Uh[ii]) / (ua - Uh[ii]);
                                Qw.Add(Point4.Interpolate(Qw[ii], Qw[ii - 1], alf));
                            }

                            if (jj >= lbz)
                            {
                                if (jj - tr <= kind - ph + oldr)
                                {
                                    double gam = (ub - Uh[jj - tr]) / den;
                                    ebpts[kj] = Point4.Interpolate(ebpts[kj], ebpts[kj + 1], gam);
                                }
                                else
                                {
                                    ebpts[kj] = Point4.Interpolate(ebpts[kj], ebpts[kj + 1], bet);
                                }
                            }

                            ii += 1;
                            jj -= 1;
                            kj -= 1;
                        }

                        first -= 1;
                        last += 1;
                    }
                }

                // End of removing knot, n = U[a].
                if (a != p)
                {
                    // Load the knot ua.
                    for (int j = 0; j < ph - oldr; j++)
                    {
                        Uh.Add(ua);
                        kind += 1;
                    }
                }

                for (int j = lbz; j <= rbz; j++)
                {
                    // Load control points into Qw.
                    Qw.Add(ebpts[j]);
                    cind += 1;
                }

                if (b < m)
                {
                    // Set up for the next pass thru loop.
                    for (int j = 0; j < r; j++)
                    {
                        bpts[j] = nextbpts[j];
                    }

                    for (int j = r; j <= p; j++)
                    {
                        bpts[j] = Pw[b - p + j];
                    }

                    a = b;
                    b += 1;
                    ua = ub;
                }
                else
                {
                    // End knot.
                    for (int j = 0; j <= ph; j++)
                    {
                        Uh.Add(ub);
                    }
                }
            }

            return new NurbsCurve(finalDegree, Uh, Qw);
        }
    }
}
