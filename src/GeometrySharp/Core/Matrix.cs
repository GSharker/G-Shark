using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GeometrySharp.Core
{
    /// <summary>
    /// A Matrix is represented by a nested list of double point numbers.
    /// So, you would write simply [[1,0],[0,1]] to create a 2x2 identity matrix.
    /// </summary>
    public class Matrix : List<List<double>>
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
        /// <param name="rows">A positive integer, for the number of rows.</param>
        /// <param name="columns">A positive integer, for the number of columns.</param>
        /// <returns>A matrix reflecting the number of rows and columns provided.</returns>
        public static Matrix Construct(int rows, int columns)
        {
            Matrix tempMatrix = new Matrix();
            if (rows == 0 || columns == 0)
            {
                throw new Exception("Matrix must be at least one row or column");
            }

            for (int i = 0; i < columns; i++)
            {
                List<double> tempRow = Sets.RepeatData(0.0, rows);
                tempMatrix.Add(tempRow);
            }

            return tempMatrix;
        }

        /// <summary>
        /// Creates an identity matrix of a given size.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="diagonalValue">The value which will be filled the diagonal.</param>
        /// <returns>Identity matrix of the given size.</returns>
        public static Matrix Identity(int size, double diagonalValue = 1.0)
        {
            Matrix m = new Matrix();
            List<Vector3> zeros = Vector3.Zero2d(size, size);
            for (int i = 0; i < size; i++)
            {
                zeros[i][i] = 1.0;
                m.Add(zeros[i]);
            }
            return m;
        }

        /// <summary>
        /// Fills the diagonal determined by the row and column position of a matrix.
        /// </summary>
        /// <param name="row">The value row where the diagonal begin.</param>
        /// <param name="column">The column value where the diagonal begin.</param>
        /// <param name="valueToFill">The value to fill on the diagonal.</param>
        /// <returns>A matrix with the identified diagonal filled.</returns>
        public Matrix FillDiagonal(int row, int column, double valueToFill)
        {
            if (this[0].Count < 2 && this.Count < 2)
            {
                throw new Exception("Matrix must be at least 2-d");
            }
            if (row < 0 || row > this.Count - 1)
            {
                throw new Exception($"Check the row value, must be not negative or bigger than {this.Count - 1}");
            }
            if (column < 0 || column > this[0].Count - 1)
            {
                throw new Exception($"Check the column value, must be not negative or bigger than {this[0].Count - 1}");
            }

            Matrix copyMatrix = Duplicate(this);
            int j = column;
            for (int i = row; i < this.Count; i++)
            {
                if(j > this[0].Count-1) break;
                copyMatrix[i][j] = valueToFill;
                j++;
            }

            return copyMatrix;
        }

        /// <summary>
        /// Gets a value indicating whether this matrix is valid.
        /// Matrix is valid when has at least one column and one row and rows have at least 2 elements.
        /// </summary>
        /// <returns>True if it is a valid matrix.</returns>
        public bool IsValid()
        {
            return Count > 0 && this.All(x => x.Count > 1);
        }

        /// <summary>
        /// Multiply a matrix by a constant.
        /// </summary>
        /// <param name="m">Matrix has to be multiply.</param>
        /// <param name="a">Value to operate the multiplication.</param>
        /// <returns>Matrix multiply by a constant.</returns>
        public static Matrix operator *(Matrix m, double a)
        {
            Matrix result = new Matrix();
            foreach (IList<double> row in m)
            {
                result.Add(row.Select(val => val * a).ToList());
            }

            return result;
        }

        /// <summary>
        /// Divides a matrix by a constant.
        /// </summary>
        /// <param name="m">Matrix has to be multiply.</param>
        /// <param name="a">Value to operate the division.</param>
        /// <returns>Matrix divided by a constant.</returns>
        public static Matrix operator /(Matrix m, double a)
        {
            Matrix result = new Matrix();
            foreach (IList<double> row in m)
            {
                result.Add(row.Select(val => val / a).ToList());
            }

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
            {
                throw new Exception("Either first matrix or second matrix are Invalid.");
            }

            int aRows = a.Count;
            int aCols = a[0].Count;

            int bRows = b.Count;
            int bCols = b[0].Count;

            if(aCols != bRows)
            {
                throw new Exception("Non-conformable matrices.");
            }

            Matrix resultMatrix = new Matrix();

            for (int i = 0; i < aRows; ++i)
            {
                List<double> tempRow = Sets.RepeatData(0.0, bCols);
                for (int j = 0; j < bCols; ++j)
                {
                    double value = 0.0;
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
        /// Adds two matrices.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>The result sum matrix.</returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (!a.IsValid() || !b.IsValid())
            {
                throw new Exception("Either first matrix of second matrix are Invalid.");
            }

            int aRows = a.Count;
            int aCols = a[0].Count;

            int bRows = b.Count;
            int bCols = b[0].Count;

            if (aCols != bCols || aRows != bRows)
            {
                throw new Exception("Non-conformable matrices.");
            }

            Matrix result = new Matrix();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add((a[i].Select((val, j) => val + b[i][j]).ToList()));
            }
            return result;
        }

        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>The subtraction matrix.</returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (!a.IsValid() || !b.IsValid())
            {
                throw new Exception("Either first matrix of second matrix are Invalid.");
            }

            int aRows = a.Count;
            int aCols = a[0].Count;

            int bRows = b.Count;
            int bCols = b[0].Count;

            if (aCols != bCols || aRows != bRows)
            {
                throw new Exception("Non-conformable matrices.");
            }

            Matrix result = new Matrix();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add((a[i].Select((val, j) => val - b[i][j]).ToList()));
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
            if (Count == 0)
            {
                return null;
            }

            Matrix transposeMatrix = new Matrix();
            int rows = Count;
            int columns = this[0].Count;
            for (int c = 0; c < columns; c++)
            {
                List<double> rr = new List<double>();
                for (int r = 0; r < rows; r++)
                {
                    rr.Add(this[r][c]);
                }
                transposeMatrix.Add(rr);
            }
            return transposeMatrix;
        }

        /// <summary>
        /// Creates a copy.
        /// </summary>
        /// <param name="matrix">Matrix has to be duplicated.</param>
        /// <returns>Copied matrix.</returns>
        public static Matrix Duplicate(Matrix matrix)
        {

            Matrix copy = Matrix.Construct(matrix[0].Count, matrix.Count);

            for (int i = 0; i < matrix.Count; ++i)
            {
                for (int j = 0; j < matrix[i].Count; ++j)
                {
                    copy[i][j] = matrix[i][j];
                }
            }
            return copy;
        }

        /// <summary>
        /// Checks if the matrix is non-singular (i.e. invertible).
        /// </summary>
        /// <param name="matrix">Matrix to check.</param>
        /// <returns>True if is non-singular.</returns>
        private static bool IsNonSingular(Matrix matrix)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                if (Math.Abs(matrix[i][i]) < GeoSharpMath.EPSILON)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This routine uses Doolittle's method to solve the linear equation Ax = B.
        /// This routine is called after the matrix A has been decomposed.
        /// The solution proceeds by solving the linear equation Ly = B for y and,
        /// subsequently solving the linear equation Ux = y for x.
        /// </summary>
        /// <param name="LuMatrix">Decomposed matrix in lower and upper triangle.</param>
        /// <param name="permutation">The permutation row, or pivot row interchanged with row i.</param>
        /// <param name="b">Column vector.</param>
        /// <returns>The solution of the equation Ax = B is a vector.</returns>
        public static Vector3 Solve(Matrix LuMatrix, int[] permutation, Vector3 b)
        {
            int rows = LuMatrix.Count;
            int cols = LuMatrix[0].Count;

            if (rows != cols)
            {
                throw new Exception("Attempt to decompose a non-squared matrix");
            }

            if (rows != b.Count)
            {
                throw new Exception("The matrix should have the same number of rows as the decomposition b parameter.");
            }

            if (!IsNonSingular(LuMatrix))
            {
                throw new Exception("Matrix is singular.");
            }

            int n = LuMatrix.Count;
            double[] bCopy = new double[n];

            for (int i = 0; i < b.Count; i++)
            {
                bCopy[i] = b[permutation[i]];
            }

            // Solve L*Y = B(piv,:)
            int t = 0;
            while (t < n)
            {
                int j = 0;
                while (j < t)
                {
                    bCopy[t] -= bCopy[j] * LuMatrix[t][j];
                    j++;
                }

                t++;
            }

            // Solve U*X = Y;
            int r = n - 1;
            while (r >= 0)
            {
                int j = r + 1;
                while (j < n)
                {
                    bCopy[r] -= bCopy[j] * LuMatrix[r][j];
                    j++;
                }

                bCopy[r] /= LuMatrix[r][r];
                r--;
            }

            return bCopy.ToVector();
        }

        /// <summary>
        /// This routine uses Doolittle's method with partial pivoting to decompose the n x n matrix A,
        /// into a unit lower triangular matrix L and an upper triangular matrix U and P is a permutation array
        /// such that PA = LU. With this method you can always have a LU decomposition, rather than LU factorization.
        /// The LU decomposition with pivoting always exists, even if the matrix is singular, so the constructor will never fail.
        /// The primary use of the LU decomposition is in the solution of square systems of simultaneous linear equations.
        /// The LUP decomposition provides a more robust method of solving linear systems than LU decomposition without pivoting, and it is approximately the same cost.
        /// This will fail if non singular.
        /// </summary>
        /// <param name="m">Matrix has to be decomposed.</param>
        /// <param name="permutation">The row pivot information is in one-dimensional array.</param>
        /// <returns>The matrix representing the lower matrix and upper matrix together.</returns>
        public static Matrix Decompose(Matrix m, out int[] permutation)
        {
            // http://www.mymathlib.com/c_source/matrices/linearsystems/doolittle_pivot.c
            // https://github.com/sloisel/numeric/blob/master/src/numeric.js
            // https://github.com/accord-net/framework/blob/development/Sources/Accord.Math/Decompositions/LuDecomposition.cs

            int rows = m.Count;
            int cols = m[0].Count;

            if(rows != cols)
            {
                throw new Exception("Attempt to decompose a non-squared matrix");
            }

            int n = rows;
            int[] tempPermutation = new int[n];
            Matrix copyMatrix = Matrix.Duplicate(m);

            for (int i = 0; i < n; ++i) { tempPermutation[i] = i; } // Populate the permutation.

            int k = 0;
            while (k < n)
            {
                int permutationValueK = k;
                double maxColumnValue = Math.Abs(copyMatrix[k][k]); // Find largest value in the column.

                // Find the pivot row.
                int j = k + 1;
                while (j < n)
                {
                    double absValueAt = Math.Abs(copyMatrix[j][k]);
                    if (maxColumnValue < absValueAt)
                    {
                        maxColumnValue = absValueAt;
                        permutationValueK = j;
                    }
                    j++;
                }

                if (maxColumnValue < GeoSharpMath.EPSILON)
                {
                    throw new Exception("Failed, matrix is degenerate.");
                }

                // If the pivot row differs from the current row, then
                // interchange the two rows.
                if (permutationValueK != k)
                {
                    List<double> copyRow = copyMatrix[permutationValueK];
                    copyMatrix[permutationValueK] = copyMatrix[k];
                    copyMatrix[k] = copyRow;

                    int tempPermutationVal = tempPermutation[permutationValueK];
                    tempPermutation[permutationValueK] = tempPermutation[k];
                    tempPermutation[k] = tempPermutationVal;
                }

                // Find the lower triangular matrix elements.
                int i = k + 1;
                while (i < n)
                {
                    copyMatrix[i][k] /= copyMatrix[k][k];
                    j = k + 1;
                    while (j < n - 1)
                    {
                        copyMatrix[i][j] -= copyMatrix[i][k] * copyMatrix[k][j];
                        j++;
                        copyMatrix[i][j] -= copyMatrix[i][k] * copyMatrix[k][j];
                        j++;
                    }

                    if (j == n - 1)
                    {
                        copyMatrix[i][j] -= copyMatrix[i][k] * copyMatrix[k][j];
                    }

                    i++;
                }

                k++;
            }

            permutation = tempPermutation;
            return copyMatrix;
        }

        /// <summary>
        /// Computes the inverse of a Matrix.   
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverted matrix.</returns>
        public static Matrix Inverse(Matrix matrix)
        {
            Matrix matrixLu = Decompose(matrix, out int[] permutation);

            if(!IsNonSingular(matrixLu))
            {
                throw new Exception("Matrix is singular");
            }

            int rows = matrixLu.Count;

            Matrix matrixResult = Matrix.Construct(rows, rows);

            for (int i = 0; i < rows; i++)
            {
                int k = permutation[i];
                matrixResult[i][k] = 1;
            }

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < rows; k++)
            {
                for (int i = k + 1; i < rows; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        matrixResult[i][j] -= matrixResult[k][j] * matrixLu[i][k];
                    }
                }
            }

            // Solve U*X = I;
            for (int k = rows - 1; k >= 0; k--)
            {
                for (int j = 0; j < rows; j++)
                {
                    matrixResult[k][j] /= matrixLu[k][k];
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        matrixResult[i][j] -= matrixResult[k][j] * matrixLu[i][k];
                    }
                }
            }

            return matrixResult;
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
