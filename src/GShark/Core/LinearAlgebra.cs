using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Core
{
    /// <summary>
    /// Provides linear algebra utility functions.
    /// </summary>
    public class LinearAlgebra
    {
        /// <summary>
        /// Transforms a collection of points into their homogeneous equivalents.<br/>
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints">Control points, a set of size (points count x points dimension).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (points count x 1).</param>
        /// <returns>A set of control points where each point is (wi*pi, wi)<br/>
        /// where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<Vector3> PointsHomogeniser(List<Vector3> controlPoints, List<double> weights)
        {
            if (controlPoints.Count < weights.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(weights),
                    "The weights set is bigger than the control points, it must be the same dimension");
            }

            if (controlPoints.Count > weights.Count)
            {
                int diff = controlPoints.Count - weights.Count;
                List<double> dataFilled = Sets.RepeatData(1.0, diff);
                weights.AddRange(dataFilled);
            }

            List<Vector3> controlPtsHomogenized = new List<Vector3>();

            for (int i = 0; i < controlPoints.Count; i++)
            {
                Vector3 tempPt = new Vector3();
                for (int j = 0; j < controlPoints[0].Count; j++)
                {
                    tempPt.Add(controlPoints[i][j] * weights[i]);
                }

                // Added the weight to the point.
                tempPt.Add(weights[i]);
                controlPtsHomogenized.Add(tempPt);
            }

            return controlPtsHomogenized;
        }

        /// <summary>
        /// Transforms a collection of points into their homogeneous equivalents, by a given weight value.<br/>
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints">Control points, a set of size (points count x points dimension).</param>
        /// <param name="weight">Weight value for each point.</param>
        /// <returns>A set of control points where each point is (wi*pi, wi)<br/>
        /// where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<Vector3> PointsHomogeniser(List<Vector3> controlPoints, double weight)
        {
            List<Vector3> controlPtsHomogenized = new List<Vector3>();

            for (int i = 0; i < controlPoints.Count; i++)
            {
                Vector3 tempPt = new Vector3();
                for (int j = 0; j < controlPoints[0].Count; j++)
                {
                    tempPt.Add(controlPoints[i][j] * weight);
                }

                // Added the weight to the point.
                tempPt.Add(weight);
                controlPtsHomogenized.Add(tempPt);
            }

            return controlPtsHomogenized;
        }

        /// <summary>
        /// Transforms a two-dimension collection of points into their homogeneous equivalents.<br/>
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints">Control points, a two-dimensional set of size (points count x points dimension).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (points count x 1).</param>
        /// <returns>A two-dimensional set of control points where each point is (wi*pi, wi)<br/>
        /// where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<List<Vector3>> PointsHomogeniser2d(List<List<Vector3>> controlPoints, List<List<double>> weights = null)
        {
            int rows = controlPoints.Count;
            List<List<Vector3>> controlPtsHomogenized = new List<List<Vector3>>();
            List<List<double>> usedWeights = weights;
            if (weights == null || weights.Count == 0)
            {
                usedWeights = new List<List<double>>();
                for (int i = 0; i < rows; i++)
                {
                    usedWeights.Add(Sets.RepeatData(1.0, controlPoints[i].Count));
                }
            }
            if (controlPoints.Count < usedWeights.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(weights), "The weights set is bigger than the control points, it must be the same dimension");
            }

            for (int i = 0; i < rows; i++)
            {
                controlPtsHomogenized.Add(PointsHomogeniser(controlPoints[i], usedWeights[i]));
            }

            return controlPtsHomogenized;
        }

        /// <summary>
        /// Obtains the weight from a collection of points in homogeneous space, assuming all are the same dimension.
        /// </summary>
        /// <param name="homoPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A set of values, represented by a set pi with length (dim).</returns>
        public static List<double> GetWeights(List<Vector3> homoPts)
        {
            if (homoPts.Any(pt => pt.Count != homoPts[0].Count))
            {
                throw new ArgumentOutOfRangeException(nameof(homoPts), "Homogeneous points must have the same dimension.");
            }

            return homoPts.Select(pt => pt[^1]).ToList();
        }

        /// <summary>
        /// Obtains the weight from a two-dimensional collection of points in homogeneous space, assuming all are the same dimension.
        /// </summary>
        /// <param name="homoPts">Two-dimensional set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Two-dimensional set of values, each represented by an array pi with length (dim)</returns>
        public static List<List<double>> GetWeights2d(List<List<Vector3>> homoPts)
        {
            return homoPts.Select(pts => GetWeights(pts).ToList()).ToList();
        }

        /// <summary>
        /// Gets a dehomogenized point.
        /// </summary>
        /// <param name="homoPt">A point represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A dehomogenized point.</returns>
        public static Vector3 PointDehomogenizer(Vector3 homoPt)
        {
            int dim = homoPt.Count - 1;
            double weight = homoPt[dim];
            Vector3 point = new Vector3();

            for (int i = 0; i < dim; i++)
            {
                point.Add(homoPt[i] / weight);
            }

            return point;
        }

        /// <summary>
        /// Gets a set of dehomogenized points.
        /// </summary>
        /// <param name="homoPts">A collection of points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of dehomogenized points.</returns>
        public static List<Vector3> PointDehomogenizer1d(List<Vector3> homoPts)
        {
            return homoPts.Select(PointDehomogenizer).ToList();
        }

        /// <summary>
        /// Gets a two-dimensional set of dehomogenized points.
        /// </summary>
        /// <param name="homoPts">Two-dimensional set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Two-dimensional set of dehomogenized points.</returns>
        public static List<List<Vector3>> PointDehomogenizer2d(List<List<Vector3>> homoPts)
        {
            return homoPts.Select(PointDehomogenizer1d).ToList();
        }

        /// <summary>
        /// Obtains the point from homogeneous point without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homoPts">Sets of points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of rational points.</returns>
        public static List<Vector3> RationalPoints(List<Vector3> homoPts)
        {
            int dim = homoPts[0].Count - 1;
            return homoPts.Select(pt => new Vector3(pt.GetRange(0, dim))).ToList();
        }

        /// <summary>
        /// Obtains the point from a two-dimensional set of homogeneous points without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homoPoints">Two-dimensional set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Two-dimensional set of rational points.</returns>
        public static List<List<Vector3>> Rational2d(List<List<Vector3>> homoLstPts)
        {
            return homoLstPts.Select(vecs => RationalPoints(vecs)).ToList();
        }

        /// <summary>
        /// Finds the Tait-Byran angles (also loosely called Euler angles) for a rotation transformation.<br/>
        /// yaw - angle (in radians) to rotate about the Z axis.<br/>
        /// pitch -  angle(in radians) to rotate about the Y axis.<br/>
        /// roll - angle(in radians) to rotate about the X axis.
        /// </summary>
        /// <param name="transform">Transformation to check.</param>
        /// <returns>A dictionary collecting the 3 values.</returns>
        public static Dictionary<string, double> GetYawPitchRoll(Transform transform)
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            if ((Math.Abs(transform[1][0]) < GeoSharpMath.MinTolerance && Math.Abs(transform[0][0]) < GeoSharpMath.MinTolerance) ||
                (Math.Abs(transform[2][1]) < GeoSharpMath.MinTolerance && Math.Abs(transform[2][2]) < GeoSharpMath.MinTolerance) ||
                (Math.Abs(transform[2][0]) >= 1.0))
            {
                values.Add("Pitch" , (transform[2][0] > 0) ? -Math.PI / 2.0 : Math.PI / 2.0);
                values.Add("Yaw", Math.Atan2(-transform[0][1], transform[1][1]));
                values.Add("Roll", 0.0);
                return values;
            }

            values.Add("Pitch", Math.Atan2(transform[2][1], transform[2][2]));
            values.Add("Yaw", Math.Atan2(transform[1][0], transform[0][0]));
            values.Add("Roll", Math.Asin(-transform[2][0]));

            return values;
        }

        /// <summary>
        /// Finds the rotation axis used in the transformation.<br/>
        /// https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/index.htm
        /// </summary>
        /// <param name="transform">Transformation to check.</param>
        /// <returns>The rotation axis used for the transformation.</returns>
        public static Vector3 GetRotationAxis(Transform transform)
        {
            Vector3 axis = Vector3.Unset;

            if (Math.Abs(transform[0][1] + transform[1][0]) < GeoSharpMath.MinTolerance || 
                Math.Abs(transform[0][2] + transform[2][0]) < GeoSharpMath.MinTolerance ||
                Math.Abs(transform[1][2] + transform[2][1]) < GeoSharpMath.MinTolerance)
            {
                double xx = (transform[0][0] + 1) / 2;
                double yy = (transform[1][1] + 1) / 2;
                double zz = (transform[2][2] + 1) / 2;
                double xy = (transform[0][1] + transform[1][0]) / 4;
                double xz = (transform[0][2] + transform[2][0]) / 4;
                double yz = (transform[1][2] + transform[2][1]) / 4;
                if ((xx > yy) && (xx > zz))
                { // m[0][0] is the largest diagonal term
                    if (xx < GeoSharpMath.MinTolerance)
                    {
                        axis[0] = 0;
                        axis[1] = 0.7071;
                        axis[2] = 0.7071;
                    }
                    else
                    {
                        axis[0] = Math.Sqrt(xx);
                        axis[1] = xy / axis[0];
                        axis[2] = xz / axis[0];
                    }
                }
                else if (yy > zz)
                { // m[1][1] is the largest diagonal term
                    if (yy < GeoSharpMath.MinTolerance)
                    {
                        axis[0] = 0.7071;
                        axis[1] = 0;
                        axis[2] = 0.7071;
                    }
                    else
                    {
                        axis[1] = Math.Sqrt(yy);
                        axis[0] = xy / axis[1];
                        axis[2] = yz / axis[1];
                    }
                }
                else
                { // m[2][2] is the largest diagonal term so base result on this
                    if (zz < GeoSharpMath.MinTolerance)
                    {
                        axis[0] = 0.7071;
                        axis[1] = 0.7071;
                        axis[2] = 0;
                    }
                    else
                    {
                        axis[2] = Math.Sqrt(zz);
                        axis[0] = xz / axis[2];
                        axis[1] = yz / axis[2];
                    }
                }
                return axis; // return 180 deg rotation
            }

            double v = Math.Sqrt(Math.Pow(transform[2][1] - transform[1][2], 2) + Math.Pow(transform[0][2] - transform[2][0], 2) + Math.Pow(transform[1][0] - transform[0][1], 2));

            axis[0] = (transform[2][1] - transform[1][2]) / v;
            axis[1] = (transform[0][2] - transform[2][0]) / v;
            axis[2] = (transform[1][0] - transform[0][1]) / v;

            return axis;
        }

        /// <summary>
        /// Gets the orientation between tree points in the plane.<br/>
        /// The order can be: collinear (result 0), clockwise (result 1), counterclockwise (result 2)<br/>
        /// https://www.geeksforgeeks.org/orientation-3-ordered-points/
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <returns>The result expressed as a value between 0 and 2.</returns>
        public static int Orientation(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            double result = (pt2[1] - pt1[1]) * (pt3[0] - pt2[0]) - (pt2[0] - pt1[0]) * (pt3[1] - pt2[1]);

            if (Math.Abs(result) < GeoSharpMath.Epsilon)
            {
                return 0;
            }

            return (result > 0) ? 1 : 2;
        }

        /// <summary>
        /// Computes the binomial coefficient (denoted by n choose k).<br/>
        /// Please see the following website for details: http://mathworld.wolfram.com/BinomialCoefficient.html
        /// </summary>
        /// <param name="n">Size of the set of distinct elements.</param>
        /// <param name="k">Size of the subsets.</param>
        /// <returns>Combination of k and n</returns>
        public static double GetBinomial(int n, int k)
        {
            (int n, int k, double val) storage = (0, 0, 0.0);

            if (k == 0)
            {
                return 1.0;
            }

            if (n == 0 || k > n)
            {
                return 0.0;
            }

            if (k > n - k)
            {
                k = n - k;
            }

            if (storage.n == n && storage.k == k)
            {
                return storage.val;
            }

            double r = 1.0;
            int n0 = n;

            for (int d = 1; d < k + 1; d++)
            {
                if (storage.n == n0 && storage.k == d)
                {
                    n--;
                    r = storage.val;
                    continue;
                }

                r *= n--;
                r /= d;

                if (storage.n == n0)
                {
                    continue;
                }

                storage.n = n0;
                storage.k = d;
                storage.val = r;
            }

            return r;
        }
    }
}
