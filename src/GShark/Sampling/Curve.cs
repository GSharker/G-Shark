using System;
using System.Collections.Generic;
using System.Linq;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;

namespace GShark.Sampling
{
    /// <summary>
    /// Contains static algorithms for tessellation and division of NURBS curves.<br/>
    /// Some of these algorithms are "adaptive" - using certain heuristics to sample geometry where such samples make sense - while<br/>
    /// others are "regular" in that they sample regularly throughout a parametric domain.There are tradeoffs here.<br/>
    /// While adaptive algorithms can sometimes yield "better" results that are smaller or more economical, this can sometimes come at increased computational cost.
    /// For example, it is sometimes necessarily to compute higher order derivatives in order to<br/>
    /// obtain these more economical results.Your usage of these algorithms should consider these tradeoffs.
    /// </summary>
    public static class Curve
    {
        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="divisions">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        internal static List<double> ByCount(NurbsBase curve, int divisions)
        {
            double approximatedLength = Analyze.Curve.Length(curve);
            double arcLengthSeparation = approximatedLength / divisions;
            var divisionByLength = ByLength(curve, arcLengthSeparation);
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
        internal static (List<double> tValues, List<double> lengths) ByLength(NurbsBase curve, double length)
        {
            List<NurbsBase> curves = Modify.Curve.DecomposeIntoBeziers(curve);
            List<double> curveLengths = curves.Select(NurbsBase => Analyze.Curve.BezierLength(NurbsBase)).ToList();
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

                while (segmentLength < sum + GSharkMath.Epsilon)
                {
                    double t = Analyze.Curve.BezierParameterAtLength(curves[i], segmentLength - sum2, GSharkMath.MinTolerance);

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
        /// Samples a curve at equally spaced parametric intervals.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="numSamples">Number of samples.</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        internal static (List<double> tvalues, List<Point3> pts) RegularSample(NurbsBase curve, int numSamples)
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
        public static (List<double> tValues, List<Point3> pts) AdaptiveSample(NurbsBase curve, double tolerance = GSharkMath.MinTolerance)
        {
            if (curve.Degree != 1) return AdaptiveSampleRange(curve, curve.Knots[0], curve.Knots[curve.Knots.Count - 1], tolerance);
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
        /// <param name="curve">The curve to sampling.</param>
        /// <param name="start">The start parameter for sampling.</param>
        /// <param name="end">The end parameter for sampling.</param>
        /// <param name="tolerance">Tolerance for the adaptive scheme. The default tolerance is set as (1e-6).</param>
        /// <returns>A tuple with the set of points and the t parameter where the point was evaluated.</returns>
        public static (List<double> tValues, List<Point3> pts) AdaptiveSampleRange(NurbsBase curve, double start, double end, double tolerance = GSharkMath.MinTolerance)
        {
            // Sample curve at three pts.
            Random random = new Random();
            double t = 0.5 + 0.2 * random.NextDouble();
            double mid = start + (end - start) * t;

            Point3 pt1 = Point4.PointDehomogenizer(Evaluate.Curve.PointAt(curve, start));
            Point3 pt2 = Point4.PointDehomogenizer(Evaluate.Curve.PointAt(curve, mid));
            Point3 pt3 = Point4.PointDehomogenizer(Evaluate.Curve.PointAt(curve, end));

            Vector3 diff = pt1 - pt3;
            Vector3 diff2 = pt1 - pt2;

            if ((Vector3.DotProduct(diff, diff) < tolerance && Vector3.DotProduct(diff2, diff2) > tolerance)
                || !Trigonometry.ArePointsCollinear(pt1, pt2, pt3, tolerance))
            {
                // Get the exact middle value or a random value start + (end - start) * (0.45 + 0.1 * random.NextDouble());
                double tMiddle = start + (end - start) * 0.5;

                // Recurse the two halves.
                (List<double> tValues, List<Point3> pts) leftHalves = AdaptiveSampleRange(curve, start, tMiddle, tolerance);
                (List<double> tValues, List<Point3> pts) rightHalves = AdaptiveSampleRange(curve, tMiddle, end, tolerance);

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
