using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Provides various methods for dividing and splitting NURBS geometry.
    /// </summary>
    public class Divide
    {
        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="divisions">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        internal static List<double> CurveByCount(ICurve curve, int divisions)
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
        internal static (List<double> tValues, List<double> lengths) CurveByLength(ICurve curve, double length)
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
                        GeoSharkMath.MaxTolerance, curveLengths[i]); //***this is getting the parameter on the next curve. does this mean decompose into bezier maintains original params?

                    tValues.Add(t);
                    divisionLengths.Add(segmentLength);

                    segmentLength += length;
                }

                sum2 += curveLengths[i];
                i++;
            }

            return (tValues, divisionLengths);
        }
    }
}
