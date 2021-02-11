using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Geometry;

namespace GeometrySharp.Core
{
    /// <summary>
    /// A Matrix is represented by a nested list of double point numbers.
    /// So, you would write simply [[1,0],[0,1]] to create a 2x2 identity matrix.
    /// </summary>
    public class Matrix : List<IList<double>>
    {
        /// <summary>
        /// Initialize an empty matrix.
        /// </summary>
        public Matrix()
        {
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
        /// Gets a value indicating whether this matrix is valid.
        /// Matrix is valid when has at least one column and one row and rows have at least 2 elements.
        /// </summary>
        /// <returns>True if it is a valid matrix.</returns>
        public bool IsValid()
        {
            return this.Count > 0 && this.All(x => x.Count > 1);
        }

        /// <summary>
        /// Multiply a matrix by a constant.
        /// </summary>
        /// <param name="m">Matrix has to be multiply.</param>
        /// <param name="a">Value to operate the multiplication.</param>
        /// <returns>Matrix multiply by a constant.</returns>
        public static Matrix operator *(Matrix m, double a)
        {
            var result = new Matrix();
            foreach (var row in m)
                result.Add(row.Select(val => val * a).ToList());
            return result;
        }

        /// <summary>
        /// Multiply two matrices assuming they are of compatible dimensions.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>The product matrix.</returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (!a.IsValid() || !b.IsValid()) // this pass is not check, because we check for IsValid in another test.
                throw new Exception("Either first matrix or second matrix are Invalid.");

            var aRows = a.Count;
            var aCols = a[0].Count;

            var bRows = b.Count;
            var bCols = b[0].Count;

            if(aCols != bRows)
                throw new Exception("Non-conformable matrices.");

            var resultMatrix = new Matrix();

            for (int i = 0; i < aRows; ++i)
            {
                var tempRow = Sets.RepeatData(0.0, bCols);
                for (int j = 0; j < bCols; ++j)
                {
                    var value = 0.0;
                    for (int k = 0; k < aCols; ++k)
                    {
                        value += a[i][k] * b[k][j];
                    }
                    tempRow[j] = value;
                }
                resultMatrix.Add(tempRow);
            }

            return resultMatrix;
        }

        /// <summary>
        /// Add two matrices.
        /// </summary>
        /// <param name="a">First Matrix.</param>
        /// <param name="b">Second Matrix.</param>
        /// <returns>The sum matrix.</returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (!a.IsValid() || !b.IsValid())
                throw new Exception("Either first matrix of second matrix are Invalid.");

            var aRows = a.Count;
            var aCols = a[0].Count;

            var bRows = b.Count;
            var bCols = b[0].Count;

            if (aCols != bCols || aRows != bRows)
                throw new Exception("Non-conformable matrices.");

            var result = new Matrix();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add((a[i].Select((val, j) => val + b[i][j]).ToList()));
            }
            return result;
        }

        /// <summary>
        /// Transpose a matrix.
        /// This is like swapping rows with columns.
        /// </summary>
        /// <returns>The matrix transposed.</returns>
        public Matrix Transpose()
        {
            if (this.Count == 0) return null;
            Matrix transposeMatrix = new Matrix();
            var rows = this.Count;
            var columns = this[0].Count;
            for (var c = 0; c < columns; c++)
            {
                var rr = new List<double>();
                for (var r = 0; r < rows; r++)
                {
                    rr.Add(this[r][c]);
                }
                transposeMatrix.Add(rr);
            }
            return transposeMatrix;
        }

        public Matrix Inverse()
        {
            return new Matrix();
        }

        /// <summary>
        /// Constructs the string representation the matrix.
        /// </summary>
        /// <returns>Text string.</returns>
        public override string ToString()
        {
            return string.Join("\n", this.Select(first => $"({string.Join(",", first)})"));
        }
    }
}
