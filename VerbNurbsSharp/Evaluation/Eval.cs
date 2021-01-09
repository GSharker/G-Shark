using VerbNurbsSharp.Core;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using VerbNurbsSharp.Geometry;

namespace VerbNurbsSharp.Evaluation
{
    public class Eval
    {
        
        /// <summary>
        /// Find the span on the knot Array without supplying a number of control points.
        /// </summary>
        /// <param name="degree">Integer degree of function.</param>
        /// <param name="u">Parameter.</param>
        /// <param name="knots">Array of non decreasing knot values.</param>
        /// <returns></returns>
        public static int KnotSpan(int degree, double u, KnotArray knots) => KnotSpan(knots.Count - degree - 2, degree, u, knots);

        /// <summary>
        /// Find the span on the knot list of the given parameter,
        /// (corresponds to algorithm 2.1 from the NURBS book, piegl & Tiller 2nd edition).
        /// </summary>
        /// <param name="numberOfControlPts">Number of control points - 1.</param>
        /// <param name="degree">Integer degree of function.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="knots">Array of non decreasing knot values.</param>
        /// <returns>The index of the knot span.</returns>
        public static int KnotSpan(int numberOfControlPts, int degree, double parameter, KnotArray knots)
        {
            // special case if parameter == knots[numberOfControlPts+1]
            if (parameter > knots[numberOfControlPts + 1] - Constants.EPSILON) return numberOfControlPts;

            if (parameter < knots[degree] + Constants.EPSILON) return degree;

            var low = degree;
            var high = numberOfControlPts + 1;
            int mid = (int) Math.Floor((double)(low + high) / 2);

            while (parameter < knots[mid] || parameter >= knots[mid + 1])
            {
                if (parameter < knots[mid])
                    high = mid;
                else
                    low = mid;

                mid = (int)Math.Floor((double)(low + high) / 2);
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
        /// <summary>
        /// Obtain the weight from a collection of points in homogeneous space, assuming all are the same dimension.
        /// </summary>
        /// <param name="homogeneousPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A set of numbers represented by an set pi with length (dim).</returns>
        public static List<double> Weight1d(List<Vector> homogeneousPts)
        {
            if(homogeneousPts.Any(vec => vec.Count != homogeneousPts[0].Count))
                throw new ArgumentOutOfRangeException(nameof(homogeneousPts), "Homogeneous points must have the same dimension.");
            return homogeneousPts.Select(vec => vec[^1]).ToList();
        }
        /// <summary>
        /// Dehomogenize a point.
        /// </summary>
        /// <param name="homogeneousPt">A point represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A point represented by an array pi with length (dim).</returns>
        public static Vector Dehomogenize(Vector homogeneousPt)
        {
            var dim = homogeneousPt.Count;
            var weight = homogeneousPt[dim - 1];
            var point = new Vector();

            for (int i = 0; i < dim-1; i++)
                point.Add(homogeneousPt[i]/weight);

            return point;
        }
        /// <summary>
        /// Dehomogenize an set of points.
        /// </summary>
        /// <param name="homogeneousPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of points, each of length dim.</returns>
        public static List<Vector> Dehomogenize1d(List<Vector> homogeneousPts) => homogeneousPts.Select(Dehomogenize).ToList();
    }
}
