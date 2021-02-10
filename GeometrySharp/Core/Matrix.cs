using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Geometry;

// ToDo this class has to be tested.
// ToDo this class has to be commented on all of the parts.
// ToDo remove code that is not necessary.
namespace GeometrySharp.Core
{
    /// <summary>
    /// A Matrix is represented by a nested list of double point numbers.
    /// So, you would write simply [[1,0],[0,1]] to create a 2x2 identity matrix.
    /// </summary>
    public class Matrix : List<IList<double>>
    {
        private readonly List<IList<double>> matrixData;

        /// <summary>
        /// Initialize an empty matrix.
        /// </summary>
        public Matrix()
        {
            matrixData = new List<IList<double>>();
        }

        /// <summary>
        /// Constructs a matrix by given number of rows and columns.
        /// All the parameters are set to zero.
        /// </summary>
        /// <param name="row">A positive integer, for the number of rows.</param>
        /// <param name="column">A positive integer, for the number of columns.</param>
        public static Matrix Construct(int row, int column)
        {
            var tempMatrix = new Matrix();
            if (row == 0 || column == 0)
                throw new Exception("Matrix must be at least one row or column");
            for (int i = 0; i < column; i++)
            {
                var tempRow = Sets.RepeatData(0.0, row);
                tempMatrix.Add(tempRow);
            }

            return tempMatrix;
        }

        /// <summary>
        /// Creates an identity matrix of a given size.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>Identity matrix of the given size.</returns>
        public static Matrix Identity(int size)
        {
            var m = new Matrix();
            var zeros = Vector3.Zero2d(size, size);
            for (int i = 0; i < size; i++)
            {
                zeros[i][i] = 1.0;
                m.Add(zeros[i]);
            }
            return m;
        }

        /// <summary>
        /// Multiply a `Matrix` by a constant.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Muliplication(double a, Matrix b)
        {
            Matrix r = new Matrix();
            foreach (var l in b)
                r.Add((Vector3)l * a);
            return r;
        }

        /// <summary>
        /// Multiply two matrices assuming they are of compatible dimensions.
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
                var foo = new Vector3();
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
                r.Add((Vector3)a[i]+(Vector3)b[i]);
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
                r.Add((Vector3)a[i]/ b);
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
                r.Add((Vector3)a[i] - (Vector3)b[i]);
            return r;
        }
        /// <summary>
        /// Multiply a `Matrix` by a `Vector3`
        /// </summary>
        /// <param name="a">The transformation matrix.</param>
        /// <param name="b">The vector to transform.</param>
        /// <returns>The transformed vector.</returns>
        public static Vector3 Dot(Matrix a, Vector3 b)
        {
            if(b.Count != a[0].Count)
                throw new ArgumentOutOfRangeException(nameof(b), "Vector3 and Matrix must have the same dimension.");
            Vector3 r = new Vector3();
            for (int i = 0; i < a.Count; i++)
                r.Add(Vector3.Dot(new Vector3(a[i]), b));
            return r;
        }

        /// <summary>
        /// Transpose a matrix.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix Transpose(Matrix a)
        {
            if (a.Count == 0) return null;
            Matrix transposeMatrix = new Matrix();
            var rows = a.Count;
            var columns = a[0].Count;
            for (var c = 0; c < columns; c++)
            {
                var rr = new List<double>();
                for (var r = 0; r < rows; r++)
                {
                    rr.Add(a[r][c]);
                }
                transposeMatrix.Add(rr);
            }
            return transposeMatrix;
        }

        public override string ToString()
        {
            return string.Join("\n", this.Select(first => $"({string.Join(",", first)})"));
        }
    }
}
