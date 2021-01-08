using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// Like a Point, a Vector in verb is represented simply by an list of double point numbers.
    /// So, you would write simply [1,0,0] to create a Vector in the X direction.
    /// </summary>
    public class Vector : List<double>
    {
        public Vector()
        {
        }

        public Vector(IEnumerable<double> values)
        {
            this.AddRange(values);
        }
        /// <summary>
        /// Gets the value of a point at location Constants.UNSETVALUE,Constants.UNSETVALUE,Constants.UNSETVALUE.
        /// </summary>
        public static Vector Unset => new Vector(){ Constants.UNSETVALUE, Constants.UNSETVALUE, Constants.UNSETVALUE };
        /// <summary>
        /// The angle in radians between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The angle in radians</returns>
        public static double AngleBetween(Vector a, Vector b)
        {
            return Math.Acos(Dot(a, b) / (Length(a) * Length(b)));
        }
        /// <summary>
        /// Reverses this vector in place (reverses the direction).
        /// </summary>
        /// <param name="a">The vector to reverse.</param>
        /// <returns>The reversed vector.</returns>
        public static Vector Reverse(Vector a) => new Vector(a.Select(x => -x));

        /// <summary>
        /// Gets a value indicating whether this vector is valid.
        /// A valid vector must be formed of finite numbers.
        /// </summary>
        /// <param name="a">The vector to be valued.</param>
        /// <returns>True if the vector is valid.</returns>
        public bool IsValid() => this.Any(Constants.IsValidDouble);

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        /// <param name="a">The vector to check.</param>
        /// <returns>True if all the component are less than Epsilon.</returns>
        public bool IsZero() => this.All(value => Math.Abs(value) < Constants.EPSILON);

        // ToDo probably this method should be in another class.
        /// <summary>
        /// Gets the vector amplified by a scalar value along a direction.
        /// </summary>
        /// <param name="origin">The start point or vector.</param>
        /// <param name="dir">The direction.</param>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The vector amplified.</returns>
        public static Vector OnRay(IList<double> origin, Vector dir, double amplitude)
        {
            var vectorAmplified = new Vector(Constants.Multiplication(Vector.Normalized(dir), amplitude));
            return new Vector(Constants.Addition(origin, vectorAmplified));
        }

        /// <summary>
        /// Vector normalized.
        /// </summary>
        /// <param name="a">The vector has to be normalized.</param>
        /// <returns>The vector normalized.</returns>
        public static Vector Normalized(Vector a)
        {
            return new Vector(Constants.Division(a, Length(a)));
        }
        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public static double Length(Vector a)
        {
            if (!a.IsValid() || a.IsZero()) return 0.0;
            return Math.Sqrt(SquaredLength(a));
        }
        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The sum of each value squared.</returns>
        public static double SquaredLength(Vector a)
        {
            return a.Aggregate(0.0, (x, a) => a * a + x);
        }

        /// <summary>
        /// Cross product.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Compute the cross product.</returns>
        public static Vector Cross(Vector u, Vector v) =>
            new Vector() {
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
        public static double Dot(Vector a, Vector b)
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
        public static Vector Zero1d(int rows)
        {
            Vector v = new Vector();
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
        public static List<Vector> Zero2d(int rows, int cols)
        {
            List<Vector> lv = new List<Vector>();
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
        public static List<List<Vector>> Zero3d(int rows, int cols, int depth)
        {
            List<List<Vector>> llv = new List<List<Vector>>();
            for (int i = 0; i < rows; i++)
                llv.Add(Zero2d(cols, depth));
            return llv;
        }
        /// <summary>
        /// Constructs the string representation of the vector.
        /// </summary>
        /// <returns>The vector in string format</returns>
        public override string ToString()
        {
            return $"{Math.Round(this[0],6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}";
        }

        // ToDo implement them if necessary.
        // Get the angle between two vectors in 2d, for a 2 dimension.
        // use the perp dot product other words the two dimensional cross-product.
        // http://johnblackburne.blogspot.com/2012/02/perp-dot-product.html
        // http://www.sunshine2k.de/articles/Notes_PerpDotProduct_R2.pdf
        public static double AngleBetweenNormalized2d(Vector a, Vector b) => throw new NotImplementedException();
        // Return a angle in between a and b, not clear what n vector is used for.
        public static double PositiveAngleBetween(Vector a, Vector b, Vector n) => throw new NotImplementedException();
        // Return a angle in between a and b, not clear what n vector is used for. Angle is degrees.
        public static double SignedAngleBetween(Vector a, Vector b, Vector n) => throw new NotImplementedException();
        public static double Sum(Vector a) => throw new NotImplementedException(); // This sum the value of a vector.
        public static double AddAll(List<Vector> a) => throw new NotImplementedException(); // This sum all the vectors.
        // This sum all the vectors to the first.
        public static double AddAllMutate(List<Vector> a) => throw new NotImplementedException();
        // This multiple all the vectors for a value and add them to the first one
        public static double AddMulMutate(Vector a, Vector b, double s) => throw new NotImplementedException();
        // This subtract all the vectors for a value and add them to the first one
        public static double SubMulMutate(Vector a, Vector b, double s) => throw new NotImplementedException();
        // Like addition but modifying the first vector.
        public static double AddMutate(Vector a, Vector b) => throw new NotImplementedException();
        // Like subtraction but modifying the first vector.
        public static double SubMutate(Vector a, Vector b) => throw new NotImplementedException();
        // Like multiplication but modifying the first vector.
        public static double MulMutate(double a, Vector b) => throw new NotImplementedException();
    }
}
