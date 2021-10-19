using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace GShark.Core
{
    /// <summary>
    /// Represents the values in a 4 x 4 transformation matrix.
    /// </summary>
    public class Transform : List<List<double>>
    {
        internal readonly double M00;
        internal readonly double M01;
        internal readonly double M02;
        internal readonly double M03;
        internal readonly double M10;
        internal readonly double M11;
        internal readonly double M12;
        internal readonly double M13;
        internal readonly double M20;
        internal readonly double M21;
        internal readonly double M22;
        internal readonly double M23;
        internal readonly double M30;
        internal readonly double M31;
        internal readonly double M32;
        internal readonly double M33;

        /// <summary>
        /// Initializes a 4 x 4 transformation matrix.<br/>
        /// All the values are set to zero.
        /// </summary>
        public Transform()
        {
            AddRange(Matrix.Construct(4, 4));
            M00 = this[0][0];
            M01 = this[0][1];
            M02 = this[0][2];
            M03 = this[0][3];
            M10 = this[1][0];
            M11 = this[1][1];
            M12 = this[1][2];
            M13 = this[1][3];
            M20 = this[2][0];
            M21 = this[2][1];
            M22 = this[2][2];
            M23 = this[2][3];
            M30 = this[3][0];
            M31 = this[3][1];
            M32 = this[3][2];
            M33 = this[3][3];

        }

        /// <summary>
        /// Gets a identity transformation matrix. An identity matrix defines no transformation.<br/>
        /// The diagonal is (1,1,1,1).
        /// </summary>
        /// <returns>The identity transformation matrix.</returns>
        public static Transform Identity()
        {
            Transform transform = new Transform
            {
                [0] = { [0] = 1 },
                [1] = { [1] = 1 },
                [2] = { [2] = 1 },
                [3] = { [3] = 1 },
            };
            return transform;
        }

        /// <summary>
        /// Constructs a new translation transformation.
        /// </summary>
        /// <param name="v">Translation vector.</param>
        /// <returns>A transformation matrix which moves the geometry along the vector.</returns>
        public static Transform Translation(Vector3 v)
        {
            return Translation(v[0], v[1], v[2]);
        }

        /// <summary>
        /// Constructs a new translation transformation.
        /// </summary>
        /// <param name="x">Transform the x value.</param>
        /// <param name="y">Transform the y value.</param>
        /// <param name="z">Transform the z value.</param>
        /// <returns>A transformation matrix which moves the geometry along the vector.</returns>
        public static Transform Translation(double x, double y, double z)
        {
            Transform transform = Identity();
            transform[0][3] = x;
            transform[1][3] = y;
            transform[2][3] = z;
            transform[3][3] = 1.0;

            return transform;
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified radians angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="radiansAngle">Angle in radians of the rotation.</param>
        /// <param name="axis">Axis direction.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        public Transform Rotation(double radiansAngle, Line axis)
        {
            return Rotation(Math.Sin(radiansAngle), Math.Cos(radiansAngle), axis.Direction, axis.StartPoint);
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified radians angle, rotation center and rotation axis pointing up.
        /// </summary>
        /// <param name="radiansAngle">Angle in radians of the rotation.</param>
        /// <param name="center">Center point of rotation. Rotation axis is vertical.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        public static Transform Rotation(double radiansAngle, Point3 center)
        {
            return Rotation(Math.Sin(radiansAngle), Math.Cos(radiansAngle), Vector3.ZAxis, center);
        }

        /// <summary>
        /// Constructs a new rotation transformation with Sin and Cos radians angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="sinAngle">Sin radians angle.</param>
        /// <param name="cosAngle">Cos radians angle.</param>
        /// <param name="axis">Axis direction.</param>
        /// <param name="origin">Rotation center.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        private static Transform Rotation(double sinAngle, double cosAngle, Vector3 axis, Point3 origin)
        {
            double sAngle = sinAngle;
            double cAngle = cosAngle;

            GSharkMath.KillNoise(ref sAngle, ref cAngle);

            Transform transform = Identity();
            double oneMinusCosAngle = 1 - cosAngle;

            transform[0][0] = axis[0] * axis[0] * oneMinusCosAngle + cAngle;
            transform[0][1] = axis[0] * axis[1] * oneMinusCosAngle - axis[2] * sAngle;
            transform[0][2] = axis[0] * axis[2] * oneMinusCosAngle + axis[1] * sAngle;

            transform[1][0] = axis[1] * axis[0] * oneMinusCosAngle + axis[2] * sAngle;
            transform[1][1] = axis[1] * axis[1] * oneMinusCosAngle + cAngle;
            transform[1][2] = axis[1] * axis[2] * oneMinusCosAngle - axis[0] * sAngle;

            transform[2][0] = axis[2] * axis[0] * oneMinusCosAngle - axis[1] * sAngle;
            transform[2][1] = axis[2] * axis[1] * oneMinusCosAngle + axis[0] * sAngle;
            transform[2][2] = axis[2] * axis[2] * oneMinusCosAngle + cAngle;

            if (!origin.Equals(new Point3(0, 0, 0)))
            {
                transform[0][3] = -((transform[0][0] - 1) * origin[0] + transform[0][1] * origin[1] + transform[0][2] * origin[2]);
                transform[1][3] = -(transform[1][0] * origin[0] + (transform[1][1] - 1) * origin[1] + transform[1][2] * origin[2]);
                transform[2][3] = -(transform[2][0] * origin[0] + transform[2][1] * origin[1] + (transform[2][2] - 1) * origin[2]);
            }

            transform[3][0] = transform[3][1] = transform[3][0] = 0.0;
            transform[3][3] = 1;

            return transform;
        }

        /// <summary>
        /// Creates a uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="anchorPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static Transform Scale(Point3 anchorPoint, double scaleFactor)
        {
            return Scale(anchorPoint, scaleFactor, scaleFactor, scaleFactor);
        }

        /// <summary>
        /// Creates non uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="anchorPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static Transform Scale(Point3 anchorPoint, double factorX, double factorY, double factorZ)
        {
            var origin = new Point3(0.0, 0.0, 0.0);
            Transform scale = Scale(factorX, factorY, factorZ);
            if (anchorPoint.Equals(origin))
            {
                return scale;
            }

            var dir = anchorPoint - origin;
            Transform t0 = Translation(-dir);
            Transform t1 = Translation(dir);

            return t1 * scale * t0;
        }

        /// <summary>
        /// Creates non uniform scale transformation matrix with the anchor point in the origin.
        /// </summary>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static Transform Scale(double factorX, double factorY, double factorZ)
        {
            Transform tIdentity = Transform.Identity();
            tIdentity[0][0] = factorX;
            tIdentity[1][1] = factorY;
            tIdentity[2][2] = factorZ;
            return tIdentity;
        }

        /// <summary>
        /// Creates a transform matrix copying another transform.
        /// </summary>
        /// <param name="other">The transform to copy.</param>
        /// <returns>The copied transformation matrix.</returns>
        public static Transform Copy(Transform other)
        {
            Transform transformCopy = new Transform();
            for (int i = 0; i < other.Count; i++)
            {
                for (int j = 0; j < other[0].Count; j++)
                {
                    transformCopy[i][j] = other[i][j];
                }
            }

            return transformCopy;
        }

        /// <summary>
        /// Multiplies two transformation matrix.
        /// </summary>
        /// <param name="t0">First transformation.</param>
        /// <param name="t1">Second transformation.</param>
        /// <returns>A new transformation.</returns>
        public static Transform operator *(Transform t0, Transform t1)
        {
            Transform t = new Transform
            {
                [0] = { [0] = t0[0][0] * t1[0][0] + t0[0][1] * t1[1][0] + t0[0][2] * t1[2][0] + t0[0][3] * t1[3][0] },
                [0] = { [1] = t0[0][0] * t1[0][1] + t0[0][1] * t1[1][1] + t0[0][2] * t1[2][1] + t0[0][3] * t1[3][1] },
                [0] = { [2] = t0[0][0] * t1[0][2] + t0[0][1] * t1[1][2] + t0[0][2] * t1[2][2] + t0[0][3] * t1[3][2] },
                [0] = { [3] = t0[0][0] * t1[0][3] + t0[0][1] * t1[1][3] + t0[0][2] * t1[2][3] + t0[0][3] * t1[3][3] },
                [1] = { [0] = t0[1][0] * t1[0][0] + t0[1][1] * t1[1][0] + t0[1][2] * t1[2][0] + t0[1][3] * t1[3][0] },
                [1] = { [1] = t0[1][0] * t1[0][1] + t0[1][1] * t1[1][1] + t0[1][2] * t1[2][1] + t0[1][3] * t1[3][1] },
                [1] = { [2] = t0[1][0] * t1[0][2] + t0[1][1] * t1[1][2] + t0[1][2] * t1[2][2] + t0[1][3] * t1[3][2] },
                [1] = { [3] = t0[1][0] * t1[0][3] + t0[1][1] * t1[1][3] + t0[1][2] * t1[2][3] + t0[1][3] * t1[3][3] },
                [2] = { [0] = t0[2][0] * t1[0][0] + t0[2][1] * t1[1][0] + t0[2][2] * t1[2][0] + t0[2][3] * t1[3][0] },
                [2] = { [1] = t0[2][0] * t1[0][1] + t0[2][1] * t1[1][1] + t0[2][2] * t1[2][1] + t0[2][3] * t1[3][1] },
                [2] = { [2] = t0[2][0] * t1[0][2] + t0[2][1] * t1[1][2] + t0[2][2] * t1[2][2] + t0[2][3] * t1[3][2] },
                [2] = { [3] = t0[2][0] * t1[0][3] + t0[2][1] * t1[1][3] + t0[2][2] * t1[2][3] + t0[2][3] * t1[3][3] },
                [3] = { [0] = t0[3][0] * t1[0][0] + t0[3][1] * t1[1][0] + t0[3][2] * t1[2][0] + t0[3][3] * t1[3][0] },
                [3] = { [1] = t0[3][0] * t1[0][1] + t0[3][1] * t1[1][1] + t0[3][2] * t1[2][1] + t0[3][3] * t1[3][1] },
                [3] = { [2] = t0[3][0] * t1[0][2] + t0[3][1] * t1[1][2] + t0[3][2] * t1[2][2] + t0[3][3] * t1[3][2] },
                [3] = { [3] = t0[3][0] * t1[0][3] + t0[3][1] * t1[1][3] + t0[3][2] * t1[2][3] + t0[3][3] * t1[3][3] }
            };

            return t;
        }

        /// <summary>
        /// Creates a transformation matrix to reflect about a plane.
        /// </summary>
        /// <param name="plane">The plane used to reflect.</param>
        /// <returns>The mirror transformation matrix.</returns>
        public static Transform Reflection(Plane plane)
        {
            Point3 pt = plane.Origin;
            Vector3 normal = plane.ZAxis;
            Transform transform = Identity();

            Vector3 unitizedN = normal.Unitize();
            Vector3 translation = unitizedN * (2.0 * (unitizedN[0] * pt[0] + unitizedN[1] * pt[1] + unitizedN[1] * pt[1]));

            transform[0][0] = 1 - 2.0 * unitizedN[0] * unitizedN[0];
            transform[0][1] = -2.0 * unitizedN[0] * unitizedN[1];
            transform[0][2] = -2.0 * unitizedN[0] * unitizedN[2];
            transform[0][3] = translation[0];

            transform[1][0] = -2.0 * unitizedN[1] * unitizedN[0];
            transform[1][1] = 1 - 2.0 * unitizedN[1] * unitizedN[1];
            transform[1][2] = -2.0 * unitizedN[1] * unitizedN[2];
            transform[1][3] = translation[1];

            transform[2][0] = -2.0 * unitizedN[2] * unitizedN[0];
            transform[2][1] = -2.0 * unitizedN[2] * unitizedN[1];
            transform[2][2] = 1 - 2.0 * unitizedN[2] * unitizedN[2];
            transform[2][3] = translation[2];

            return transform;
        }

        /// <summary>
        /// Gets the transformation that project to a plane.<br/>
        /// The transformation maps a point to the point closest to the plane.
        /// </summary>
        /// <param name="plane">Plane to project to.</param>
        /// <returns>A transformation matrix which projects geometry onto a specified plane.</returns>
        public static Transform PlanarProjection(Plane plane)
        {
            Transform transform = Transform.Identity();
            Vector3 x = plane.XAxis;
            Vector3 y = plane.YAxis;
            Point3 pt = plane.Origin;
            double[] q = new double[3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    transform[i][j] = x[i] * x[j] + y[i] * y[j];
                }

                q[i] = transform[i][0] * pt[0] + transform[i][1] * pt[1] + transform[i][2] * pt[2];

                transform[3][i] = 0.0;
                transform[i][3] = pt[i] - q[i];
            }

            return transform;
        }

        /// <summary>
        /// Creates a transformation that orients a planeA to a planeB.
        /// </summary>
        /// <param name="a">The plane to orient from.</param>
        /// <param name="b">The plane to orient to.</param>
        /// <returns>The translation transformation.</returns>
        public static Transform PlaneToPlane(Plane a, Plane b)
        {
            var pt0 = a.Origin;
            var x0 = a.XAxis;
            var y0 = a.YAxis;
            var z0 = a.ZAxis;

            var pt1 = b.Origin;
            var x1 = b.XAxis;
            var y1 = b.YAxis;
            var z1 = b.ZAxis;

            var origin = new Point3(0, 0, 0);

            // Translating point pt0 to (0,0,0)
            var translationA = new Matrix();
            translationA.AddRange(Translation(origin - pt0));
            // Translating point (0,0,0) to pt1
            var translation1 = new Matrix();
            translation1.AddRange(Translation(pt1 - origin));
            
            //plane a as 4x4 transform matrix with axes as column vectors
            Matrix mapA = Matrix.Identity(4);
            mapA[0][0] = x0[0]; mapA[0][1] = y0[0]; mapA[0][2] = z0[0];
            mapA[1][0] = x0[1]; mapA[1][1] = y0[1]; mapA[1][2] = z0[1];
            mapA[2][0] = x0[2]; mapA[2][1] = y0[2]; mapA[2][2] = z0[2];

            //plane b as 4x4 transform matrix with axes as column vectors
            Matrix mapB = Matrix.Identity(4);
            mapB[0][0] = x1[0]; mapB[0][1] = y1[0]; mapB[0][2] = z1[0];
            mapB[1][0] = x1[1]; mapB[1][1] = y1[1]; mapB[1][2] = z1[1];
            mapB[2][0] = x1[2]; mapB[2][1] = y1[2]; mapB[2][2] = z1[2];

            //Transpose plane a matrix. Square matrix transpose same as inverse but cheaper.
            var mapATransposed = mapA.Transpose();
            Matrix result = translation1 * mapB * mapATransposed * translationA;
            var xForm = new Transform();
            xForm.Clear();
            xForm.AddRange(result);
            return xForm;
        }

        /// <summary>
        /// Combines two transformations.<br/>
        /// This is the same as the * operator.
        /// </summary>
        /// <param name="t">Transformation to combine.</param>
        /// <returns>Transformation combined.</returns>
        public Transform Combine(Transform t)
        {
            return this * t;
        }

        /// <summary>
        /// The transform represented in string format.
        /// </summary>
        /// <returns>A text representation.</returns>
        public override string ToString()
        {
            StringBuilder transBuilder = new StringBuilder();

            transBuilder.AppendLine($"R0=({this[0][0]},{this[0][1]},{this[0][2]},{this[0][3]})");
            transBuilder.AppendLine($"R1=({this[1][0]},{this[1][1]},{this[1][2]},{this[1][3]})");
            transBuilder.AppendLine($"R2=({this[2][0]},{this[2][1]},{this[2][2]},{this[2][3]})");
            transBuilder.AppendLine($"R3=({this[3][0]},{this[3][1]},{this[3][2]},{this[3][3]})");

            return transBuilder.ToString();
        }

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

            if ((Math.Abs(transform[1][0]) < GSharkMath.MinTolerance && Math.Abs(transform[0][0]) < GSharkMath.MinTolerance) ||
                (Math.Abs(transform[2][1]) < GSharkMath.MinTolerance && Math.Abs(transform[2][2]) < GSharkMath.MinTolerance) ||
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

            if (Math.Abs(transform[0][1] + transform[1][0]) < GSharkMath.MinTolerance ||
                Math.Abs(transform[0][2] + transform[2][0]) < GSharkMath.MinTolerance ||
                Math.Abs(transform[1][2] + transform[2][1]) < GSharkMath.MinTolerance)
            {
                double xx = (transform[0][0] + 1) / 2;
                double yy = (transform[1][1] + 1) / 2;
                double zz = (transform[2][2] + 1) / 2;
                double xy = (transform[0][1] + transform[1][0]) / 4;
                double xz = (transform[0][2] + transform[2][0]) / 4;
                double yz = (transform[1][2] + transform[2][1]) / 4;
                if ((xx > yy) && (xx > zz))
                { // m[0][0] is the largest diagonal term
                    if (xx < GSharkMath.MinTolerance)
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
                    if (yy < GSharkMath.MinTolerance)
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
                    if (zz < GSharkMath.MinTolerance)
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
    }
}
