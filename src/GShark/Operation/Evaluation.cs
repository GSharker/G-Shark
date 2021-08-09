using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Provides all of the core algorithms for evaluating points and derivatives on NURBS curves and surfaces.<br/>
    /// Many of these algorithms owe their implementation to The NURBS Book by Piegl and Tiller.
    /// </summary>
    public class Evaluation
    {
        /// <summary>
        /// Computes the non-vanishing basis functions.<br/>
        /// <em>Implementation of Algorithm A2.2 from The NURBS Book by Piegl and Tiller.<br/>
        /// Uses recurrence to compute the basis functions, also known as Cox - deBoor recursion formula.</em>
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="knot">knot value.</param>
        /// <returns>List of non-vanishing basis functions.</returns>
        public static List<double> BasisFunction(int degree, KnotVector knots, double knot)
        {
            int span = knots.Span(degree, knot);
            return BasisFunction(degree, knots, span, knot);
        }

        /// <summary>
        /// Computes the non-vanishing basis functions.<br/>
        /// <em>Implementation of Algorithm A2.2 from The NURBS Book by Piegl and Tiller.<br/>
        /// Uses recurrence to compute the basis functions, also known as Cox - deBoor recursion formula.</em>
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="span">Index span of knots.</param>
        /// <param name="knot">knot value.</param>
        /// <returns>List of non-vanishing basis functions.</returns>
        public static List<double> BasisFunction(int degree, KnotVector knots, int span, double knot)
        {
            Vector left = Vector.Zero1d(degree + 1);
            Vector right = Vector.Zero1d(degree + 1);
            // N[0] = 1.0 by definition;
            Vector N = Vector.Zero1d(degree + 1);
            N[0] = 1.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = knot - knots[span + 1 - j];
                right[j] = knots[span + j] - knot;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    double temp = N[r] / (right[r + 1] + left[j - r]);
                    N[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                N[j] = saved;
            }

            return N;
        }

        /// <summary>
        /// Computes the value of a basis function for a single value.<br/>
        /// <em>Implementation of Algorithm A2.4 from The NURBS Book by Piegl and Tiller.</em><br/>
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="span">Index span of knots.</param>
        /// <param name="knot">knot value.</param>
        /// <returns>The single parameter value of the basis function.</returns>
        public static double OneBasisFunction(int degree, KnotVector knots, int span, double knot)
        {
            // Special case at boundaries.
            if ((span == 0 && Math.Abs(knot - knots[0]) < GeoSharkMath.MaxTolerance) ||
                (span == knots.Count - degree - 2) && Math.Abs(knot - knots[knots.Count - 1]) < GeoSharkMath.MaxTolerance)
            {
                return 1.0;
            }

            // Local property, parameter is outside of span range.
            if (knot < knots[span] || knot >= knots[span + degree + 1])
            {
                return 0.0;
            }

            List<double> N = Sets.RepeatData(0.0, degree + span + 1);
            // Initialize the zeroth degree basic functions.
            for (int j = 0; j < degree + 1; j++)
            {
                if (knot >= knots[span + j] && knot < knots[span + j + 1])
                {
                    N[j] = 1.0;
                }
            }

            // Compute triangular table of basic functions.
            for (int k = 1; k < degree + 1; k++)
            {
                double saved = 0.0;
                if (N[0] != 0.0)
                {
                    saved = ((knot - knots[span]) * N[0]) / (knots[span + k] - knots[span]);
                }

                for (int j = 0; j < degree - k + 1; j++)
                {
                    double uLeft = knots[span + j + 1];
                    double uRight = knots[span + j + k + 1];

                    if (N[j + 1] == 0.0)
                    {
                        N[j] = saved;
                        saved = 0.0;
                    }
                    else
                    {
                        double temp = N[j + 1] / (uRight - uLeft);
                        N[j] = saved + (uRight - knot) * temp;
                        saved = (knot - uLeft) * temp;
                    }
                }
            }

            return N[0];
        }

        /// <summary>
        /// This method evaluate a B-spline span using the deBoor algorithm.
        /// https://github.com/mcneel/opennurbs/blob/2b96cf31429dab25bf8a1dbd171227c506b06f88/opennurbs_evaluate_nurbs.cpp#L1249
        /// This method is not implemented for clamped knots.
        /// </summary>
        /// <param name="controlPts">The control points of the curve.</param>
        /// <param name="knots">The knot vector of the curve.</param>
        /// <param name="degree">The value degree of the curve.</param>
        /// <param name="t">The parameter value where the curve is evaluated.</param>
        internal static void DeBoor(ref List<Point4> controlPts, KnotVector knots, int degree, double t)
        {
            if (Math.Abs(knots[degree] - knots[degree - 1]) < GeoSharkMath.Epsilon)
            {
                throw new Exception($"DeBoor evaluation failed: {knots[degree]} == {knots[degree + 1]}");
            }

            // deltaT = {knot[order-1] - t, knot[order] -  t, .. knot[2*order-3] - t}
            List<double> deltaT = new List<double>();

            for (int k = 0; k < degree; k++)
            {
                deltaT.Add(knots[degree + 1 + k] - t);
            }

            for (int i = degree; i > 0; --i)
            {
                for (int j = 0; j < i; j++)
                {
                    double k0 = knots[degree + 1 - i + j];
                    double k1 = knots[degree + 1 + j];

                    double alpha0 = deltaT[j] / (k1 - k0);
                    double alpha1 = 1.0 - alpha0;

                    Point4 cv1 = controlPts[j + 1];
                    Point4 cv0 = controlPts[j];

                    controlPts[j] = (cv0 * alpha0) + (cv1 * alpha1);
                }
            }
        }

        /// <summary>
        /// Computes a point on a non-uniform, non-rational b-spline curve.<br/>
        /// <em>Corresponds to algorithm 3.1 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="t">Parameter on the curve at which the point is to be evaluated</param>
        /// <returns>The evaluated point on the curve.</returns>
        public static Point3 CurvePointAt(ICurve curve, double t)
        {
            List<Point4> controlPts = curve.ControlPoints;
            KnotVector knots = curve.Knots;

            int n = knots.Count - curve.Degree - 2;

            int knotSpan = knots.Span(n, curve.Degree, t);
            List<double> basisValue = BasisFunction(curve.Degree, knots, knotSpan, t);
            Point4 pointOnCurve = Point4.Zero;

            for (int i = 0; i <= curve.Degree; i++)
            {
                double valToMultiply = basisValue[i];
                Point4 pt = controlPts[knotSpan - curve.Degree + i];

                pointOnCurve.X += valToMultiply * pt.X;
                pointOnCurve.Y += valToMultiply * pt.Y;
                pointOnCurve.Z += valToMultiply * pt.Z;
                pointOnCurve.W += valToMultiply * pt.W;
            }

            return new Point3(pointOnCurve);
        }


        /// <summary>
        /// Computes the tangent at a point on a curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="t">The parameter on the curve.</param>
        /// <returns>The tangent vector at the given parameter.</returns>
        public static Vector3 RationalCurveTangent(ICurve curve, double t)
        {
            List<Vector3> derivatives = RationalCurveDerivatives(curve, t, 1);
            return derivatives[1];
        }

        /// <summary>
        /// Calculates the centroid averaging the points.  
        /// </summary>
        /// <param name="pts">The points collection to evaluate.</param>
        /// <returns>The centroid point.</returns>
        public static Point3 CentroidByVertices(IList<Point3> pts)
        {
            Point3 centroid = new Point3();
            bool isClosed = pts[0] == pts[pts.Count - 1];
            int count = pts.Count;

            for (int i = 0; i < count && !(i == count - 1 & isClosed); i++)
            {
                centroid += pts[i];
            }

            return !isClosed ? centroid / count : centroid / (count - 1);
        }

        /// <summary>
        /// Calculates all the extrema on a curve.<br/>
        /// Extrema are calculated for each dimension, rather than for the full curve, <br/>
        /// so that the result is not the number of convex/concave transitions, but the number of those transitions for each separate dimension.
        /// </summary>
        /// <param name="curve">Curve where the extrema are calculated.</param>
        /// <returns>The extrema.</returns>
        public static Extrema ComputeExtrema(ICurve curve)
        {
            var derivPts = DerivativeCoordinates(curve.LocationPoints);
            Extrema extrema = new Extrema();

            int dim = derivPts[0][0].Size;

            for (int j = 0; j < dim; j++)
            {
                List<double> p0 = new List<double>();

                for (int i = 0; i < derivPts[0].Count; i++)
                {
                    p0.Add(derivPts[0][i][j]);
                }

                List<double> result = new List<double>(DerivRoots(p0));

                if (curve.Degree == 3)
                {
                    IList<double> p1 = new List<double>();

                    for (int i = 0; i < derivPts[1].Count; i++)
                    {
                        p1.Add(derivPts[1][i][j]);
                    }

                    result = result.Concat(DerivRoots(p1).ToList()).ToList();
                }

                result = result.Where((t) => t >= 0 && t <= 1).ToList();
                result.Sort();
                extrema[j] = result;
            }

            extrema.Values = extrema[0].Union(extrema[1]).Union(extrema[2]).OrderBy(x => x).ToList();
            return extrema;
        }

        /// <summary>
        /// Computes the derivatives of a Bezier.
        /// https://pomax.github.io/bezierinfo/#derivatives
        /// https://github.com/Pomax/bezierjs/blob/9ac7cec37fc56621dceabc430a7862b54917c3e2/dist/bezier.cjs#L199
        /// </summary>
        /// <param name="pts">The collection of coordinate points.</param>
        /// <returns>The derivative coordinates.</returns>
        internal static List<List<Point3>> DerivativeCoordinates(List<Point3> pts)
        {
            List<List<Point3>> derivPts = new List<List<Point3>>();

            List<Point3> p = new List<Point3>(pts);
            int d = p.Count;
            int c = d - 1;

            for (; d > 1; d--, c--)
            {
                List<Point3> list = new List<Point3>();
                for (int j = 0; j < c; j++)
                {
                    Vector3 dpt = (p[j + 1] - p[j]) * c;

                    list.Add(dpt);
                }

                derivPts.Add(list);
                p = list;
            }

            return derivPts;
        }

        /// <summary>
        /// Find all roots (derivative=0) for both derivatives.<br/>
        /// https://pomax.github.io/bezierinfo/#extremities
        /// </summary>
        /// <param name="derivatives">The derivatives.</param>
        /// <returns>The roots values.</returns>
        internal static IList<double> DerivRoots(IList<double> derivatives)
        {
            // quadratic roots
            if (derivatives.Count == 3)
            {
                var a = derivatives[0];
                var b = derivatives[1];
                var c = derivatives[2];
                var d = a - 2 * b + c;
                if (Math.Abs(d) * double.Epsilon != 0)
                {
                    double m1 = -Math.Sqrt(b * b - a * c);
                    double m2 = -a + b;
                    double v1 = -(m1 + m2) / d;
                    double v2 = -(-m1 + m2) / d;
                    return new[] { v1, v2 };
                }
                if (Math.Abs(b - c) > GeoSharkMath.Epsilon && Math.Abs(d) * double.Epsilon == 0.0)
                {
                    return new[] { (2 * b - c) / (2 * (b - c)) };
                }
                return Array.Empty<double>();
            }

            // linear roots
            if (derivatives.Count == 2)
            {
                var a = derivatives[0];
                var b = derivatives[1];
                if (a != b)
                {
                    return new[] { a / (a - b) };
                }
                return Array.Empty<double>();
            }

            return Array.Empty<double>();
        }

        /// <summary>
        /// Determines the derivatives of a curve at a given parameter.<br/>
        /// <em>Corresponds to algorithm 4.2 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="parameter">Parameter on the curve at which the point is to be evaluated</param>
        /// <param name="numberOfDerivatives"></param>
        /// <returns>The derivatives.</returns>
        public static List<Vector3> RationalCurveDerivatives(ICurve curve, double parameter, int numberOfDerivatives = 1)
        {
            List<Point4> derivatives = CurveDerivatives(curve, parameter, numberOfDerivatives);
            // Array of derivative of A(t).
            // Where A(t) is the vector - valued function whose coordinates are the first three coordinates
            // of an homogenized pts.
            // Correspond in the book to Aders.
            List<Point3> rationalDerivativePoints = Point4.RationalPoints(derivatives);
            // Correspond in the book to wDers.
            List<double> weightDers = Point4.GetWeights(derivatives);
            List<Vector3> CK = new List<Vector3>();

            for (int k = 0; k < numberOfDerivatives + 1; k++)
            {
                Point3 rationalDerivativePoint = rationalDerivativePoints[k];

                for (int i = 1; i < k + 1; i++)
                {
                    double valToMultiply = LinearAlgebra.GetBinomial(k, i) * weightDers[i];
                    var pt = CK[k - i];
                    for (int j = 0; j < rationalDerivativePoint.Size; j++)
                    {
                        rationalDerivativePoint[j] = rationalDerivativePoint[j] - valToMultiply * pt[j];
                    }
                }

                for (int j = 0; j < rationalDerivativePoint.Size; j++)
                {
                    rationalDerivativePoint[j] = rationalDerivativePoint[j] * (1 / weightDers[0]);
                }

                CK.Add(rationalDerivativePoint);
            }
            // Return C(t) derivatives.
            return CK;
        }

        /// <summary>
        /// Determines the derivatives of a NURBS curve at a given parameter.<br/>
        /// <em>Corresponds to algorithm 3.2 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="parameter">Parameter on the curve at which the point is to be evaluated.</param>
        /// <param name="numberDerivs">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <returns>The derivatives.</returns>
        public static List<Point4> CurveDerivatives(ICurve curve, double parameter, int numberDerivs)
        {
            List<Point4> curveHomogenizedPoints = curve.ControlPoints;

            int n = curve.Knots.Count - curve.Degree - 2;
            int derivateOrder = numberDerivs < curve.Degree ? numberDerivs : curve.Degree;

            Point4[] ck = new Point4[numberDerivs + 1];
            int knotSpan = curve.Knots.Span(n, curve.Degree, parameter);
            List<Vector> derived2d = DerivativeBasisFunctionsGivenNI(knotSpan, parameter, curve.Degree, derivateOrder, curve.Knots);

            for (int k = 0; k < derivateOrder + 1; k++)
            {
                for (int j = 0; j < curve.Degree + 1; j++)
                {
                    double valToMultiply = derived2d[k][j];
                    Point4 pt = curveHomogenizedPoints[knotSpan - curve.Degree + j];
                    for (int i = 0; i < pt.Size; i++)
                    {
                        ck[k][i] = ck[k][i] + (valToMultiply * pt[i]);
                    }
                }
            }
            return ck.ToList();
        }

        /// <summary>
        /// Computes the non-vanishing basis functions and their derivatives.<br/>
        /// <em>Corresponds to algorithm 2.3 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="span">Span index.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="degree">Curve degree.</param>
        /// <param name="order">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <param name="knots">Sets of non-decreasing knot values.</param>
        /// <returns>The derivatives at the given parameter.</returns>
        public static List<Vector> DerivativeBasisFunctionsGivenNI(int span, double parameter, int degree,
            int order, KnotVector knots)
        {
            Vector left = Vector.Zero1d(degree + 1);
            Vector right = Vector.Zero1d(degree + 1);
            // N[0][0] = 1.0 by definition
            List<Vector> ndu = Vector.Zero2d(degree + 1, degree + 1);
            ndu[0][0] = 1.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = parameter - knots[span + 1 - j];
                right[j] = knots[span + j] - parameter;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    ndu[j][r] = right[r + 1] + left[j - r];
                    double temp = ndu[r][j - 1] / ndu[j][r];

                    ndu[r][j] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                ndu[j][j] = saved;
            }

            // Load the basic functions.
            List<Vector> ders = Vector.Zero2d(order + 1, degree + 1);
            for (int j = 0; j < degree + 1; j++)
            {
                ders[0][j] = ndu[j][degree];
            }

            // Start calculating derivatives.
            List<Vector> a = Vector.Zero2d(2, degree + 1);
            // Loop over function index.
            for (int r = 0; r < degree + 1; r++)
            {
                // Alternate row in array a.
                int s1 = 0;
                int s2 = 1;
                a[0][0] = 1.0;

                // Loop to compute Kth derivative.
                for (int k = 1; k < order + 1; k++)
                {
                    double d = 0.0;
                    int rk = r - k;
                    int pk = degree - k;
                    int j1, j2;

                    if (r >= k)
                    {
                        a[s2][0] = a[s1][0] / ndu[pk + 1][rk];
                        d = a[s2][0] * ndu[rk][pk];
                    }

                    if (rk >= -1)
                    {
                        j1 = 1;
                    }
                    else
                    {
                        j1 = -rk;
                    }

                    if (r - 1 <= pk)
                    {
                        j2 = k - 1;
                    }
                    else
                    {
                        j2 = degree - r;
                    }

                    for (int j = j1; j < j2 + 1; j++)
                    {
                        a[s2][j] = (a[s1][j] - a[s1][j - 1]) / ndu[pk + 1][rk + j];
                        d += a[s2][j] * ndu[rk + j][pk];
                    }

                    if (r <= pk)
                    {
                        a[s2][k] = -a[s1][k - 1] / ndu[pk + 1][r];
                        d += a[s2][k] * ndu[r][pk];
                    }

                    ders[k][r] = d;

                    // Switch rows.
                    int tempVal = s1;
                    s1 = s2;
                    s2 = tempVal;
                }
            }

            // Multiply through by the the correct factors.
            int acc = degree;
            for (int k = 1; k < order + 1; k++)
            {
                for (int j = 0; j < degree + 1; j++)
                {
                    ders[k][j] *= acc;
                }

                acc *= degree - k;
            }

            return ders;
        }

        /// <summary>
        /// Computes the derivatives at the given U and V parameters on a NURBS surface.<br/>
        /// <para>Returns a two dimensional array containing the derivative vectors.<br/>
        /// Increasing U partial derivatives are increasing row-wise.Increasing V partial derivatives are increasing column-wise.<br/>
        /// Therefore, the[0,0] position is a point on the surface, [n,0] is the nth V partial derivative, the[n,n] position is twist vector or mixed partial derivative UV.</para>
        /// <em>Corresponds to algorithm 4.4 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="u">The u parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The v parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">Number of derivatives to evaluate, set as default to 1.</param>
        /// <returns>The derivatives.</returns>
        public static Vector3[,] RationalSurfaceDerivatives(NurbsSurface surface, double u, double v, int numDerivs = 1)
        {
            if (u < 0.0 || u > 1.0)
            {
                throw new ArgumentException("The U parameter is not into the domain 0.0 to 1.0.");
            }

            if (v < 0.0 || v > 1.0)
            {
                throw new ArgumentException("The V parameter is not into the domain 0.0 to 1.0.");
            }

            var derivatives = SurfaceDerivatives(surface, u, v, numDerivs);
            Vector3[,] SKL = new Vector3[numDerivs + 1, numDerivs + 1];

            for (int k = 0; k < numDerivs + 1; k++)
            {
                for (int l = 0; l < numDerivs - k + 1; l++)
                {
                    Vector3 t = derivatives.Item1[k, l];
                    for (int j = 1; j < l + 1; j++)
                    {
                        t -= SKL[k, l - j] * (LinearAlgebra.GetBinomial(l, j) * derivatives.Item2[0, j]);
                    }

                    for (int i = 1; i < k + 1; i++)
                    {
                        t -= SKL[k - i, l] * (LinearAlgebra.GetBinomial(k, i) * derivatives.Item2[i, 0]);
                        Vector3 t2 = Vector3.Zero;
                        for (int j = 1; j < l + 1; j++)
                        {
                            t2 += SKL[k - i, l - j] * (LinearAlgebra.GetBinomial(l, j) * derivatives.Item2[i, j]);
                        }

                        t -= t2 * LinearAlgebra.GetBinomial(k, i);
                    }
                    SKL[k, l] = t / derivatives.Item2[0, 0];
                }
            }

            return SKL;
        }

        /// <summary>
        /// Computes the derivatives on a non-uniform, non-rational B-spline surface.<br/>
        /// SKL is the derivative S(u,v) with respect to U K-times and V L-times.<br/>
        /// <em>Corresponds to algorithm 3.6 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="u">The U parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The V parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">Number of derivatives to evaluate, set as default to 1.</param>
        /// <returns>A tuple with 2D collection representing all derivatives(k,L) and weights(k,l): U derivatives increase by row, V by column.</returns>
        internal static Tuple<Vector3[,], double[,]> SurfaceDerivatives(NurbsSurface surface, double u, double v, int numDerivs = 1)
        {
            // number of basis function.
            int n = surface.KnotsU.Count - surface.DegreeU - 2;
            int m = surface.KnotsV.Count - surface.DegreeV - 2;

            // number of derivatives.
            int du = Math.Min(numDerivs, surface.DegreeU);
            int dv = Math.Min(numDerivs, surface.DegreeV);

            int knotSpanU = surface.KnotsU.Span(n, surface.DegreeU, u);
            int knotSpanV = surface.KnotsV.Span(m, surface.DegreeV, v);

            List<Vector> uDerivs = DerivativeBasisFunctionsGivenNI(knotSpanU, u, surface.DegreeU, n, surface.KnotsU);
            List<Vector> vDerivs = DerivativeBasisFunctionsGivenNI(knotSpanV, v, surface.DegreeV, m, surface.KnotsV);

            int dim = surface.ControlPoints[0][0].Size;
            List<List<Vector>> SKLw = Vector.Zero3d(numDerivs + 1, numDerivs + 1, dim);
            List<Vector> temp = Vector.Zero2d(surface.DegreeV + 1, dim);
            Vector3[,] SKL = new Vector3[numDerivs + 1, numDerivs + 1];
            double[,] weights = new double[numDerivs + 1, numDerivs + 1];

            for (int k = 0; k < du + 1; k++)
            {
                for (int s = 0; s < surface.DegreeV + 1; s++)
                {
                    temp[s] = Vector.Zero1d(dim);
                    for (int r = 0; r < surface.DegreeU + 1; r++)
                    {
                        Vector.AddMulMutate(temp[s], uDerivs[k][r], surface.ControlPoints[knotSpanU - surface.DegreeU + r][knotSpanV - surface.DegreeV + s]);
                    }
                }

                int dd = Math.Min(numDerivs - k, dv);

                for (int l = 0; l < dd + 1; l++)
                {
                    SKLw[k][l] = Vector.Zero1d(dim);
                    for (int s = 0; s < surface.DegreeV + 1; s++)
                    {
                        Vector.AddMulMutate(SKLw[k][l], vDerivs[l][s], temp[s]);
                    }

                    SKL[k, l] = new Vector3(SKLw[k][l][0], SKLw[k][l][1], SKLw[k][l][2]); // Extracting the derivatives.
                    weights[k, l] = SKLw[k][l][3]; // Extracting the weights.
                }
            }

            return new Tuple<Vector3[,], double[,]>(SKL, weights);
        }

        /// <summary>
        /// Computes a point on a non-uniform, non-rational B-spline surface.<br/>
        /// The U and V parameters have to be between 0.0 to 1.0.<br/>
        /// <em>Corresponds to algorithm 3.5 from The NURBS book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The NURBS surface.</param>
        /// <param name="u">The U parameter on the surface at which the point is to be evaluated.</param>
        /// <param name="v">The V parameter on the surface at which the point is to be evaluated.</param>
        /// <returns>The evaluated point.</returns>
        public static Point4 SurfacePointAt(NurbsSurface surface, double u, double v)
        {
            if (u < 0.0 || u > 1.0)
            {
                throw new ArgumentException("The U parameter is not into the domain 0.0 to 1.0.");
            }

            if (v < 0.0 || v > 1.0)
            {
                throw new ArgumentException("The V parameter is not into the domain 0.0 to 1.0.");
            }

            int n = surface.KnotsU.Count - surface.DegreeU - 2;
            int m = surface.KnotsV.Count - surface.DegreeV - 2;
            int knotSpanU = surface.KnotsU.Span(n, surface.DegreeU, u);
            int knotSpanV = surface.KnotsV.Span(m, surface.DegreeV, v);
            List<double> basisUValue = BasisFunction(surface.DegreeU, surface.KnotsU, knotSpanU, u);
            List<double> basisVValue = BasisFunction(surface.DegreeV, surface.KnotsV, knotSpanV, v);
            int uIndex = knotSpanU - surface.DegreeU;
            Point4 evaluatedPt = Point4.Zero;

            for (int l = 0; l < surface.DegreeV + 1; l++)
            {
                var temp = Point4.Zero;
                var vIndex = knotSpanV - surface.DegreeV + l;
                for (int k = 0; k < surface.DegreeU + 1; k++)
                {
                    temp.X += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].X;
                    temp.Y += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].Y;
                    temp.Z += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].Z;
                    temp.W += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].W;
                }

                evaluatedPt.X += basisVValue[l] * temp.X;
                evaluatedPt.Y += basisVValue[l] * temp.Y;
                evaluatedPt.Z += basisVValue[l] * temp.Z;
                evaluatedPt.W += basisVValue[l] * temp.W;
            }
            return evaluatedPt;
        }
    }
}