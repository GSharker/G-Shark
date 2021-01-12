using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.ExtendedMethods;
using Math = VerbNurbsSharp.Core.Math;

namespace VerbNurbsSharp.Geometry
{
    /// <summary>
    /// Like a Vector3, a Vector3 in verb is represented simply by an list of double point numbers.
    /// So, you would write simply [1,0,0] to create a Vector3 in the X direction.
    /// </summary>
    public class Vector3 : List<double>
    {
        public Vector3()
        {
        }

        public Vector3(IEnumerable<double> values)
        {
            this.AddRange(values);
        }
        /// <summary>
        /// Gets the value of a point at location Math.UNSETVALUE,Math.UNSETVALUE,Math.UNSETVALUE.
        /// </summary>
        public static Vector3 Unset => new Vector3(){ Math.UNSETVALUE, Math.UNSETVALUE, Math.UNSETVALUE };
        /// <summary>
        /// The angle in radians between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The angle in radians</returns>
        public static double AngleBetween(Vector3 a, Vector3 b)
        {
            return System.Math.Acos(Dot(a, b) / (Length(a) * Length(b)));
        }
        /// <summary>
        /// Reverses this vector in place (reverses the direction).
        /// </summary>
        /// <param name="a">The vector to reverse.</param>
        /// <returns>The reversed vector.</returns>
        public static Vector3 Reverse(Vector3 a) => new Vector3(a.Select(x => -x));

        /// <summary>
        /// Gets a value indicating whether this vector is valid.
        /// A valid vector must be formed of finite numbers.
        /// </summary>
        /// <param name="a">The vector to be valued.</param>
        /// <returns>True if the vector is valid.</returns>
        public bool IsValid() => this.Any(Math.IsValidDouble);

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        /// <param name="a">The vector to check.</param>
        /// <returns>True if all the component are less than Epsilon.</returns>
        public bool IsZero() => this.All(value => System.Math.Abs(value) < Math.EPSILON);

        // ToDo probably this method should be in another class.
        /// <summary>
        /// Gets the vector amplified by a scalar value along a direction.
        /// </summary>
        /// <param name="origin">The start point or vector.</param>
        /// <param name="dir">The direction.</param>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The vector amplified.</returns>
        public static Vector3 OnRay(Vector3 origin, Vector3 dir, double amplitude)
        {
            var vectorAmplified = Vector3.Normalized(dir) * amplitude;
            return origin + vectorAmplified;
        }

        /// <summary>
        /// Vector3 normalized.
        /// </summary>
        /// <param name="a">The vector has to be normalized.</param>
        /// <returns>The vector normalized.</returns>
        public static Vector3 Normalized(Vector3 v)
        {
            return v / Length(v);
        }
        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public static double Length(Vector3 a)
        {
            if (!a.IsValid() || a.IsZero()) return 0.0;
            return System.Math.Sqrt(SquaredLength(a));
        }
        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The sum of each value squared.</returns>
        public static double SquaredLength(Vector3 a)
        {
            return a.Aggregate(0.0, (x, a) => a * a + x);
        }
        /// <summary>
        /// Cross product.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Compute the cross product.</returns>
        public static Vector3 Cross(Vector3 u, Vector3 v) =>
            new Vector3() {
                u[1] * v[2] - u[2] * v[1],
                u[2] * v[0] - u[0] * v[2],
                u[0] * v[1] - u[1] * v[0]
            };
        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector with which compute the dot product.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector3 a, Vector3 b)
        {
            double sum = 0d;
            for (int i = 0; i < a.Count; i++)
                sum += a[i] * b[i];
            return sum;
        }
        /// <summary>
        /// Create a list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <returns>Get a vector of r dimension.</returns>
        public static Vector3 Zero1d(int rows)
        {
            Vector3 v = new Vector3();
            for (int i = 0; i < rows; i++)
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
            List<Vector3> lv = new List<Vector3>();
            for (int i = 0; i < rows; i++)
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
            List<List<Vector3>> llv = new List<List<Vector3>>();
            for (int i = 0; i < rows; i++)
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
            if (this.Count != v.Count) throw new Exception("The two list doesn't match in length.");
            return System.Math.Sqrt(this.Select((val, i) => System.Math.Pow(val - v[i], 2)).Sum());
            //return Math.Sqrt(a.Zip(b, (first, second) => Math.Pow(first - second, 2)).Sum());
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
        /// Determine whether this vector's components are equal to those of v, within tolerance.
        /// </summary>
        /// <param name="v">The vector to compare.</param>
        /// <returns>True if the difference of this vector and the supplied vector's components are all within Tolerance, otherwise false.</returns>
        public bool IsAlmostEqualTo(Vector3 v)
        {
            return this.Select((val, i) => System.Math.Round(val - v[i]))
                .All(val => val < Math.EPSILON);
        }
        /// <summary>
        /// Constructs the string representation of the vector.
        /// </summary>
        /// <returns>The vector in string format</returns>
        public override string ToString()
        {
            return $"{System.Math.Round(this[0],6)},{System.Math.Round(this[1], 6)},{System.Math.Round(this[2], 6)}";
        }



        // ToDo implement them if necessary.
        // Get the angle between two vectors in 2d, for a 2 dimension.
        // use the perp dot product other words the two dimensional cross-product.
        // http://johnblackburne.blogspot.com/2012/02/perp-dot-product.html
        // http://www.sunshine2k.de/articles/Notes_PerpDotProduct_R2.pdf
        public static double AngleBetweenNormalized2d(Vector3 a, Vector3 b) => throw new NotImplementedException();
        // Return a angle in between a and b, not clear what n vector is used for.
        public static double PositiveAngleBetween(Vector3 a, Vector3 b, Vector3 n) => throw new NotImplementedException();
        // Return a angle in between a and b, not clear what n vector is used for. Angle is degrees.
        public static double SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n) => throw new NotImplementedException();
        public static double Sum(Vector3 a) => throw new NotImplementedException(); // This sum the value of a vector.
        public static double AddAll(List<Vector3> a) => throw new NotImplementedException(); // This sum all the vectors.
        // This sum all the vectors to the first.
        public static double AddAllMutate(List<Vector3> a) => throw new NotImplementedException();
        // This multiple all the vectors for a value and add them to the first one
        public static double AddMulMutate(Vector3 a, Vector3 b, double s) => throw new NotImplementedException();
        // This subtract all the vectors for a value and add them to the first one
        public static double SubMulMutate(Vector3 a, Vector3 b, double s) => throw new NotImplementedException();
        // Like addition but modifying the first vector.
        public static double AddMutate(Vector3 a, Vector3 b) => throw new NotImplementedException();
        // Like subtraction but modifying the first vector.
        public static double SubMutate(Vector3 a, Vector3 b) => throw new NotImplementedException();
        // Like multiplication but modifying the first vector.
        public static double MulMutate(double a, Vector3 b) => throw new NotImplementedException();
    }
}
