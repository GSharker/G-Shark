using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Mat
    {
        /// <summary>
        /// Multiply a `Matrix` by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Mul(double a, Matrix b)
        {
            Matrix r = new Matrix();
            foreach (var l in b)
                r.Add(Vec.Mul(a, (Vector)l));
            return r;
        }

        /// <summary>
        /// Multiply two matrices assuming they are of compatible dimensions.
        /// Based on the numeric.js routine - `numeric.dotMMsmall`
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Mutl(Matrix x, Matrix y)
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
        public static Matrix Add(Matrix a, Matrix b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add(Vec.Add((Vector)a[i], (Vector)b[i]));
            return r;
        }

        /// <summary>
        /// Divide each of entry of a Matrix by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Div(Matrix a, double b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add(Vec.Div((Vector)a[i], b));
            return r;
        }

        /// <summary>
        /// Subtract two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Sub(Matrix a, Matrix b)
        {
            Matrix r = new Matrix();
            for (int i = 0; i < a.Count; i++)
                r.Add(Vec.Sub((Vector)a[i], (Vector)b[i]));
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
                r.Add(Vec.Dot((Vector)a[i], b));
            return r;
        }


        public static Vector Identity(int i) => throw new NotImplementedException();
        public static Matrix Transpose(Matrix a) => throw new NotImplementedException();
        public static Vector Solve(Matrix a, Vector b) => throw new NotImplementedException();
        public static Vector LUSolve(LUDecomp LUP, Vector b) => throw new NotImplementedException();
        public static LUDecomp LU(Matrix a) => throw new NotImplementedException();
    }

    public class LUDecomp
    {
        public Matrix LU { get; set; }
        public List<int> P { get; set; }
        public LUDecomp(Matrix lU, List<int> p)
        {
            LU = lU;
            P = p;
        }
    }
}
