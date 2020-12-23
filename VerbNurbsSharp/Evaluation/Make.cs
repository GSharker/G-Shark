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
        public static NurbsCurveData Polyline(List<Point> pts)
        {
            var knots = new KnotArray() { 0, 0 };
            double lsum = 0;

            for (int i = 0; i < pts.Count - 1; i++)
            {
                lsum += Vec.Dist(pts[i], pts[i + 1]);
                knots.Add(lsum);
            }
            knots.Add(lsum);
            knots = (KnotArray)Vec.ScalarMult(1 / lsum, knots);

            var weights = new List<double>();
            pts.ForEach(x => weights.Add(1));

            return new NurbsCurveData(1, new KnotArray(), Eval.Homogenize1d(pts, weights));
        }

        //public static NurbsCurveData RationalInterpCurve(List<Point> points, int degree = 3, bool homogeneousPoints = false, Point start_tangent = null, Point end_tangent = null) { }
    }
}
