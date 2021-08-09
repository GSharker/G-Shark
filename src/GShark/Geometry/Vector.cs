using GShark.Core;
using GShark.ExtendedMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// Vector represents list of doubles.
    ///  </summary>
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
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        /// <param name="a">The vector to check.</param>
        /// <returns>True if all the component are less than Epsilon.</returns>
        public bool IsZero()
        {
            return this.All(value => Math.Abs(value) < GeoSharkMath.Epsilon);
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
        /// <returns>The sum of each value squared.</returns>
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
        public static double Dot(Vector a, Vector b)
        {
            return a.Select((t, i) => t * b[i]).Sum();
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
            return obj is Vector _;
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
            return string.Join(",", this.Select(e => GeoSharkMath.Truncate(e)));
        }
    }
}