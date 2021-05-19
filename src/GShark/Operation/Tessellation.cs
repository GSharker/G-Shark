using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Tessellation contains static, immutable algorithms for tessellation of nurbs curves and sufaces.Tessellation is the decomposition
    /// of the analytical nurbs representation into discrete meshes or polylines that are useful for drawing.
    /// Some of these algorithms are "adaptive" - using certain heuristics to sample geometry where such samples make sense - while
    /// others are "regular" in that they sample regularly throughout a parametric domain.There are tradeoffs here.While
    /// adaptive algorithms can sometimes yield "better" results that are smaller or more economical, this can sometimes come at
    /// increased computational cost.For example, it is sometimes necessarily to compute higher order derivatives in order to
    /// obtain these more economical results.Your usage of these algorithms should consider these tradeoffs.
    /// </summary>
    public class Tessellation
    {
        /// <summary>
        /// Samples a nurbs curve at equally spaced parametric intervals.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="numSamples">Number of samples.</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tvalues, List<Vector3> pts) RegularSample(ICurve curve, int numSamples)
        {
            if (numSamples < 1)
                throw new Exception("Number of sample must be at least 1 and not negative.");

            double start = curve.Knots[0];
            double end = curve.Knots[^1];

            double span = (end - start) / (numSamples - 1);
            List<Vector3> pts = new List<Vector3>();
            List<double> tValues = new List<double>();

            for (int i = 0; i < numSamples; i++)
            {
                double t = start + span * i;

                Vector3 ptEval = curve.PointAt(t);
                pts.Add(ptEval);
                tValues.Add(t);
            }

            return (tValues, pts);
        }

        /// <summary>
        /// Samples a curve in an adaptive way.
        /// Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme.</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Vector3> pts) AdaptiveSample(ICurve curve, double tolerance)
        {
            List<double> tValues = new List<double>();
            List<Vector3> pts = new List<Vector3>();
            double start = curve.Knots[0];
            double end = curve.Knots[^1];

            if (curve.Degree != 1) return AdaptiveSampleRange(curve, start, end, tolerance);

            List<Vector3> controlPoints = curve.ControlPoints;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                tValues.Add(curve.Knots[i + 1]);
                pts.Add(controlPoints[i]);
            }

            return (tValues, pts);
        }

        /// <summary>
        /// Samples a curve in an adaptive way.
        /// Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf
        /// https://www.modelical.com/en/grasshopper-scripting-107/
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="start">Parameter for sampling.</param>
        /// <param name="end">Parameter for sampling.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme.
        /// If tolerance is <= 0.0, the tolerance used is set as MAXTOLERANCE (1e-6).</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Vector3> pts) AdaptiveSampleRange(ICurve curve, double start, double end, double tolerance)
        {
            double setTolerance = (tolerance <= 0.0) ? GeoSharpMath.MAXTOLERANCE : tolerance;

            List<double> tValues = new List<double>();
            List<Vector3> pts = new List<Vector3>();

            // Sample curve at three pts.
            Random random = new Random();
            double t = 0.5 + 0.2 * random.NextDouble();
            double mid = start + (end - start) * t;

            Vector3 pt1 = curve.PointAt(start);
            Vector3 pt2 = curve.PointAt(mid);
            Vector3 pt3 = curve.PointAt(end);

            Vector3 diff = pt1 - pt3;
            Vector3 diff2 = pt1 - pt2;

            if ((Vector3.Dot(diff, diff) < setTolerance && Vector3.Dot(diff2, diff2) > setTolerance)
                || !Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, setTolerance))
            {
                // Get the exact middle value.
                double midValue = start + (end - start) * 0.5;

                // Recurse the two halves.
                (List<double> leftValuesT, List<Vector3> leftPts) = AdaptiveSampleRange(curve, start, midValue, setTolerance);
                (List<double> rightValuesT, List<Vector3> rightPts) = AdaptiveSampleRange(curve, midValue, end, setTolerance);

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
