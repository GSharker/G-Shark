using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using GShark.Core;

namespace GShark.Geometry
{
    /// <summary>
    /// Defines a Vector in Euclidean space with coordinates X, Y, and Z.
    /// Referenced from https://github.com/mcneel/rhinocommon/blob/master/dotnet/opennurbs/opennurbs_point.cs
    /// </summary>
    public struct Point3d : IEquatable<Point3d>, IComparable<Point3d>, IComparable
    {
        private double _x;
        private double _y;
        private double _z;

        /// <summary>
        /// Initializes a new point by defining the X, Y and Z coordinates.
        /// </summary>
        /// <param name="x">The value of the X (first) coordinate.</param>
        /// <param name="y">The value of the Y (second) coordinate.</param>
        /// <param name="z">The value of the Z (third) coordinate.</param>
        public Point3d(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from the components of a vector.
        /// </summary>
        /// <param name="vector">A vector.</param>
        public Point3d(Vector3d vector) : this(vector.X, vector.Y, vector.Z)
        {
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from another point.
        /// </summary>
        /// <param name="point">A point.</param>
        public Point3d(Point3d point) : this(point.X, point.Y, point.Z)
        {
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from a four-dimensional point.
        /// The first three coordinates are divided by the last one.
        /// If the W (fourth) dimension of the input point is zero, then it will be discarded.
        /// </summary>
        /// <param name="point">A point.</param>
        public Point3d(Point4d point)
        {
            double w = (Math.Abs(point.W - 1.0) > GeoSharpMath.EPSILON && point.W != 0.0) ? 1.0 / point.W : 1.0;
            
            _x = point.X * w;
            _y = point.Y * w;
            _z = point.Z * w;
        }

        /// <summary>
        /// Gets the value of a point at location 0,0,0.
        /// </summary>
        public static Point3d Origin => new Point3d(0, 0, 0);

        /// <summary>
        /// Gets the value of a point at location RhinoMath.UnsetValue,RhinoMath.UnsetValue,RhinoMath.UnsetValue.
        /// </summary>
        public static Point3d Unset => new Point3d(GeoSharpMath.UNSET_VALUE, GeoSharpMath.UNSET_VALUE, GeoSharpMath.UNSET_VALUE);

        /// <summary>
        /// Multiplies a <see cref="Point3d"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinate-wise multiplied by t.</returns>
        public static Point3d operator *(Point3d point, double t)
        {
            return new Point3d(point._x * t, point._y * t, point._z * t);
        }

        /// <summary>
        /// Multiplies a <see cref="Point3d"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinate-wise multiplied by t.</returns>
        public static Point3d operator *(double t, Point3d point)
        {
            return new Point3d(point._x * t, point._y * t, point._z * t);
        }

        /// <summary>
        /// Divides a <see cref="Point3d"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinate-wise divided by t.</returns>
        public static Point3d operator /(Point3d point, double t)
        {
            return new Point3d(point._x / t, point._y / t, point._z / t);
        }

        /// <summary>
        /// Sums two <see cref="Point3d"/> instances.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">A point.</param>
        /// <returns>A new point that results from the addition of point1 and point2.</returns>
        public static Point3d operator +(Point3d point1, Point3d point2)
        {
            return new Point3d(point1._x + point2._x, point1._y + point2._y, point1._z + point2._z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3d operator +(Point3d point, Vector3d vector)
        {
            return new Point3d(point._x + vector.X, point._y + vector.Y, point._z + vector.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3d operator +(Vector3d vector, Point3d point)
        {
            return new Point3d(point._x + vector.X, point._y + vector.Y, point._z + vector.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// <para>(Provided for languages that do not support operator overloading. You can use the + operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3d Add(Vector3d vector, Point3d point)
        {
            return new Point3d(point._x + vector.X, point._y + vector.Y, point._z + vector.Z);
        }

        /// <summary>
        /// Subtracts a vector from a point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that is the difference of point minus vector.</returns>
        public static Point3d operator -(Point3d point, Vector3d vector)
        {
            return new Point3d(point._x - vector.X, point._y - vector.Y, point._z - vector.Z);
        }

        /// <summary>
        /// Subtracts a point from another point.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">Another point.</param>
        /// <returns>A new vector that is the difference of point minus vector.</returns>
        public static Vector3d operator -(Point3d point1, Point3d point2)
        {
            return new Vector3d(point1._x - point2._x, point1._y - point2._y, point1._z - point2._z);
        }

        /// <summary>
        /// Computes the additive inverse of all coordinates in the point, and returns the new point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>A point value that, when summed with the point input, yields the <see cref="Origin"/>.</returns>
        public static Point3d operator -(Point3d point)
        {
            return new Point3d(-point._x, -point._y, -point._z);
        }

        /// <summary>
        /// Determines whether two Point3d have equal values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the coordinates of the two points are exactly equal; otherwise false.</returns>
        public static bool operator ==(Point3d a, Point3d b)
        {
            //ToDo Embed Epsilon comparison here instead of implementing a separate IEpsilonEquatable?
            return (a._x == b._x && a._y == b._y && a._z == b._z);
        }

        /// <summary>
        /// Determines whether two Point3d have different values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the two points differ in any coordinate; false otherwise.</returns>
        public static bool operator !=(Point3d a, Point3d b)
        {
            return (a._x != b._x || a._y != b._y || a._z != b._z);
        }

        /// <summary>
        /// Converts a point in a control point, without needing casting.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The control point.</returns>
        public static implicit operator Point4d(Point3d point)
        {
            return new Point4d(point);
        }

        /// <summary>
        /// Converts a point in a vector, without needing casting.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>The resulting vector.</returns>
        public static implicit operator Vector3d(Point3d point)
        {
            return new Vector3d(point);
        }

        /// <summary>
        /// Converts a vector in a point, without needing casting.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <returns>The resulting point.</returns>
        public static implicit operator Point3d(Vector3d vector)
        {
            return new Point3d(vector);
        }

        /// <summary>
        /// Determines whether the first specified point comes before (has inferior sorting value than) the second point.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z;
        /// otherwise, false.</returns>
        public static bool operator <(Point3d a, Point3d b)
        {
            if (a.X < b.X)
                return true;
            if (a.X == b.X)
            {
                if (a.Y < b.Y)
                    return true;
                if (a.Y == b.Y && a.Z < b.Z)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified point comes before
        /// (has inferior sorting value than) the second point, or it is equal to it.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &lt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator <=(Point3d a, Point3d b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Determines whether the first specified point comes after (has superior sorting value than) the second point.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z;
        /// otherwise, false.</returns>
        public static bool operator >(Point3d a, Point3d b)
        {
            if (a.X > b.X)
                return true;
            if (a.X == b.X)
            {
                if (a.Y > b.Y)
                    return true;
                if (a.Y == b.Y && a.Z > b.Z)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified point comes after
        /// (has superior sorting value than) the second point, or it is equal to it.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &gt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator >=(Point3d a, Point3d b)
        {
            return a.CompareTo(b) >= 0;
        }

        /// <summary>
        /// Gets or sets the X (first) coordinate of this point.
        /// </summary>
        public double X { get => _x; set => _x = value;
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
        /// Each coordinate of the point must pass the <see cref="GeoSharpMath.IsValidDouble"/> test.
        /// </summary>
        public bool IsValid => GeoSharpMath.IsValidDouble(_x) && GeoSharpMath.IsValidDouble(_y) && GeoSharpMath.IsValidDouble(_z);

        /// <summary>
        /// Gets the smallest (both positive and negative) coordinate value in this point.
        /// </summary>
        public double MinimumCoordinate
        {
            get
            {
                double minimumCoordinate;
                if (GeoSharpMath.IsValidDouble(_x))
                {
                    minimumCoordinate = System.Math.Abs(_x);
                    if (GeoSharpMath.IsValidDouble(_y) && System.Math.Abs(_y) < minimumCoordinate)
                        minimumCoordinate = System.Math.Abs(_y);
                    if (GeoSharpMath.IsValidDouble(_z) && System.Math.Abs(_z) < minimumCoordinate)
                        minimumCoordinate = System.Math.Abs(_z);
                }
                else if (GeoSharpMath.IsValidDouble(_y))
                {
                    minimumCoordinate = System.Math.Abs(_y);
                    if (GeoSharpMath.IsValidDouble(_z) && System.Math.Abs(_z) < minimumCoordinate)
                        minimumCoordinate = System.Math.Abs(_z);
                }
                else if (GeoSharpMath.IsValidDouble(_z))
                {
                    minimumCoordinate = System.Math.Abs(_z);
                }
                else
                    minimumCoordinate = GeoSharpMath.UNSET_VALUE;
                return minimumCoordinate;
            }
        }

        /// <summary>
        /// Gets the largest (both positive and negative) valid coordinate in this point,
        /// or RhinoMath.UnsetValue if no coordinate is valid.
        /// </summary>
        public double MaximumCoordinate
        {
            get
            {
                double maximumCoordinate;
                if (GeoSharpMath.IsValidDouble(_x))
                {
                    maximumCoordinate = System.Math.Abs(_x);
                    if (GeoSharpMath.IsValidDouble(_y) && System.Math.Abs(_y) > maximumCoordinate)
                        maximumCoordinate = System.Math.Abs(_y);
                    if (GeoSharpMath.IsValidDouble(_z) && System.Math.Abs(_z) > maximumCoordinate)
                        maximumCoordinate = System.Math.Abs(_z);
                }
                else if (GeoSharpMath.IsValidDouble(_y))
                {
                    maximumCoordinate = System.Math.Abs(_y);
                    if (GeoSharpMath.IsValidDouble(_z) && System.Math.Abs(_z) > maximumCoordinate)
                        maximumCoordinate = System.Math.Abs(_z);
                }
                else if (GeoSharpMath.IsValidDouble(_z))
                {
                    maximumCoordinate = System.Math.Abs(_z);
                }
                else
                    maximumCoordinate = GeoSharpMath.UNSET_VALUE;
                return maximumCoordinate;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is a <see cref="Point3d"/> and has the same values as the present point.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is a Point3d and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Point3d && this == (Point3d)obj);
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Point3d other, double epsilon)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares this <see cref="Point3d" /> with another <see cref="Point3d" />.
        /// <para>Component evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Point3d" /> to use in comparison.</param>
        /// <returns>
        /// <para> 0: if this is identical to other</para>
        /// <para>-1: if this.X &lt; other.X</para>
        /// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
        /// <para>-1: if this.X == other.X and this.Y == other.Y and this.Z &lt; other.Z</para>
        /// <para>+1: otherwise.</para>
        /// </returns>
        public int CompareTo(Point3d other)
        {
            if (_x < other._x)
                return -1;
            if (_x > other._x)
                return 1;

            if (_y < other._y)
                return -1;
            if (_y > other._y)
                return 1;

            if (_z < other._z)
                return -1;
            if (_z > other._z)
                return 1;

            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Point3d point3d)
                return CompareTo(point3d);

            throw new ArgumentException("Input must be of type Point3d", nameof(obj));
        }

        /// <summary>
        /// Determines whether the specified <see cref="Point3d"/> has the same values as the present point.
        /// </summary>
        /// <param name="point">The specified point.</param>
        /// <returns>True if point has the same coordinates as this; otherwise false.</returns>
        public bool Equals(Point3d point)
        {
            return this == point;
        }

        /// <summary>
        /// Computes a hash code for the present point.
        /// </summary>
        /// <returns>A non-unique integer that represents this point.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        /// Interpolate between two points.
        /// </summary>
        /// <param name="pA">First point.</param>
        /// <param name="pB">Second point.</param>
        /// <param name="t">Interpolation parameter. 
        /// If t=0 then this point is set to pA. 
        /// If t=1 then this point is set to pB. 
        /// Values of t in between 0.0 and 1.0 result in points between pA and pB.</param>
        public void Interpolate(Point3d pA, Point3d pB, double t)
        {
            _x = pA._x + t * (pB._x - pA._x);
            _y = pA._y + t * (pB._y - pA._y);
            _z = pA._z + t * (pB._z - pA._z);
        }

        /// <summary>
        /// Constructs the string representation for the current point.
        /// </summary>
        /// <returns>The point representation in the form X,Y,Z.</returns>
        public override string ToString()
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return $"{_x.ToString(culture)},{_y.ToString(culture)},{_z.ToString(culture)}";
        }

        /// <summary>
        /// Computes the distance between two points.
        /// </summary>
        /// <param name="other">Other point for distance measurement.</param>
        /// <returns>The length of the line between this and the other point; or 0 if any of the points is not valid.</returns>
        /// <example>
        /// <code source='examples\vbnet\ex_intersectcurves.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_intersectcurves.cs' lang='cs'/>
        /// <code source='examples\py\ex_intersectcurves.py' lang='py'/>
        /// </example>
        public double DistanceTo(Point3d other)
        {
            throw new NotImplementedException();
            //double d;
            //if (IsValid && other.IsValid)
            //{
            //    double dx = other._x - _x;
            //    double dy = other._y - _y;
            //    double dz = other._z - _z;
            //    d = Vector3d.GetLengthHelper(dx, dy, dz);
            //}
            //else
            //{
            //    d = 0.0;
            //}
            //return d;
        }

        /// <summary>
        /// Transforms the point using a transformation matrix.
        /// </summary>
        /// <param name="t">The transformation matrix.</param>
        /// <returns>The transformed point as a new instance.</returns>
        public Point3d Transform(Transform t)
        {
            double x;
            double y;
            double z;
            double w;

            //ToDO Convert to Matrix multiplication! Create a column row vector from the point. i.e. IVector

            x = t[0][0] * X + t[0][1] * Y + t[0][2] * Z + t[0][3];
            y = t[1][0] * X + t[1][1] * Y + t[1][2] * Z + t[1][3];
            z = t[2][0] * X + t[2][1] * Y + t[2][2] * Z + t[2][3];
            w = t[3][0] * X + t[3][1] * Y + t[3][2] * Z + t[3][3];

            if (w <= 0.0)
            {
                return new Point3d(x, y, z);
            }

            double w2 = 1.0 / w;
            x *= w2;
            y *= w2;
            z *= w2;

            return new Point3d(x, y, z);
        }

        /// <summary>
        /// Removes duplicates in the supplied set of points.
        /// </summary>
        /// <param name="points">A list, an array or any enumerable of <see cref="Point3d"/>.</param>
        /// <param name="tolerance">The minimum distance between points.
        /// <para>Points that fall within this tolerance will be discarded.</para>
        /// .</param>
        /// <returns>An array of points without duplicates; or null on error.</returns>
        public static Point3d[] CullDuplicates(System.Collections.Generic.IEnumerable<Point3d> points, double tolerance)
        {
            if (null == points)
                return null;

            // This code duplicates the static function CullDuplicatePoints in tl_brep_intersect.cpp
            var pointList = new List<Point3d>(points);
            int count = pointList.Count;
            if (0 == count)
                return null;

            bool[] dup_list = new bool[count];
            var nonDups = new List<Point3d>();

            for (int i = 0; i < count; i++)
            {
                // Check if the entry has been flagged as a duplicate
                if (dup_list[i] == false)
                {
                    nonDups.Add(pointList[i]);
                    // Only compare with entries that haven't been checked
                    for (int j = i + 1; j < count; j++)
                    {
                        if (pointList[i].DistanceTo(pointList[j]) <= tolerance)
                            dup_list[j] = true;
                    }
                }
            }

            return nonDups.ToArray();
        }
    }
}
