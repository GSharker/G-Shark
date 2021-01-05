using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A Matrix is represented by a nested list of double point numbers.
    /// So, you would write simply [[1,0],[0,1]] to create a 2x2 identity matrix.
    /// </summary>
    public class Matrix : List<IList<double>>
    {
        public Matrix()
        {

        }

        /// <summary>
        /// Multiply a `Matrix` by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Muliplication(double a, Matrix b)
        {
            Matrix r = new Matrix();
            foreach (var l in b)
                r.Add(Constants.Multiplication((Vector)l, a));
            return r;
        }

        /// <summary>
        /// Multiply two matrices assuming they are of compatible dimensions.
        /// Based on the numeric.js routine - `numeric.dotMMsmall`
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Multiplication(Matrix x, Matrix y)
        {
            var p = x.Count();
            var q = y.Count();
            var r = y[0].Count();

            var ret = new Matrix();
            var i = p - 1;
            var j = 0;
            var k = 0;

            while (i >= 0)
            {
                var foo = new Vector();
                var bar = x[i];

                k = r - 1;
                while (k >= 0)
                {
                    var woo = bar[q - 1] * y[q - 1][k];

                    j = q - 2;
                    while (j >= 1)
                    {
                        var i0 = j - 1;
                        woo += bar[j] * y[j][k] + bar[i0] * y[i0][k];
                        j -= 2;
                    }
                    if (j == 0) { woo += bar[0] * y[0][k]; }
                    foo[k] = woo;
                    k--;
                }
                ret[i] = foo;
                i--;
            }
            return ret;

        }

        /// <summary>
        /// Add two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Addition(Matrix a, Matrix b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add(Constants.Addition((Vector)a[i], (Vector)b[i]));
            return r;
        }

        /// <summary>
        /// Divide each of entry of a Matrix by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Division(Matrix a, double b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add(Constants.Division((Vector)a[i], b));
            return r;
        }

        /// <summary>
        /// Subtract two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Subtraction(Matrix a, Matrix b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add(Constants.Subtraction((Vector)a[i], (Vector)b[i]));
            return r;
        }


        /// <summary>
        /// Multiply a `Matrix` by a `Vector`
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector Dot(Matrix a, Vector b)
        {
            Vector r = new Vector();
            for (int i = 0; i < a.Count; i++)
                r.Add(Vector.Dot((Vector)a[i], b));
            return r;
        }

        /// <summary>
        /// Build an identity matrix of a given size
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Matrix Identity(int n)
        {
            Matrix m = new Matrix();
            var zeros = Vector.Zero2d(n, n);
            for (int i = 0; i < n; i++)
            {
                zeros[i][i] = 1.0;
                m.Add(zeros[i].ToList());
            }
            return m;
        }

        /// <summary>
        /// Transpose a matrix
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix Transpose(Matrix a)
        {
            if (a.Count == 0) return null;
            Matrix t = new Matrix();
            var rows = a.Count;
            var columns = a[0].Count;
            for (var c = 0; c < columns; c++)
            {
                var rr = new List<double>();
                for (var r = 0; r < rows; r++)
                {
                    rr.Add(a[r][c]);
                }
                t.Add(rr);
            }
            return t;
        }
    }
}

//        /// <summary>
//        /// Solve a system of equations
//        /// </summary>
//        /// <param name="a"></param>
//        /// <param name="b"></param>
//        /// <returns></returns>
//        public static Vector Solve(Matrix a, Vector b) => LUSolve(LU(a), b);

//        /// <summary>
//        /// Based on methods from numeric.js
//        /// </summary>
//        /// <param name="LUP"></param>
//        /// <param name="b"></param>
//        /// <returns></returns>
//        public static Vector LUSolve(LUDecomp LUP, Vector b)
//        {
//            var LU = LUP.LU;
//            var n = LU.Count;
//            var x = b;
//            var P = LUP.P;

//            var i = n - 1;
//            while (i != -1)
//            {
//                x[i] = b[i];
//                --i;
//            }

//            i = 0;
//            while (i < n)
//            {
//                var Pi = P[i];
//                if (P[i] != i)
//                {
//                    var tmp = x[i];
//                    x[i] = x[Pi];
//                    x[Pi] = tmp;
//                }

//                var LUi = LU[i];
//                var j = 0;
//                while (j < i)
//                {
//                    x[i] -= x[j] * LUi[j];
//                    ++j;
//                }
//                ++i;
//            }

//            i = n - 1;
//            while (i >= 0)
//            {
//                var LUi = LU[i];
//                var j = i + 1;
//                while (j < n)
//                {
//                    x[i] -= x[j] * LUi[j];
//                    ++j;
//                }

//                x[i] /= LUi[i];
//                --i;
//            }

//            return x;
//        }

//        /// <summary>
//        /// Based on methods from numeric.js
//        /// </summary>
//        /// <param name="a"></param>
//        /// <returns></returns>
//        public static LUDecomp LU(Matrix a) {
//            var n = a.Count;
//            var n1 = n - 1;
//            var p = new List<int>();

//            var k = 0;
//            while (k < n)
//            {
//                var Pk = k;
//                var Ak = a[k];
//                var max = Math.Abs(Ak[k]);

//                var j = k + 1;
//                while (j < n)
//                {
//                    var absAjk = Math.Abs(a[j][k]);
//                    if (max < absAjk)
//                    {
//                        max = absAjk;
//                        Pk = j;
//                    }
//                    ++j;
//                }
//                p[k] = Pk;

//                if (Pk != k)
//                {
//                    a[k] = a[Pk];
//                    a[Pk] = Ak;
//                    Ak = a[k];
//                }

//                var Akk = Ak[k];

//                var i = k + 1;
//                while (i < n)
//                {
//                    a[i][k] /= Akk;
//                    ++i;
//                }

//                i = k + 1;
//                while (i < n)
//                {
//                    var Ai = a[i];
//                    j = k + 1;
//                    while (j < n1)
//                    {
//                        Ai[j] -= Ai[k] * Ak[j];
//                        ++j;
//                        Ai[j] -= Ai[k] * Ak[j];
//                        ++j;
//                    }
//                    if (j == n1) Ai[j] -= Ai[k] * Ak[j];
//                    ++i;
//                }

//                ++k;
//            }
//            return new LUDecomp(a, p);
//        }
//    }

//    public class LUDecomp
//    {
//        public Matrix LU { get; set; }
//        public List<int> P { get; set; }
//        public LUDecomp(Matrix lU, List<int> p)
//        {
//            LU = lU;
//            P = p;
//        }
//    }
