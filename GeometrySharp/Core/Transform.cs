using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
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
