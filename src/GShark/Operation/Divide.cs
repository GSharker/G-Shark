using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace GShark.Operation
{
    /// <summary>
    /// Provides various methods for dividing and splitting NURBS geometry.
    /// </summary>
    public class Divide
    {
        /// <summary>
		/// Splits a curve into two parts at a given parameter.
		/// </summary>
		/// <param name="curve">The curve object.</param>
		/// <param name="t">The parameter where to split the curve.</param>
		/// <returns>Two new curves, defined by degree, knots, and control points.</returns>
		public static List<ICurve> SplitCurve(ICurve curve, double t)
        {
            int degree = curve.Degree;

            List<double> knotsToInsert = Sets.RepeatData(t, degree + 1);

            ICurve refinedCurve = Modify.CurveKnotRefine(curve, knotsToInsert);

            int s = curve.Knots.Span(degree, t);

            KnotVector knots0 = refinedCurve.Knots.ToList().GetRange(0, s + degree + 2).ToKnot();
            KnotVector knots1 = refinedCurve.Knots.GetRange(s + 1, refinedCurve.Knots.Count - (s + 1)).ToKnot();

            List<Point3> controlPoints0 = refinedCurve.LocationPoints.GetRange(0, s + 1);
            List<Point3> controlPoints1 = refinedCurve.LocationPoints.GetRange(s + 1, refinedCurve.LocationPoints.Count - (s + 1));

            return new List<ICurve> { new NurbsCurve(degree, knots0, controlPoints0), new NurbsCurve(degree, knots1, controlPoints1) };
        }

        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="divisions">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        public static (List<double> tValues, List<double> lengths) CurveByCount(ICurve curve, int divisions)
        {
            double approximatedLength = Analyze.CurveLength(curve);
            double arcLengthSeparation = approximatedLength / divisions;

            return CurveByLength(curve, arcLengthSeparation);
        }

        /// <summary>
        /// Divides a curve for a given length, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="length">The length separating the resultant samples.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        public static (List<double> tValues, List<double> lengths) CurveByLength(ICurve curve, double length)
        {
            List<ICurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
            List<double> curveLengths = curves.Select(nurbsCurve => Analyze.BezierCurveLength(nurbsCurve)).ToList();
            double totalLength = curveLengths.Sum();

            List<double> tValues = new List<double> { curve.Knots[0] };
            List<double> divisionLengths = new List<double> { 0.0 };

            if (length > totalLength) return (tValues, divisionLengths);

            int i = 0;
            double sum = 0.0;
            double sum2 = 0.0;
            double segmentLength = length;

            while (i < curves.Count)
            {
                sum += curveLengths[i];

                while (segmentLength < sum + GeoSharkMath.Epsilon)
                {
                    double t = Analyze.BezierCurveParamAtLength(curves[i], segmentLength - sum2,
                        GeoSharkMath.MaxTolerance, curveLengths[i]);

                    tValues.Add(t);
                    divisionLengths.Add(segmentLength);

                    segmentLength += length;
                }

                sum2 += curveLengths[i];
                i++;
            }

            return (tValues, divisionLengths);
        }

        /// <summary>
        /// Creates rotation minimized perpendicular frames at given t parameters along the curve.
        /// </summary>
        public static List<Plane> PerpendicularFrames(ICurve curve, List<double> tValues)
        {
            var pointsOnCurve = tValues.Select(t => curve.PointAt(t)).ToList();
            var pointsOnCurveTan = tValues.Select(t => Evaluation.RationalCurveTangent(curve, t)).ToList();
            var firstParameter = tValues[0];
            var origin = curve.PointAt(firstParameter);
            var crvTan = Evaluation.RationalCurveTangent(curve, firstParameter);
            var crvNormal = Vector3.PerpendicularTo(crvTan);
            var yAxis = Vector3.CrossProduct(crvTan, crvNormal);
            var xAxis = Vector3.CrossProduct(yAxis, crvTan);
            Plane[] output = new Plane[pointsOnCurve.Count];
            output[0] = new Plane(origin, xAxis, yAxis);

            //Wang, W., J¨uttler, B., Zheng, D., and Liu, Y. 2008. "Computation of rotation minimizing frame."
            for (int i = 0; i < pointsOnCurve.Count - 1; i++)
            {
                Vector3 v1 = pointsOnCurve[i + 1] - pointsOnCurve[i];
                double c1 = v1 * v1;
                Vector3 rLi = output[i].XAxis - (2 / c1) * (v1 * output[i].XAxis) * v1;
                Vector3 tLi = pointsOnCurveTan[i] - (2 / c1) * (v1 * pointsOnCurveTan[i]) * v1;
                Vector3 v2 = pointsOnCurveTan[i + 1] - tLi;
                double c2 = v2 * v2;
                Vector3 rINext = rLi - (2 / c2) * (v2 * rLi) * v2;
                var sINext = Vector3.CrossProduct(pointsOnCurveTan[i + 1], rINext);
                var pln = new Plane();
                pln.Origin = pointsOnCurve[i + 1];
                pln.XAxis = rINext;
                pln.YAxis = sINext;
                pln.ZAxis = pointsOnCurveTan[i + 1];
                output[i + 1] = pln;
            }
        }
    }
}
