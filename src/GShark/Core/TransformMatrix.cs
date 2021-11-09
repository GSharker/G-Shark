using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GShark.Geometry;

namespace GShark.Core
{
    /// <summary>
    /// A column ordered 4x4 transformation matrix.
    /// </summary>
    public class TransformMatrix : IEquatable<TransformMatrix>
    {
        /// <summary>
        /// Creates a TransformMatrix set to the identity matrix.
        /// 1000<br/>
        /// 0100<br/>
        /// 0010<br/>
        /// 0001<br/>
        /// </summary>
        public TransformMatrix()
        {
        }

        /// <summary>
        /// Creates a TransformMatrix from a collection of 12 values representing 4 column vectors of 3 values each.
        /// </summary>
        /// <param name="values"></param>
        public TransformMatrix(IEnumerable<double> values)
        {
            var valuesList = values.ToList();
            if (valuesList.Count != 12)
            {
                throw new ArgumentException($"Expected 12 values, received {valuesList.Count} instead.");
            }

            M00 = valuesList[0];
            M01 = valuesList[1];
            M02 = valuesList[2];

            M10 = valuesList[3];
            M11 = valuesList[4];
            M12 = valuesList[5];

            M20 = valuesList[6];
            M21 = valuesList[7];
            M22 = valuesList[8];

            M30 = valuesList[9];
            M31 = valuesList[10];
            M32 = valuesList[11];

        }

        /// <summary>
        /// Creates a TransformMatrix by copying the values from another.
        /// </summary>
        /// <param name="m">TransformMatrix to copy the values from.</param>
        public TransformMatrix(TransformMatrix m)
        {
            M00 = m.M00;
            M01 = m.M01;
            M02 = m.M02;
            M03 = m.M03;
            M10 = m.M10;
            M11 = m.M11;
            M12 = m.M12;
            M13 = m.M13;
            M20 = m.M20;
            M21 = m.M21;
            M22 = m.M22;
            M23 = m.M23;
            M30 = m.M30;
            M31 = m.M31;
            M32 = m.M32;
            M33 = m.M33;
        }

        public double M00 { get; set; } = 1;

        public double M01 { get; set; } = 0;

        public double M02 { get; set; } = 0;

        public double M03 { get; set; } = 0;

        public double M10 { get; set; } = 0;

        public double M11 { get; set; } = 1;

        public double M12 { get; set; } = 0;

        public double M13 { get; set; } = 0;

        public double M20 { get; set; } = 0;

        public double M21 { get; set; } = 0;

        public double M22 { get; set; } = 1;

        public double M23 { get; set; } = 0;

        public double M30 { get; set; } = 0;

        public double M31 { get; set; } = 0;

        public double M32 { get; set; } = 0;

        public double M33 { get; set; } = 1;

        internal Matrix Matrix
        {
            get
            {
                var matrix = Matrix.Identity(4);

                //column 1
                matrix[0][0] = M00;
                matrix[1][0] = M01;
                matrix[2][0] = M02;
                matrix[3][0] = M03;

                //column 2
                matrix[0][1] = M10;
                matrix[1][1] = M11;
                matrix[2][1] = M12;
                matrix[3][1] = M13;

                //column 3
                matrix[0][2] = M20;
                matrix[1][2] = M21;
                matrix[2][2] = M22;
                matrix[3][2] = M23;

                //column 4
                matrix[0][3] = M30;
                matrix[1][3] = M31;
                matrix[2][3] = M32;
                matrix[3][3] = M33;
                
                return matrix;
            }
        }

        /// <summary>
        /// Set last column values to 0.0, 0.0, 0.0, 1.0.
        /// </summary>
        internal void ZeroTranslation()
        {
            M30 = M31 = M32 = 0.0;
        }

        /// <summary>
        /// Creates a transformation matrix that represents a rotation in radians about an axis.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="theta">Angle in radians.</param>
        /// <returns></returns>
        public static TransformMatrix Rotation(Vector3 axis, double theta)
        {
            var result = new TransformMatrix();
            var axisUnitVector = axis.IsUnitVector ? axis : axis.Unitize();
            var axisX = axisUnitVector.X;
            var axisY = axisUnitVector.Y;
            var axisZ = axisUnitVector.Z;
            var sinTheta = Math.Sin(theta);
            var cosTheta = Math.Cos(theta);
            var k = 1 - cosTheta;
            var kX = k * axisX;
            var kY = k * axisY;
            var kZ = k * axisZ;

            //column 0
            result.M00 = kX * axisX + cosTheta;
            result.M01 = kX * axisY + axisZ * sinTheta;
            result.M02 = kX * axisZ - axisY * sinTheta;
            
            //column 1
            result.M10 = kY * axisX - axis.Z * sinTheta;
            result.M11 = kY * axisY + cosTheta;
            result.M12 = kY * axisZ + axisX * sinTheta;

            //column 2
            result.M20 = kZ * axisX + axisY * sinTheta;
            result.M21 = kZ * axisY - axisX * sinTheta;
            result.M22 = kZ * axisZ + cosTheta;

            return result;
        }

        /// <summary>
        /// Creates a transformation matrix that represents a translation along a vector.
        /// </summary>
        /// <param name="vector">Translation vector.</param>
        /// <returns></returns>
        public static TransformMatrix Translation(Vector3 vector)
        {
            var result = new TransformMatrix
            {
                M30 = vector.X,
                M31 = vector.Y,
                M32 = vector.Z
            };

            return result;
        }

        /// <summary>
        /// Creates a transformation matrix to reflect about a plane.
        /// </summary>
        /// <param name="plane">The plane used to reflect.</param>
        /// <returns>The mirror transformation matrix.</returns>
        public static TransformMatrix Reflection(Plane plane)
        {
            Point3 pt = plane.Origin;
            Vector3 normal = plane.ZAxis;
            var result = new TransformMatrix();

            Vector3 n = normal.Unitize();
            Vector3 t = n * (2.0 * (n[0] * pt[0] + n[1] * pt[1] + n[1] * pt[1]));

            result.M00 = 1 - 2.0 * n[0] * n[0];
            result.M10 = -2.0 * n[0] * n[1];
            result.M20 = -2.0 * n[0] * n[2];
            result.M30 = t[0];

            result.M01 = -2.0 * n[1] * n[0];
            result.M11 = 1 - 2.0 * n[1] * n[1];
            result.M21 = -2.0 * n[1] * n[2];
            result.M31 = t[1];

            result.M02 = -2.0 * n[2] * n[0];
            result.M12 = -2.0 * n[2] * n[1];
            result.M22 = 1 - 2.0 * n[2] * n[2];
            result.M32 = t[2];

            return result;
        }

        /// <summary>
        /// Creates a transformation matrix that represents a non-uniform scaling about an origin point.
        /// </summary>
        /// <param name="xFactor">Scale factor in the x direction.</param>
        /// <param name="yFactor">Scale factor in the y direction.</param>
        /// <param name="zFactor">Scale factor in the z direction.</param>
        /// <returns></returns>
        public static TransformMatrix Scale(double xFactor, double yFactor, double zFactor)
        {
            var result = new TransformMatrix
            {
                M00 = xFactor,
                M11 = yFactor,
                M22 = zFactor
            };

            return result;
        }

        /// <summary>
        /// Gets the transformation that project to a plane.<br/>
        /// The transformation maps a point to the point closest to the plane.
        /// </summary>
        /// <param name="plane">Plane to project to.</param>
        /// <returns>A transformation matrix which projects geometry onto a specified plane.</returns>
        public static TransformMatrix Projection(Plane plane)
        {
            var n = plane.ZAxis.Unitize();
            var origin = plane.Origin;
            var transform = new TransformMatrix();

            transform.M00 = 1.0 - n.X * n.X;
            transform.M11 = 1.0 - n.Y * n.Y;
            transform.M22 = 1.0 - n.Z * n.Z;

            transform.M01 = transform.M10 = -n.X * n.Y;
            transform.M02 = transform.M20 = -n.X * n.Z;
            transform.M12 = transform.M21 = -n.Y * n.Z;

            var q = new Vector3();
            q.X = transform.M00 * origin.X + transform.M10 * origin.Y + transform.M20 * origin.Z;
            q.Y = transform.M01 * origin.X + transform.M11 * origin.Y + transform.M21 * origin.Z;
            q.Z = transform.M02 * origin.X + transform.M12 * origin.Y + transform.M22 * origin.Z;

            var translation = origin - q;

            transform.M30 = translation.X;
            transform.M31 = translation.Y;
            transform.M32 = translation.Z;

            return transform;
        }

        /// <summary>
        /// Transform the specified vector.
        /// </summary>
        /// <param name="p">The vector to transform.</param>
        /// <param name="m">The transformation matrix.</param>
        /// <returns></returns>
        public static Point3 operator *(Point3 p, TransformMatrix m)
        {
            return new Point3(
                p.X * m.M00 + p.Y * m.M10 + p.Z * m.M20 + m.M30,
                p.X * m.M01 + p.Y * m.M11 + p.Z * m.M21 + m.M31,
                p.X * m.M02 + p.Y * m.M12 + p.Z * m.M22 + m.M32
            );
        }

        /// <summary>
        /// Multiplies two transform matrices.
        /// </summary>
        /// <param name="a">Matrix A.</param>
        /// <param name="b">Matrix B.</param>
        /// <returns>The result matrix AB.</returns>
        public static TransformMatrix operator *(TransformMatrix a, TransformMatrix b)
        {
            var result = new TransformMatrix()
            {
                //row 1
                M00 = a.M00 * b.M00 + a.M10 * b.M01 + a.M20 * b.M02 + a.M30 * b.M03,
                M10 = a.M00 * b.M10 + a.M10 * b.M11 + a.M20 * b.M12 + a.M30 * b.M13,
                M20 = a.M00 * b.M20 + a.M10 * b.M21 + a.M20 * b.M22 + a.M30 * b.M23,
                M30 = a.M00 * b.M30 + a.M10 * b.M31 + a.M20 * b.M32 + a.M30 * b.M33,

                //row 2
                M01 = a.M01 * b.M00 + a.M11 * b.M01 + a.M21 * b.M02 + a.M31 * b.M03,
                M11 = a.M01 * b.M10 + a.M11 * b.M11 + a.M21 * b.M12 + a.M31 * b.M13,
                M21 = a.M01 * b.M20 + a.M11 * b.M21 + a.M21 * b.M22 + a.M31 * b.M23,
                M31 = a.M01 * b.M30 + a.M11 * b.M31 + a.M21 * b.M32 + a.M31 * b.M33,

                //row 3
                M02 = a.M02 * b.M00 + a.M12 * b.M01 + a.M22 * b.M02 + a.M32 * b.M03,
                M12 = a.M02 * b.M10 + a.M12 * b.M11 + a.M22 * b.M12 + a.M32 * b.M13,
                M22 = a.M02 * b.M20 + a.M12 * b.M21 + a.M22 * b.M22 + a.M32 * b.M23,
                M32 = a.M02 * b.M30 + a.M12 * b.M31 + a.M22 * b.M32 + a.M32 * b.M33,

                //row 4
                M03 = a.M03 * b.M00 + a.M13 * b.M01 + a.M23 * b.M02 + a.M33 * b.M03,
                M13 = a.M03 * b.M10 + a.M13 * b.M11 + a.M23 * b.M12 + a.M33 * b.M13,
                M23 = a.M03 * b.M20 + a.M13 * b.M21 + a.M23 * b.M22 + a.M33 * b.M23,
                M33 = a.M03 * b.M30 + a.M13 * b.M31 + a.M23 * b.M32 + a.M33 * b.M33,

            };

            return result;
        }

        public TransformMatrix Transpose()
        {
            var a = new TransformMatrix
            {
                M00 = M00,
                M01 = M10,
                M02 = M20,
                M10 = M01,
                M11 = M11,
                M12 = M21,
                M20 = M02,
                M21 = M12,
                M22 = M22,
                M30 = M03,
                M31 = M13,
                M32 = M23,
                M33 = M33
            };
            return a;
        }

        /// <summary>
        /// Combines this transform matrix with another. Order matters.<br/>
        /// E.g. If you call translation.Combine(rotation), the combined transformation will represent a translation, followed by a rotation. <br/>
        /// This is the same as using the * operator where M = rotation * translation.
        /// </summary>
        /// <returns>TransformMatrix representing the combined transformations.</returns>
        public TransformMatrix Combine(TransformMatrix other)
        {
            return other * this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var row1 = $"R1:[{M00}] [{M10}] [{M20}] [{M30}]";
            var row2 = $"R2:[{M01}] [{M11}] [{M21}] [{M31}]";
            var row3 = $"R3:[{M02}] [{M12}] [{M22}] [{M32}]";
            var row4 = $"R4:[{M03}] [{M13}] [{M23}] [{M33}]";
            sb.AppendLine(row1);
            sb.AppendLine(row2);
            sb.AppendLine(row3);
            sb.AppendLine(row4);
            return sb.ToString();
        }

        public bool Equals(TransformMatrix other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return M00.Equals(other.M00) &&
                   M01.Equals(other.M01) &&
                   M02.Equals(other.M02) &&
                   M03.Equals(other.M03) &&
                   M10.Equals(other.M10) &&
                   M11.Equals(other.M11) &&
                   M12.Equals(other.M12) &&
                   M13.Equals(other.M13) &&
                   M20.Equals(other.M20) &&
                   M21.Equals(other.M21) &&
                   M22.Equals(other.M22) &&
                   M23.Equals(other.M23) &&
                   M30.Equals(other.M30) &&
                   M31.Equals(other.M31) &&
                   M32.Equals(other.M32) &&
                   M33.Equals(other.M33);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TransformMatrix)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = M00.GetHashCode();
                hashCode = (hashCode * 397) ^ M01.GetHashCode();
                hashCode = (hashCode * 397) ^ M02.GetHashCode();
                hashCode = (hashCode * 397) ^ M03.GetHashCode();
                hashCode = (hashCode * 397) ^ M10.GetHashCode();
                hashCode = (hashCode * 397) ^ M11.GetHashCode();
                hashCode = (hashCode * 397) ^ M12.GetHashCode();
                hashCode = (hashCode * 397) ^ M13.GetHashCode();
                hashCode = (hashCode * 397) ^ M20.GetHashCode();
                hashCode = (hashCode * 397) ^ M21.GetHashCode();
                hashCode = (hashCode * 397) ^ M22.GetHashCode();
                hashCode = (hashCode * 397) ^ M23.GetHashCode();
                hashCode = (hashCode * 397) ^ M30.GetHashCode();
                hashCode = (hashCode * 397) ^ M31.GetHashCode();
                hashCode = (hashCode * 397) ^ M32.GetHashCode();
                hashCode = (hashCode * 397) ^ M33.GetHashCode();
                return hashCode;
            }
        }
    }
}
