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
        public static int knotSpan(int degree, float u, KnotArray knots)
        {
            return knotSpanGivenN(knots.Count - degree - 2, degree, u, knots);
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
        public static int knotSpanGivenN(int n, int degree, float u, KnotArray knots)
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

        public static List<Point> Homogenize1d(List<Point> controlPoints, List<double> weights)
        {
            throw new NotImplementedException();
        }
    }
}
