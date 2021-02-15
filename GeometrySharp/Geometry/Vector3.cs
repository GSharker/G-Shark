using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// Vector3 is represented simply by an list of double point numbers.
    /// So, you would write simply [1,0,0] to create a Vector3 in the X direction.
    /// </summary>
    public class Vector3 : List<double>, IEquatable<Vector3>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Vector3()
        {
        }

        /// <summary>
        /// Vector3 initialized by a list of values.
        /// </summary>
        /// <param name="values">List of values.</param>
        public Vector3(IEnumerable<double> values)
        {
            AddRange(values);
        }

        /// <summary>
        /// Gets the value of a point at location GeoSharpMath.UNSETVALUE,GeoSharpMath.UNSETVALUE,GeoSharpMath.UNSETVALUE.
        /// </summary>
        public static Vector3 Unset => new Vector3
            {GeoSharpMath.UNSETVALUE, GeoSharpMath.UNSETVALUE, GeoSharpMath.UNSETVALUE};

        /// <summary>
        /// Gets the value of the vector with components 1,0,0.
        /// </summary>
        public static Vector3 XAxis => new Vector3 { 1.0, 0.0, 0.0 };

        /// <summary>
        /// Gets the value of the vector with components 0,1,0.
        /// </summary>
        public static Vector3 YAxis => new Vector3 { 0.0, 1.0, 0.0 };

        /// <summary>
        /// Gets the value of the vector with components 0,0,1.
        /// </summary>
        public static Vector3 ZAxis => new Vector3 { 0.0, 0.0, 1.0 };

        /// <summary>
        /// The angle in radians between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The angle in radians</returns>
        public static double AngleBetween(Vector3 a, Vector3 b)
        {
            return Math.Acos(Dot(a, b) / (a.Length() * b.Length()));
        }

        /// <summary>
        /// Reverses this vector in place (reverses the direction).
        /// </summary>
        /// <param name="a">The vector to reverse.</param>
        /// <returns>The reversed vector.</returns>
        public static Vector3 Reverse(Vector3 a)
        {
            return new Vector3(a.Select(value => -value));
        }

        /// <summary>
        /// Gets a value indicating whether this vector is valid.
        /// A valid vector must be formed of finite numbers.
        /// </summary>
        /// <param name="a">The vector to be valued.</param>
        /// <returns>True if the vector is valid.</returns>
        public bool IsValid()
        {
            return this.Any(GeoSharpMath.IsValidDouble);
        }

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        /// <param name="a">The vector to check.</param>
        /// <returns>True if all the component are less than Epsilon.</returns>
        public bool IsZero()
        {
            return this.All(value => Math.Abs(value) < GeoSharpMath.EPSILON);
        }

        /// <summary>
        /// Gets the vector amplified by a scalar value.
        /// </summary>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The vector amplified.</returns>
        public Vector3 Amplify(double amplitude)
        {
            return new Vector3(this.Unitize() * amplitude);
        }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public double Length()
        {
            if (!IsValid() || IsZero()) return 0.0;

            return Math.Sqrt(SquaredLength());
        }

        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The sum of each value squared.</returns>
        public double SquaredLength()
        {
            return this.Aggregate(0.0, (x, a) => a * a + x);
        }

        /// <summary>
        /// Determines if this vector is perpendicular to within one degree of another one.
        /// </summary>
        /// <param name="other">Vector to compare to.</param>
        /// <param name="tolerance">Angle tolerance (in radians), if not set used default one degree, expressed in radians.</param>
        /// <returns>True if both vectors are perpendicular.</returns>
        public bool IsPerpendicularTo(Vector3 other, double tolerance = -1)
        {
            var result = false;
            var toleranceSet = (tolerance < 0) ? GeoSharpMath.ANGLETOLERANCE : tolerance;
            var length = this.Length() * other.Length();
            var dotUnitize = Vector3.Dot(this, other) / length;
            if (length > 0 && dotUnitize <= Math.Sin(toleranceSet)) result = true;
            return result;
        }

        /// <summary>
        /// Determines whether this vector is parallel to another vector, within a provided tolerance.
        /// </summary>
        /// <param name="other">Vector to compare to.</param>
        /// <param name="tolerance">Angle tolerance (in radians), if not set used default one degree, expressed in radians.</param>
        /// <returns>A parallel indicator:
        /// 1 vectors are parallel,
        /// 0 vectors are not parallel,
        /// -1 vectors are parallel but with opposite directions </returns>
        public int IsParallelTo(Vector3 other, double tolerance = -1)
        {
            var result = 0;
            var toleranceSet = (tolerance < 0) ? Math.Cos(GeoSharpMath.ANGLETOLERANCE) : Math.Cos(tolerance);
            var length = this.Length() * other.Length();
            var dotUnitize = Vector3.Dot(this, other) / length;
            if (!(length > 0)) return result;
            if (dotUnitize >= toleranceSet) result = 1;
            if (dotUnitize <= -toleranceSet) result = -1;
            return result;
        }

        /// <summary>
        /// Rotates this vector around a given axis.
        /// The rotation is computed using Rodrigues Rotation formula.
        /// https://handwiki.org/wiki/Rodrigues%27_rotation_formula
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle of rotation (in radians).</param>
        /// <returns>Rotated vector.</returns>
        public Vector3 Rotate(Vector3 axis, double angle)
        {
            var cosAngle = Math.Cos(angle);
            var sinAngle = Math.Sin(angle);

            GeoSharpMath.KillNoise(ref sinAngle, ref cosAngle);

            var unitizedAxis = axis.Unitize();
            var cross = Vector3.Cross(unitizedAxis, this);
            var dot = Vector3.Dot(unitizedAxis, this);

            return (this * cosAngle) + (cross * sinAngle) + (unitizedAxis * dot * (1 - cosAngle));
        }

        /// <summary>
        /// Cross product.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Compute the cross product.</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3
            {
                a[1] * b[2] - a[2] * b[1],
                a[2] * b[0] - a[0] * b[2],
                a[0] * b[1] - a[1] * b[0]
            };
        }

        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector with which compute the dot product.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector3 a, Vector3 b)
        {
            return a.Select((t, i) => t * b[i]).Sum();
        }

        /// <summary>
        /// Unitizes the vector. A unit vector has length 1 unit.
        /// </summary>
        /// <returns>A new vector unitized.</returns>
        public Vector3 Unitize()
        {
            var l = this.Length();
            if (l <= 0.0)
                throw new Exception("An invalid or zero length vector cannot be unitized.");
            return this * (1 / l);
        }

        /// <summary>
        /// Create a list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <returns>Get a vector of r dimension.</returns>
        public static Vector3 Zero1d(int rows)
        {
            var v = new Vector3();
            for (var i = 0; i < rows; i++)
                v.Add(0.0);

            return v;
        }

        /// <summary>
        /// Create a 2 dimensional list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <param name="cols">Columns dimension.</param>
        /// <returns>Get a vector of r*c dimension.</returns>
        public static List<Vector3> Zero2d(int rows, int cols)
        {
            var lv = new List<Vector3>();
            for (var i = 0; i < rows; i++) 
                lv.Add(Zero1d(cols));

            return lv;
        }

        /// <summary>
        /// Create a 3 dimensional list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <param name="cols">Columns dimension.</param>
        /// <param name="depth">Depth dimension.</param>
        /// <returns>Get a vector of r*c*d dimension.</returns>
        public static List<List<Vector3>> Zero3d(int rows, int cols, int depth)
        {
            var llv = new List<List<Vector3>>();
            for (var i = 0; i < rows; i++) 
                llv.Add(Zero2d(cols, depth));

            return llv;
        }

        /// <summary>
        /// The distance from this point to b.
        /// </summary>
        /// <param name="v">The target vector.</param>
        /// <returns>The distance between this vector and the provided vector.</returns>
        public double DistanceTo(Vector3 v)
        {
            if (Count != v.Count) throw new Exception("The two list doesn't match in length.");
            return Math.Sqrt(this.Select((val, i) => Math.Pow(val - v[i], 2)).Sum());
        }

        /// <summary>
        /// Get the distance of a point to a line.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <param name="line">The line from which to calculate the distance.</param>
        /// <returns>The distance.</returns>
        public double DistanceTo(Line line)
        {
            var projectedPt = line.ClosestPoint(this);
            var ptToProjectedPt = projectedPt - this;
            return ptToProjectedPt.Length();
        }

        /// <summary>
        /// Determinate if the provided point lies on the plane.
        /// </summary>
        /// <param name="plane">The plane on which to find if the point lies on.</param>
        /// <param name="tol">If the tolerance is not set, as per default is use 1e-6</param>
        /// <returns>Whether the point is on the plane.</returns>
        public bool IsPointOnPlane(Plane plane, double tol = 1e-6)
        {
            return Math.Abs(Dot(this - plane.Origin, plane.Normal)) < tol;
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        /// <param name="v">The vector to multiply.</param>
        /// <param name="a">The scalar value to multiply.</param>
        /// <returns>A vector whose magnitude is multiplied by a.</returns>
        public static Vector3 operator *(Vector3 v, double a)
        {
            return v.Select(val => val * a).ToVector();
        }

        /// <summary>
        /// Multiplying an n x m matrix by a m x 1.
        /// Transform the vector.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="m">Matrix to multiply.</param>
        /// <returns>Vector resulting of multiplying an n x m matrix by a m x 1.</returns>
        public static Vector3 operator *(Vector3 v, Matrix m)
        {
            var mRows = m.Count;
            var mCols = m[0].Count;

            if(mCols != v.Count)
                throw new Exception("Non-conformable matrix and vector");

            var resultVector = Vector3.Zero1d(mRows);

            for (int i = 0; i < mRows; i++)
            {
                var tempValue = 0.0;
                for (int j = 0; j < mCols; j++)
                {
                    tempValue += m[i][j] * v[j];
                }

                resultVector[i] = tempValue;
            }

            return resultVector;
        }

        /// <summary>
        /// Multiplies a 4x4 transformation matrix.
        /// </summary>
        /// <param name="v">A vector.</param>
        /// <param name="t">A transformation.</param>
        /// <returns>The transformed vector.</returns>
        public static Vector3 operator *(Vector3 v, Transform t)
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            var w = 0.0;

            if (v.Count == 4)
            {
                x = t[0][0] * v[0] + t[0][1] * v[1] + t[0][2] * v[2] + t[0][3] * v[3];
                y = t[1][0] * v[0] + t[1][1] * v[1] + t[1][2] * v[2] + t[1][3] * v[3];
                z = t[2][0] * v[0] + t[2][1] * v[1] + t[2][2] * v[2] + t[2][3] * v[3];
                w = t[3][0] * v[0] + t[3][1] * v[1] + t[3][2] * v[2] + t[3][3] * v[3];

                return new Vector3 { x, y, z, w};
            }

            x = t[0][0] * v[0] + t[0][1] * v[1] + t[0][2] * v[2] + t[0][3];
            y = t[1][0] * v[0] + t[1][1] * v[1] + t[1][2] * v[2] + t[1][3];
            z = t[2][0] * v[0] + t[2][1] * v[1] + t[2][2] * v[2] + t[2][3];
            w = t[3][0] * v[0] + t[3][1] * v[1] + t[3][2] * v[2] + t[3][3];

            if (w > 0.0)
            {
                var w2 = 1.0 / w;
                x *= w2;
                y *= w2;
                z *= w2;
            }

            return new Vector3 {x, y, z};
        }

        /// <summary>
        /// Divide a vector by a scalar.
        /// </summary>
        /// <param name="a">The scalar divisor.</param>
        /// <param name="v">The vector to divide.</param>
        /// <returns>A vector whose magnitude is multiplied by a.</returns>
        public static Vector3 operator /(Vector3 v, double a)
        {
            return v.Select(val => val / a).ToVector();
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the difference between a and b.</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return a.Select((val, i) => val - b[i]).ToVector();
        }

        /// <summary>
        /// Add two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the sum of a and b.</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return a.Select((val, i) => val + b[i]).ToVector();
        }

        /// <summary>
        /// Are the two vectors the same within Epsilon?
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Are the two vectors the same within Epsilon?
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Is the vector equal to the provided vector?
        /// </summary>
        /// <param name="obj">The vector to test.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Vector3)) return false;

            var vec = (Vector3)obj;
            return IsAlmostEqualTo(vec);
        }

        /// <summary>
        /// Is the vector equal to the provided vector?
        /// </summary>
        /// <param name="obj">The vector to test.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public bool Equals(Vector3 other)
        {
            return IsAlmostEqualTo(other);
        }

        /// <summary>
        /// Determine whether this vector's components are equal to those of v, within tolerance.
        /// </summary>
        /// <param name="v">The vector to compare.</param>
        /// <returns>
        /// True if the difference of this vector and the supplied vector's components are all within Tolerance, otherwise
        /// false.
        /// </returns>
        public bool IsAlmostEqualTo(Vector3 v)
        {
            return this.Select((val, i) => Math.Abs(val - v[i]))
                .All(val => val < GeoSharpMath.EPSILON);
        }

        /// <summary>
        /// Compute the perpendicular to another vector. Result is not unitized.
        /// </summary>
        /// <param name="other">Vector to use as guide.</param>
        /// <returns>Return the perpendicular vector.</returns>
        /// https://stackoverflow.com/questions/11132681/what-is-a-formula-to-get-a-vector-perpendicular-to-another-vector
        public Vector3 PerpendicularTo(Vector3 other)
        {
            var tempVector = new double[3]; 
            int i, j, k;
            double a, b;
            k = 2;
            if (Math.Abs(other[1]) > Math.Abs(other[0]))
            {
                if (Math.Abs(other[2]) > Math.Abs(other[1]))
                {
                    // |v.z| > |v.y| > |v.x|
                    i = 2;
                    j = 1;
                    k = 0;
                    a = other[2];
                    b = -other[1];
                }
                else if (Math.Abs(other[2]) > Math.Abs(other[0]))
                {
                    // |v.y| >= |v.z| >= |v.x|
                    i = 1;
                    j = 2;
                    k = 0;
                    a = other[1];
                    b = -other[2];
                }
                else
                {
                    // |v.y| > |v.x| > |v.z|
                    i = 1;
                    j = 0;
                    k = 2;
                    a = other[1];
                    b = -other[0];
                }
            }
            else if (Math.Abs(other[2]) > Math.Abs(other[0]))
            {
                // |v.z| > |v.x| >= |v.y|
                i = 2;
                j = 0;
                k = 1;
                a = other[2];
                b = -other[0];
            }
            else if (Math.Abs(other[2]) > Math.Abs(other[1]))
            {
                // |v.x| >= |v.z| > |v.y|
                i = 0;
                j = 2;
                k = 1;
                a = other[0];
                b = -other[2];
            }
            else
            {
                // |v.x| >= |v.y| >= |v.z|
                i = 0;
                j = 1;
                k = 2;
                a = other[0];
                b = -other[1];
            }

            tempVector[i] = b;
            tempVector[j] = a;
            tempVector[k] = 0.0;

            return tempVector.ToVector();
        }

        /// <summary>
        /// Constructs the string representation of the vector.
        /// </summary>
        /// <returns>The vector in string format</returns>
        public override string ToString()
        {
            return this.Count == 4 
                ? $"{Math.Round(this[0], 6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}, {this[3]}" 
                : $"{Math.Round(this[0], 6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}";
        }
    }
}