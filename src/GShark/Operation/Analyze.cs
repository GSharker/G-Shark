using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Contains methods for analyzing NURBS geometry.
    /// </summary>
    public class Analyze
    {
        /// <summary>
        /// Computes the approximate length of a rational curve by gaussian quadrature - assumes a smooth curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="u">The parameter at which to approximate the length.</param>
        /// <param name="gaussDegIncrease">
        /// The degree of gaussian quadrature to perform.
        /// A higher number yields a more exact result, default set to 16.
        /// </param>
        /// <returns>The approximate length.</returns>
        public static double CurveLength(ICurve curve, double u = -1.0, int gaussDegIncrease = 16)
        {
            double uSet = u < 0.0 ? curve.Knots.Last() : u;

            List<ICurve> crvs = Modify.DecomposeCurveIntoBeziers(curve);
            int i = 0;
            double sum = 0.0;
            ICurve tempCrv = crvs[0];

            while (i < crvs.Count && tempCrv.Knots[0] + GeoSharkMath.Epsilon < uSet)
            {
                tempCrv = crvs[i];
                double param = Math.Min(tempCrv.Knots.Last(), uSet);
                sum += BezierCurveLength(tempCrv, param, gaussDegIncrease);
                i += 1;
            }

            return sum;
        }

        /// <summary>
        /// Computes the approximate length of a rational bezier curve by gaussian quadrature - assumes a smooth curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="u">The parameter at which to approximate the length.</param>
        /// <param name="gaussDegIncrease">
        /// the degree of gaussian quadrature to perform.
        /// A higher number yields a more exact result, default set to 16.
        /// </param>
        /// <returns>The approximate length of a bezier.</returns>
        public static double BezierCurveLength(ICurve curve, double u = -1.0, int gaussDegIncrease = 16)
        {
            double uSet = u < 0.0 ? curve.Knots.Last() : u;
            double z = (uSet - curve.Knots[0]) / 2;
            double sum = 0.0;
            int gaussDegree = curve.Degree + gaussDegIncrease;

            for (int i = 0; i < gaussDegree; i++)
            {
                double cu = z * LegendreGaussData.tValues[gaussDegree][i] + z + curve.Knots[0];
                List<Vector3d> tan = Evaluation.RationalCurveDerivatives(curve, cu);

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
        /// <param name="curveLength">The length of curve if computer, if not will be computed.</param>
        /// <returns>The parameter at the given length.</returns>
        public static double BezierCurveParamAtLength(ICurve curve, double segmentLength, double tolerance,
            double curveLength = -1)
        {
            if (segmentLength < 0) return curve.Knots[0];

            // We compute the whole length, if the curve lengths is not provided.
            double setCurveLength = curveLength < 0 ? BezierCurveLength(curve) : curveLength;

            if (segmentLength > setCurveLength) return curve.Knots[^1];

            // Divide and conquer.
            double setTolerance = tolerance <= 0.0 ? GeoSharkMath.Epsilon : tolerance;

            double startT = curve.Knots[0];
            double startLength = 0.0;

            double endT = curve.Knots[^1];
            double endLength = setCurveLength;

            while (endLength - startLength > setTolerance)
            {
                double midT = (startT + endT) / 2;
                double midLength = BezierCurveLength(curve, midT);

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
        /// Computes the closest point on a curve to a given point.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="point">Point to search from.</param>
        /// <param name="t">Parameter of local closest point.</param>
        /// <returns>The closest point on the curve.</returns>
        public static Point3  CurveClosestPoint(ICurve curve, Point3  point, out double t)
        {
            t = CurveClosestParameter(curve, point);
            return LinearAlgebra.PointDehomogenizer(Evaluation.CurvePointAt(curve, t));
        }

        /// <summary>
        /// Computes the closest parameters on a curve to a given point.<br/>
        /// <em>Corresponds to page 244 chapter six from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="point">Point to search from.</param>
        /// <returns>The closest parameter on the curve.</returns>
        public static double CurveClosestParameter(ICurve curve, Point3 point)
        {
            double minimumDistance = double.PositiveInfinity;
            double tParameter = default(double);
            List<Point3> ctrlPts = curve.ControlPoints;

            (List<double> tValues, List<Point3> pts) = Tessellation.CurveRegularSample(curve, ctrlPts.Count * curve.Degree);

            for (int i = 0; i < pts.Count - 1; i++)
            {
                double t0 = tValues[i];
                double t1 = tValues[i + 1];

                Point3 pt0 = pts[i];
                Point3 pt1 = pts[i + 1];

                (double tValue, Point3 pt) projection = Trigonometry.ClosestPointToSegment(point, pt0, pt1, t0, t1);
                double distance = projection.pt.DistanceTo(point);

                if (!(distance < minimumDistance)) continue;
                minimumDistance = distance;
                tParameter = projection.tValue;
            }

            int maxIterations = 5;
            int j = 0;
            // Two zero tolerances can be used to indicate convergence:
            double tol1 = GeoSharkMath.MaxTolerance; // a measure of Euclidean distance;
            double tol2 = 0.0005; // a zero cosine measure.
            double tVal0 = curve.Knots[0];
            double tVal1 = curve.Knots[^1];
            bool isCurveClosed = (ctrlPts[0] - ctrlPts[^1]).SquareLength < GeoSharkMath.Epsilon;
            double Cu = tParameter;

            // To avoid infinite loop we limited the interaction.
            while (j < maxIterations)
            {
                List<Vector3d> e = Evaluation.RationalCurveDerivatives(curve, Cu, 2);
                Vector3 diff = e[0] - new Vector3{point.X, point.Y, point.Z}; // C(u) - P

                // First condition, point coincidence:
                // |C(u) - p| < e1
                double c1v = diff.Length();
                bool c1 = c1v <= tol1;

                // Second condition, zero cosine:
                // C'(u) * (C(u) - P)
                // ------------------ < e2
                // |C'(u)| |C(u) - P|
                double c2n = Vector3.Dot(e[1], diff);
                double c2d = (e[1] * c1v).Length;
                double c2v = c2n / c2d;
                bool c2 = Math.Abs(c2v) <= tol2;

                // If at least one of these conditions is not satisfied,
                // a new value, ui+l> is computed using the NewtonIteration.
                // Then two more conditions are checked.
                if (c1 && c2) return Cu;
                double ct = NewtonIteration(Cu, e, diff);

                // Ensure that the parameter stays within the boundary of the curve.
                if (ct < tVal0) ct = isCurveClosed ? tVal1 - (ct - tVal0) : tVal0;
                if (ct > tVal1) ct = isCurveClosed ? tVal0 + (ct - tVal1) : tVal1;

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
        private static double NewtonIteration(double u, List<Vector3d> derivativePts, Vector3 difference)
        {
            // The distance from P to C(u) is minimum when f(u) = 0, whether P is on the curve or not.
            // C'(u) * ( C(u) - P ) = 0 = f(u)
            // C(u) is the curve, p is the point, * is a dot product
            double f = Vector3.Dot(derivativePts[1], difference);

            //	f' = C"(u) * ( C(u) - p ) + C'(u) * C'(u)
            double s0 = Vector3.Dot(derivativePts[2], difference);
            double s1 = Vector3.Dot(derivativePts[1], derivativePts[1]);
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
        public static double CurveParameterAtLength(NurbsCurve curve, double segmentLength, double tolerance = -1)
        {
            if (segmentLength < GeoSharkMath.Epsilon) return curve.Knots[0];
            if (Math.Abs(curve.Length() - segmentLength) < GeoSharkMath.Epsilon) return curve.Knots[^1];

            List<ICurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
            int i = 0;
            double curveLength = -GeoSharkMath.Epsilon;

            while (curveLength < segmentLength && i < curves.Count)
            {
                double bezierLength = BezierCurveLength(curve);
                curveLength += bezierLength;

                if (segmentLength < curveLength + GeoSharkMath.Epsilon)
                    return BezierCurveParamAtLength(curve, segmentLength, tolerance, bezierLength);
                i++;
            }

            return -1;
        }
    }
}