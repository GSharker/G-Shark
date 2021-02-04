using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.Geometry;

namespace GeometrySharp.Operation
{
    /// <summary>
    /// Analyze contains methods for analyzing NURBS geometry.
    /// </summary>
    public class Analyze
    {
        /// <summary>
        /// Approximate the length of a rational curve by gaussian quadrature - assumes a smooth curve.
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="u">The parameter at which to approximate the length.</param>
        /// <param name="gaussDegIncrease">the degree of gaussian quadrature to perform.
        /// A higher number yields a more exact result, default set to 16.</param>
        /// <returns>Return the approximate length.</returns>
        public static double RationalCurveArcLength(NurbsCurve curve, double u = -1.0, int gaussDegIncrease = 16)
        {
            var uSet = u < 0.0 ? curve.Knots.Last() : u;

            var crvs = Modify.DecomposeCurveIntoBeziers(curve);
            var i = 0;
            var sum = 0.0;
            var tempCrv = crvs[0];

            while (i < crvs.Count && tempCrv.Knots[0] + GeoSharpMath.EPSILON < uSet)
            {
                tempCrv = crvs[i];
                var param = Math.Min(tempCrv.Knots.Last(), uSet);
                sum += RationalBezierCurveLength(tempCrv, param, gaussDegIncrease);
                i += 1;
            }

            return sum;
        }

        /// <summary>
        /// Approximate the length of a rational bezier curve by gaussian quadrature - assumes a smooth curve.
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="u">The parameter at which to approximate the length.</param>
        /// <param name="gaussDegIncrease">the degree of gaussian quadrature to perform.
        /// A higher number yields a more exact result, default set to 16.</param>
        /// <returns>Return the approximate length.</returns>
        public static double RationalBezierCurveLength(NurbsCurve curve, double u = -1.0, int gaussDegIncrease = 16)
        {
            var uSet = u < 0.0 ? curve.Knots.Last() : u;
            var z = (uSet - curve.Knots[0]) / 2;
            var sum = 0.0;
            var gaussDegree = curve.Degree + gaussDegIncrease;

            for (int i = 0; i < gaussDegree; i++)
            {
                var cu = z * LegendreGaussData.tValues[gaussDegree][i] + z + curve.Knots[0];
                var tan = Evaluation.RationalCurveDerivatives(curve, cu, 1);

                sum += LegendreGaussData.cValues[gaussDegree][i] * tan[1].Length();
            }

            return z * sum;
        }

        /// <summary>
        /// Get the curve parameter t at a given length.
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="segmentLength">The length to find the parameter.</param>
        /// <param name="tolerance">If set less or equal 0.0, the tolerance used is 1e-10.</param>
        /// <param name="curveLength">The length of curve if computer, if not will be computed.</param>
        /// <returns>The parameter t at the given length.</returns>
        public static double RationalBezierCurveParamAtLength(NurbsCurve curve, double segmentLength, double tolerance, double curveLength = -1)
        {
            if (segmentLength < 0) return curve.Knots[0];

            // We compute the whole length, if the curve lengths is not provided.
            var setCurveLength = (curveLength < 0) ? RationalBezierCurveLength(curve) : curveLength;

            if (segmentLength > setCurveLength) return curve.Knots[^1];

            // Divide and conquer.
            var setTolerance = (tolerance <= 0.0) ? GeoSharpMath.EPSILON : tolerance;

            var startT = curve.Knots[0];
            var startLength = 0.0;

            var endT = curve.Knots[^1];
            var endLength = setCurveLength;

            while ((endLength - startLength) > setTolerance)
            {
                var midT = (startT + endT) / 2;
                var midLength = RationalBezierCurveLength(curve, midT);

                if (midLength > segmentLength)
                {
                    endT = midT;
                    endLength = midLength;
                }
                else
                {
                    startT = midT;
                    startLength = midLength;
                }
            }

            return (startT + endT) / 2;
        }

        public static Vector3 RationalCurveClosestPoint(NurbsCurve curve, Vector3 point)
        {
            var tValue = RationalCurveClosestParameter(curve, point);
            return Evaluation.CurvePointAt(curve, tValue);
        }

        public static double RationalCurveClosestParameter(NurbsCurve curve, Vector3 point)
        {
            // (Piegl & Tiller suggest) page 244 chapter six

            var minimumDistance = double.PositiveInfinity;
            var tParameter = default(double);
            var ctrlPts = curve.ControlPoints;

            var (tValues, pts) = Tessellation.RegularSample(curve, ctrlPts.Count * curve.Degree);

            for (int i = 0; i < pts.Count - 1 ; i++)
            {
                var t0 = tValues[i];
                var t1 = tValues[i + 1];

                var pt0 = pts[i];
                var pt1 = pts[i + 1];

                var projection = Trigonometry.ClosestPointToSegment(point, pt0, pt1, t0, t1);
                var distance = (point - projection.pt).Length();

                if (!(distance < minimumDistance)) continue;
                minimumDistance = distance;
                tParameter = projection.tValue;
            }

            var maxInteraction = 5;
            var j = 0;
            // Two zero tolerances can be used to indicate convergence:
            var tol1 = 0.0001; // a measure of Euclidean distance;
            var tol2 = 0.0005; // a zero cosine measure.
            var tVal0 = curve.Knots[0];
            var tVal1 = curve.Knots[^1];
            var isCurveClosed = (ctrlPts[0] - ctrlPts[^1]).SquaredLength() < GeoSharpMath.EPSILON;
            var Cu = tParameter;

            while (j < maxInteraction)
            {
                var e = Evaluation.RationalCurveDerivatives(curve, Cu, 2);
                var diff = e[0] - point; // C(u) - P

                // First condition, point coincidence:
                // |C(u) - p| < e1
                var c1v = diff.Length();
                var c1 = c1v <= tol1;

                // Second condition, zero cosine:
                // C'(u) * (C(u) - P)
                // ------------------ < e2
                // |C'(u)| |C(u) - P|
                var c2n = Vector3.Dot(e[1], diff);
                var c2d = (e[1] * c1v).Length();
                var c2v = c2n / c2d;
                var c2 = Math.Abs(c2v) <= tol2;

                // If at least one of these conditions is not satisfied,
                // a new value, ui+l> is computed using the NewtonIteration.
                // Then two more conditions are checked.
                if (c1 && c2) return Cu;
                var ct = NewtonIteration(Cu, e, diff);

                // Ensure that the parameter stays within the boundary of the curve.
                if (ct < tVal0) ct = isCurveClosed ? tVal1 - (ct - tVal0) : tVal0;
                if (ct > tVal1) ct = isCurveClosed ? tVal0 + (ct - tVal1) : tVal1;

                // the parameter does not change significantly, the point is off the end of the curve.
                var c3v = (e[1] * (ct - Cu)).Length();
                if (c3v < tol1) return Cu;

                Cu = ct;
                j++;
            }

            return Cu;
        }

        /// <summary>
        /// Newton iteration to minimize the distance between a point and a curve.
        /// </summary>
        /// <param name="u">The parameter obtained at the ith Newton iteration.</param>
        /// <param name="derivativePts">Point on curve identify as C'(u)</param>
        /// <param name="difference">Representing the difference from C(u) - P.</param>
        /// <returns></returns>
        private static double NewtonIteration(double u, List<Vector3> derivativePts, Vector3 difference)
        {
            // The distance from P to C(u) is minimum when f(u) = 0, whether P is on the curve or not.
            // C'(u) * ( C(u) - P ) = 0 = f(u)
            // C(u) is the curve, p is the point, * is a dot product
            var f = Vector3.Dot(derivativePts[1], difference);

            //	f' = C"(u) * ( C(u) - p ) + C'(u) * C'(u)
            var s0 = Vector3.Dot(derivativePts[2], difference);
            var s1 = Vector3.Dot(derivativePts[1], derivativePts[1]);
            var df = s0 + s1;

            return u - f / df;
        }
    }
}