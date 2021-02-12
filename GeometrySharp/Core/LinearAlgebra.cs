using GeometrySharp.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometrySharp.Core
{
    /// <summary>
    /// Provides linear algebra utility functions.
    /// </summary>
    public class LinearAlgebra
    {
        /// <summary>
        /// Transform a 1d array of points into their homogeneous equivalents.
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
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
        /// Transform a 2d array of points into their homogeneous equivalents.
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints"> Control points, a 2d set of size (m x dim).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (m x 1).</param>
        /// <returns> 2d set of control points where each point is (wi*pi, wi) where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<List<Vector3>> Homogenize2d(List<List<Vector3>> controlPoints, List<List<double>>? weights = null)
        {
            var rows = controlPoints.Count;
            var controlPtsHomogenized = new List<List<Vector3>>();
            var newWeights = weights;
            if (weights == null || weights.Count == 0)
            {
                newWeights = new List<List<double>>();
                for (int i = 0; i < rows; i++)
                    newWeights.Add(Sets.RepeatData(1.0, controlPoints[i].Count));
            }
            if (controlPoints.Count < newWeights.Count) throw new ArgumentOutOfRangeException(nameof(weights), "The weights set is bigger than the control points, it must be the same dimension");
            for (int i = 0; i < rows; i++)
                controlPtsHomogenized.Add(Homogenize1d(controlPoints[i], newWeights[i]));
            return controlPtsHomogenized;
        }

        /// <summary>
        /// Obtain the weight from a collection of points in homogeneous space, assuming all are the same dimension.
        /// </summary>
        /// <param name="homoPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A set of numbers represented by an set pi with length (dim).</returns>
        public static List<double> Weight1d(List<Vector3> homoPts)
        {
            if (homoPts.Any(vec => vec.Count != homoPts[0].Count))
                throw new ArgumentOutOfRangeException(nameof(homoPts), "Homogeneous points must have the same dimension.");
            return homoPts.Select(vec => vec[^1]).ToList();
        }

        /// <summary>
        /// Obtain the weight from a collection of points in homogeneous space, assuming all are the same dimension
        /// </summary>
        /// <param name="homoPts">rray of arrays of of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>array of arrays of points, each represented by an array pi with length (dim)</returns>
        public static List<List<double>> Weight2d(List<List<Vector3>> homoPts) =>
            homoPts.Select(vecs => Weight1d(vecs).ToList()).ToList();

        /// <summary>
        /// Dehomogenize a point.
        /// </summary>
        /// <param name="homoPt">A point represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A point represented by an array pi with length (dim).</returns>
        public static Vector3 Dehomogenize(Vector3 homoPt)
        {
            var dim = homoPt.Count;
            var weight = homoPt[dim - 1];
            var point = new Vector3();

            for (int i = 0; i < dim - 1; i++)
                point.Add(homoPt[i] / weight);

            return point;
        }


        /// <summary>
        /// Dehomogenize an set of points.
        /// </summary>
        /// <param name="homoPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of points, each of length dim.</returns>
        public static List<Vector3> Dehomogenize1d(List<Vector3> homoPts) =>
            homoPts.Select(Dehomogenize).ToList();

        /// <summary>
        /// Dehomogenize an 2d set of points.
        /// </summary>
        /// <param name="homoLstPts">List of list of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Set of set of points, each of length dim.</returns>
        public static List<List<Vector3>> Dehomogenize2d(List<List<Vector3>> homoLstPts) => homoLstPts.Select(Dehomogenize1d).ToList();

        /// <summary>
        /// Obtain the point from a point in homogeneous space without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homoPoints">Sets of points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns> Set of points represented by an array (wi*pi) with length (dim).</returns>
        public static List<Vector3> Rational1d(List<Vector3> homoPoints)
        {
            var dim = homoPoints[0].Count - 1;
            return homoPoints.Select(pt => new Vector3(pt.GetRange(0, dim))).ToList();
        }

        /// <summary>
        /// Obtain the point from a 2d set of points in homogeneous space without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homoPoints">Set of set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns> Set of points represented by an array (wi*pi) with length (dim).</returns>
        public static List<List<Vector3>> Rational2d(List<List<Vector3>> homoLstPts) => homoLstPts.Select(vecs => Rational1d(vecs)).ToList();
    }
}
