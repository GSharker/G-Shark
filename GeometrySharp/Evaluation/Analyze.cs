using System;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
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
                sum += RationalBezierCurveArcLength(tempCrv, param, gaussDegIncrease);
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
        public static double RationalBezierCurveArcLength(NurbsCurve curve, double u = -1.0, int gaussDegIncrease = 16)
        {
            var uSet = u < 0.0 ? curve.Knots.Last() : u;
            var z = (uSet - curve.Knots[0]) / 2;
            var sum = 0.0;
            var gaussDegree = curve.Degree + gaussDegIncrease;

            for (int i = 0; i < gaussDegree; i++)
            {
                var cu = z * LegendreGaussData.tValues[gaussDegree][i] + z + curve.Knots[0];
                var tan = Eval.RationalCurveDerivatives(curve, cu, 1);

                sum += LegendreGaussData.cValues[gaussDegree][i] * tan[1].Length();
            }

            return z * sum;
        }
    }
}