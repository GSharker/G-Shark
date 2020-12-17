using VerbNurbsSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Evaluation
{
    public class Make
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="degree"></param>
        /// <param name="homogeneousPoints"></param>
        /// <param name="startTangent"></param>
        /// <param name="endTangent"></param>
        /// <returns></returns>
        internal static NurbsCurveData RationalInterpCurve(List<Point> points, int degree = 3, bool homogeneousPoints = false, Point startTangent = null, Point endTangent = null)
        {
            // 0) build knot vector for curve by normalized chord length
            // 1) construct effective basis function in square matrix (W)
            // 2) construct set of coordinattes to interpolate vector (p)
            // 3) set of control points (c)
            //  Wc = p
            // 4) solve for c in all 3 dimensions
            if (points.Count < degree + 1)
                throw new ArgumentException("You need to supply at least degree + 1 points! You only supplied " + points.Count + " points.");
            var us = new List<double> { 0.0 };
            for (int i = 1; i < points.Count; i++)
            {
                var chord = Vec.Norm(Vec.Sub(Vec.FromPoint(points[i]), Vec.FromPoint(points[i - 1])));
                var last = us[us.Count - 1];
                us.Add(last + chord);
            }

            //Normalize
            var max = us[us.Count - 1];
            for (int i = 0; i < us.Count; i++)
                us[i] = us[i] / max;

            var knotsStart = Vec.Rep(degree + 1, 0.0);
            var hasTangents = startTangent != null && endTangent != null;
            var start = hasTangents ? 0 : 1;
            var end = hasTangents ? us.Count - degree + 1 : us.Count - degree;

            for (int i = start; i < end; i++)
            {
                var weightSums = 0.0;
                for (int j = 0; j < degree; j++)
                    weightSums += us[i + j];
                knotsStart.Add((1 / degree) * weightSums);
            }
            var knots = knotsStart.Concat(Vec.Rep(degree + 1, 1.0)).ToList();
            var A = new Matrix();
            var n = hasTangents ? points.Count + 1 : points.Count - 1;
            var lst = hasTangents ? 1 : 0;
            var ld = hasTangents ? points.Count - (degree - 1) : points.Count - (degree + 1);

            foreach(var u in us)
            {
                var span = Eval.KnotSpanGivenN(n, degree, u, knots);
            }

            return null;
        }
    }
}
