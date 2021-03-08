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
            List<double> neWeights = weights;
            if (weights == null || weights.Count == 0)
            {
                neWeights = Sets.RepeatData(1.0, controlPoints.Count);
            }
            else
            {
                if (controlPoints.Count < weights.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(weights), "The weights set is bigger than the control points, it must be the same dimension");
                }

                if (controlPoints.Count > weights.Count)
                {
                    int diff = controlPoints.Count - weights.Count;
                    List<double> dataFilled = Sets.RepeatData(1.0, diff);
                    neWeights.AddRange(dataFilled);
                }
            }

            int rows = controlPoints.Count;
            int dim = controlPoints[0].Count;
            List<Vector3> controlPtsHomogenized = new List<Vector3>();

            for (int i = 0; i < rows; i++)
            {
                Vector3 tempPt = new Vector3();
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
            int rows = controlPoints.Count;
            List<List<Vector3>> controlPtsHomogenized = new List<List<Vector3>>();
            List<List<double>> newWeights = weights;
            if (weights == null || weights.Count == 0)
            {
                newWeights = new List<List<double>>();
                for (int i = 0; i < rows; i++)
                {
                    newWeights.Add(Sets.RepeatData(1.0, controlPoints[i].Count));
                }
            }
            if (controlPoints.Count < newWeights.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(weights), "The weights set is bigger than the control points, it must be the same dimension");
            }

            for (int i = 0; i < rows; i++)
            {
                controlPtsHomogenized.Add(Homogenize1d(controlPoints[i], newWeights[i]));
            }

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
            {
                throw new ArgumentOutOfRangeException(nameof(homoPts), "Homogeneous points must have the same dimension.");
            }

            return homoPts.Select(vec => vec[^1]).ToList();
        }

        /// <summary>
        /// Obtain the weight from a collection of points in homogeneous space, assuming all are the same dimension
        /// </summary>
        /// <param name="homoPts">rray of arrays of of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>array of arrays of points, each represented by an array pi with length (dim)</returns>
        public static List<List<double>> Weight2d(List<List<Vector3>> homoPts)
        {
            return homoPts.Select(vecs => Weight1d(vecs).ToList()).ToList();
        }

        /// <summary>
        /// Dehomogenize a point.
        /// </summary>
        /// <param name="homoPt">A point represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A point represented by an array pi with length (dim).</returns>
        public static Vector3 Dehomogenize(Vector3 homoPt)
        {
            int dim = homoPt.Count;
            double weight = homoPt[dim - 1];
            Vector3 point = new Vector3();

            for (int i = 0; i < dim - 1; i++)
            {
                point.Add(homoPt[i] / weight);
            }

            return point;
        }


        /// <summary>
        /// Dehomogenize an set of points.
        /// </summary>
        /// <param name="homoPts">Points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of points, each of length dim.</returns>
        public static List<Vector3> Dehomogenize1d(List<Vector3> homoPts)
        {
            return homoPts.Select(Dehomogenize).ToList();
        }

        /// <summary>
        /// Dehomogenize an 2d set of points.
        /// </summary>
        /// <param name="homoLstPts">List of list of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Set of set of points, each of length dim.</returns>
        public static List<List<Vector3>> Dehomogenize2d(List<List<Vector3>> homoLstPts)
        {
            return homoLstPts.Select(Dehomogenize1d).ToList();
        }

        /// <summary>
        /// Obtain the point from a point in homogeneous space without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homoPoints">Sets of points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns> Set of points represented by an array (wi*pi) with length (dim).</returns>
        public static List<Vector3> Rational1d(List<Vector3> homoPoints)
        {
            int dim = homoPoints[0].Count - 1;
            return homoPoints.Select(pt => new Vector3(pt.GetRange(0, dim))).ToList();
        }

        /// <summary>
        /// Find the Tait-Byran angles (also loosely called Euler angles) for a rotation transformation.
        /// yaw - angle (in radians) to rotate about the Z axis.
        /// pitch -  angle(in radians) to rotate about the Y axis.
        /// roll - angle(in radians) to rotate about the X axis.
        /// </summary>
        /// <param name="transform">Transformation to check.</param>
        /// <returns>A dictionary collecting the 3 values.</returns>
        public static Dictionary<string, double> GetYawPitchRoll(Transform transform)
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            if ((transform[1][0] == 0.0 && transform[0][0] == 0.0) ||
                (transform[2][1] == 0.0 && transform[2][2] == 0.0) ||
                (Math.Abs(transform[2][0]) >= 1.0))
            {
                values.Add("Pitch" , (transform[2][0] > 0) ? -Math.PI / 2.0 : Math.PI / 2.0);
                values.Add("Yaw", Math.Atan2(-transform[0][1], transform[1][1]));
                values.Add("Roll", 0.0);
            }
            else
            {
                values.Add("Pitch", Math.Atan2(transform[2][1], transform[2][2]));
                values.Add("Yaw", Math.Atan2(transform[1][0], transform[0][0]));
                values.Add("Roll", Math.Asin(-transform[2][0]));
            }

            return values;
        }

        /// <summary>
        /// Find the rotation axis used in the transformation.
        /// https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/index.htm
        /// </summary>
        /// <param name="t">Transformation to check.</param>
        /// <returns>The rotation axis used for the transformation.</returns>
        public static Vector3 GetRotationAxis(Transform t)
        {
            Vector3 axis = Vector3.Unset;

            if (Math.Abs(t[0][1] + t[1][0]) < GeoSharpMath.MINTOLERANCE || 
                Math.Abs(t[0][2] + t[2][0]) < GeoSharpMath.MINTOLERANCE ||
                Math.Abs(t[1][2] + t[2][1]) < GeoSharpMath.MINTOLERANCE)
            {
                double xx = (t[0][0] + 1) / 2;
                double yy = (t[1][1] + 1) / 2;
                double zz = (t[2][2] + 1) / 2;
                double xy = (t[0][1] + t[1][0]) / 4;
                double xz = (t[0][2] + t[2][0]) / 4;
                double yz = (t[1][2] + t[2][1]) / 4;
                if ((xx > yy) && (xx > zz))
                { // m[0][0] is the largest diagonal term
                    if (xx < GeoSharpMath.MINTOLERANCE)
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
                    if (yy < GeoSharpMath.MINTOLERANCE)
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
                    if (zz < GeoSharpMath.MINTOLERANCE)
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

            double v = Math.Sqrt(Math.Pow(t[2][1] - t[1][2], 2) + Math.Pow(t[0][2] - t[2][0], 2) + Math.Pow(t[1][0] - t[0][1], 2));

            axis[0] = (t[2][1] - t[1][2]) / v;
            axis[1] = (t[0][2] - t[2][0]) / v;
            axis[2] = (t[1][0] - t[0][1]) / v;

            return axis;
        }

        /// <summary>
        /// Obtain the point from a 2d set of points in homogeneous space without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homoPoints">Set of set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns> Set of points represented by an array (wi*pi) with length (dim).</returns>
        public static List<List<Vector3>> Rational2d(List<List<Vector3>> homoLstPts)
        {
            return homoLstPts.Select(vecs => Rational1d(vecs)).ToList();
        }

        /// <summary>
        /// Get the orientation between tree points in the plane.
        /// The order can be: collinear (result 0), clockwise (result 1), counterclockwise (result 2)
        /// https://www.geeksforgeeks.org/orientation-3-ordered-points/
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <returns>The result expressed as a value between 0 and 2.</returns>
        public static int Orientation(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            double result = (pt2[1] - pt1[1]) * (pt3[0] - pt2[0]) - (pt2[0] - pt1[0]) * (pt3[1] - pt2[1]);

            if (Math.Abs(result) < GeoSharpMath.EPSILON) return 0;

            return (result > 0) ? 1 : 2;
        }
    }
}
