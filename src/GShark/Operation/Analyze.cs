using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Enum;
using GShark.Geometry.Interfaces;
using GShark.Operation.Enum;
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
        /// A higher number yields a more exact result, default set to 17.
        /// </param>
        /// <returns>The approximate length.</returns>
        public static double CurveLength(NurbsCurve curve, double u = -1.0, int gaussDegIncrease = 17)
        {
            double uSet = u < 0.0 ? curve.Knots.Last() : u;

            List<NurbsCurve> crvs = Modify.DecomposeCurveIntoBeziers(curve);
            int i = 0;
            double sum = 0.0;
            NurbsCurve tempCrv = crvs[0];

            while (i < crvs.Count && tempCrv.Knots[0] + GSharkMath.Epsilon < uSet)
            {
                tempCrv = crvs[i];
                double param = Math.Min(tempCrv.Knots.Last(), uSet);
                sum += BezierCurveLength(tempCrv, param, gaussDegIncrease);
                i += 1;
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
        public static Vector3 Curvature(Vector3 derivative1, Vector3 derivative2)
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
                throw new Exception("Curvature is infinite.");
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
        public static double BezierCurveLength(NurbsCurve curve, double u = -1.0, int gaussDegIncrease = 17)
        {
            double uSet = u < 0.0 ? curve.Knots.Last() : u;
            double z = (uSet - curve.Knots[0]) / 2;
            double sum = 0.0;
            int gaussDegree = curve.Degree + gaussDegIncrease;

            for (int i = 0; i < gaussDegree; i++)
            {
                double cu = z * LegendreGaussData.tValues[gaussDegree][i] + z + curve.Knots[0];
                List<Vector3> tan = Evaluation.RationalCurveDerivatives(curve, cu);

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
        //ToDo Refactor to remove curveLength parameter.
        public static double BezierCurveParamAtLength(NurbsCurve curve, double segmentLength, double tolerance,
            double curveLength = -1)
        {
            if (segmentLength < 0) return curve.Knots[0];

            // We compute the whole length, if the curve lengths is not provided.
            double setCurveLength = curveLength < 0 ? BezierCurveLength(curve) : curveLength;

            if (segmentLength > setCurveLength) return curve.Knots[curve.Knots.Count - 1];

            // Divide and conquer.
            double setTolerance = tolerance <= 0.0 ? GSharkMath.Epsilon : tolerance;

            double startT = curve.Knots[0];
            double startLength = 0.0;

            double endT = curve.Knots[curve.Knots.Count - 1];
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
        public static Point3 CurveClosestPoint(NurbsCurve curve, Point3 point, out double t)
        {
            t = CurveClosestParameter(curve, point);
            return Evaluation.CurvePointAt(curve, t);
        }

        /// <summary>
        /// Computes the closest parameters on a curve to a given point.<br/>
        /// <em>Corresponds to page 244 chapter six from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="point">Point to search from.</param>
        /// <returns>The closest parameter on the curve.</returns>
        public static double CurveClosestParameter(NurbsCurve curve, Point3 point)
        {
            double minimumDistance = double.PositiveInfinity;
            double tParameter = 0D;
            List<Point3> ctrlPts = curve.ControlPointLocations;

            (List<double> tValues, List<Point3> pts) = Tessellation.CurveRegularSample(curve, ctrlPts.Count * curve.Degree);

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
                List<Vector3> e = Evaluation.RationalCurveDerivatives(curve, Cu, 2);
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
                // a new value, ui+l> is computed using the NewtonIterationCurve.
                // Then two more conditions are checked.
                if (c1 && c2) return Cu;
                double ct = NewtonIterationCurve(Cu, e, diff);

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
        /// Computes the closest parameters on a surface to a given point.<br/>
        /// <em>Corresponds to page 244 chapter six from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface object.</param>
        /// <param name="point">Point to search from.</param>
        /// <returns>The closest parameter on the surface.</returns>
        public static (double u, double v) SurfaceClosestParameter(NurbsSurface surface, Point3 point)
        {
            double minimumDistance = double.PositiveInfinity;
            (double u, double v) selectedUV = (0.5, 0.5);
            NurbsSurface splitSrf = surface;
            double param = 0.5;
            int maxIterations = 5;

            for (int i = 0; i < maxIterations; i++)
            {
                NurbsSurface[] surfaces = splitSrf.Split(0.5, SplitDirection.Both);
                Point3[] pts = surfaces.Select(s => s.PointAt(0.5, 0.5)).ToArray();
                double[] distanceBetweenPts = pts.Select(point.DistanceTo).ToArray();
                if (distanceBetweenPts.All(d => d > minimumDistance)) break;

                (double, double)[] srfUV = DefiningUV(selectedUV, param);

                for (int j = 0; j < distanceBetweenPts.Length; j++)
                {
                    if (!(distanceBetweenPts[j] < minimumDistance)) continue;
                    minimumDistance = distanceBetweenPts[j];
                    selectedUV = srfUV[j];
                    splitSrf = surfaces[j];
                }

                param *= 0.5;
            }

            int t = 0;
            // Two zero tolerances can be used to indicate convergence:
            double tol1 = GSharkMath.MaxTolerance; // a measure of Euclidean distance;
            double tol2 = 0.0005; // a zero cosine measure.
            double minU = surface.KnotsU[0];
            double maxU = surface.KnotsU.Last();
            double minV = surface.KnotsV[0];
            double maxV = surface.KnotsV.Last();
            bool closedDirectionU = surface.IsClosed(SurfaceDirection.U);
            bool closedDirectionV = surface.IsClosed(SurfaceDirection.V);

            // To avoid infinite loop we limited the interaction.
            while (t < maxIterations)
            {
                // Get derivatives.
                var eval = Evaluation.RationalSurfaceDerivatives(surface, selectedUV.u, selectedUV.v, 2);

                // Convergence criteria:
                // First condition, point coincidence:
                // |S(u,v) - p| < e1
                Vector3 diff = eval[0, 0] - new Vector3(point);
                double c1v = diff.Length;
                bool c1 = c1v <= tol1;

                // Second condition, zero cosine:
                // |Su(u,v)*(S(u,v) - P)|
                // ----------------------  < e2
                // |Su(u,v)| |S(u,v) - P|
                //
                // |Sv(u,v)*(S(u,v) - P)|
                // ----------------------  < e2
                // |Sv(u,v)| |S(u,v) - P|
                double c2an = eval[1, 0] * diff;
                double c2ad = eval[1, 0].Length * c1v;

                double c2bn = eval[0, 1] * diff;
                double c2bd = eval[0, 1].Length * c1v;

                double c2av = c2an / c2ad;
                double c2bv = c2bn / c2bd;

                bool c2a = c2av <= tol2;
                bool c2b = c2bv <= tol2;

                // If all the criteria are satisfied we are done.
                if (c1 && c2a && c2b)
                {
                    return selectedUV;
                }

                // Otherwise a new value ( Ui + 1, Vi + 1) is computed using Eq. 6.7
                var ct = NewtonIterationSurface(selectedUV, eval, diff);

                // Ensure the parameters stay in range (Ui+1 E [minU, maxU] and Vi+1 E [minV, maxV]).
                if (ct.u < minU)
                {
                    ct = (closedDirectionU) ? (maxU - (minU - ct.u), ct.v) : (minU, ct.v);
                }

                if (ct.u > maxU)
                {
                    ct = (closedDirectionU) ? (minU + (ct.u - maxU), ct.v) : (maxU, ct.v);
                }

                if (ct.v < minV)
                {
                    ct = (closedDirectionV) ? (ct.u, maxV - (minV - ct.v)) : (ct.u, minV);
                }

                if (ct.v > maxV)
                {
                    ct = (closedDirectionV) ? (ct.u, minV + (ct.v - maxV)) : (ct.u, maxV);
                }

                // Parameters do not change significantly.
                double c3u = (eval[1, 0] * (ct.u - selectedUV.u)).Length;
                double c3v = (eval[0, 1] * (ct.v - selectedUV.v)).Length;

                if (c3u + c3v < tol1)
                {
                    return selectedUV;
                }

                selectedUV = ct;
                t++;
            }

            return selectedUV;
        }

        /// <summary>
        /// Newton iteration to minimize the distance between a point and a surface.
        /// <em>Corresponds to Eq. 6.5 at page 232 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="uv">The parameter uv obtained at the ith Newton iteration.</param>
        /// <param name="derivatives">Derivatives of the surface identify as S(u,v).</param>
        /// <param name="r">Representing the difference from S(u,v) - P.</param>
        /// <returns>The minimized parameter.</returns>
        private static (double u, double v) NewtonIterationSurface((double u, double v) uv, Vector3[,] derivatives, Vector3 r)
        {
            Vector3 Su = derivatives[1, 0];
            Vector3 Sv = derivatives[0, 1];

            Vector3 Suu = derivatives[2, 0];
            Vector3 Svv = derivatives[0, 2];

            Vector3 Suv = derivatives[1, 1];
            Vector3 Svu = derivatives[1, 1];

            double f = Su * r;
            double g = Sv * r;

            // Eq. 6.5
            Vector k = new Vector { -f, -g };

            Matrix J = new Matrix
            {
                new List<double> {Su * Su + Suu * r, Su * Sv + Suv * r},
                new List<double> {Su * Sv + Svu * r, Sv * Sv + Svv * r}
            };

            // Eq. 6.6
            Matrix matrixLu = Matrix.Decompose(J, out int[] permutation);
            Vector d = Matrix.Solve(matrixLu, permutation, k);

            // Eq. 6.7
            return (d[0] + uv.u, d[1] + uv.v);
        }

        /// <summary>
        /// Defines the U and V parameters for a surface split in both direction, subtracting or adding half of the input parameter based on the quadrant.
        /// </summary>
        private static (double u, double v)[] DefiningUV((double u, double v) surfaceUV, double parameter)
        {
            double halfParameter = parameter * 0.5;
            var UV = new (double u, double v)[4]
            {
                (surfaceUV.u + halfParameter, surfaceUV.v - halfParameter),
                (surfaceUV.u + halfParameter, surfaceUV.v + halfParameter),
                (surfaceUV.u - halfParameter, surfaceUV.v - halfParameter),
                (surfaceUV.u - halfParameter, surfaceUV.v + halfParameter)
            };

            return UV;
        }

        /// <summary>
        /// Newton iteration to minimize the distance between a point and a curve.
        /// </summary>
        /// <param name="u">The parameter obtained at the ith Newton iteration.</param>
        /// <param name="derivativePts">Point on curve identify as C'(u)</param>
        /// <param name="difference">Representing the difference from C(u) - P.</param>
        /// <returns>The minimized parameter.</returns>
        private static double NewtonIterationCurve(double u, List<Vector3> derivativePts, Vector3 difference)
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
        public static double CurveParameterAtLength(NurbsCurve curve, double segmentLength, double tolerance = -1)
        {
            if (segmentLength < GSharkMath.Epsilon) return curve.Knots[0];
            if (Math.Abs(curve.Length - segmentLength) < GSharkMath.Epsilon) return curve.Knots[curve.Knots.Count - 1];

            List<NurbsCurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
            int i = 0;
            double curveLength = -GSharkMath.Epsilon;
            double segmentLengthLeft = segmentLength;

            // Iterate through the curves consuming the bezier's, summing their length along the way.
            while (curveLength < segmentLength && i < curves.Count)
            {
                double bezierLength = BezierCurveLength(curves[i]);
                curveLength += bezierLength;

                if (segmentLength < curveLength + GSharkMath.Epsilon)
                    return BezierCurveParamAtLength(curves[i], segmentLengthLeft, tolerance, bezierLength);
                i++;
                segmentLengthLeft -= bezierLength;
            }

            return -1;
        }

        /// <summary>
        /// Extracts the isoparametric curves (isocurves) at the given parameter and surface direction.
        /// </summary>
        /// <param name="surface">The surface object to extract the isocurve.</param>
        /// <param name="parameter">The parameter between 0.0 to 1.0 whether the isocurve will be extracted.</param>
        /// <param name="direction">The U or V direction whether the isocurve will be extracted.</param>
        /// <returns>The isocurve extracted.</returns>
        public static NurbsCurve Isocurve(NurbsSurface surface, double parameter, SurfaceDirection direction)
        {
            KnotVector knots = (direction == SurfaceDirection.V) ? surface.KnotsV : surface.KnotsU;
            int degree = (direction == SurfaceDirection.V) ? surface.DegreeV : surface.DegreeU;

            Dictionary<double, int> knotMultiplicity = knots.Multiplicities();
            // If the knotVector already exists in the array, don't make duplicates.
            double knotKey = -1;
            foreach (KeyValuePair<double, int> keyValuePair in knotMultiplicity)
            {
                if (!(Math.Abs(parameter - keyValuePair.Key) < GSharkMath.Epsilon)) continue;
                knotKey = keyValuePair.Key;
                break;
            }

            int knotToInsert = degree + 1;
            if (knotKey >= 0)
            {
                knotToInsert = knotToInsert - knotMultiplicity[knotKey];
            }

            // Insert knots
            NurbsSurface refinedSurface = surface;
            if (knotToInsert > 0)
            {
                List<Double> knotsToInsert = Sets.RepeatData(parameter, knotToInsert);
                refinedSurface = Modify.SurfaceKnotRefine(surface, knotsToInsert, direction);
            }

            // Obtain the correct index of control points to extract.
            int span = knots.Span(degree, parameter);

            if (Math.Abs(parameter - knots[0]) < GSharkMath.Epsilon)
            {
                span = 0;
            }
            if (Math.Abs(parameter - knots.Last()) < GSharkMath.Epsilon)
            {
                span = (direction == SurfaceDirection.V)
                    ? refinedSurface.ControlPoints[0].Count - 1
                    : refinedSurface.ControlPoints.Count - 1;
            }

            return direction == SurfaceDirection.V 
                ? new NurbsCurve(refinedSurface.DegreeU, refinedSurface.KnotsU, Sets.Reverse2DMatrixData(refinedSurface.ControlPoints)[span]) 
                : new NurbsCurve(refinedSurface.DegreeV, refinedSurface.KnotsV, refinedSurface.ControlPoints[span]);
        }
    }
}