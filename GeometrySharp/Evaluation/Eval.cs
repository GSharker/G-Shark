using GeometrySharp.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
    public class Eval
    {
        /// <summary>
        /// Compute the non-vanishing basis functions.
        /// Implementation of Algorithm A2.2 from The NURBS Book by Piegl & Tiller.
        /// Uses recurrence to compute the basis functions, also known as Cox - deBoor recursion formula.
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="parameter">Parameter.</param>
        /// <returns>List of non-vanishing basis functions.</returns>
        public static List<double> BasicFunction(int degree, Knot knots, double parameter)
        {
            var span = knots.Span(degree, parameter);
            return BasicFunction(degree, knots, span, parameter);
        }

        /// <summary>
        /// Compute the non-vanishing basis functions.
        /// Implementation of Algorithm A2.2 from The NURBS Book by Piegl & Tiller.
        /// Uses recurrence to compute the basis functions, also known as Cox - deBoor recursion formula.
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="span">Index span of knots.</param>
        /// <param name="parameter">Parameter.</param>
        /// <returns>List of non-vanishing basis functions.</returns>
        public static List<double> BasicFunction(int degree, Knot knots, int span, double parameter)
        {
            var left = Vector3.Zero1d(degree + 1);
            var right = Vector3.Zero1d(degree + 1);
            // N[0] = 1.0 by definition;
            var N = Vector3.Zero1d(degree + 1);
            N[0] = 1.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = parameter - knots[span + 1 - j];
                right[j] = knots[span + j] - parameter;
                var saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    var temp = N[r] / (right[r + 1] + left[j - r]);
                    N[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                N[j] = saved;
            }

            return N;
        }

        /// <summary>
        /// Compute a point on a non-uniform, non-rational b-spline curve.
        /// Corresponds to algorithm 3.1 from The NURBS book, Piegl & Tiller 2nd edition.
        /// </summary>
        /// <param name="curve">Object representing the curve.</param>
        /// <param name="u">Parameter on the curve at which the point is to be evaluated</param>
        /// <returns>The evaluated point.</returns>
        public static Vector3 CurvePointAt(NurbsCurve curve, double u)
        {
            var degree = curve.Degree;
            var controlPts = curve.ControlPoints;
            var knots = curve.Knots;

            if (!curve.Knots.AreValidRelations(degree, controlPts.Count))
                throw new ArgumentException("Invalid relations between control points, knot");

            var n = knots.Count - degree - 2;

            var knotSpan = knots.Span(n, degree, u);
            var basisValue = BasicFunction(degree, knots, knotSpan, u);
            var position = Vector3.Zero1d(controlPts[0].Count);

            for (int i = 0; i < degree + 1; i++)
            {
                var valToMultiply = basisValue[i];
                var pt = controlPts[knotSpan - degree + i];
                for (int j = 0; j < position.Count; j++)
                    position[j] = position[j] + valToMultiply * pt[j];
            }

            return position;
        }

        /// <summary>
        /// Compute the tangent at a point on a NURBS curve.
        /// </summary>
        /// <param name="curve">NurbsCurve object representing the curve.</param>
        /// <param name="t">Parameter.</param>
        /// <returns>A Vector represented by an array of length (dim).</returns>
        public static Vector3 RationalCurveTanget(NurbsCurve curve, double t)
        {
            var derivs = RationalCurveDerivatives(curve, t, 1);
            return derivs[1];
        }

        /// <summary>
        /// Determine the derivatives of a NURBS curve at a given parameter.
        /// </summary>
        /// <param name="curve">Curve object representing the curve - the control points are in homogeneous coordinates.</param>
        /// <param name="parameter">Parameter on the curve at which the point is to be evaluated</param>
        /// <param name="numberDerivs">Number of derivatives to evaluate</param>
        /// <returns>A point represented by an array of length (dim).</returns>
        public static List<Vector3> RationalCurveDerivatives(NurbsCurve curve, double parameter, int numberDerivs = 1)
        {
            var derivatives = CurveDerivatives(curve, parameter, numberDerivs);
            var ptsDerivatives = LinearAlgebra.Rational1d(derivatives);
            var weightDerivatives = LinearAlgebra.Weight1d(derivatives);
            var CK = new List<Vector3>();

            for (int k = 0; k < numberDerivs + 1; k++)
            {
                var v = ptsDerivatives[k];

                for (int i = 1; i < k + 1; i++)
                {
                    var valToMultiply = Binomial.Get(k, i) * weightDerivatives[i];
                    var pt = CK[k - i];
                    for (int j = 0; j < v.Count; j++)
                    {
                        v[j] = v[j] - valToMultiply * pt[j];
                        v[j] = v[j] * (1 / weightDerivatives[0]);
                    }
                }

                CK.Add(v);
            }

            return CK;
        }

        /// <summary>
        /// Determine the derivatives of a non-uniform, non-rational B-spline curve at a given parameter.
        /// Corresponds to algorithm 3.2 from The NURBS book, Piegl & Tiller 2nd edition.
        /// </summary>
        /// <param name="curve">NurbsCurve object representing the curve.</param>
        /// <param name="parameter">Parameter on the curve at which the point is to be evaluated.</param>
        /// <param name="numberDerivs">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <returns></returns>
        public static List<Vector3> CurveDerivatives(NurbsCurve curve, double parameter, int numberDerivs)
        {
            var degree = curve.Degree;
            var controlPts = curve.ControlPoints;
            var knots = curve.Knots;

            if (!curve.Knots.AreValidRelations(degree, controlPts.Count))
                throw new ArgumentException("Invalid relations between control points, knot");

            var n = knots.Count - degree - 2;

            var ptDimension = controlPts[0].Count;
            //var derivateOrder = numberDerivs < degree ? numberDerivs : degree;
            var derivateOrder = Math.Min(numberDerivs, degree);
            var CK = Vector3.Zero2d(numberDerivs + 1, degree + 1);
            var knotSpan = knots.Span(n, degree, parameter);
            var derived2d = DerivativeBasisFunctionsGivenNI(knotSpan, parameter, degree, derivateOrder, knots);

            for (int k = 0; k < derivateOrder + 1; k++)
            {
                for (int j = 0; j < degree + 1; j++)
                {
                    var valToMultiply = derived2d[k][j];
                    var pt = controlPts[knotSpan - degree + j];
                    for (int i = 0; i < CK[k].Count; i++)
                        CK[k][i] = CK[k][i] + (valToMultiply * pt[i]);
                }
            }
            return CK;
        }

        /// <summary>
        /// Compute the non-vanishing basis functions and their derivatives.
        /// (corresponds to algorithm 2.3 from The NURBS book, Piegl & Tiller 2nd edition).
        /// </summary>
        /// <param name="span">Span index.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="degree">Curve degree.</param>
        /// <param name="order">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <param name="knots">Sets of non-decreasing knot values.</param>
        /// <returns></returns>
        public static List<Vector3> DerivativeBasisFunctionsGivenNI(int span, double parameter, int degree,
            int order, Knot knots)
        {
            var left = Vector3.Zero1d(degree + 1);
            var right = Vector3.Zero1d(degree + 1);
            // N[0][0] = 1.0 by definition
            var ndu = Vector3.Zero2d(degree + 1, degree + 1);
            ndu[0][0] = 1.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = parameter - knots[span + 1 - j];
                right[j] = knots[span + j] - parameter;
                var saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    ndu[j][r] = right[r + 1] + left[j - r];
                    var temp = ndu[r][j - 1] / ndu[j][r];

                    ndu[r][j] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                ndu[j][j] = saved;
            }

            // Load the basic functions.
            var ders = Vector3.Zero2d(order + 1, degree + 1);
            for (int j = 0; j < degree + 1; j++)
                ders[0][j] = ndu[j][degree];

            // Start calculating derivatives.
            var a = Vector3.Zero2d(2, degree + 1);
            // Loop over function index.
            for (int r = 0; r < degree + 1; r++)
            {
                // Alternate row in array a.
                var s1 = 0;
                var s2 = 1;
                a[0][0] = 1.0;

                // Loop to compute Kth derivative.
                for (int k = 1; k < order + 1; k++)
                {
                    var d = 0.0;
                    var rk = r - k;
                    var pk = degree - k;
                    int j1, j2;

                    if (r >= k)
                    {
                        a[s2][0] = a[s1][0] / ndu[pk + 1][rk];
                        d = a[s2][0] * ndu[rk][pk];
                    }

                    if (rk >= -1)
                        j1 = 1;
                    else
                        j1 = -rk;

                    if (r - 1 <= pk)
                        j2 = k - 1;
                    else
                        j2 = degree - r;

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
                    var tempVal = s1;
                    s1 = s2;
                    s2 = tempVal;
                }
            }

            // Multiply through by the the correct factors.
            var acc = degree;
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
    }
}