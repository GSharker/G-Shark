using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Core
{
    public class MatrixTests
    {
        public static Matrix IdentityMatrix = new Matrix()
        {
            new List<double> { 1, 0, 0 },
            new List<double> { 0, 1, 0 },
            new List<double> { 0, 0, 1 }
        };

        public static Matrix TransformationMatrixExample = new Matrix()
        {
            new List<double>{1.0, 0.0, 0.0, -10.0 },
            new List<double>{0.0, 1.0, 0.0, 20.0 },
            new List<double>{0.0, 0.0, 1.0, 1.0 },
            new List<double>{0.0, 0.0, 0.0, 1.0 }
        };

        public static TheoryData<Matrix, bool> InvalidMatrices => new TheoryData<Matrix, bool>()
        {
            {new Matrix {new List<double> {1}, new List<double> {4, 5, 6}}, false},
            {new Matrix {new List<double> {1}, new List<double> {4}}, false},
            {new Matrix {new List<double> {1, 2}, new List<double> {4, 5}}, true},
            {new Matrix {new List<double> {1, 2}, new List<double> {}}, false}
        };

        public static TheoryData<Matrix, Matrix, int[]> DecomposedMatrixLuData => new TheoryData<Matrix, Matrix, int[]>
        {
            {new Matrix {new List<double> {2,1}, new List<double> {-4,-6}}, 
                new Matrix {new List<double> {-4,-6}, new List<double> {-0.5,-2}},
                new []{1,0}
            },
            {new Matrix {new List<double> {2,1,-4}, new List<double> {2,2,-2}, new List<double> {6,3,11}}, 
                new Matrix {new List<double> {6,3,11}, new List<double> { 0.33333333333333, 1, -5.6666666666667 }, new List<double> { 0.33333333333333, 0, -7.6666666666667 }},
                new []{2,1,0}
            },
            {new Matrix {new List<double> {1,3,2}, new List<double> {2,8,5}, new List<double> {1,11,4}},
                new Matrix {new List<double> {2,8,5}, new List<double> {0.5,7,1.5}, new List<double> {0.5, -0.14285714285714, -0.28571428571429 }},
                new []{1,2,0}
            }
        };

        public static TheoryData<Matrix, Vector3, Vector3> SolveMatrixEquation => new TheoryData<Matrix, Vector3, Vector3>
        {
            {new Matrix {new List<double> {1,2,4}, new List<double> {3,8,14}, new List<double> {2,6,13}}, new Vector3{3,13,4}, new Vector3{3,4,-2}},
            {new Matrix {new List<double> {3,1,6}, new List<double> {-6,0,-16}, new List<double> {0,8,-17}}, new Vector3{0,4,17}, new Vector3{2,0,-1}}
        };

        private readonly ITestOutputHelper _testOutput;
        public MatrixTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Initializes_A_Matrix()
        {
            var matrix = new Matrix();

            matrix.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidMatrices))]
        public void It_Checks_If_Matrix_IsValid(Matrix m, bool result)
        {
            m.IsValid().Should().Be(result);
        }

        [Fact]
        public void It_Creates_A_Matrix_By_Given_Rows_And_Columns()
        {
            var matrix = Matrix.Construct(3, 3);

            matrix.Count.Should().Be(3);
            matrix[0].Count.Should().Be(3);
            matrix.Select(x => x.Sum()).Sum().Should().Be(0.0);
        }
        
        [Fact]
        public void It_Creates_An_Identity_Matrix()
        { 
            var matrix = Matrix.Identity(3);
            
            matrix.Should().BeEquivalentTo(IdentityMatrix);
        }

        [Fact]
        public void It_Returns_A_Transpose_Matrix()
        {
            var matrix = new Matrix {new List<double> {1, 2}, new List<double> {3, 4}, new List<double> {5, 6}};
            var expectedMatrix = new Matrix {new List<double> {1, 3, 5}, new List<double> {2, 4, 6}};

            var transposedMatrix = matrix.Transpose();

            transposedMatrix.Should().BeEquivalentTo(expectedMatrix);
        }

        [Fact]
        public void It_Returns_A_Matrix_Multiply_By_A_Constant()
        {
            var matrix = new Matrix { new List<double> { 4, 0 }, new List<double> { 1, -9 } };
            var matrixExpected = new Matrix { new List<double> { 8, 0 }, new List<double> { 2, -18 } };

            var resultMatrix = matrix * 2;

            resultMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void Matrix_Product_Throws_An_Exception_If_Two_Matrices_Are_Not_Compatible()
        {
            var matrixA = new Matrix { new List<double> { 1, 2, 3 }, new List<double> { 4, 5, 6 } };
            var matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 }};

            Func<Matrix> func = () => matrixA * matrixB;

            func.Should().Throw<Exception>().WithMessage("Non-conformable matrices.");
        }

        [Fact]
        public void It_Returns_The_Product_Between_Two_Matrices()
        {
            var matrixA = new Matrix {new List<double> {1, 2, 3}, new List<double> {4, 5, 6}};
            var matrixB = new Matrix {new List<double> { 7, 8 }, new List<double> { 9, 10 }, new List<double> { 11, 12 }};
            var matrixExpected = new Matrix { new List<double> { 58,64 }, new List<double> { 139,154 } };

            var productMatrix = matrixA * matrixB;

            productMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void Matrix_Addition_Throws_An_Exception_If_Two_Matrices_Are_Not_Compatible()
        {
            var matrixA = new Matrix { new List<double> { 1, 2, 3 }, new List<double> { 4, 5, 6 } };
            var matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 } };

            Func<Matrix> func = () => matrixA + matrixB;

            func.Should().Throw<Exception>().WithMessage("Non-conformable matrices.");
        }

        [Fact]
        public void It_Returns_The_Sum_Between_Two_Matrices()
        {
            var matrixA = new Matrix { new List<double> { 1, 2 }, new List<double> { 4, 5 } };
            var matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 }};
            var matrixExpected = new Matrix { new List<double> { 8, 10 }, new List<double> { 13, 15 } };

            var productMatrix = matrixA + matrixB;

            productMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void It_Returns_A_Copy_Of_The_Given_Matrix()
        {
            var matrix = new Matrix { new List<double> { 1, 2, 3 }, new List<double> { 4, 5, 6 } };

            var copiedMatrix = Matrix.Duplicate(matrix);

            copiedMatrix.Should().BeEquivalentTo(matrix);
        }

        [Theory]
        [MemberData(nameof(DecomposedMatrixLuData))]
        public void Compute_The_LU_Factorization_Of_A_Matrix(Matrix matrix, Matrix LuMatrixExpected, int[] permutationExpected)
        {
            // Results compared with https://keisan.casio.com/exec/system/15076953047019

            var matrixLu = Matrix.Decompose(matrix, out int[] permutation);

            _testOutput.WriteLine($"matrixLu -> {matrixLu}");
            foreach (var i in permutation)
                _testOutput.WriteLine($"permutation -> {i}");

            permutation.Should().BeEquivalentTo(permutationExpected);
            matrixLu.Should().BeEquivalentTo(LuMatrixExpected, options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }

        [Theory]
        [MemberData(nameof(SolveMatrixEquation))]
        public void Compute_The_Equation_Between_A_LuMatrix_And_A_Vector(Matrix matrix, Vector3 vector, Vector3 result)
        {
            var matrixLu = Matrix.Decompose(matrix, out int[] permutation);
            var solution = Matrix.Solve(matrixLu, permutation, vector);

            solution.Equals(result).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_An_Exception_If_The_Matrix_Is_Singular()
        {
            var matrix = new Matrix { new List<double> { 2,4,6 }, new List<double> { 2,0,2 }, new List<double> { 6,8,14 } };
            var vector = new Vector3 {3, 13, 4};
            var pivot = new[] {1, 1, 1};

            Func<object> func = () => Matrix.Solve(matrix, pivot, vector);

            func.Should().Throw<Exception>().WithMessage("Matrix is singular.");
        }

        [Fact]
        public void It_Returns_An_Exception_If_The_Decomposition_Value_Dimension_Is_Different_Of_Matrix_Row_Dimension()
        {
            var matrix = new Matrix {new List<double> {1, 2, 4}, new List<double> {3, 8, 14}, new List<double> {2, 6, 13}};
            var vector = new Vector3 { 3, 13};
            var pivot = new[] { 1, 1, 1 };

            Func<object> func = () => Matrix.Solve(matrix, pivot, vector);

            func.Should().Throw<Exception>().WithMessage("The matrix should have the same number of rows as the decomposition b parameter.");
        }

        [Fact]
        public void It_Returns_The_Inverse_Of_A_Matrix()
        {
            var matrix = new Matrix { new List<double> { 1, 2, 4 }, new List<double> { 3, 8, 14 }, new List<double> { 2, 6, 13 } };
            var matrixExpected = new Matrix
            {
                new List<double> { 3.333333333333334, -0.33333333333333304, -0.6666666666666671 }, 
                new List<double> { -1.8333333333333337, 0.8333333333333331, -0.333333333333333 }, 
                new List<double> { 0.3333333333333334, -0.33333333333333326, 0.3333333333333332 }
            };

            var invertedMatrix = Matrix.Inverse(matrix);

            _testOutput.WriteLine(invertedMatrix.ToString());
            invertedMatrix.Should().BeEquivalentTo(matrixExpected);
        }
    }
}