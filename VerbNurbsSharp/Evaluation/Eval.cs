using VerbNurbsSharp.Core;
using System.Collections.Generic;
using System;

namespace VerbNurbsSharp.Evaluation
{
    public class Eval
    {
        /// <summary>
        /// Find the span on the knot Array without supplying n
        /// </summary>
        /// <param name="degree">integer degree of function</param>
        /// <param name="u">float parameter</param>
        /// <param name="knots">array of nondecreasing knot values</param>
        /// <returns></returns>
        public static int KnotSpan(int degree, float u, KnotArray knots)
        {
            return KnotSpanGivenN(knots.Count - degree - 2, degree, u, knots);
        }

        /// <summary>
        /// find the span on the knot list of the given parameter, (corresponds to algorithm 2.1 from the NURBS book, piegl & Tiller 2nd edition)
        /// 
        /// </summary>
        /// <param name="n">integer number of basis functions - 1 = knots.length - degree - 2</param>
        /// <param name="degree">integer degree of function</param>
        /// <param name="u">parameter</param>
        /// <param name="knots">array of nondecreasing knot values</param>
        /// <returns>the index of the knot span</returns>
        public static int KnotSpanGivenN(int n, int degree, float u, KnotArray knots)
        {
            if (u > knots[n + 1] - Constants.EPSILON)
                return n;

            if (u < knots[degree] + Constants.EPSILON)
                return degree;

            var low = degree;
            var high = n + 1;
            int mid = (int)Math.Floor((decimal)(low + high) / 2);

            while (u < knots[mid] || u >= knots[mid + 1])
            {
                if (u < knots[mid])
                    high = mid;
                else
                    low = mid;

                mid = (int)Math.Floor((decimal)(low + high) / 2);
            }

            return mid;
        }

        /// <summary>
        /// Transform a 1d array of points into their homogeneous equivalents
        /// </summary>
        /// <param name="controlPoints">1d array of control points, (actually a 2d array of size (m x dim) )</param>
        /// <param name="weights">array of control point weights, the same size as the array of control points (m x 1)</param>
        /// <returns>
        /// 1d array of control points where each point is (wi*pi,  i) where wi
        ///i the ith control point weight and pi is the ith control point,
        ///hence the dimension of the point is dim + 1
        ///</returns>
        public static List<Point> Homogenize1d(List<Point> pts, List<double> weights = null)
        {
            var rows = pts.Count;
            var dim = pts[0].Count;
            var homoPts = new List<Point>();
            double wt = 0.0;
            Point refPt = new Point();
            weights = weights != null ? weights : Sets.RepeatData(1.0d, pts.Count);
            for (int i = 0; i < rows; i++)
            {
                var pt = new Point();
                refPt = pts[i];
                wt = weights[i];
                for (int k = 0; k < dim; k++)
                    pt.Add(refPt[k] * wt);
                pt.Add(wt);
                homoPts.Add(pt);
            }
            return homoPts;
        }
    }
}
