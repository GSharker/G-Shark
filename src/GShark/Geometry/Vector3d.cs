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
        /// <summary>
        /// Initializes a new instance of a vector, using its three components.
        /// </summary>
        /// <param name="x">The X (first) component.</param>
        /// <param name="y">The Y (second) component.</param>
        /// <param name="z">The Z (third) component.</param>
        public Vector3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
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
            return new Vector3d(vector.X * t, vector.Y * t, vector.Z * t);
        }

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// </summary>
        /// <param name="t">A number.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3d operator *(double t, Vector3d vector)
        {
            return new Vector3d(vector.X * t, vector.Y * t, vector.Z * t);
        }

        /// <summary>
        /// Divides a <see cref="Vector3d"/> by a number, having the effect of shrinking it.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is componentwise divided by t.</returns>
        public static Vector3d operator /(Vector3d vector, double t)
        {
            return new Vector3d(vector.X / t, vector.Y / t, vector.Z / t);
        }

        /// <summary>
        /// Sums up two vectors.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise addition of the two vectors.</returns>
        public static Vector3d operator +(Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>
        /// Subtracts the second vector from the first one.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise difference of vector1 - vector2.</returns>
        public static Vector3d operator -(Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
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
            return (vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z);
        }

        /// <summary>
        /// Computes the reversed vector.
        /// </summary>
        /// <param name="vector">A vector to negate.</param>
        /// <returns>A new vector where all components were multiplied by -1.</returns>
        public static Vector3d operator -(Vector3d vector)
        {
            return new Vector3d(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// Determines whether two vectors have the same value.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>true if all coordinates are pairwise equal; false otherwise.</returns>
        public static bool operator ==(Vector3d a, Vector3d b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// Determines whether two vectors have different values.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>true if any coordinate pair is different; false otherwise.</returns>
        public static bool operator !=(Vector3d a, Vector3d b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
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
            return new Vector3d(a.Y * b.Z - b.Y * a.Z, a.Z * b.X - b.Z * a.X, a.X * b.Y - b.X * a.Y);
        }

        /// <summary>
        /// Compute the angle between two vectors.
        /// <para>This operation is commutative.</para>
        /// </summary>
        /// <param name="a">First vector for angle.</param>
        /// <param name="b">Second vector for angle.</param>
        /// <returns>If the input is valid, the angle in radians between a and b; GeoSharpMath.UNSET_VALUE otherwise.</returns>
        public static double VectorAngle(Vector3d a, Vector3d b)
        {
            if (!a.IsValid || !b.IsValid)
            {
                return GeoSharpMath.UnsetValue;
            }

            //compute dot product
            double dot = DotProduct(a.Unitize(), b.Unitize());
            
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
        public double X { get => X; set => X = value; }
        /// <summary>
        /// Gets or sets the Y (second) component of the vector.
        /// </summary>
        public double Y { get => Y; set => Y = value; }
        /// <summary>
        /// Gets or sets the Z (third) component of the vector.
        /// </summary>
        public double Z { get => Z; set => Z = value; }

        /// <summary>
        /// Gets a value indicating whether this vector is valid. 
        /// A valid vector must be formed of valid component values for x, y and z.
        /// </summary>
        public bool IsValid => GeoSharpMath.IsValidDouble(X) && GeoSharpMath.IsValidDouble(Y) && GeoSharpMath.IsValidDouble(Z);

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// This is an application of Pythagoras' theorem.
        /// If this vector is invalid, its length is considered 0.
        /// </summary>
        public double Length => GetLengthHelper(X, Y, Z);

        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// This is an application of Pythagoras' theorem.
        /// While the Length property checks for input validity,
        /// this property does not. You should check validity in advance,
        /// if this vector can be invalid.
        /// </summary>
        public double SquareLength => (X * X) + (Y * Y) + (Z * Z);

        /// <summary>
        /// Gets a value indicating whether or not this is a unit vector. 
        /// A unit vector has length 1.
        /// </summary>
        public bool IsUnitVector
        {
            get
            {
                // checks for invalid values and returns 0.0 if there are any
                double length = GetLengthHelper(X, Y, Z);
                //ToDo Rhino implements this check against SqrtEpsilon. Is it necessary?
                return Math.Abs(length - 1.0) <= GeoSharpMath.Epsilon;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        public bool IsZero => (X == 0.0 && Y == 0.0 && Z == 0.0);

        //Indexer to allow access to properties as array.
        public double this[int i]
        {
            get
            {
                if (i < 0 || i > 2) throw new ArgumentOutOfRangeException();
                if (i == 0) return X;
                if (i == 1) return Y;
                return Z;
            }
            set
            {
                if (i < 0 || i > 2) throw new ArgumentOutOfRangeException();
                if (i == 0) X = value;
                if (i == 1) Y = value;
                Z = value;
            }
        }

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

        public bool EpsilonEquals(Vector3d vector, double epsilon)
        {
            return Math.Abs(X - vector.X) <= epsilon &&
                   Math.Abs(Y - vector.Y) <= epsilon &&
                   Math.Abs(Z - vector.Z) <= epsilon;

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
            if (X < other.X)
            {
                return -1;
            }

            if (X > other.X)
            {
                return 1;
            }

            if (Y < other.Y)
            {
                return -1;
            }

            if (Y > other.Y)
            {
                return 1;
            }

            if (Z < other.Z)
            {
                return -1;
            }

            if (Z > other.Z)
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        /// Returns the string representation of the current vector, in the form X,Y,Z.
        /// </summary>
        /// <returns>A string with the current location of the point.</returns>
        public override string ToString()
        {
            return $"Vector3d: ({X},{Y},{Z})";
        }

        /// <summary>
        /// Unitizes the vector. A unit vector has length 1 unit.
        /// </summary>
        /// <returns>A new vector unitized.</returns>
        public Vector3d Unitize()
        {
            if (IsUnitVector)
            {
                return this;
            }

            if (Length <= double.Epsilon)
            {
                return Unset;
            }

            return this * (1 / Length);
        }

        /// <summary>
        /// Rotates this vector around a given axis.<br/>
        /// The rotation is computed using Rodrigues Rotation formula.<br/>
        /// https://handwiki.org/wiki/Rodrigues%27_rotation_formula
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle of rotation (in radians).</param>
        /// <returns>Rotated vector.</returns>
        public Vector3d Rotate(Vector3d axis, double angle)
        {
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);

            GeoSharpMath.KillNoise(ref sinAngle, ref cosAngle);

            Vector3d unitizedAxis = axis.Unitize();
            Vector3d crossProduct = CrossProduct(unitizedAxis, this);
            double dotProduct = DotProduct(unitizedAxis, this);

            return (this * cosAngle) + (crossProduct * sinAngle) + (unitizedAxis * dotProduct * (1 - cosAngle));
        }

        /// <summary>
        /// Gets a new amplified vector by unitizing and uniformly scaling this vector by the amplitude value.
        /// </summary>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The amplified vector.</returns>
        public Vector3d Amplify(double amplitude)
        {
            if (!GeoSharpMath.IsValidDouble(amplitude))
            {
                throw new ArgumentException("Invalid double value.");
            }

            Vector3d unitVector = Unitize();
            return (unitVector * amplitude);
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

            X = -X;
            Y = -Y;
            Z = -Z;
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
            int result = 0;
            double toleranceSet = Math.Cos(angleTolerance);
            double length = Length * other.Length;
            double dotUnitize = DotProduct(this, other) / length;
            if (!(length > 0))
            {
                return result;
            }

            if (dotUnitize >= toleranceSet)
            {
                result = 1;
            }

            if (dotUnitize <= -toleranceSet)
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// Computes the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector with which compute the dot product.</param>
        /// <returns>The dot product.</returns>
        public static double DotProduct(Vector3d vector1, Vector3d vector2)
        {
            if (!vector1.IsValid || !vector2.IsValid)
            {
                throw new ArgumentException("One of the vectors is invalid.");
            }

            double result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
            return result;
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
                if (Math.Abs((X * other.X + Y * other.Y + Z * other.Z) / ll) <= Math.Sin(angleTolerance))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Computes the perpendicular to another vector.<br/>
        /// https://stackoverflow.com/questions/11132681/what-is-a-formula-to-get-a-vector-perpendicular-to-another-vector <br/>
        /// Result is not unitized.
        /// </summary>
        /// <param name="vector">The other vector.</param>
        /// <returns>The perpendicular vector.</returns>
        public Vector3d PerpendicularTo(Vector3d vector)
            {
                double[] vectorComponents = {vector.X, vector.Y, vector.Z};
                double[] tempVector = new double[3];
                int i, j, k;
                double a, b;
                if (Math.Abs(vectorComponents[1]) > Math.Abs(vectorComponents[0]))
                {
                    if (Math.Abs(vectorComponents[2]) > Math.Abs(vectorComponents[1]))
                    {
                        // |v.z| > |v.y| > |v.x|
                        i = 2;
                        j = 1;
                        k = 0;
                        a = vectorComponents[2];
                        b = -vectorComponents[1];
                    }
                    else if (Math.Abs(vectorComponents[2]) > Math.Abs(vectorComponents[0]))
                    {
                        // |v.y| >= |v.z| >= |v.x|
                        i = 1;
                        j = 2;
                        k = 0;
                        a = vectorComponents[1];
                        b = -vectorComponents[2];
                    }
                    else
                    {
                        // |v.y| > |v.x| > |v.z|
                        i = 1;
                        j = 0;
                        k = 2;
                        a = vectorComponents[1];
                        b = -vectorComponents[0];
                    }
                }
                else if (Math.Abs(vectorComponents[2]) > Math.Abs(vectorComponents[0]))
                {
                    // |v.z| > |v.x| >= |v.y|
                    i = 2;
                    j = 0;
                    k = 1;
                    a = vectorComponents[2];
                    b = -vectorComponents[0];
                }
                else if (Math.Abs(vectorComponents[2]) > Math.Abs(vectorComponents[1]))
                {
                    // |v.x| >= |v.z| > |v.y|
                    i = 0;
                    j = 2;
                    k = 1;
                    a = vectorComponents[0];
                    b = -vectorComponents[2];
                }
                else
                {
                    // |v.x| >= |v.y| >= |v.z|
                    i = 0;
                    j = 1;
                    k = 2;
                    a = vectorComponents[0];
                    b = -vectorComponents[1];
                }

                tempVector[i] = b;
                tempVector[j] = a;
                tempVector[k] = 0.0;

                return new Vector3d(tempVector[0], tempVector[1], tempVector[2]);
            }

        /// <summary>
        /// Computes the perpendicular of a vector given three points.<br/>
        /// Result is not unitized.
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <returns>The perpendicular vector.</returns>
        public Vector3d PerpendicularTo(Point3d pt1, Point3d pt2, Point3d pt3)
        {
            Vector3d vec0 = pt3 - pt2;
            Vector3d vec1 = pt1 - pt3;
            Vector3d vec2 = pt2 - pt1;

            Vector3d normal0 = CrossProduct(vec1, vec2);
            if (normal0.Length <= double.Epsilon)
            {
                return Unset;
            }
            Vector3d normal1 = CrossProduct(vec2, vec0);
            if (normal1.Length <= double.Epsilon)
            {
                return Unset;
            }
            Vector3d normal2 = CrossProduct(vec0, vec1);
            if (normal2.Length <= double.Epsilon)
            {
                return Unset;
            }

            double s0 = 1.0 / vec0.Length;
            double s1 = 1.0 / vec1.Length;
            double s2 = 1.0 / vec2.Length;

            // choose normal with smallest total error
            double e0 = s0 * Math.Abs(DotProduct(normal0, vec0)) +
                        s1 * Math.Abs(DotProduct(normal0, vec1)) +
                        s2 * Math.Abs(DotProduct(normal0, vec2));

            double e1 = s0 * Math.Abs(DotProduct(normal1, vec0)) +
                        s1 * Math.Abs(DotProduct(normal1, vec1)) +
                        s2 * Math.Abs(DotProduct(normal1, vec2));

            double e2 = s0 * Math.Abs(DotProduct(normal2, vec0)) +
                        s1 * Math.Abs(DotProduct(normal2, vec1)) +
                        s2 * Math.Abs(DotProduct(normal2, vec2));

            if (e0 <= e1)
            {
                return e0 <= e2 ? normal0 : normal2;
            }
            return e1 <= e2 ? normal1 : normal2;
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
            //ToDo Substitute with GSharkMath const.
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
