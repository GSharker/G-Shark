using GShark.Core;
using GShark.ExtendedMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// Vector3 is represented simply by an list of double numbers, Vector3 is also used to represent point.<br/>
    /// So, you would write simply [1,0,0] to create a Vector3 in the X direction.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Geometry/VectorTests.cs?name=example)]
    /// </example>
    public class Vector : List<double>, IEquatable<Vector>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Vector()
        {
        }

        /// <summary>
        /// Vector3 initialized by a list of values.
        /// </summary>
        /// <param name="values">List of values.</param>
        public Vector(IEnumerable<double> values)
        : base(values)
        {
        }

        /// <summary>
        /// Reverses this vector in place (reverses the direction).
        /// </summary>
        /// <param name="a">The vector to reverse.</param>
        /// <returns>The reversed vector.</returns>
        public static Vector Reverse(Vector a)
        {
            return new Vector(a.Select(value => -value));
        }

        /// <summary>
        /// Gets a value indicating whether this vector is valid.<br/>
        /// A valid vector must be formed of finite numbers.
        /// </summary>
        /// <param name="a">The vector to be valued.</param>
        /// <returns>True if the vector is valid.</returns>
        public bool IsValid()
        {
            return this.Any(GeoSharkMath.IsValidDouble);
        }

        /// <summary>
        /// Test a vector to see if it is unitzed.
        /// </summary>
        /// <returns>True if vector is unitized.</returns>
        public bool IsUnitized()
        {
            return (GeoSharkMath.IsValidDouble(this[0]) &&
                    GeoSharkMath.IsValidDouble(this[1]) &&
                    GeoSharkMath.IsValidDouble(this[2]) &&
                    (Math.Abs(Length() - 1.0) <= GeoSharkMath.Epsilon));
        }

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        /// <param name="a">The vector to check.</param>
        /// <returns>True if all the component are less than Epsilon.</returns>
        public bool IsZero()
        {
            return this.All(value => Math.Abs(value) < GeoSharkMath.Epsilon);
        }

        /// <summary>
        /// Gets the vector amplified by a scalar value.
        /// </summary>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The vector amplified.</returns>
        public Vector Amplify(double amplitude)
        {
            return new Vector(this.Unitize() * amplitude);
        }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        //ToDo Check references. Does not seem relevant if treating like an array of doubles. Length would be length of list??
        public double Length()
        {
            if (!IsValid() || IsZero()) return 0.0;

            return Math.Sqrt(SquaredLength());
        }

        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// </summary>
        /// <returns>The sum of each value squared.</returns>
        //ToDo Check references. Does not seem relevant if treating like an array of doubles.
        public double SquaredLength()
        {
            return this.Aggregate(0.0, (x, a) => a * a + x);
        }

        /// <summary>
        /// Computes the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector with which compute the dot product.</param>
        /// <returns>The dot product.</returns>
        //ToDo Check references. Does not seem relevant if treating like an array of doubles. Length would be length of list??
        public static double Dot(Vector a, Vector b)
        {
            return a.Select((t, i) => t * b[i]).Sum();
        }

        /// <summary>
        /// Unitizes the vector. A unit vector has length 1 unit.
        /// </summary>
        /// <returns>A new vector unitized.</returns>
        //ToDo Check references. Does not seem relevant if treating like an array of doubles. Length would be length of list??
        public Vector Unitize()
        {
            if (IsUnitized()) return this;
            double l = this.Length();
            if (l <= double.Epsilon)
                throw new Exception("An invalid or zero length vector cannot be unitized.");
            return this * (1 / l);
        }

        /// <summary>
        /// Creates a list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <returns>Get a vector of r dimension.</returns>
        public static Vector Zero1d(int rows)
        {
            Vector v = new Vector();
            for (int i = 0; i < rows; i++)
                v.Add(0.0);

            return v;
        }

        /// <summary>
        /// Creates a 2 dimensional list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <param name="cols">Columns dimension.</param>
        /// <returns>Get a vector of r*c dimension.</returns>
        public static List<Vector> Zero2d(int rows, int cols)
        {
            List<Vector> lv = new List<Vector>();
            for (int i = 0; i < rows; i++)
                lv.Add(Zero1d(cols));

            return lv;
        }

        /// <summary>
        /// Creates a 3 dimensional list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <param name="cols">Columns dimension.</param>
        /// <param name="depth">Depth dimension.</param>
        /// <returns>Get a vector of r*c*d dimension.</returns>
        public static List<List<Vector>> Zero3d(int rows, int cols, int depth)
        {
            List<List<Vector>> llv = new List<List<Vector>>();
            for (int i = 0; i < rows; i++)
                llv.Add(Zero2d(cols, depth));

            return llv;
        }

       // Note: this is mutable.
        /// <summary>
        /// Adds to each component of the first vector the respective component of the second vector multiplied by a scalar.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="s">Scalar value.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>A mutated vector.</returns>
        public static void AddMulMutate(Vector a, double s, Vector b)
        {
            for (int i = 0; i < a.Count; i++)
                a[i] = a[i] + s * b[i];
        }

        // Note: this is mutable.
        /// <summary>
        /// Subtracts to each component of the first vector the respective component of the second vector multiplied by a scalar.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="s">Scalar value.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>A mutated vector.</returns>
        public static void SubMulMutate(Vector a, double s, Vector b)
        {
            for (int i = 0; i < a.Count; i++)
                a[i] = a[i] - s * b[i];
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        /// <param name="v">The vector to multiply.</param>
        /// <param name="a">The scalar value to multiply.</param>
        /// <returns>A vector whose magnitude is multiplied by a.</returns>
        public static Vector operator *(Vector v, double a)
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
        public static Vector operator *(Vector v, Matrix m)
        {
            int mRows = m.Count;
            int mCols = m[0].Count;

            if (mCols != v.Count)
                throw new Exception("Non-conformable matrix and vector");

            Vector resultVector = Vector.Zero1d(mRows);

            for (int i = 0; i < mRows; i++)
            {
                double tempValue = 0.0;
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
        public static Vector operator *(Vector v, Transform t)
        {
            double x = 0.0;
            double y = 0.0;
            double z = 0.0;
            double w = 0.0;

            if (v.Count == 4)
            {
                x = t[0][0] * v[0] + t[0][1] * v[1] + t[0][2] * v[2] + t[0][3] * v[3];
                y = t[1][0] * v[0] + t[1][1] * v[1] + t[1][2] * v[2] + t[1][3] * v[3];
                z = t[2][0] * v[0] + t[2][1] * v[1] + t[2][2] * v[2] + t[2][3] * v[3];
                w = t[3][0] * v[0] + t[3][1] * v[1] + t[3][2] * v[2] + t[3][3] * v[3];

                return new Vector { x, y, z, w };
            }

            x = t[0][0] * v[0] + t[0][1] * v[1] + t[0][2] * v[2] + t[0][3];
            y = t[1][0] * v[0] + t[1][1] * v[1] + t[1][2] * v[2] + t[1][3];
            z = t[2][0] * v[0] + t[2][1] * v[1] + t[2][2] * v[2] + t[2][3];
            w = t[3][0] * v[0] + t[3][1] * v[1] + t[3][2] * v[2] + t[3][3];

            if (w > 0.0)
            {
                double w2 = 1.0 / w;
                x *= w2;
                y *= w2;
                z *= w2;
            }

            return new Vector { x, y, z };
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="a">The scalar divisor.</param>
        /// <param name="v">The vector to divide.</param>
        /// <returns>A vector whose magnitude is multiplied by a.</returns>
        public static Vector operator /(Vector v, double a)
        {
            return v.Select(val => val / a).ToVector();
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the difference between a and b.</returns>
        public static Vector operator -(Vector a, Vector b)
        {
            return a.Select((val, i) => val - b[i]).ToVector();
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the sum of a and b.</returns>
        public static Vector operator +(Vector a, Vector b)
        {
            return a.Select((val, i) => val + b[i]).ToVector();
        }

        /// <summary>
        /// Checks if the two vectors are the same.<br/>
        /// Two vectors are the same, if all components of the two vectors are within Epsilon.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>True if are the same, otherwise false.</returns>
        public static bool operator ==(Vector a, Vector b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Checks if the two vectors are the same.<br/>
        /// Two vectors are the same, if all components of the two vectors are within Epsilon.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>True if are the same, otherwise false.</returns>
        public static bool operator !=(Vector a, Vector b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Checks if the two vectors are the same.<br/>
        /// Two vectors are the same, if all components of the two vectors are within Epsilon.
        /// </summary>
        /// <param name="obj">The vector to test.</param>
        /// <returns>True if are the same, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Vector)) return false;

            return true;
        }

        /// <summary>
        /// Checks if the two vectors are the same.<br/>
        /// Two vectors are the same, if all components of the two vectors are within Epsilon.
        /// </summary>
        /// <param name="other">The vector to test.</param>
        /// <returns>True if are the same, otherwise false.</returns>
        public bool Equals(Vector other)
        {
            if (other == null) return false;
            if (!this.IsValid() || !other.IsValid()) return false;
            if (Count != other.Count) return false;

            for (var i = 0; i < Count; i++)
            {
                if (!(Math.Abs(this[i] - other[i]) <= GeoSharkMath.Epsilon)) return false;
            }
            
            return true;
        }

        /// <summary>
        /// Constructs the string representation of the vector.
        /// </summary>
        /// <returns>The vector in string format.</returns>
        public override string ToString()
        {
            return this.Count == 4
                ? $"{Math.Round(this[0], 6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}, {this[3]}"
                : $"{Math.Round(this[0], 6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}";
        }
    }
}