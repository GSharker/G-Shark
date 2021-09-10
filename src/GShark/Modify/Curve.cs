using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using GShark.Operation;

namespace GShark.Modify
{
    /// <summary>
    /// Contains many fundamental algorithms for modifying the properties of a NURBS curve.<br/>
    /// These include algorithms for: knot insertion, knot refinement, degree elevation, re-parametrization.<br/>
    /// </summary>
    public static class Curve
    {
        /// <summary>
        /// Inserts a collection of knots on a curve.<br/>
        /// <em>Implementation of Algorithm A5.4 of The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="knotsToInsert">The set of knots.</param>
        /// <returns>A curve with refined knots.</returns>
        internal static NurbsBase KnotRefine(NurbsBase curve, IList<double> knotsToInsert)
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

                    if (Math.Abs(alfa) < GSharkMath.Epsilon)
                        controlPointsPost[ind - 1] = controlPointsPost[ind];
                    else
                    {
                        alfa /= (knotsPost[k + l] - knots[g - degree + l]);
                        controlPointsPost[ind - 1] = (controlPointsPost[ind - 1] * alfa) +
                                                     (controlPointsPost[ind] * (1.0 - alfa));
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
        internal static List<NurbsBase> DecomposeIntoBeziers(NurbsBase curve, bool normalize = false)
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
                List<double> knotsToInsert = CollectionHelpers.RepeatData(kvp.Key, reqMultiplicity - kvp.Value);
                NurbsBase curveTemp = new NurbsCurve(degree, knots, controlPoints);
                NurbsBase curveResult = KnotRefine(curveTemp, knotsToInsert);
                knots = curveResult.Knots;
                controlPoints = curveResult.ControlPoints;
            }

            int crvKnotLength = reqMultiplicity * 2;
            List<NurbsBase> curves = new List<NurbsBase>();
            int i = 0;

            while (i < controlPoints.Count)
            {

                KnotVector knotsRange = (normalize)
                    ? knots.GetRange(i, crvKnotLength).ToKnot().Normalize()
                    : knots.GetRange(i, crvKnotLength).ToKnot();
                List<Point4> ptsRange = controlPoints.GetRange(i, reqMultiplicity);

                NurbsBase tempCrv = new NurbsCurve(degree, knotsRange, ptsRange);
                curves.Add(tempCrv);
                i += reqMultiplicity;
            }

            return curves;
        }

        /// <summary>
        /// Elevates the degree of a curve.
        /// <em>Implementation of Algorithm A5.9 of The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The object curve to elevate.</param>
        /// <param name="finalDegree">The expected final degree. If the supplied degree is less or equal the curve is returned unmodified.</param>
        /// <returns>The curve after degree elevation.</returns>
        internal static NurbsBase ElevateDegree(NurbsBase curve, int finalDegree)
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
            int ph2 = (int)Math.Floor((double)(ph / 2));

            // Output values;
            List<Point4> Qw = new List<Point4>();
            KnotVector Uh = new KnotVector();

            // Compute Bezier degree elevation coefficients.
            bezalfs[0, 0] = bezalfs[ph, p] = 1.0;
            for (int i = 1; i <= ph2; i++)
            {
                double inv = 1.0 / GSharkMath.GetBinomial(ph, i);
                int mpi = Math.Min(p, i);

                for (int j = Math.Max(0, i - t); j <= mpi; j++)
                {
                    bezalfs[i, j] = inv * GSharkMath.GetBinomial(p, j) * GSharkMath.GetBinomial(t, i - j);
                }
            }

            for (int i = ph2 + 1; i <= ph - 1; i++)
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

            for (int i = 0; i <= ph; i++)
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
                while (b < m && Math.Abs(U[b] - U[b + 1]) < GSharkMath.Epsilon)
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
                int lbz = (oldr > 0) ? (int)Math.Floor((double)((2 + oldr) / 2)) : 1;
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
                    }
                }

                for (int j = lbz; j <= rbz; j++)
                {
                    // Load control points into Qw.
                    Qw.Add(ebpts[j]);
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

        /// <summary>
        /// Bezier degree reduction.
        /// <em>Refer to The NURBS Book by Piegl and Tiller at page 220.</em>
        /// </summary>
        private static double BezierDegreeReduce(Point4[] bpts, int degree, out Point4[] rbpts)
        {
            // Eq. 5.40
            int r = (int)Math.Floor(((double)degree - 1) / 2);

            Point4[] P = new Point4[degree];
            P[0] = bpts[0];
            P[P.Length - 1] = bpts.Last();

            bool isDegreeOdd = degree % 2 != 0;

            int r1 = (isDegreeOdd) ? r - 1 : r;

            // Eq. 5.41
            for (int i = 1; i <= r1; i++)
            {
                double alphaI = (double)i / degree;
                P[i] = (bpts[i] - (P[i - 1] * alphaI)) / (1 - alphaI);
            }

            for (int i = degree - 2; i >= r + 1; i--)
            {
                double alphaI = (double)(i + 1) / degree;
                P[i] = (bpts[i + 1] - (P[i + 1] * (1 - alphaI))) / alphaI;
            }

            /* Equations (5.43) and (5.44) express the parametric error {distance between points at corresponding parameter values);
               the maximums of geometric and parametric errors are not necessarily at the same u value.
               For p even the maximum error occurs at u = 1/2; for p odd the error is zero at u = 1/2, and it has two peaks a bit to the left and
               right of u = 1/2. */

            // Eq. 5.43 p even.
            List<double> Br = Evaluation.BasisFunction(degree - 1, new KnotVector(degree - 1, P.Length), 0.5);
            double parametricError = bpts[r + 1].DistanceTo((P[r] + P[r + 1]) * 0.5) * Br[r + 1];

            // Eq. 5.42
            if (isDegreeOdd)
            {
                double alphaR = (double)r / degree;
                Point4 PLeft = (bpts[r] - (P[r - 1] * alphaR)) / (1 - alphaR);

                double alphaR1 = (double)(r + 1) / degree;
                Point4 PRight = (bpts[r + 1] - (P[r + 1] * (1 - alphaR1))) / alphaR1;

                P[r] = (PLeft + PRight) * 0.5;
                // Eq. 5.44 p odd.
                parametricError = ((1 - alphaR) * 0.5) * ((Br[r] - Br[r + 1]) * PLeft.DistanceTo(PRight));
            }

            rbpts = P;
            return parametricError;
        }

        /// <summary>
        /// Reduce the degree of a NURBS curve.
        /// </summary>
        /// <param name="curve">The object curve to elevate.</param>
        /// <param name="tolerance">Tolerance value declaring if the curve is degree reducible.</param>
        /// <returns>The curve after degree reduction, the curve will be degree - 1 from the input.</returns>
        internal static NurbsBase ReduceDegree(NurbsBase curve, double tolerance)
        {
            int n = curve.Knots.Count - curve.Degree - 2;
            int p = curve.Degree;
            KnotVector U = curve.Knots;
            List<Point4> Qw = curve.ControlPoints;

            // local arrays.
            Point4[] bpts = new Point4[p + 1];
            Point4[] nextbpts = new Point4[p - 1];
            Point4[] rbpts = new Point4[p];
            double[] alphas = new double[p - 1];

            // Output values;
            List<Point4> Pw = new List<Point4>();
            KnotVector Uh = new KnotVector();

            // Initialize some variables.
            int ph = p - 1;
            int mh = ph;
            int kind = ph + 1;
            int r = -1;
            int a = p;
            int b = p + 1;
            int m = n + p + 1;
            Pw.Add(Qw[0]);

            // Compute left end of knot vector.
            for (int i = 0; i <= ph; i++)
            {
                Uh.Add(U[0]);
            }

            // Initialize first Bezier segment.
            for (int i = 0; i <= p; i++)
            {
                bpts[i] = Qw[i];
            }

            // Initialize error vector.
            Vector e = Vector.Zero1d(m);

            // Loop through the knot vector.
            while (b < m)
            {
                // First compute knot multiplicity.
                int i = b;
                while (b < m && Math.Abs(U[b] - U[b + 1]) < GSharkMath.Epsilon)
                {
                    b += 1;
                }

                int mult = b - i + 1;
                mh = mh + mult - 1;
                int oldr = r;
                r = p - mult;

                // Insert knot U[b] r times.
                // Checks for integer arithmetic.
                int lbz = (oldr > 0) ? (int)Math.Floor((double)((oldr + 2) / 2)) : 1;

                if (r > 0)
                {
                    // Inserts knot to get Bezier segment.
                    double numer = U[b] - U[a];
                    for (int k = p; k > mult; k--)
                    {
                        alphas[k - mult - 1] = numer / (U[a + k] - U[a]);
                    }

                    for (int j = 1; j <= r; j++)
                    {
                        int save = r - j;
                        int s = mult + j;
                        for (int k = p; k >= s; k--)
                        {
                            bpts[k] = (bpts[k] * alphas[k - s]) + (bpts[k - 1] * (1.0 - alphas[k - s]));
                        }

                        nextbpts[save] = bpts[p];
                    }
                }

                // Degree reduce Bezier segment.
                double maxError = BezierDegreeReduce(bpts, p, out rbpts);
                e[a] += maxError;
                if (e[a] > tolerance)
                {
                    throw new Exception("Curve not degree reducible");
                }

                // Remove knot U[a] oldr times.
                if (oldr > 0)
                {
                    // Must remove knot u=U[a] oldr times.
                    int first = kind;
                    int last = kind;
                    for (int k = 0; k < oldr; k++)
                    {
                        // Knot removal loop.
                        int l = first;
                        int j = last;
                        int kj = j - kind;

                        while (j - l > k)
                        {
                            double alfa = (U[a] - Uh[l - 1]) / (U[b] - Uh[l - 1]);
                            double beta = (U[a] - Uh[j - k - 1]) / (U[b] - Uh[j - k - 1]);
                            Pw[l - 1] = (Pw[l - 1] - Pw[l - 2] * (1.0 - alfa)) / alfa;
                            rbpts[kj] = (rbpts[kj] - rbpts[kj + 1] * beta) / (1.0 - beta);

                            l += 1;
                            j -= 1;
                            kj -= 1;
                        }

                        // Compute knot removal error bounds (Br).
                        double Br;
                        if (j - l < k)
                        {
                            Br = Pw[l - 2].DistanceTo(rbpts[kj + 1]);
                        }
                        else
                        {
                            double delta = (U[a] - Uh[l - 1]) / (U[b] - Uh[l - 1]);
                            Point4 A = (rbpts[kj + 1] * delta) + (Pw[l - 2] * (1.0 - delta));
                            Br = Pw[l - 1].DistanceTo(A);
                        }

                        // Update the error vector.
                        int K = a + oldr - k;
                        int q = (int)Math.Floor((double)(2 * p - k + 1) / 2);
                        int L = K - q;

                        for (int ii = L; ii <= a; ii++)
                        {
                            // These knot vectors were effected.
                            e[ii] += Br;
                            if (e[ii] > tolerance)
                            {
                                throw new Exception("Curve not degree reducible");
                            }
                        }

                        first -= 1;
                        last += 1;
                    }
                }

                // Load knot vector and control points.
                if (a != p)
                {
                    // Load the knot ua.
                    for (int j = 0; j < ph - oldr; j++)
                    {
                        Uh[kind] = U[a];
                        kind += 1;
                    }
                }

                for (int j = lbz; j <= ph; j++)
                {
                    // Load control points into Qw.
                    Pw.Add(rbpts[j]);
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
                        bpts[j] = Qw[b - p + j];
                    }

                    a = b;
                    b += 1;
                }
                else
                {
                    // End knot.
                    for (int j = 0; j <= ph; j++)
                    {
                        Uh.Add(U[b]);
                    }
                }
            }

            return new NurbsCurve(p - 1, Uh, Pw);
        }
    }
}
