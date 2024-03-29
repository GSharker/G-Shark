﻿using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Analyze
{
    /// <summary>
    /// Contains methods for analyzing curves.
    /// </summary>
    internal static class Curve
    {
        /// <summary>
        /// Computes the approximate length of a rational curve by gaussian quadrature - assumes a smooth curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="u">The parameter at which to approximate the length.</param>
        /// <param name="gaussDegIncrease">
        /// The degree of gaussian quadrature to perform.
        /// A higher number yields a more exact result, default set to 17.
        /// </param>
        /// <returns>The approximate length.</returns>
        internal static double Length(NurbsBase curve, double u = -1.0, int gaussDegIncrease = 17)
        {
            double uSet = u < 0.0 ? curve.Knots.Last() : u;

            List<NurbsBase> crvs = Modify.Curve.DecomposeIntoBeziers(curve);
            double sum = 0.0;

            foreach (NurbsBase bezier in crvs)
            {
                if (!(bezier.Knots[0] + GSharkMath.Epsilon < uSet))
                {
                    break;
                }
                double param = Math.Min(bezier.Knots.Last(), uSet);
                sum += BezierLength(bezier, param, gaussDegIncrease);
            }

            return sum;
        }

        /// <summary>
        /// Computes the curvature vector.
        /// The vector has length equal to the radius of the curvature circle and with direction to the center of the circle.
        /// https://github.com/mcneel/opennurbs/blob/484ba88836bbedff8fe0b9e574fcd6434b49c21c/opennurbs_math.cpp#L839
        /// </summary>
        /// <param name="derivative1">The first derivative.</param>
        /// <param name="derivative2">The second derivative.</param>
        /// <returns>The curvature vector.</returns>
        internal static Vector3 Curvature(Vector3 derivative1, Vector3 derivative2)
        {
            double d1 = derivative1.Length;

            // If the first derivative exists and is zero the curvature is a zero vector.
            if (d1 == 0.0)
            {
                return Vector3.Zero;
            }

            Vector3 tangent = derivative1 / d1;
            double d2DotTang = (derivative2 * -1) * tangent;
            d1 = 1.0 / (d1 * d1);
            Vector3 curvature = d1 * (derivative2 + d2DotTang * tangent); // usually identified as k.

            double curvatureLength = curvature.Length;
            if (curvatureLength < 1.490116119385E-08) // SqrtEpsilon value that is used when comparing square roots.
            {
                curvatureLength = 1e10;
            }

            double radius = (curvature / (curvatureLength * curvatureLength)).Length;
            return curvature.Unitize().Amplify(radius);
        }

        /// <summary>
        /// Computes the approximate length of a rational bezier curve by gaussian quadrature - assumes a smooth curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="u">The parameter at which to approximate the length.</param>
        /// <param name="gaussDegIncrease">
        /// the degree of gaussian quadrature to perform.
        /// A higher number yields a more exact result, default set to 17.
        /// </param>
        /// <returns>The approximate length of a bezier.</returns>
        internal static double BezierLength(NurbsBase curve, double u = -1.0, int gaussDegIncrease = 17)
        {
            double uSet = u < 0.0 ? curve.Knots.Last() : u;
            double z = (uSet - curve.Knots[0]) / 2;
            double sum = 0.0;
            int gaussDegree = curve.Degree + gaussDegIncrease;

            for (int i = 0; i < gaussDegree; i++)
            {
                double cu = z * LegendreGaussData.tValues[gaussDegree][i] + z + curve.Knots[0];
                List<Vector3> tan = Evaluate.Curve.RationalDerivatives(curve, cu);

                sum += LegendreGaussData.cValues[gaussDegree][i] * tan[1].Length;
            }

            return z * sum;
        }

        /// <summary>
        /// Computes the curve parameter at a given length.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="segmentLength">The length to find the parameter.</param>
        /// <param name="tolerance">If set less or equal 0.0, the tolerance used is 1e-10.</param>
        /// <returns>The parameter at the given length.</returns>
        internal static double BezierParameterAtLength(NurbsBase curve, double segmentLength, double tolerance)
        {
            if (segmentLength < 0)
            {
                return curve.Knots[0];
            }

            // We compute the whole length.
            double curveLength = BezierLength(curve);

            if (segmentLength > curveLength)
            {
                return curve.Knots[curve.Knots.Count - 1];
            }

            // Divide and conquer.
            double setTolerance = tolerance <= 0.0 ? GSharkMath.Epsilon : tolerance;

            double startT = curve.Knots[0];
            double startLength = 0.0;

            double endT = curve.Knots[curve.Knots.Count - 1];
            double endLength = curveLength;

            while (endLength - startLength > setTolerance)
            {
                double midT = (startT + endT) / 2;
                double midLength = BezierLength(curve, midT);

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

        /// <summary>
        /// Computes the closest parameters on a curve to a given point.<br/>
        /// <em>Corresponds to page 244 chapter six from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="point">Point to search from.</param>
        /// <returns>The closest parameter on the curve.</returns>
        internal static double ClosestParameter(NurbsBase curve, Point3 point)
        {
            double minimumDistance = double.PositiveInfinity;
            double tParameter = 0D;
            List<Point3> ctrlPts = curve.ControlPointLocations;

            (List<double> tValues, List<Point3> pts) = Sampling.Curve.RegularSample(curve, ctrlPts.Count * curve.Degree);

            for (int i = 0; i < pts.Count - 1; i++)
            {
                double t0 = tValues[i];
                double t1 = tValues[i + 1];

                Point3 pt0 = pts[i];
                Point3 pt1 = pts[i + 1];

                var (tValue, pt) = Trigonometry.ClosestPointToSegment(point, pt0, pt1, t0, t1);
                double distance = pt.DistanceTo(point);

                if (!(distance < minimumDistance)) continue;
                minimumDistance = distance;
                tParameter = tValue;
            }

            int maxIterations = 5;
            int j = 0;
            // Two zero tolerances can be used to indicate convergence:
            double tol1 = GSharkMath.MaxTolerance; // a measure of Euclidean distance;
            double tol2 = 0.0005; // a zero cosine measure.
            double tVal0 = curve.Knots[0];
            double tVal1 = curve.Knots[curve.Knots.Count - 1];
            bool closedCurve = (ctrlPts[0] - ctrlPts[ctrlPts.Count - 1]).SquareLength < GSharkMath.Epsilon;
            double Cu = tParameter;

            // To avoid infinite loop we limited the interaction.
            while (j < maxIterations)
            {
                List<Vector3> e = Evaluate.Curve.RationalDerivatives(curve, Cu, 2);
                Vector3 diff = e[0] - new Vector3(point); // C(u) - P


                // First condition, point coincidence:
                // |C(u) - p| < e1
                double c1v = diff.Length;
                bool c1 = c1v <= tol1;

                // Second condition, zero cosine:
                // C'(u) * (C(u) - P)
                // ------------------ < e2
                // |C'(u)| |C(u) - P|
                double c2n = Vector3.DotProduct(e[1], diff);
                double c2d = (e[1] * c1v).Length;
                double c2v = c2n / c2d;
                bool c2 = Math.Abs(c2v) <= tol2;

                // If at least one of these conditions is not satisfied,
                // a new value, ui+l> is computed using the NewtonIteration.
                // Then two more conditions are checked.
                if (c1 && c2) return Cu;
                double ct = NewtonIteration(Cu, e, diff);

                // Ensure that the parameter stays within the boundary of the curve.
                if (ct < tVal0) ct = closedCurve ? tVal1 - (ct - tVal0) : tVal0;
                if (ct > tVal1) ct = closedCurve ? tVal0 + (ct - tVal1) : tVal1;

                // the parameter does not change significantly, the point is off the end of the curve.
                double c3v = (e[1] * (ct - Cu)).Length;
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
        /// <returns>The minimized parameter.</returns>
        private static double NewtonIteration(double u, List<Vector3> derivativePts, Vector3 difference)
        {
            // The distance from P to C(u) is minimum when f(u) = 0, whether P is on the curve or not.
            // C'(u) * ( C(u) - P ) = 0 = f(u)
            // C(u) is the curve, p is the point, * is a dot product
            double f = Vector3.DotProduct(derivativePts[1], difference);

            //	f' = C"(u) * ( C(u) - p ) + C'(u) * C'(u)
            double s0 = Vector3.DotProduct(derivativePts[2], difference);
            double s1 = Vector3.DotProduct(derivativePts[1], derivativePts[1]);
            double df = s0 + s1;

            return u - f / df;
        }

        /// <summary>
        /// Approximates the parameter at a given length on a curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="segmentLength">The arc length for which to do the procedure.</param>
        /// <param name="tolerance">If set less or equal 0.0, the tolerance used is 1e-10.</param>
        /// <returns>The parameter on the curve.</returns>
        internal static double ParameterAtLength(NurbsBase curve, double segmentLength, double tolerance = -1)
        {
            if (segmentLength < GSharkMath.Epsilon) return curve.Knots[0];
            if (Math.Abs(curve.Length - segmentLength) < GSharkMath.Epsilon) return curve.Knots[curve.Knots.Count - 1];

            List<NurbsBase> curves = Modify.Curve.DecomposeIntoBeziers(curve);
            int i = 0;
            double curveLength = -GSharkMath.Epsilon;
            double segmentLengthLeft = segmentLength;

            // Iterate through the curves consuming the bezier's, summing their length along the way.
            while (curveLength < segmentLength && i < curves.Count)
            {
                double bezierLength = BezierLength(curves[i]);
                curveLength += bezierLength;

                if (segmentLength < curveLength + GSharkMath.Epsilon)
                    return BezierParameterAtLength(curves[i], segmentLengthLeft, tolerance);
                i++;
                segmentLengthLeft -= bezierLength;
            }

            return -1;
        }
    }
}