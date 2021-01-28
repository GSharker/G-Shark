using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Core;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
    public class Tessellation
    {
        /*
        `Tess` contains static, immutable algorithms for tessellation of NURBS curves and sufaces. Tessellation is the decomposition
        of the analytical NURBS representation into discrete meshes or polylines that are useful for drawing.
        
        Some of these algorithms are "adaptive" - using certain heuristics to sample geometry where such samples make sense - while
        others are "regular" in that they sample regularly throughout a parametric domain. There are tradeoffs here. While
        adaptive algorithms can sometimes yield "better" results that are smaller or more economical, this can sometimes come at
        increased computational cost. For example, it is sometimes necessarily to compute higher order derivatives in order to
        obtain these more economical results. Your usage of these algorithms should consider these tradeoffs.
        */

        /// <summary>
        /// Sample a NURBS curve at equally spaced parametric intervals.
        /// </summary>
        /// <param name="curve">NurbsCurve object.</param>
        /// <param name="numSamples">Number of samples.</param>
        /// <returns>Return a tuple with the set of points and the t parameter used to find the point.</returns>
        public static (List<double> tvalues, List<Vector3> pts) RationalCurveRegularSample(NurbsCurve curve, int numSamples)
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
    }
}
