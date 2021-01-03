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
            return Math.Round(Math.Acos(Dot(a, b) / (Length(a) * Length(b))),6);
        }

        public static double AngleBetweenNormalized2d(Vector a, Vector b) => throw new NotImplementedException();

        public static double PositiveAngleBetween(Vector a, Vector b, Vector n) => throw new NotImplementedException();

        public static double SignedAngleBetween(Vector a, Vector b, Vector n) => throw new NotImplementedException();

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

        public static Vector OnRay(Point origin, Vector dir, double u) => throw new NotImplementedException();
        public static Vector Lerp(double i, Vector u, Vector v) => throw new NotImplementedException();

        /// <summary>
        /// Vector normalized.
        /// </summary>
        /// <param name="a">The vector has to be normalized.</param>
        /// <returns>The vector normalized.</returns>
        public static Vector Normalized(Vector a)
        {
            return Division(a, Length(a));
        }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public static double Length(Vector a)
        {
            if (!a.IsValid()) return 0.0;
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

        public static double Dist(Vector a, Vector b) => throw new NotImplementedException();
        public static double DistSquared(Vector a, Vector b) => throw new NotImplementedException();

        public static double Sum(Vector a) => throw new NotImplementedException(); //not really clear
        public static double AddAll(List<Vector> a) => throw new NotImplementedException(); //not really clear
        public static double AddAllMutate(List<Vector> a) => throw new NotImplementedException(); //not really clear
        public static double AddMulMutate(Vector a, Vector b, double s) => throw new NotImplementedException();
        public static double SubMulMutate(Vector a, Vector b, double s) => throw new NotImplementedException();
        public static double AddMutate(Vector a, Vector b) => throw new NotImplementedException();
        public static double SubMutate(Vector a, Vector b) => throw new NotImplementedException();
        public static double MulMutate(double a, Vector b) => throw new NotImplementedException();

        /// <summary>
        /// Create a list of zero values
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static Vector Zero1d(int rows)
        {
            Vector v = new Vector();
            for (int i = 0; i < rows; i++)
                v.Add(0.0);
            return v;

        }

        /// <summary>
        /// Create a 2 dimensional list of zero values
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static List<Vector> Zero2d(int rows, int cols)
        {
            List<Vector> lv = new List<Vector>();
            for (int i = 0; i < rows; i++)
                lv.Add(Zero1d(cols));
            return lv;
        }

        /// <summary>
        /// Create a 3 dimensional list of zero values
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static List<List<Vector>> Zero3d(int rows, int cols, int depth)
        {
            List<List<Vector>> llv = new List<List<Vector>>();
            for (int i = 0; i < rows; i++)
                llv.Add(Zero2d(cols, depth));
            return llv;
        }

        /// <summary>
        /// Add two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the sum of a and b.</returns>
        public static Vector Addition(IList<double> a, IList<double> b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] + b[i]);
            return vec;
        }
        /// <summary>
        /// Multiply a scalar and a vector.
        /// </summary>
        /// <param name="a">The vector to divide.</param>
        /// <param name="b">The scalar value to multiply.</param>
        /// <returns>A vector whose magnitude is multiplied by b.</returns>
        public static Vector Multiplication(IList<double> a, double b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] * b);
            return vec;
        }
        /// <summary>
        /// Divide a vector by a scalar.
        /// </summary>
        /// <param name="a">The vector to divide.</param>
        /// <param name="b">The scalar divisor.</param>
        /// <returns>A vector whose magnitude is multiplied by b.</returns>
        public static Vector Division(IList<double> a, double b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] / b);
            return vec;
        }
        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the difference between a and b.</returns>
        public static Vector Subtraction(IList<double> a, IList<double> b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i]-b[i]);
            return vec;
        }
        public static bool IsZero(Vector a) => throw new NotImplementedException();
        public static Vector SortedSetUnion(Vector a, Vector b) => throw new NotImplementedException();
        public static Vector SortedSetSub(Vector a, Vector b) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static List<T> Rep<T>(int num, T ele)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < num; i++)
                list.Add(ele);
            return list;
        }

        /// <summary>
        /// Constructs the string representation of the vector.
        /// </summary>
        /// <returns>The vector in string format</returns>
        public override string ToString()
        {
            return $"{Math.Round(this[0],6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}";
        }
    }
}
