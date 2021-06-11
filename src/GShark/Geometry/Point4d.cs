using GShark.Core;
using System;

namespace GShark.Geometry
{
    public struct Point4d : IEquatable<Point4d>
    {
        private double _x;
        private double _y;
        private double _z;
        private double _w;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4d"/> class based on coordinates.
        /// </summary>
        /// <param name="x">The X (first) dimension.</param>
        /// <param name="y">The Y (second) dimension.</param>
        /// <param name="z">The Z (third) dimension.</param>
        /// <param name="w">The W (fourth) dimension, or weight.</param>
        public Point4d(double x, double y, double z, double w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4d"/> class from the coordinates of a point.
        /// </summary>
        /// <param name="point">.</param>
        public Point4d(Point3d point)
        {
            _x = point.X;
            _y = point.Y;
            _z = point.Z;
            _w = 1.0;
        }

        /// <summary>
        /// Gets or sets the X (first) coordinate of this point.
        /// </summary>
        public double X { get => _x;
            set => _x = value;
        }

        /// <summary>
        /// Gets or sets the Y (second) coordinate of this point.
        /// </summary>
        public double Y { get => _y; set => _y = value; }

        /// <summary>
        /// Gets or sets the Z (third) coordinate of this point.
        /// </summary>
        public double Z { get => _z; set => _z = value; }

        /// <summary>
        /// Gets or sets the W (fourth) coordinate -or weight- of this point.
        /// </summary>
        public double W { get => _w; set => _w = value; }

        /// <summary>
        /// Sums two <see cref="Point4d"/> together.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>A new point that results from the weighted addition of point1 and point2.</returns>
        public static Point4d operator +(Point4d point1, Point4d point2)
        {
            Point4d result = point1; //copy of the value
            if (point2._w == point1._w)
            {
                result._x += point2._x;
                result._y += point2._y;
                result._z += point2._z;
            }
            else if (point2._w == 0)
            {
                result._x += point2._x;
                result._y += point2._y;
                result._z += point2._z;
            }
            else if (point1._w == 0)
            {
                result._x += point2._x;
                result._y += point2._y;
                result._z += point2._z;
                result._w = point2._w;
            }
            else
            {
                double sw1 = (point1._w > 0.0) ? Math.Sqrt(point1._w) : -Math.Sqrt(-point1._w);
                double sw2 = (point2._w > 0.0) ? Math.Sqrt(point2._w) : -Math.Sqrt(-point2._w);
                double s1 = sw2 / sw1;
                double s2 = sw1 / sw2;
                result._x = point1._x * s1 + point2._x * s2;
                result._y = point1._y * s1 + point2._y * s2;
                result._z = point1._z * s1 + point2._z * s2;
                result._w = sw1 * sw2;
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
            if (point2._w == point1._w)
            {
                result._x -= point2._x;
                result._y -= point2._y;
                result._z -= point2._z;
            }
            else if (point2._w == 0.0)
            {
                result._x -= point2._x;
                result._y -= point2._y;
                result._z -= point2._z;
            }
            else if (point1._w == 0.0)
            {
                result._x -= point2._x;
                result._y -= point2._y;
                result._z -= point2._z;
                result._w = point2._w;
            }
            else
            {
                double sw1 = (point1._w > 0.0) ? Math.Sqrt(point1._w) : -Math.Sqrt(-point1._w);
                double sw2 = (point2._w > 0.0) ? Math.Sqrt(point2._w) : -Math.Sqrt(-point2._w);
                double s1 = sw2 / sw1;
                double s2 = sw1 / sw2;
                result._x = point1._x * s1 - point2._x * s2;
                result._y = point1._y * s1 - point2._y * s2;
                result._z = point1._z * s1 - point2._z * s2;
                result._w = sw1 * sw2;
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
            return new Point4d(point._x * d, point._y * d, point._z * d, point._w * d);
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
            return (point1._x * point2._x) +
              (point1._y * point2._y) +
              (point1._z * point2._z) +
              (point1._w * point2._w);
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
            return Math.Abs(_x - other.X) <= epsilon &&
                   Math.Abs(_y - other.Y) <= epsilon &&
                   Math.Abs(_z - other.Z) <= epsilon &&
                   Math.Abs(_z - other.W) <= epsilon;
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
