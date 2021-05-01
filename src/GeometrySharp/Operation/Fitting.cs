using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;

namespace GeometrySharp.Operation
{
    /// <summary>
    /// Fitting provides functions for interpolating and approximating nurbs curves and surfaces from points.
    /// Approximation uses least squares algorithm.
    /// </summary>
    public static class Fitting
    {
        public static NurbsCurve InterpolatedCurve(List<Vector3> pts, int degree, Vector3 startTangent = null,
            Vector3 endTangent = null)
        {
            return new NurbsCurve();
        }

        public static NurbsCurve ApproximateCurve(List<Vector3> pts, int degree)
        {
            return new NurbsCurve();
        }
    }
}
