using GShark.Core;
using System;

namespace GShark.Geometry
{
    //ToDo Add Operator overload for Vector3d * 4x4 Matrix (represent vector as x,y,z,0 column see https://www.euclideanspace.com/maths/geometry/affine/matrix4x4/index.htm)
    /// <summary>
    /// Defines a Vector in Euclidean space with coordinates X, Y, and Z.
    /// Referenced from https://github.com/mcneel/rhinocommon/blob/master/dotnet/opennurbs/opennurbs_point.cs
    /// </summary>
    public struct Vector3d : IEquatable<Vector3d>, IComparable<Vector3d>, IComparable
    {
        private double _x;
        private double _y;
        private double _z;

        /// <summary>
        /// Initializes a new instance of a vector, using its three components.
        /// </summary>
        /// <param name="x">The X (first) component.</param>
        /// <param name="y">The Y (second) component.</param>
        /// <param name="z">The Z (third) component.</param>
        public Vector3d(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Initializes a new instance of a vector, copying the three components from the three coordinates of a point.
        /// </summary>
        /// <param name="point">The point to copy from.</param>
        public Vector3d(Point3d point) : this(point.X, point.Y, point.Z)
        {
        }

        /// <summary>
        /// Initializes a new instance of a vector, copying the three components from a vector.
        /// </summary>
        /// <param name="vector">A double-precision vector.</param>
        public Vector3d(Vector3d vector) : this(vector.X, vector.Y, vector.Z)
        {
        }

        /// <summary>
        /// Gets the value of the vector with components 0,0,0.
        /// </summary>
        public static Vector3d Zero => new Vector3d(0,0,0);

        /// <summary>
        /// Gets the value of the vector with components 1,0,0.
        /// </summary>
        public static Vector3d XAxis => new Vector3d(1.0, 0.0, 0.0);

        /// <summary>
        /// Gets the value of the vector with components 0,1,0.
        /// </summary>
        public static Vector3d YAxis => new Vector3d(0.0, 1.0, 0.0);

        /// <summary>
        /// Gets the value of the vector with components 0,0,1.
        /// </summary>
        public static Vector3d ZAxis => new Vector3d(0.0, 0.0, 1.0);

        /// <summary>
        /// Gets the value of the vector with each component set to GeoSharpMath.UNSET_VALUE.
        /// </summary>
        public static Vector3d Unset => new Vector3d(GeoSharpMath.UnsetValue, GeoSharpMath.UnsetValue, GeoSharpMath.UnsetValue);

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3d operator *(Vector3d vector, double t)
        {
            return new Vector3d(vector._x * t, vector._y * t, vector._z * t);
        }

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// </summary>
        /// <param name="t">A number.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3d operator *(double t, Vector3d vector)
        {
            return new Vector3d(vector._x * t, vector._y * t, vector._z * t);
        }

        /// <summary>
        /// Divides a <see cref="Vector3d"/> by a number, having the effect of shrinking it.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is componentwise divided by t.</returns>
        public static Vector3d operator /(Vector3d vector, double t)
        {
            return new Vector3d(vector._x / t, vector._y / t, vector._z / t);
        }

        /// <summary>
        /// Sums up two vectors.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise addition of the two vectors.</returns>
        public static Vector3d operator +(Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1._x + vector2._x, vector1._y + vector2._y, vector1._z + vector2._z);
        }

        /// <summary>
        /// Subtracts the second vector from the first one.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise difference of vector1 - vector2.</returns>
        public static Vector3d operator -(Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1._x - vector2._x, vector1._y - vector2._y, vector1._z - vector2._z);
        }

        /// <summary>
        /// Multiplies two vectors together, returning the dot product (or inner product).
        /// This differs from the cross product.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>
        /// A value that results from the evaluation of v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z.
        /// <para>This value equals v1.Length * v2.Length * cos(alpha), where alpha is the angle between vectors.</para>
        /// </returns>
        public static double operator *(Vector3d vector1, Vector3d vector2)
        {
            return (vector1._x * vector2._x + vector1._y * vector2._y + vector1._z * vector2._z);
        }

        /// <summary>
        /// Computes the reversed vector.
        /// </summary>
        /// <param name="vector">A vector to negate.</param>
        /// <returns>A new vector where all components were multiplied by -1.</returns>
        public static Vector3d operator -(Vector3d vector)
        {
            return new Vector3d(-vector._x, -vector._y, -vector._z);
        }

        /// <summary>
        /// Computes the opposite vector.
        /// <para>(Provided for languages that do not support operator overloading. You can use the - unary operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector to negate.</param>
        /// <returns>A new vector where all components were multiplied by -1.</returns>
        public static Vector3d Negate(Vector3d vector)
        {
            return new Vector3d(-vector._x, -vector._y, -vector._z);
        }

        /// <summary>
        /// Determines whether two vectors have the same value.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>true if all coordinates are pairwise equal; false otherwise.</returns>
        public static bool operator ==(Vector3d a, Vector3d b)
        {
            return a._x == b._x && a._y == b._y && a._z == b._z;
        }

        /// <summary>
        /// Determines whether two vectors have different values.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>true if any coordinate pair is different; false otherwise.</returns>
        public static bool operator !=(Vector3d a, Vector3d b)
        {
            return a._x != b._x || a._y != b._y || a._z != b._z;
        }

        /// <summary>
        /// Computes the cross product (or vector product, or exterior product) of two vectors.
        /// <para>This operation is not commutative.</para>
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>A new vector that is perpendicular to both a and b,
        /// <para>has Length == a.Length * b.Length and</para>
        /// <para>with a result that is oriented following the right hand rule.</para>
        /// </returns>
        public static Vector3d CrossProduct(Vector3d a, Vector3d b)
        {
            return new Vector3d(a._y * b._z - b._y * a._z, a._z * b._x - b._z * a._x, a._x * b._y - b._x * a._y);
        }

        /// <summary>
        /// Compute the angle between two vectors.
        /// <para>This operation is commutative.</para>
        /// </summary>
        /// <param name="a">First vector for angle.</param>
        /// <param name="b">Second vector for angle.</param>
        /// <returns>If the input is valid, the angle (in radians) between a and b; GeoSharpMath.UNSET_VALUE otherwise.</returns>
        public static double VectorAngle(Vector3d a, Vector3d b)
        {
            if (!a.Unitize() || !b.Unitize())
            {
                return GeoSharpMath.UnsetValue;
            }

            //compute dot product
            double dot = a._x * b._x + a._y * b._y + a._z * b._z;
            // remove any "noise"
            if (dot > 1.0)
            {
                dot = 1.0;
            }

            if (dot < -1.0)
            {
                dot = -1.0;
            }

            double radians = Math.Acos(dot);
            return radians;
        }

        /// <summary>
        /// Computes the angle on a plane between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="plane">Two-dimensional plane on which to perform the angle measurement.</param>
        /// <returns>On success, the angle (in radians) between a and b as projected onto the plane; GeoSharpMath.UNSET_VALUE on failure.</returns>
        public static double VectorAngle(Vector3d a, Vector3d b, Plane plane)
        {
            throw new NotImplementedException();
            //{ // Project vectors onto plane.
            //    Point3d pA = plane.Origin + a;
            //    Point3d pB = plane.Origin + b;

            //    pA = plane.ClosestPoint(pA);
            //    pB = plane.ClosestPoint(pB);

            //    a = pA - plane.Origin;
            //    b = pB - plane.Origin;
            //}

            //// Abort on invalid cases.
            //if (!a.Unitize()) { return GeoSharpMath.UNSET_VALUE; }
            //if (!b.Unitize()) { return GeoSharpMath.UNSET_VALUE; }

            //double dot = a * b;
            //{ // Limit dot product to valid range.
            //    if (dot >= 1.0)
            //    { dot = 1.0; }
            //    else if (dot < -1.0)
            //    { dot = -1.0; }
            //}

            //double angle = Math.Acos(dot);
            //{ // Special case (anti)parallel vectors.
            //    if (Math.Abs(angle) < 1e-64) { return 0.0; }
            //    if (Math.Abs(angle - Math.PI) < 1e-64) { return Math.PI; }
            //}

            //Vector3d cross = Vector3d.CrossProduct(a, b);
            //if (plane.ZAxis.IsParallelTo(cross) == +1)
            //{
            //    return angle;
            //}

            //return 2.0 * Math.PI - angle;
        }

        /// <summary>
        /// Determines whether the first specified vector comes before (has inferior sorting value than) the second vector.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z;
        /// otherwise, false.</returns>
        public static bool operator <(Vector3d a, Vector3d b)
        {
            if (a.X < b.X)
            {
                return true;
            }

            if (a.X == b.X)
            {
                if (a.Y < b.Y)
                {
                    return true;
                }

                if (a.Y == b.Y && a.Z < b.Z)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified vector comes before
        /// (has inferior sorting value than) the second vector, or it is equal to it.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &lt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator <=(Vector3d a, Vector3d b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Determines whether the first specified vector comes after (has superior sorting value than)
        /// the second vector.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z;
        /// otherwise, false.</returns>
        public static bool operator >(Vector3d a, Vector3d b)
        {
            if (a.X > b.X)
            {
                return true;
            }

            if (a.X == b.X)
            {
                if (a.Y > b.Y)
                {
                    return true;
                }

                if (a.Y == b.Y && a.Z > b.Z)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified vector comes after (has superior sorting value than)
        /// the second vector, or it is equal to it.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &gt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator >=(Vector3d a, Vector3d b)
        {
            return a.CompareTo(b) >= 0;
        }

        /// <summary>
        /// Gets or sets the X (first) component of the vector.
        /// </summary>
        public double X { get => _x; set => _x = value; }
        /// <summary>
        /// Gets or sets the Y (second) component of the vector.
        /// </summary>
        public double Y { get => _y; set => _y = value; }
        /// <summary>
        /// Gets or sets the Z (third) component of the vector.
        /// </summary>
        public double Z { get => _z; set => _z = value; }

        /// <summary>
        /// Gets a value indicating whether this vector is valid. 
        /// A valid vector must be formed of valid component values for x, y and z.
        /// </summary>
        public bool IsValid => GeoSharpMath.IsValidDouble(_x) && GeoSharpMath.IsValidDouble(_y) && GeoSharpMath.IsValidDouble(_z);

        /// <summary>
        /// Gets the smallest (both positive and negative) component value in this vector.
        /// </summary>
        public double MinimumCoordinate
        {
            get
            {
                Point3d p = new Point3d(this);
                return p.MinimumCoordinate;
            }
        }

        /// <summary>
        /// Gets the largest (both positive and negative) component value in this vector.
        /// </summary>
        public double MaximumCoordinate
        {
            get
            {
                Point3d p = new Point3d(this);
                return p.MaximumCoordinate;
            }
        }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// This is an application of Pythagoras' theorem.
        /// If this vector is invalid, its length is considered 0.
        /// </summary>
        public double Length => GetLengthHelper(_x, _y, _z);

        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// This is an application of Pythagoras' theorem.
        /// While the Length property checks for input validity,
        /// this property does not. You should check validity in advance,
        /// if this vector can be invalid.
        /// </summary>
        public double SquareLength => (_x * _x) + (_y * _y) + (_z * _z);

        /// <summary>
        /// Gets a value indicating whether or not this is a unit vector. 
        /// A unit vector has length 1.
        /// </summary>
        public bool IsUnitVector
        {
            get
            {
                // checks for invalid values and returns 0.0 if there are any
                double length = GetLengthHelper(_x, _y, _z);
                //ToDo Rhino implements this check against SqrtEpsilon. Is it necessary?
                return Math.Abs(length - 1.0) <= GeoSharpMath.Epsilon;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        public bool IsZero => (_x == 0.0 && _y == 0.0 && _z == 0.0);


        /// <summary>
        /// Determines whether the specified System.Object is a Vector3d and has the same values as the present vector.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is a Vector3d and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector3d && this == (Vector3d)obj);
        }

        /// <summary>
        /// Determines whether the specified vector has the same value as the present vector.
        /// </summary>
        /// <param name="vector">The specified vector.</param>
        /// <returns>true if vector has the same coordinates as this; otherwise false.</returns>
        public bool Equals(Vector3d vector)
        {
            return this == vector;
        }

        /// <summary>
        /// Compares this <see cref="Vector3d" /> with another <see cref="Vector3d" />.
        /// <para>Component evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Vector3d" /> to use in comparison.</param>
        /// <returns>
        /// <para> 0: if this is identical to other</para>
        /// <para>-1: if this.X &lt; other.X</para>
        /// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
        /// <para>-1: if this.X == other.X and this.Y == other.Y and this.Z &lt; other.Z</para>
        /// <para>+1: otherwise.</para>
        /// </returns>
        public int CompareTo(Vector3d other)
        {
            if (_x < other._x)
            {
                return -1;
            }

            if (_x > other._x)
            {
                return 1;
            }

            if (_y < other._y)
            {
                return -1;
            }

            if (_y > other._y)
            {
                return 1;
            }

            if (_z < other._z)
            {
                return -1;
            }

            if (_z > other._z)
            {
                return 1;
            }

            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Vector3d)
            {
                return CompareTo((Vector3d)obj);
            }

            throw new ArgumentException("Input must be of type Vector3d", "obj");
        }

        /// <summary>
        /// Computes the hash code for the current vector.
        /// </summary>
        /// <returns>A non-unique number that represents the components of this vector.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        /// <summary>
        /// Returns the string representation of the current vector, in the form X,Y,Z.
        /// </summary>
        /// <returns>A string with the current location of the point.</returns>
        public override string ToString()
        {
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
            return string.Format("{0},{1},{2}",
              _x.ToString(culture), _y.ToString(culture), _z.ToString(culture));
        }

        /// <summary>
        /// Unitizes the vector in place. A unit vector has length 1 unit. 
        /// <para>An invalid or zero length vector cannot be unitized.</para>
        /// </summary>
        /// <returns>true on success or false on failure.</returns>
        public bool Unitize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transforms the vector in place.
        /// <para>The transformation matrix acts on the left of the vector; i.e.,</para>
        /// <para>result = transformation*vector</para>
        /// </summary>
        /// <param name="transformation">Transformation matrix to apply.</param>
        public void Transform(Transform transformation)
        {
            throw new NotImplementedException();
            //double xx = transformation.m_00 * _x + transformation.m_01 * _y + transformation.m_02 * _z;
            //double yy = transformation.m_10 * _x + transformation.m_11 * _y + transformation.m_12 * _z;
            //double zz = transformation.m_20 * _x + transformation.m_21 * _y + transformation.m_22 * _z;

            //_x = xx;
            //_y = yy;
            //_z = zz;
        }

        /// <summary>
        /// Rotates this vector around a given axis.
        /// </summary>
        /// <param name="angleRadians">Angle of rotation (in radians).</param>
        /// <param name="rotationAxis">Axis of rotation.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double angleRadians, Vector3d rotationAxis)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Reverses (inverts) this vector in place.
        /// <para>If this vector is Invalid, no changes will occur and false will be returned.</para>
        ///</summary>
        ///<returns>true on success or false if the vector is invalid.</returns>
        public bool Reverse()
        {
            if (!IsValid)
            {
                return false;
            }

            _x = -_x;
            _y = -_y;
            _z = -_z;
            return true;
        }

        /// <summary>
        /// Determines whether this vector is parallel to another vector, within a provided tolerance. 
        /// </summary>
        /// <param name="other">Vector to use for comparison.</param>
        /// <param name="angleTolerance">Angle tolerance (in radians).</param>
        /// <returns>
        /// Parallel indicator:
        /// <para>+1 = both vectors are parallel.</para>
        /// <para>0 = vectors are not parallel or at least one of the vectors is zero.</para>
        /// <para>-1 = vectors are anti-parallel.</para>
        /// </returns>
        public int IsParallelTo(Vector3d other, double angleTolerance = GeoSharpMath.AngleTolerance)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Determines whether this vector is perpendicular to another vector, within a provided angle tolerance. 
        ///</summary>
        /// <param name="other">Vector to use for comparison.</param>
        /// <param name="angleTolerance">Angle tolerance (in radians).</param>
        ///<returns>true if vectors form Pi-radians (90-degree) angles with each other; otherwise false.</returns>
        public bool IsPerpendicularTo(Vector3d other, double angleTolerance = GeoSharpMath.AngleTolerance)
        {
            bool result = false;
            double ll = Length * other.Length;
            if (ll > 0.0)
            {
                if (Math.Abs((_x * other._x + _y * other._y + _z * other._z) / ll) <= Math.Sin(angleTolerance))
                {
                    result = true;
                }
            }
            return result;
        }

        ///<summary>
        /// Sets this vector to be perpendicular to another vector. 
        /// Result is not unitized.
        ///</summary>
        /// <param name="other">Vector to use as guide.</param>
        ///<returns>true on success, false if input vector is zero or invalid.</returns>
        public bool PerpendicularTo(Vector3d other)
        {
            throw new NotImplementedException();
        }
        internal static double GetLengthHelper(double dx, double dy, double dz)
        {
            if (!GeoSharpMath.IsValidDouble(dx) ||
                !GeoSharpMath.IsValidDouble(dy) ||
                !GeoSharpMath.IsValidDouble(dz))
            {
                return 0.0;
            }

            double len;
            double fx = Math.Abs(dx);
            double fy = Math.Abs(dy);
            double fz = Math.Abs(dz);
            if (fy >= fx && fy >= fz)
            {
                len = fx; fx = fy; fy = len;
            }
            else if (fz >= fx && fz >= fy)
            {
                len = fx; fx = fz; fz = len;
            }

            // 15 September 2003 Dale Lear
            //     For small denormalized doubles (positive but smaller
            //     than DBL_MIN), some compilers/FPUs set 1.0/fx to +INF.
            //     Without the ON_DBL_MIN test we end up with
            //     microscopic vectors that have infinite length!
            //
            //     Since this code starts with floats, none of this
            //     should be necessary, but it doesn't hurt anything.
            const double ON_DBL_MIN = 2.2250738585072014e-308;
            if (fx > ON_DBL_MIN)
            {
                len = 1.0 / fx;
                fy *= len;
                fz *= len;
                len = fx * Math.Sqrt(1.0 + fy * fy + fz * fz);
            }
            else if (fx > 0.0 && GeoSharpMath.IsValidDouble(fx))
            {
                len = fx;
            }
            else
            {
                len = 0.0;
            }

            return len;
        }
    }
}
