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
        public static int KnotSpan(int degree, double u, KnotArray knots)
        {
            return KnotSpanGivenN(knots.Count - degree - 2, degree, u, knots);
        }

        /// <summary>
        /// find the span index on the knot list of the given parameter, 
        /// (corresponds to algorithm A2.1 from the NURBS book, piegl & Tiller 2nd edition)
        /// </summary>
        /// <param name="n">integer number of basis functions - 1 = knots.length - degree - 2</param>
        /// <param name="degree">integer degree of function</param>
        /// <param name="u">parameter</param>
        /// <param name="knots">array of nondecreasing knot values</param>
        /// <returns>the index of the knot span</returns>
        public static int KnotSpanGivenN(int n, int degree, double u, KnotArray knots)
        {
            //special case
            if (u > knots[n + 1] - Constants.EPSILON)
                return n;

            if (u < knots[degree] + Constants.EPSILON)
                return degree;

            //do binary search
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

        public static List<Vector> Homogenize1d(List<Vector> controlPoints, List<double> weights = null)
        {
            int rows = controlPoints.Count;
            int dim = controlPoints[0].Count;
            List<Vector> homo_controlPoints = new List<Vector>();
            double wt = 0.0;
            Vector ref_pt = new Vector();
            weights = weights != null ? weights : Sets.RepeatData(1.0, controlPoints.Count);

            for (int i = 0; i < rows; i++) 
            {
                List<double> pt = new List<double>();
                ref_pt = controlPoints[i];
                wt = weights[i];

                for (int k = 0; k < dim; k++)
                {
                    pt.Add(ref_pt[k] * wt);
                }

                //append the weight
                pt.Add(wt);
                homo_controlPoints.Add((Vector)pt);
            }
            return homo_controlPoints;
        }
    }
}
