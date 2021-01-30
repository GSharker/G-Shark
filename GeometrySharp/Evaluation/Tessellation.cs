using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
    /// <summary>
    /// Tessellation contains static, immutable algorithms for tessellation of NURBS curves and sufaces. Tessellation is the decomposition
    /// of the analytical NURBS representation into discrete meshes or polylines that are useful for drawing.
    /// Some of these algorithms are "adaptive" - using certain heuristics to sample geometry where such samples make sense - while
    /// others are "regular" in that they sample regularly throughout a parametric domain.There are tradeoffs here.While
    /// adaptive algorithms can sometimes yield "better" results that are smaller or more economical, this can sometimes come at
    /// increased computational cost.For example, it is sometimes necessarily to compute higher order derivatives in order to
    /// obtain these more economical results.Your usage of these algorithms should consider these tradeoffs.
    /// </summary>
    public class Tessellation
    {
        /// <summary>
        /// Sample a NURBS curve at equally spaced parametric intervals.
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="numSamples">Number of samples.</param>
        /// <returns>Return a tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tvalues, List<Vector3> pts) RegularSample(NurbsCurve curve, int numSamples)
        {
            if (numSamples < 1)
                throw new Exception("Number of sample must be at least 1 and not negative.");

            var start = curve.Knots[0];
            var end = curve.Knots.Last();
            var span = (end - start) / (numSamples - 1);
            var pts = new List<Vector3>();
            var tValues = new List<double>();

            for (int i = 0; i < numSamples; i++)
            {
                var t = start + span * i;

                var ptEval = curve.PointAt(t);
                pts.Add(ptEval);
                tValues.Add(t);
            }

            return (tValues, pts);
        }

        /// <summary>
        /// Sample a curve in an adaptive way.
        /// Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme.</param>
        /// <returns>Return a tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Vector3> pts) AdaptiveSample(NurbsCurve curve, double tolerance = 1e-6)
        {
            var tValues = new List<double>();
            var pts = new List<Vector3>();
            var start = curve.Knots[0];
            var end = curve.Knots.Last();

            if (curve.Degree != 1) return AdaptiveSampleRange(curve, start, end, tolerance);

            var controlPoints = curve.ControlPoints;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                tValues.Add(curve.Knots[i + 1]);
                pts.Add(controlPoints[i]);
            }

            return (tValues, pts);
        }

        // ToDo tolerance can't be 0.0, in this case rhino looks like usa a regular sample, what do we want to do?
        // Note investigate it this method can be simplify using this solution.
        // https://www.modelical.com/en/grasshopper-scripting-107/
        /// <summary>
        /// Sample a curve in an adaptive way.
        /// Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="start">Parameter for sampling.</param>
        /// <param name="end">Parameter for sampling.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme.</param>
        /// <returns>Return a tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Vector3> pts) AdaptiveSampleRange(NurbsCurve curve, double start, double end, double tolerance = 1e-6)
        {
            var tValues = new List<double>();
            var pts = new List<Vector3>();

            // Sample curve at three pts.
            var random = new Random();
            var t = 0.5 + 0.2 * random.NextDouble();
            var mid = start + (end - start) * t;

            var pt1 = curve.PointAt(start);
            var pt2 = curve.PointAt(mid);
            var pt3 = curve.PointAt(end);

            var diff = pt1 - pt3;
            var diff2 = pt1 - pt2;

            if ((Vector3.Dot(diff, diff) < tolerance && Vector3.Dot(diff2, diff2) > tolerance) 
                || !Trigonometry.AreThreePointsCollinear(pt1, pt2,pt3, tolerance))
            {
                // Get the exact middle value.
                var midValue = start + (end - start) * 0.5;

                // Recurse the two halves.
                var (leftValuesT,leftPts) = AdaptiveSampleRange(curve, start, midValue, tolerance);
                var (rightValuesT, rightPts) = AdaptiveSampleRange(curve, midValue, end, tolerance);

                tValues.AddRange(leftValuesT.SkipLast(1).ToList());
                tValues.AddRange(rightValuesT);

                pts.AddRange(leftPts.SkipLast(1).ToList());
                pts.AddRange(rightPts);

                return (tValues, pts);
            }
            else
            {
                return (new List<double> { start, end }, new List<Vector3> { pt1, pt3 });
            }
        }
    }
}
