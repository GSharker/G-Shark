using GShark.Geometry;
using System;
using System.Collections.Generic;

namespace GShark.Core
{
    /// <summary>
    /// Provides linear algebra utility functions.
    /// </summary>
    public class LinearAlgebra
    {
        /// <summary>
        /// Finds the Tait-Byran angles (also loosely called Euler angles) for a rotation transformation.<br/>
        /// yaw - angle (in radians) to rotate about the Z axis.<br/>
        /// pitch -  angle (in radians) to rotate about the Y axis.<br/>
        /// roll - angle (in radians) to rotate about the X axis.
        /// </summary>
        /// <param name="transform">Transformation to check.</param>
        /// <returns>A dictionary collecting the 3 values.</returns>
        public static Dictionary<string, double> GetYawPitchRoll(Transform transform)
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            if ((Math.Abs(transform[1][0]) < GeoSharkMath.MinTolerance && Math.Abs(transform[0][0]) < GeoSharkMath.MinTolerance) ||
                (Math.Abs(transform[2][1]) < GeoSharkMath.MinTolerance && Math.Abs(transform[2][2]) < GeoSharkMath.MinTolerance) ||
                (Math.Abs(transform[2][0]) >= 1.0))
            {
                values.Add("Pitch", (transform[2][0] > 0) ? -Math.PI / 2.0 : Math.PI / 2.0);
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

            if (Math.Abs(transform[0][1] + transform[1][0]) < GeoSharkMath.MinTolerance ||
                Math.Abs(transform[0][2] + transform[2][0]) < GeoSharkMath.MinTolerance ||
                Math.Abs(transform[1][2] + transform[2][1]) < GeoSharkMath.MinTolerance)
            {
                double xx = (transform[0][0] + 1) / 2;
                double yy = (transform[1][1] + 1) / 2;
                double zz = (transform[2][2] + 1) / 2;
                double xy = (transform[0][1] + transform[1][0]) / 4;
                double xz = (transform[0][2] + transform[2][0]) / 4;
                double yz = (transform[1][2] + transform[2][1]) / 4;
                if ((xx > yy) && (xx > zz))
                { // m[0][0] is the largest diagonal term
                    if (xx < GeoSharkMath.MinTolerance)
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
                    if (yy < GeoSharkMath.MinTolerance)
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
                    if (zz < GeoSharkMath.MinTolerance)
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
        public static int Orientation(Point3 pt1, Point3 pt2, Point3 pt3)
        {
            double result = (pt2[1] - pt1[1]) * (pt3[0] - pt2[0]) - (pt2[0] - pt1[0]) * (pt3[1] - pt2[1]);

            if (Math.Abs(result) < GeoSharkMath.Epsilon)
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
