using VerbNurbsSharp.Core;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

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
        /// find the span on the knot list of the given parameter, (corresponds to algorithm 2.1 from the NURBS book, piegl & Tiller 2nd edition)
        /// 
        /// </summary>
        /// <param name="n">integer number of basis functions - 1 = knots.length - degree - 2</param>
        /// <param name="degree">integer degree of function</param>
        /// <param name="u">parameter</param>
        /// <param name="knots">array of nondecreasing knot values</param>
        /// <returns>the index of the knot span</returns>
        public static int KnotSpanGivenN(int n, int degree, double u, KnotArray knots)
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
        /// Transform a 1d array of points into their homogeneous equivalents.
        /// </summary>
        /// <param name="controlPoints"> Control points, a 2d set of size (m x dim).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (m x 1).</param>
        /// <returns> 1d set of control points where each point is (wi*pi, wi) where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<Vector> Homogenize1d(List<Vector> controlPoints, List<double> weights = null)
        {
            var neWeights = weights;
            if (weights == null || weights.Count == 0)
                neWeights = Sets.RepeatData(1.0, controlPoints.Count);
            else
            {
                if (controlPoints.Count < weights.Count) throw new ArgumentOutOfRangeException(nameof(weights), "The weights set is bigger than the control points, it must be the same dimension");
                if (controlPoints.Count > weights.Count)
                {
                    var diff = controlPoints.Count - weights.Count;
                    var dataFilled = Sets.RepeatData(1.0, diff);
                    neWeights.AddRange(dataFilled);
                }
            }

            var rows = controlPoints.Count;
            var dim = controlPoints[0].Count;
            var controlPtsHomogenized = new List<Vector>();

            for (int i = 0; i < rows; i++)
            {
                var tempPt = new Vector();
                for (int j = 0; j < dim; j++)
                {
                    tempPt.Add(controlPoints[i][j] * neWeights[i]);
                }
                // Added the weight to the point.
                tempPt.Add(neWeights[i]);
                controlPtsHomogenized.Add(tempPt);
            }

            return controlPtsHomogenized;
        }

        public static IList<double> Weight1d(List<Vector> homogeneousPts)
        {
            if(homogeneousPts.Any(vec => vec.Count != homogeneousPts[0].Count))
                throw new ArgumentOutOfRangeException(nameof(homogeneousPts), "Homogeneous points must have the same dimension.");
            return homogeneousPts.Select(vec => vec[^1]).ToList();
        }
    }
}
