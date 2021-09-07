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
        /// The curves are brought to a common degree and knots.
        /// </summary>
        internal static IList<NurbsCurve> HomogenizedCurves(IList<NurbsCurve> copyCurves)
        {
            // Unify knots, normalized them.
            copyCurves = copyCurves
                .Select(curve => curve.Knots.GetDomain(curve.Degree).Length > 1
                    ? new NurbsCurve(curve.Degree, curve.Knots.Normalize(), curve.ControlPoints)
                    : curve).ToList();

            // Unify curves by degree.
            int targetDegree = copyCurves.Max(c => c.Degree);
            copyCurves = copyCurves
                .Select(curve => curve.Degree != targetDegree
                    ? Modify.ElevateDegree(curve, targetDegree)
                    : curve).ToList();

            // Unify curves by knots.
            KnotVector combinedKnots = copyCurves.First().Knots.Copy();
            foreach (NurbsCurve curve in copyCurves.Skip(1))
            {
                combinedKnots.AddRange(curve.Knots.Where(k => !combinedKnots.Contains(k)).ToList());
            }

            copyCurves = (from curve in copyCurves
                          let knotToInsert = combinedKnots.OrderBy(k => k).Where(k => !curve.Knots.Contains(k)).ToKnot()
                          select Modify.CurveKnotRefine(curve, knotToInsert)).ToList();
            return copyCurves;
        }
    }
}
