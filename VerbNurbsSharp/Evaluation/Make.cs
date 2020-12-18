using VerbNurbsSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verb.core;

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

        public static NurbsCurveData RationalInterpCurve(List<Point> points, int degree = 3, bool homogeneousPoints = false, Point start_tangent = null, Point end_tangent = null)
        {
            if (points.Count < degree + 1)
                throw new ArgumentException("You need to supply at least degree + 1 points! You only supplied " + points.Count + " points.");

            var us = new List<double>() { 0 };
            for (int i = 1; i < points.Count; i++)
            {
                var chord = Vec.Norm(Vec.Sub(points[i], points[i - 1]));
                var last = us[us.Count - 1];
                us.Add(last + chord);
            }

            var max = us[us.Count - 1];
            for (int i = 1; i < us.Count; i++)
                us[i] = us[i] / max;

            var knotsStart = Vec.Rep(degree + 1, 0.0);

            var hasTangents = start_tangent != null && end_tangent != null;
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
            //build matrix of basis function coeffs (TODO: use sparse rep)
            var A = new Matrix();
            var n = hasTangents ? points.Count + 1 : points.Count - 1;

            var lst = hasTangents ? 1 : 0;
            var ld = hasTangents ? points.Count - (degree - 1) : points.Count - (degree + 1);

            foreach (var u in us)
            {
                var span = Eval.KnotSpanGivenN(n, degree, u, knots);
                var basisFuncs = Eval.BasisFunctionsGivenKnotSpanIndex(span, u, degree, knots);

                var ls = span - degree;

                var rowstart = Vec.Zeros1d(ls);
                var rowend = Vec.Zeros1d(ld - ls);

                A.Add(rowstart.Concat(basisFuncs).Concat(rowend).ToList());

                //if (hasTangents)
                //{
                //    var ln = A[0].Count - 2;

                //    var tanRow0 = new List<double>() { -1.0, 1.0 }.Concat(Vec.Zeros1d(ln)).ToList();
                //    var tanRow1 = Vec.Zeros1d(ln).Concat(new List<double>() { -1.0, 1.0 }).ToList();

                //    A.Splice(1, 0, tanRow0);
                //    A.spliceAndInsert(A.length - 1, 0, tanRow1);
                //}

                var dim = points[0].Count;
                var xs = new List<double>();

                var mult1 = (1 - knots[knots.Count - degree - 2]) / degree;
                var mult0 = knots[degree + 1] / degree;

                for (int i = 0; i < dim; i++)
                {
                    var b = new List<double>();
                    if (!hasTangents)
                        points.ForEach(x => b.Add(x[i]));
                    //        else
                    //{
                    //    //insert the tangents at the second and second to last index
                    //    b = [points[0][i]];
                    //    b.push(mult0 * start_tangent[i]);
                    //    for (j in 1...points.length - 1) b.push(points[j][i]);
                    //    b.push(mult1 * end_tangent[i]);
                    //    b.push(points.last()[i]);
                    //}

                    var x = Mat.Solve(A, b);
                    xs.Add(x);
                }
            }
        }
    }
}
