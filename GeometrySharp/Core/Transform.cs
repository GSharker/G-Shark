using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.Geometry;

namespace GeometrySharp.Core
{
    public class Transform : List<IList<double>>
    {
        /// <summary>
        /// Initializes a 4 x 4 transformation matrix.
        /// All the values are set to zero.
        /// </summary>
        public Transform()
        {
            this.AddRange(Matrix.Construct(4, 4));
        }

        /// <summary>
        /// A new identity transformation matrix. An identity matrix defines no transformation.
        /// The diagonal is (1,1,1,1)
        /// </summary>
        /// <returns>Gets the identity transformation matrix.</returns>
        public static Transform Identity()
        {
            var transform = new Transform
            {
                [0] = { [0] = 1 },
                [1] = { [1] = 1 },
                [2] = { [2] = 1 },
                [3] = { [3] = 1 },
            };
            return transform;
        }

        /// <summary>
        /// Construct a new translation transformation.
        /// </summary>
        /// <param name="v">Translation vector.</param>
        /// <returns>A transformation matrix which moves the geometry along the vector.</returns>
        public static Transform Translation(Vector3 v)
        {
            return Translation(v[0], v[1], v[2]);
        }

        /// <summary>
        /// Construct a new translation transformation.
        /// </summary>
        /// <param name="x">Transform the x value.</param>
        /// <param name="y">Transform the y value.</param>
        /// <param name="z">Transform the z value.</param>
        /// <returns>A transformation matrix which moves the geometry along the vector.</returns>
        public static Transform Translation(double x, double y, double z)
        {
            var transform = Identity();
            transform[0][3] = x;
            transform[1][3] = y;
            transform[2][3] = z;
            transform[3][3] = 1.0;

            return transform;
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="angle">Angle in radians of the rotation.</param>
        /// <param name="axis">Axis direction.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        public Transform Rotation(double angle, Line axis)
        {
            return Rotation(Math.Sin(angle), Math.Cos(angle), axis.Direction, axis.Start);
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified angle, rotation center and rotation axis pointing up.
        /// </summary>
        /// <param name="angle">Angle in radians of the rotation.</param>
        /// <param name="center">Center point of rotation. Rotation axis is vertical.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        public static Transform Rotation(double angle, Vector3 center)
        {
            return Rotation(Math.Sin(angle), Math.Cos(angle), Vector3.ZAxis, center);
        }

        /// <summary>
        /// Constructs a new rotation transformation with Sin and Cos angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="sinAngle">Sin angle.</param>
        /// <param name="cosAngle">Cos angle.</param>
        /// <param name="axis">Axis direction.</param>
        /// <param name="origin">Rotation center.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        internal static Transform Rotation(double sinAngle, double cosAngle, Vector3 axis, Vector3 origin)
        {
            var sAngle = sinAngle;
            var cAngle = cosAngle;

            GeoSharpMath.KillNoise(ref sAngle, ref cAngle);

            var transform = Identity();
            var oneMinusCosAngle = 1 - cosAngle;

            transform[0][0] = axis[0] * axis[0] * oneMinusCosAngle + cAngle;
            transform[0][1] = axis[0] * axis[1] * oneMinusCosAngle - axis[2] * sAngle;
            transform[0][2] = axis[0] * axis[2] * oneMinusCosAngle + axis[1] * sAngle;

            transform[1][0] = axis[1] * axis[0] * oneMinusCosAngle + axis[2] * sAngle;
            transform[1][1] = axis[1] * axis[1] * oneMinusCosAngle + cAngle;
            transform[1][2] = axis[1] * axis[2] * oneMinusCosAngle - axis[0] * sAngle;

            transform[2][0] = axis[2] * axis[0] * oneMinusCosAngle - axis[1] * sAngle;
            transform[2][1] = axis[2] * axis[1] * oneMinusCosAngle + axis[0] * sAngle;
            transform[2][2] = axis[2] * axis[2] * oneMinusCosAngle + cAngle;

            if (!origin.Equals(new Vector3 {0.0, 0.0, 0.0}))
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
        /// Create a uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="anchorPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static Transform Scale(Vector3 anchorPoint, double scaleFactor)
        {
            return Scale(anchorPoint, scaleFactor, scaleFactor, scaleFactor);
        }

        /// <summary>
        /// Create non uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="anchorPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static Transform Scale(Vector3 anchorPoint, double factorX, double factorY, double factorZ)
        {
            var origin = new Vector3 {0.0, 0.0, 0.0};
            var scale = Scale(factorX, factorY, factorZ);
            if(anchorPoint.Equals(origin)) return scale;

            var dir = anchorPoint - origin;
            var t0 = Translation(Vector3.Reverse(dir));
            var t1 = Translation(dir);

            return t1 * scale * t0;
        }

        /// <summary>
        /// Create non uniform scale transformation matrix with the anchor point in the origin.
        /// </summary>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static Transform Scale(double factorX, double factorY, double factorZ)
        {
            var tIdentity = Transform.Identity();
            tIdentity[0][0] = factorX;
            tIdentity[1][1] = factorY;
            tIdentity[2][2] = factorZ;
            return tIdentity;
        }

        /// <summary>
        /// Creates a transform matrix copying another transform.
        /// </summary>
        /// <param name="other">The transform to copy</param>
        public static Transform Copy(Transform other)
        {
            var transformCopy = new Transform();
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
        /// Multiply two transformation matrix.
        /// </summary>
        /// <param name="t0">First transformation.</param>
        /// <param name="t1">Second transformation.</param>
        /// <returns>New transformation.</returns>
        public static Transform operator *(Transform t0, Transform t1)
        {
            var t = new Transform
            {
                [0] = {[0] = t0[0][0] * t1[0][0] + t0[0][1] * t1[1][0] + t0[0][2] * t1[2][0] + t0[0][3] * t1[3][0]},
                [0] = {[1] = t0[0][0] * t1[0][1] + t0[0][1] * t1[1][1] + t0[0][2] * t1[2][1] + t0[0][3] * t1[3][1]},
                [0] = {[2] = t0[0][0] * t1[0][2] + t0[0][1] * t1[1][2] + t0[0][2] * t1[2][2] + t0[0][3] * t1[3][2]},
                [0] = {[3] = t0[0][0] * t1[0][3] + t0[0][1] * t1[1][3] + t0[0][2] * t1[2][3] + t0[0][3] * t1[3][3]},
                [1] = {[0] = t0[1][0] * t1[0][0] + t0[1][1] * t1[1][0] + t0[1][2] * t1[2][0] + t0[1][3] * t1[3][0]},
                [1] = {[1] = t0[1][0] * t1[0][1] + t0[1][1] * t1[1][1] + t0[1][2] * t1[2][1] + t0[1][3] * t1[3][1]},
                [1] = {[2] = t0[1][0] * t1[0][2] + t0[1][1] * t1[1][2] + t0[1][2] * t1[2][2] + t0[1][3] * t1[3][2]},
                [1] = {[3] = t0[1][0] * t1[0][3] + t0[1][1] * t1[1][3] + t0[1][2] * t1[2][3] + t0[1][3] * t1[3][3]},
                [2] = {[0] = t0[2][0] * t1[0][0] + t0[2][1] * t1[1][0] + t0[2][2] * t1[2][0] + t0[2][3] * t1[3][0]},
                [2] = {[1] = t0[2][0] * t1[0][1] + t0[2][1] * t1[1][1] + t0[2][2] * t1[2][1] + t0[2][3] * t1[3][1]},
                [2] = {[2] = t0[2][0] * t1[0][2] + t0[2][1] * t1[1][2] + t0[2][2] * t1[2][2] + t0[2][3] * t1[3][2]},
                [2] = {[3] = t0[2][0] * t1[0][3] + t0[2][1] * t1[1][3] + t0[2][2] * t1[2][3] + t0[2][3] * t1[3][3]},
                [3] = {[0] = t0[3][0] * t1[0][0] + t0[3][1] * t1[1][0] + t0[3][2] * t1[2][0] + t0[3][3] * t1[3][0]},
                [3] = {[1] = t0[3][0] * t1[0][1] + t0[3][1] * t1[1][1] + t0[3][2] * t1[2][1] + t0[3][3] * t1[3][1]},
                [3] = {[2] = t0[3][0] * t1[0][2] + t0[3][1] * t1[1][2] + t0[3][2] * t1[2][2] + t0[3][3] * t1[3][2]},
                [3] = {[3] = t0[3][0] * t1[0][3] + t0[3][1] * t1[1][3] + t0[3][2] * t1[2][3] + t0[3][3] * t1[3][3]}
            };

            return t;
        }

        /// <summary>
        /// Create a transformation matrix to reflect about a plane.
        /// </summary>
        /// <param name="plane">The plane used to reflect.</param>
        /// <returns>The mirror transformation matrix.</returns>
        public static Transform Reflection(Plane plane)
        {
            Vector3 pt = plane.Origin;
            Vector3 normal = plane.Normal;
            Transform transform = Transform.Identity();

            Vector3 unitizedN = normal.Unitize();
            Vector3 translation = unitizedN * (2.0 * (unitizedN[0]*pt[0] + unitizedN[1] * pt[1] + unitizedN[1] * pt[1]));

            transform[0][0] = 1 - 2.0 * unitizedN[0] * unitizedN[0];
            transform[0][1] = - 2.0 * unitizedN[0] * unitizedN[1];
            transform[0][2] = - 2.0 * unitizedN[0] * unitizedN[2];
            transform[0][3] = translation[0];

            transform[1][0] = - 2.0 * unitizedN[1] * unitizedN[0];
            transform[1][1] = 1 - 2.0 * unitizedN[1] * unitizedN[1];
            transform[1][2] = -2.0 * unitizedN[1] * unitizedN[2];
            transform[1][3] = translation[1];

            transform[2][0] = -2.0 * unitizedN[2] * unitizedN[0];
            transform[2][1] = -2.0 * unitizedN[2] * unitizedN[1];
            transform[2][2] = 1 - 2.0 * unitizedN[2] * unitizedN[2];
            transform[2][3] = translation[2];

            return transform;
        }

        // ToDo has to be finished.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Transform ChangeBasis(Plane a, Plane b)
        {
            return new Transform();
        }

        /// <summary>
        /// Combines two transformations.
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
    }
}
