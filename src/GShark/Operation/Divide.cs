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
        public static List<double> CurveByCount(ICurve curve, int divisions)
        {
            double approximatedLength = Analyze.CurveLength(curve);
            double arcLengthSeparation = approximatedLength / divisions;
            var divisionByLength = CurveByLength(curve, arcLengthSeparation);
            var tValues = divisionByLength.tValues;
            return tValues;
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
        /// Creates rotation minimized perpendicular frames (RMF) at given t parameters along the curve.
        /// <para>//Double reflection method taken from Wang, W., J¨uttler, B., Zheng, D., and Liu, Y. 2008. "Computation of rotation minimizing frame."</para>
        /// </summary>
        ///<param name="curve">The input curve.</param>
        /// ///<param name="uValues">The curve parameter values to locate perpendicular curve frames</param>
        public static List<Plane> PerpendicularFrames(ICurve curve, List<double> uValues)
        {
            var pointsOnCurve = uValues.Select(curve.PointAt).ToList(); //get points at t values
            var pointsOnCurveTan = uValues.Select(t => Evaluation.RationalCurveTangent(curve, t)).ToList(); //get tangents at t values
            var firstParameter = uValues[0]; //get first t value
            
            //Create initial frame at first parameter
            var origin = curve.PointAt(firstParameter);
            var crvTan = Evaluation.RationalCurveTangent(curve, firstParameter);
            var crvNormal = Vector3.PerpendicularTo(crvTan);
            var yAxis = Vector3.CrossProduct(crvTan, crvNormal);
            var xAxis = Vector3.CrossProduct(yAxis, crvTan);
            
            //Set initial frame
            Plane[] perpFrames = new Plane[pointsOnCurve.Count];
            perpFrames[0] = new Plane(origin, xAxis, yAxis);

            //Given boundary data(x0, t0; x1, t1) and an initial right-handed
            //orthonormal frame U0 = (r0, s0, t0) at x0, the next frame U1 = (r1, s1, t1)
            //at x1 for RMF approximation is computed by the double reflection method in
            //the following two steps.
            //
            //Step 1.Let R1 denote the reflection in the bisecting plane of the points x0
            //and x1(see Figure 4).Use R1 to map U0 to a left - handed orthonormal frame
            //UL0 = (rL0, sL0, tL0).
            //
            //Step 2.Let R2 denote the reflection in the bisecting plane of the points x1 + tL
            //0 and x1 +t1. Use R2 to map UL0 to a right - handed orthonormal frame U1 = (r1, s1, t1).
            //Output U1.

            for (int i = 0; i < pointsOnCurve.Count - 1; i++)
            {
                Vector3 v1 = pointsOnCurve[i + 1] - pointsOnCurve[i]; //compute reflection vector of R1
                double c1 = v1 * v1;
                Vector3 rLi = perpFrames[i].XAxis - (2 / c1) * (v1 * perpFrames[i].XAxis) * v1; //compute reflected rL vector by R1
                Vector3 tLi = pointsOnCurveTan[i] - (2 / c1) * (v1 * pointsOnCurveTan[i]) * v1; //compute reflected tL vector by R1
                Vector3 v2 = pointsOnCurveTan[i + 1] - tLi; //compute reflection vector of R2
                double c2 = v2 * v2;
                Vector3 rNext = rLi - (2 / c2) * (v2 * rLi) * v2; //compute reflected r vector by R2
                var sNext = Vector3.CrossProduct(pointsOnCurveTan[i + 1], rNext); //compute vector s[i+1] of next frame
                
                //create output frame
                var frameNext = new Plane();
                frameNext.Origin = pointsOnCurve[i + 1];
                frameNext.XAxis = rNext;
                frameNext.YAxis = sNext;
                frameNext.ZAxis = pointsOnCurveTan[i + 1];
                perpFrames[i + 1] = frameNext; //output frame
            }

            return perpFrames.ToList();
        }
    }
}
