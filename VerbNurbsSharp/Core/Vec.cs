using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// Like a Point, a Vector in verb is represented simply by an list of double point numbers.
    /// So, you would write simply [1,0,0] to create a Vector in the X direction.
    /// </summary>
    public class Vector : List<double>
    {
        /// <summary>
        /// Multiply a n dimension vector by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector Mul(double a, Vector b)
        {
            var nV = new Vector();
            foreach (var c in b)
                nV.Add(c * a);
            return nV;
        }

        public static double AngleBetween(Vector a, Vector b) => throw new NotImplementedException();

        public static double AngleBetweenNormalized2d(Vector a, Vector b) => throw new NotImplementedException();

        public static double PositiveAngleBetween(Vector a, Vector b, Vector n) => throw new NotImplementedException();

        public static double SignedAngleBetween(Vector a, Vector b, Vector n) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Domain(Vector a) => a.Last() - a.First();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector Range(int max)
        {
            var l = new Vector();
            double f = 0.0;
            for (int i = 0; i < max; i++)
            {
                l.Add(f);
                f += 1.0;
            }
            return l;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Vector Span(double min, double max, double step)
        {
            if (step < Constants.EPSILON) return new Vector();
            if (min > max && step > 0.0) return new Vector();
            if (max > min && step < 0.0) return new Vector();

            var l = new Vector();
            var cur = min;
            while (cur <= max)
            {
                l.Add(cur);
                cur += step;
            }
            return l;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector Neg(Vector a)
        {
            var negA = new Vector();
            a.ForEach(x => negA.Add(-x));
            return negA;
        }


        public static double Min(Vector a) => throw new NotImplementedException();
        public static double Max(Vector a) => throw new NotImplementedException();
        public static bool All(List<bool> a) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static List<bool> Finite(Vector a)
        {
            var fin = new List<bool>();
            a.ForEach(x => fin.Add(double.IsFinite(x)));
            return fin;
        }


        public static Vector OnRay(Point origin, Vector dir, double u) => throw new NotImplementedException();
        public static Vector Lerp(double i, Vector u, Vector v) => throw new NotImplementedException();
        public static Vector Normalized(Vector a) => throw new NotImplementedException();

        /// <summary>
        /// Cross product
        /// </summary>
        /// <param name="a">Vector</param>
        /// <param name="b">Vector</param>
        /// <returns></returns>
        public static Vector Cross(Vector u, Vector v) =>
            new Vector() {
                u[1] * v[2] - u[2] * v[1],
                u[2] * v[0] - u[0] * v[2],
                u[0] * v[1] - u[1] * v[0]
            };


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
        public static double Norm(Vector a) => throw new NotImplementedException();
        public static double NormSquared(Vector a) => throw new NotImplementedException();

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
        /// Add two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the sum of a and b.</returns>
        public static Vector operator +(Vector a, Vector b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] + b[i]);
            return vec;
        }
        /// <summary>
        /// Divide a vector by a scalar.
        /// </summary>
        /// <param name="a">The vector to divide.</param>
        /// <param name="b">The scalar divisor.</param>
        /// <returns>A vector whose magnitude is multiplied by b.</returns>
        public static Vector operator /(Vector a, double b)
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
        public static Vector operator -(Vector a, Vector b)
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
    }
}
