using GeometrySharp.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
            var left = Sets.RepeatData(0.0, degree + 1);
            var right = Sets.RepeatData(0.0, degree + 1);
            // N[0] = 1.0 by definition;
            var N = Sets.RepeatData(1.0, degree + 1);
            var saved = 0.0;
            var temp = 0.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = parameter - knots[span + 1 - j];
                right[j] = knots[span + j] - parameter;
                saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    temp = N[r] / (right[r + 1] + left[j - r]);
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
    }
}
