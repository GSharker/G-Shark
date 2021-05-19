using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometrySharp.Operation
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
        public static (List<double> tvalues, List<Vector3> pts) CurveRegularSample(ICurve curve, int numSamples)
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
        /// <param name="curve">The curve to sampling.</param>
        /// <param name="tolerance">The tolerance for the adaptive division.</param>
        /// <returns>A tuple collecting the parameter where it was sampled and the points.</returns>
        public static (List<double> tValues, List<Vector3> pts) CurveAdaptiveSample(ICurve curve, double tolerance = 1e-6)
        {
            if (curve.Degree != 1) return CurveAdaptiveSampleRange(curve, curve.Knots[0], curve.Knots[^1], tolerance);
            Knot copyKnot = new Knot(curve.Knots);
            copyKnot.RemoveAt(0);
            copyKnot.RemoveAt(copyKnot.Count - 1);
            return (copyKnot, curve.ControlPoints);
        }

        /// <summary>
        /// Samples a curve in an adaptive way.
        /// Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf
        /// https://www.modelical.com/en/grasshopper-scripting-107/
        /// </summary>
        /// <param name="curve">The curve to sampling.</param>
        /// <param name="start">The start parameter for sampling.</param>
        /// <param name="end">The end parameter for sampling.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme.
        /// If tolerance is <= 0.0, the tolerance used is set as MAXTOLERANCE (1e-6).</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Vector3> pts) CurveAdaptiveSampleRange(ICurve curve, double start, double end, double tolerance)
        {
            double setTolerance = (tolerance <= 0.0) ? GeoSharpMath.MAXTOLERANCE : tolerance;

            // Sample curve at three pts.
            Random random = new Random();
            double t = 0.5 + 0.2 * random.NextDouble();
            double mid = start + (end - start) * t;

            Vector3 pt1 = LinearAlgebra.PointDehomogenizer(Evaluation.CurvePointAt(curve, start));
            Vector3 pt2 = LinearAlgebra.PointDehomogenizer(Evaluation.CurvePointAt(curve, mid));
            Vector3 pt3 = LinearAlgebra.PointDehomogenizer(Evaluation.CurvePointAt(curve, end));

            Vector3 diff = pt1 - pt3;
            Vector3 diff2 = pt1 - pt2;

            if ((Vector3.Dot(diff, diff) < setTolerance && Vector3.Dot(diff2, diff2) > setTolerance)
                || !Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, setTolerance))
            {
                // Get the exact middle value or a random value start + (end - start) * (0.45 + 0.1 * random.NextDouble());
                double tMiddle = start + (end - start) * 0.5;

                // Recurse the two halves.
                (List<double> tValues, List<Vector3> pts) leftHalves = CurveAdaptiveSampleRange(curve, start, tMiddle, tolerance);
                (List<double> tValues, List<Vector3> pts) rightHalves = CurveAdaptiveSampleRange(curve, tMiddle, end, tolerance);

                List<double> tMerged = leftHalves.tValues.SkipLast(1).Concat(rightHalves.tValues).ToList();
                List<Vector3> ptsMerged = leftHalves.pts.SkipLast(1).Concat(rightHalves.pts).ToList();

                return (tMerged, ptsMerged);
            }
            else
            {
                return (new List<double> { start, end }, new List<Vector3> { pt1, pt3 });
            }
        }
    }
}
