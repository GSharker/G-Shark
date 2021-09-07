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
    }
}
