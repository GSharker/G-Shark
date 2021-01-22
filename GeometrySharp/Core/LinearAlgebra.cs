using GeometrySharp.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Evaluation;
using Newtonsoft.Json.Linq;

namespace GeometrySharp.Core
{
    public class LinearAlgebra
    {
        /// <summary>
        /// Transform a 1d array of points into their homogeneous equivalents.
        /// </summary>
        /// <param name="controlPoints"> Control points, a 2d set of size (m x dim).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (m x 1).</param>
        /// <returns> 1d set of control points where each point is (wi*pi, wi) where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<Vector3> Homogenize1d(List<Vector3> controlPoints, List<double> weights = null)
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
            var controlPtsHomogenized = new List<Vector3>();

            for (int i = 0; i < rows; i++)
            {
                var tempPt = new Vector3();
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
        public static List<double> Weight1d(List<Vector3> homogeneousPts)
        {
            if (homogeneousPts.Any(vec => vec.Count != homogeneousPts[0].Count))
                throw new ArgumentOutOfRangeException(nameof(homogeneousPts), "Homogeneous points must have the same dimension.");
            return homogeneousPts.Select(vec => vec[^1]).ToList();
        }

        /// <summary>
        /// Dehomogenize a point.
        /// </summary>
        /// <param name="homogeneousPt">A point represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A point represented by an array pi with length (dim).</returns>
        public static Vector3 Dehomogenize(Vector3 homogeneousPt)
        {
            var dim = homogeneousPt.Count;
            var weight = homogeneousPt[dim - 1];
            var point = new Vector3();

            for (int i = 0; i < dim - 1; i++)
                point.Add(homogeneousPt[i] / weight);

            return point;
        }

        /// <summary>
        /// Dehomogenize an set of points.
        /// </summary>
        /// <param name="homogeneousPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of points, each of length dim.</returns>
        public static List<Vector3> Dehomogenize1d(List<Vector3> homogeneousPts) => homogeneousPts.Select(Dehomogenize).ToList();
    }
}
