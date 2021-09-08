using System;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Operation;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Core
{
    internal static class CurveHelpers
    {
        /// <summary>
        /// Normalizes knot vectors of all curves to the same domain.
        /// </summary>
        internal static IList<NurbsCurve> NormalizedKnots(IList<NurbsCurve> curves)
        {
            // Unify knots, normalized them.
            curves = curves
                .Select(curve => curve.Knots.GetDomain(curve.Degree).Length > 1
                    ? new NurbsCurve(curve.Degree, curve.Knots.Normalize(), curve.ControlPoints)
                    : curve).ToList();

            // Unify curves by knots.
            KnotVector combinedKnots = curves.First().Knots.Copy();
            foreach (NurbsCurve curve in curves.Skip(1))
            {
                combinedKnots.AddRange(curve.Knots.Where(k => !combinedKnots.Contains(k)).ToList());
            }

            curves = (from curve in curves
                          let knotToInsert = combinedKnots.OrderBy(k => k).Where(k => !curve.Knots.Contains(k)).ToKnot()
                          select Modify.CurveKnotRefine(curve, knotToInsert)).ToList();
            return curves;
        }

        /// <summary>
        /// Elevates degree of all curves to highest degree among all curves.
        /// </summary>
        internal static IList<NurbsCurve> NormalizedDegree(IList<NurbsCurve> curves)
        {
            // Unify curves by degree.
            int targetDegree = curves.Max(c => c.Degree);
            curves = curves
                .Select(curve => curve.Degree != targetDegree
                    ? Modify.ElevateDegree(curve, targetDegree)
                    : curve).ToList();

            return curves;
        }

        /// <summary>
        /// The chord length parametrization is the most widely used method.
        /// The centripetal gives better results than the chord length method when the curve takes sharp turns.
        /// Refer to the Equations 9.4 and 9.5 for chord length parametrization, and Equation 9.6 for centripetal method on The NURBS Book(2nd Edition), pp.364-365.
        /// </summary>
        internal static List<double> Parametrization(List<Point3> pts, bool centripetal = false)
        {
            List<double> chords = new List<double> { 0.0 };
            for (int i = 1; i < pts.Count; i++)
            {
                double chord = (centripetal) ? Math.Sqrt((pts[i] - pts[i - 1]).Length) : (pts[i] - pts[i - 1]).Length;
                chords.Add(chord + chords.Last());
            }

            // Divide the individual chord length by the total chord length.
            List<double> curveParameters = new List<double>();
            double maxChordLength = chords.Last();
            for (int i = 0; i < pts.Count; i++)
            {
                curveParameters.Add(chords[i] / maxChordLength);
            }

            return curveParameters;
        }

        /// <summary>
        /// A quickSort algorithm used to order the curves in a sequential manner based on the distance between the first and last points of each curves.<br/>
        /// https://github.com/mcneel/opennurbs/blob/c20e599d1ff8f08a55d3dddf5b39e37e8b5cac06/opennurbs_curve.cpp#L3600 <br/>
        /// https://exceptionnotfound.net/quick-sort-csharp-the-sorting-algorithm-family-reunion/
        /// </summary>
        /// <param name="curves">The sets of curve to sort.</param>
        /// <returns>The set of curves sorted.</returns>
        internal static List<NurbsCurve> QuickSortCurve(IList<NurbsCurve> curves)
        {
            if (curves == null || curves.Count == 0)
            {
                throw new Exception("The set of curves is empty.");
            }

            if (curves.Count == 1)
            {
                return curves.ToList();
            }

            // creates the sets of data required.
            List<Point3[]> lines = curves.Select(c => new[] { c.StartPoint, c.EndPoint }).ToList();
            int[] indexes = new int[lines.Count];
            bool[] revers = new bool[lines.Count];
            for (int t = 0; t < lines.Count; t++)
            {
                revers[t] = false;
                indexes[t] = t;
            }

            // sort lines
            for (int ni = 1; ni < lines.Count; ni++)
            {
                int endI, endEnd, i;
                var startI = endI = ni;
                var startEnd = endEnd = 0;
                var startPoint = (revers[0]) ? lines[indexes[0]][1] : lines[indexes[0]][0];
                var endPoint = (revers[ni - 1]) ? lines[indexes[ni - 1]][0] : lines[indexes[ni - 1]][1];
                var startDistance = startPoint.DistanceTo(lines[indexes[startI]][0]);
                var endDistance = endPoint.DistanceTo(lines[indexes[endI]][0]);

                for (i = ni; i < lines.Count; i++)
                {
                    Point3 testingPoint = lines[indexes[i]][0];
                    for (int end = 0; end < 2; end++)
                    {
                        double testingDistance = startPoint.DistanceTo(testingPoint);
                        if (testingDistance < startDistance)
                        {
                            startI = i;
                            startEnd = end;
                            startDistance = testingDistance;
                        }

                        testingDistance = endPoint.DistanceTo(testingPoint);
                        if (testingDistance < endDistance)
                        {
                            endI = i;
                            endEnd = end;
                            endDistance = testingDistance;
                        }

                        testingPoint = lines[indexes[i]][1];
                    }
                }

                if (startDistance < endDistance)
                {
                    // N[index[startI]] will be first in list.
                    i = indexes[ni];
                    indexes[ni] = indexes[startI];
                    indexes[startI] = i;
                    startI = indexes[ni];
                    for (i = ni; i > 0; i--)
                    {
                        indexes[i] = indexes[i - 1];
                        revers[i] = revers[i - 1];
                    }
                    indexes[0] = startI;
                    revers[0] = (startEnd != 1);
                }
                else
                {
                    // N[index[endI]] will be next in the list.
                    i = indexes[ni];
                    indexes[ni] = indexes[endI];
                    indexes[endI] = i;
                    revers[ni] = (endEnd == 1);
                }
            }

            List<NurbsCurve> sortedCurves = indexes.Select(i => (revers[i]) ? curves[i].Reverse() : curves[i]).ToList();
            return sortedCurves;
        }
    }
}
