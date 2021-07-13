using GShark.Core;
using System;

namespace GShark.Geometry
{
    public struct Point4 : IEquatable<Point4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point4"/> class based on coordinates.
        /// </summary>
        /// <param name="x">The X (first) dimension.</param>
        /// <param name="y">The Y (second) dimension.</param>
        /// <param name="z">The Z (third) dimension.</param>
        /// <param name="w">The W (fourth) dimension, or weight.</param>
        public Point4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4"/> class from the coordinates of a point.
        /// </summary>
        /// <param name="point">.</param>
        public Point4(Point3 point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
            W = 1.0;
        }

        /// <summary>
        /// Gets or sets the X (first) coordinate of this point.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y (second) coordinate of this point.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the Z (third) coordinate of this point.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Gets or sets the W (fourth) coordinate -or weight- of this point.
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Dimension of point.
        /// </summary>
        public int Size => 4;

        /// <summary>
        /// Gets the value of a point with all coordinates set as RhinoMath.UnsetValue.
        /// </summary>
        public static Point4 Unset => new Point4(GeoSharkMath.UnsetValue, GeoSharkMath.UnsetValue, GeoSharkMath.UnsetValue, GeoSharkMath.UnsetValue);

        //Indexer to allow access to properties as array.
        public double this[int i]
        {
            get
            {
                return i switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    3 => W,
                    _ => throw new IndexOutOfRangeException()
                };
            }
            set
            {
                if (i < 0 || i > 3) throw new IndexOutOfRangeException();
                switch (i)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Transforms the point using a transformation matrix.
        /// </summary>
        /// <param name="t">The transformation matrix.</param>
        /// <returns>The transformed point as a new instance.</returns>
        public Point4 Transform(Transform t)
        {
            double num1 = t[0][0] * X + t[0][1] * Y + t[0][2] * Z + t[0][3] * W;
            double num2 = t[1][0] * X + t[1][1] * Y + t[1][2] * Z + t[1][3] * W;
            double num3 = t[2][0] * X + t[2][1] * Y + t[2][2] * Z + t[2][3] * W;
            double num4 = t[3][0] * X + t[3][1] * Y + t[3][2] * Z + t[3][3] * W;

            return new Point4(num1, num2, num3, num4);
        }

        /// <summary>
        /// Sums two <see cref="Point4"/> together.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>A new point that results from the weighted addition of point1 and point2.</returns>

        public static Point4 operator +(Point4 point1, Point4 point2)
        {
            Point4 result = point1; //copy of the value
            if (point2.W == point1.W)
            {
                result.X += point2.X;
                result.Y += point2.Y;
                result.Z += point2.Z;
            }
            else if (point2.W == 0)
            {
                result.X += point2.X;
                result.Y += point2.Y;
                result.Z += point2.Z;
            }
            else if (point1.W == 0)
            {
                result.X += point2.X;
                result.Y += point2.Y;
                result.Z += point2.Z;
                result.W = point2.W;
            }
            else
            {
                double sw1 = (point1.W > 0.0) ? Math.Sqrt(point1.W) : -Math.Sqrt(-point1.W);
                double sw2 = (point2.W > 0.0) ? Math.Sqrt(point2.W) : -Math.Sqrt(-point2.W);
                double s1 = sw2 / sw1;
                double s2 = sw1 / sw2;
                result.X = point1.X * s1 + point2.X * s2;
                result.Y = point1.Y * s1 + point2.Y * s2;
                result.Z = point1.Z * s1 + point2.Z * s2;
                result.W = sw1 * sw2;
            }
            return result;
        }

        /// <summary>
        /// Subtracts the second point from the first point.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>A new point that results from the weighted subtraction of point2 from point1.</returns>
        public static Point4 operator -(Point4 point1, Point4 point2)
        {
            Point4 result = point1; //copy of the value
            if (point2.W == point1.W)
            {
                result.X -= point2.X;
                result.Y -= point2.Y;
                result.Z -= point2.Z;
            }
            else if (point2.W == 0.0)
            {
                result.X -= point2.X;
                result.Y -= point2.Y;
                result.Z -= point2.Z;
            }
            else if (point1.W == 0.0)
            {
                result.X -= point2.X;
                result.Y -= point2.Y;
                result.Z -= point2.Z;
                result.W = point2.W;
            }
            else
            {
                double sw1 = (point1.W > 0.0) ? Math.Sqrt(point1.W) : -Math.Sqrt(-point1.W);
                double sw2 = (point2.W > 0.0) ? Math.Sqrt(point2.W) : -Math.Sqrt(-point2.W);
                double s1 = sw2 / sw1;
                double s2 = sw1 / sw2;
                result.X = point1.X * s1 - point2.X * s2;
                result.Y = point1.Y * s1 - point2.Y * s2;
                result.Z = point1.Z * s1 - point2.Z * s2;
                result.W = sw1 * sw2;
            }
            return result;
        }

        /// <summary>
        /// Multiplies a point by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="d">A number.</param>
        /// <returns>A new point that results from the coordinatewise multiplication of point with d.</returns>
        public static Point4 operator *(Point4 point, double d)
        {
            return new Point4(point.X * d, point.Y * d, point.Z * d, point.W * d);
        }

        /// <summary>
        /// Multiplies two <see cref="Point4"/> together, returning the dot (internal) product of the two.
        /// This is not the cross product.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>A value that results from the coordinatewise multiplication of point1 and point2.</returns>
        public static double operator *(Point4 point1, Point4 point2)
        {
            return (point1.X * point2.X) +
              (point1.Y * point2.Y) +
              (point1.Z * point2.Z) +
              (point1.W * point2.W);
        }

        /// <summary>
        /// Determines whether two Point4d have equal values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the coordinates of the two points are equal; otherwise false.</returns>
        public static bool operator ==(Point4 a, Point4 b)
        {
            return Math.Abs(a.X - b.X) <= GeoSharkMath.Epsilon &&
                   Math.Abs(a.Y - b.Y) <= GeoSharkMath.Epsilon &&
                   Math.Abs(a.Z - b.Z) <= GeoSharkMath.Epsilon &&
                   Math.Abs(a.W - b.W) <= GeoSharkMath.Epsilon;
        }

        /// <summary>
        /// Determines whether two Point4d have different values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the two points differ in any coordinate; false otherwise.</returns>
        public static bool operator !=(Point4 a, Point4 b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the specified System.Object is Point4d and has same coordinates as the present point.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is Point4d and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Point4 && this == (Point4)obj;
        }

        /// <summary>
        /// Determines whether the specified point has same value as the present point.
        /// </summary>
        /// <param name="point">The specified point.</param>
        /// <returns>true if point has the same value as this; otherwise false.</returns>
        public bool Equals(Point4 point)
        {
            return this == point;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Point4 other, double epsilon)
        {
            return Math.Abs(X - other.X) <= epsilon &&
                   Math.Abs(Y - other.Y) <= epsilon &&
                   Math.Abs(Z - other.Z) <= epsilon &&
                   Math.Abs(W - other.W) <= epsilon;
        }

        /// <summary>
        /// Computes the hash code for the present point.
        /// </summary>
        /// <returns>A non-unique hash code, which uses all coordiantes of this object.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        /// <summary>
        /// Constructs the string representation for the current point.
        /// </summary>
        /// <returns>The point representation in the form X,Y,Z,W.</returns>
        public override string ToString()
        {
            return $"Point4: ({GeoSharkMath.Truncate(X)},{GeoSharkMath.Truncate(Y)},{GeoSharkMath.Truncate(Z)},{GeoSharkMath.Truncate(W)})";
        }
    }
}
