using GShark.Core;
using System;

namespace GShark.Geometry
{
    public struct Point4d : IEquatable<Point4d>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point4d"/> class based on coordinates.
        /// </summary>
        /// <param name="x">The X (first) dimension.</param>
        /// <param name="y">The Y (second) dimension.</param>
        /// <param name="z">The Z (third) dimension.</param>
        /// <param name="w">The W (fourth) dimension, or weight.</param>
        public Point4d(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4d"/> class from the coordinates of a point.
        /// </summary>
        /// <param name="point">.</param>
        public Point4d(Point3d point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
            W = 1.0;
        }

        /// <summary>
        /// Gets or sets the X (first) coordinate of this point.
        /// </summary>
        public double X { get => X;
            set => X = value;
        }

        /// <summary>
        /// Gets or sets the Y (second) coordinate of this point.
        /// </summary>
        public double Y { get => Y; set => Y = value; }

        /// <summary>
        /// Gets or sets the Z (third) coordinate of this point.
        /// </summary>
        public double Z { get => Z; set => Z = value; }

        /// <summary>
        /// Gets or sets the W (fourth) coordinate -or weight- of this point.
        /// </summary>
        public double W { get => W; set => W = value; }

        /// <summary>
        /// Sums two <see cref="Point4d"/> together.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>A new point that results from the weighted addition of point1 and point2.</returns>
        public static Point4d operator +(Point4d point1, Point4d point2)
        {
            Point4d result = point1; //copy of the value
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
        public static Point4d operator -(Point4d point1, Point4d point2)
        {
            Point4d result = point1; //copy of the value
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
        public static Point4d operator *(Point4d point, double d)
        {
            return new Point4d(point.X * d, point.Y * d, point.Z * d, point.W * d);
        }

        /// <summary>
        /// Multiplies two <see cref="Point4d"/> together, returning the dot (internal) product of the two.
        /// This is not the cross product.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns>A value that results from the coordinatewise multiplication of point1 and point2.</returns>
        public static double operator *(Point4d point1, Point4d point2)
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
        public static bool operator ==(Point4d a, Point4d b)
        {
            return Math.Abs(a.X - b.X) <= GeoSharpMath.Epsilon &&
                   Math.Abs(a.Y - b.Y) <= GeoSharpMath.Epsilon &&
                   Math.Abs(a.Z - b.Z) <= GeoSharpMath.Epsilon &&
                   Math.Abs(a.W - b.W) <= GeoSharpMath.Epsilon;
        }

        /// <summary>
        /// Determines whether two Point4d have different values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the two points differ in any coordinate; false otherwise.</returns>
        public static bool operator !=(Point4d a, Point4d b)
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
            return obj is Point4d && this == (Point4d)obj;
        }

        /// <summary>
        /// Determines whether the specified point has same value as the present point.
        /// </summary>
        /// <param name="point">The specified point.</param>
        /// <returns>true if point has the same value as this; otherwise false.</returns>
        public bool Equals(Point4d point)
        {
            return this == point;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Point4d other, double epsilon)
        {
            return Math.Abs(X - other.X) <= epsilon &&
                   Math.Abs(Y - other.Y) <= epsilon &&
                   Math.Abs(Z - other.Z) <= epsilon &&
                   Math.Abs(Z - other.W) <= epsilon;
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
        /// Gets the value of a point with all coordinates set as RhinoMath.UnsetValue.
        /// </summary>
        public static Point4d Unset => new Point4d(GeoSharpMath.UnsetValue, GeoSharpMath.UnsetValue, GeoSharpMath.UnsetValue, GeoSharpMath.UnsetValue);
    }
}
