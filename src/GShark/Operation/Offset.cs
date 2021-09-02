using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;

namespace GShark.Operation
{
    /// <summary>
    /// Collects a set of method for offsetting primitives and NURBS geometries.
    /// </summary>
    public static class Offset
    {

        /// <summary>
        /// Computes the offset of a curve.
        /// </summary>
        /// <param name="crv">The curve to offset.</param>
        /// <param name="distance">The distance of the offset.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset curve.</returns>
        public static NurbsCurve Curve(NurbsCurve crv, double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return crv;
            }

            var (tValues, pts) = Tessellation.CurveAdaptiveSample(crv);

            List<Point3> offsetPts = new List<Point3>();
            for (int i = 0; i < pts.Count; i++)
            {
                Vector3 tangent = Evaluation.RationalCurveTangent(crv, tValues[i]);
                Vector3 vecOffset = Vector3.CrossProduct(tangent, pln.ZAxis).Amplify(distance);
                offsetPts.Add(pts[i] + vecOffset);
            }

            return Fitting.InterpolatedCurve(offsetPts, crv.Degree);
        }
    }
}
