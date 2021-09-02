using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Contains static algorithms for tessellation of NURBS curves and sufaces.<br/>
    /// Tessellation is the decomposition of the analytical NURBS representation into discrete meshes or polylines that are useful for drawing.<br/>
    /// Some of these algorithms are "adaptive" - using certain heuristics to sample geometry where such samples make sense - while<br/>
    /// others are "regular" in that they sample regularly throughout a parametric domain.There are tradeoffs here.<br/>
    /// While adaptive algorithms can sometimes yield "better" results that are smaller or more economical, this can sometimes come at<br/>
    /// increased computational cost.For example, it is sometimes necessarily to compute higher order derivatives in order to<br/>
    /// obtain these more economical results.Your usage of these algorithms should consider these tradeoffs.
    /// </summary>
    public class Tessellation
    {
        /// <summary>
        /// Samples a NURBS curve at equally spaced parametric intervals.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="numSamples">Number of samples.</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tvalues, List<Point3> pts) CurveRegularSample(NurbsCurve curve, int numSamples)
        {
            if (numSamples < 1)
                throw new Exception("Number of sample must be at least 1 and not negative.");

            double start = curve.Knots[0];
            double end = curve.Knots[curve.Knots.Count - 1];

            double span = (end - start) / (numSamples - 1);
            List<Point3> pts = new List<Point3>();
            List<double> tValues = new List<double>();

            for (int i = 0; i < numSamples; i++)
            {
                double t = start + span * i;

                Point3 ptEval = curve.PointAt(t);
                pts.Add(ptEval);
                tValues.Add(t);
            }

            return (tValues, pts);
        }

        /// <summary>
        /// Samples a curve in an adaptive way. <br/>
        /// <em>Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf </em>
        /// </summary>
        /// <param name="curve">The curve to sampling.</param>
        /// <param name="tolerance">The tolerance for the adaptive division.</param>
        /// <returns>A tuple collecting the parameter where it was sampled and the points.</returns>
        public static (List<double> tValues, List<Point3> pts) CurveAdaptiveSample(NurbsCurve curve, double tolerance = 1e-6)
        {
            if (curve.Degree != 1) return CurveAdaptiveSampleRange(curve, curve.Knots[0], curve.Knots[curve.Knots.Count - 1], tolerance);
            KnotVector copyKnot = new KnotVector(curve.Knots);
            copyKnot.RemoveAt(0);
            copyKnot.RemoveAt(copyKnot.Count - 1);
            return (copyKnot, curve.ControlPointLocations);
        }

        /// <summary>
        /// Samples a curve in an adaptive way. <br/>
        /// <em>Corresponds to this algorithm http://ariel.chronotext.org/dd/defigueiredo93adaptive.pdf <br/>
        /// https://www.modelical.com/en/grasshopper-scripting-107/ </em>
        /// </summary>
        /// <param name="curve">The curve to sample.</param>
        /// <param name="start">The start parameter for sampling.</param>
        /// <param name="end">The end parameter for sampling.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme.
        /// If tolerance is smaller or equal 0.0, the tolerance used is set as MAX_TOLERANCE (1e-6).</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Point3> pts) CurveAdaptiveSampleRange(NurbsCurve curve, double start, double end, double tolerance)
        {
            double setTolerance = (tolerance <= 0.0) ? GSharkMath.MaxTolerance : tolerance;

            // Sample curve at three pts.
            Random random = new Random();
            double t = 0.5 + 0.2 * random.NextDouble();
            double mid = start + (end - start) * t;

            Point3 pt1 = Point4.PointDehomogenizer(Evaluation.CurvePointAt(curve, start));
            Point3 pt2 = Point4.PointDehomogenizer(Evaluation.CurvePointAt(curve, mid));
            Point3 pt3 = Point4.PointDehomogenizer(Evaluation.CurvePointAt(curve, end));

            Vector3 diff = pt1 - pt3;
            Vector3 diff2 = pt1 - pt2;

            if ((Vector3.DotProduct(diff, diff) < setTolerance && Vector3.DotProduct(diff2, diff2) > setTolerance)
                || !Trigonometry.ArePointsCollinear(pt1, pt2, pt3, setTolerance))
            {
                // Get the exact middle value or a random value start + (end - start) * (0.45 + 0.1 * random.NextDouble());
                double tMiddle = start + (end - start) * 0.5;

                // Recurse the two halves.
                (List<double> tValues, List<Point3> pts) leftHalves = CurveAdaptiveSampleRange(curve, start, tMiddle, tolerance);
                (List<double> tValues, List<Point3> pts) rightHalves = CurveAdaptiveSampleRange(curve, tMiddle, end, tolerance);

                leftHalves.tValues.RemoveAt(leftHalves.tValues.Count - 1);
                List<double> tMerged = leftHalves.tValues.Concat(rightHalves.tValues).ToList();
                leftHalves.pts.RemoveAt(leftHalves.pts.Count - 1);
                List<Point3> ptsMerged = leftHalves.pts.Concat(rightHalves.pts).ToList();

                return (tMerged, ptsMerged);
            }

            return (new List<double> { start, end }, new List<Point3> { pt1, pt3 });
        }
    }
}
