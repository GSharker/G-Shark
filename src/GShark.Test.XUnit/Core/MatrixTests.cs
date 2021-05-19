using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Core
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
            {new Matrix {new List<double> {1, 2}, new List<double>()}, false}
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
            // Act
            Matrix matrix = new Matrix();

            // Assert
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
            // Act
            Matrix matrix = Matrix.Construct(3, 3);

            // Assert
            matrix.Count.Should().Be(3);
            matrix[0].Count.Should().Be(3);
            matrix.Select(x => x.Sum()).Sum().Should().Be(0.0);
        }
        
        [Fact]
        public void It_Creates_An_Identity_Matrix()
        {
            // Act
            Matrix matrix = Matrix.Identity(3);
            
            // Assert
            matrix.Should().BeEquivalentTo(IdentityMatrix);
        }

        [Fact]
        public void It_Returns_A_Transpose_Matrix()
        {
            // Arrange
            Matrix matrix = new Matrix {new List<double> {1, 2}, new List<double> {3, 4}, new List<double> {5, 6}};
            Matrix expectedMatrix = new Matrix {new List<double> {1, 3, 5}, new List<double> {2, 4, 6}};

            // Act
            Matrix transposedMatrix = matrix.Transpose();

            // Assert
            transposedMatrix.Should().BeEquivalentTo(expectedMatrix);
        }

        [Fact]
        public void It_Returns_A_Matrix_Divided_By_A_Constant()
        {
            // Arrange
            Matrix matrix = new Matrix { new List<double> { 8, 0 }, new List<double> { 2, -18 } };
            Matrix matrixExpected = new Matrix { new List<double> { 4, 0 }, new List<double> { 1, -9 } };

            // Act
            Matrix resultMatrix = matrix / 2;

            // Assert
            resultMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void It_Returns_A_Matrix_Multiply_By_A_Constant()
        {
            // Arrange
            Matrix matrix = new Matrix { new List<double> { 4, 0 }, new List<double> { 1, -9 } };
            Matrix matrixExpected = new Matrix { new List<double> { 8, 0 }, new List<double> { 2, -18 } };

            // Act
            Matrix resultMatrix = matrix * 2;

            // Assert
            resultMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void Matrix_Product_Throws_An_Exception_If_Two_Matrices_Are_Not_Compatible()
        {
            // Arrange
            Matrix matrixA = new Matrix { new List<double> { 1, 2, 3 }, new List<double> { 4, 5, 6 } };
            Matrix matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 }};

            // Act
            Func<Matrix> func = () => matrixA * matrixB;

            // Assert
            func.Should().Throw<Exception>().WithMessage("Non-conformable matrices.");
        }

        [Fact]
        public void It_Returns_The_Product_Between_Two_Matrices()
        {
            // Arrange
            Matrix matrixA = new Matrix {new List<double> {1, 2, 3}, new List<double> {4, 5, 6}};
            Matrix matrixB = new Matrix {new List<double> { 7, 8 }, new List<double> { 9, 10 }, new List<double> { 11, 12 }};
            Matrix matrixExpected = new Matrix { new List<double> { 58,64 }, new List<double> { 139,154 } };

            // Act
            Matrix productMatrix = matrixA * matrixB;

            // Assert
            productMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void Matrix_Addition_Throws_An_Exception_If_Two_Matrices_Are_Not_Compatible()
        {
            // Arrange
            Matrix matrixA = new Matrix { new List<double> { 1, 2, 3 }, new List<double> { 4, 5, 6 } };
            Matrix matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 } };

            // Act
            Func<Matrix> func = () => matrixA + matrixB;

            // Assert
            func.Should().Throw<Exception>().WithMessage("Non-conformable matrices.");
        }

        [Fact]
        public void It_Returns_The_Sum_Between_Two_Matrices()
        {
            // Arrange
            Matrix matrixA = new Matrix { new List<double> { 1, 2 }, new List<double> { 4, 5 } };
            Matrix matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 }};
            Matrix matrixExpected = new Matrix { new List<double> { 8, 10 }, new List<double> { 13, 15 } };

            // Act
            Matrix productMatrix = matrixA + matrixB;

            // Assert
            productMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void It_Returns_The_Subtraction_Between_Two_Matrices()
        {
            // Arrange
            Matrix matrixA = new Matrix { new List<double> { 1, 2 }, new List<double> { 4, 5 } };
            Matrix matrixB = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 } };
            Matrix matrixExpected = new Matrix { new List<double> { -6, -6 }, new List<double> { -5, -5 } };

            // Act
            Matrix productMatrix = matrixA - matrixB;

            // Assert
            productMatrix.Should().BeEquivalentTo(matrixExpected);
        }

        [Fact]
        public void It_Returns_A_Copy_Of_The_Given_Matrix()
        {
            // Arrange
            Matrix matrix = new Matrix { new List<double> { 1, 2, 3 }, new List<double> { 4, 5, 6 } };

            // Act
            Matrix copiedMatrix = Matrix.Duplicate(matrix);

            // Assert
            copiedMatrix.Should().BeEquivalentTo(matrix);
        }

        [Theory]
        [MemberData(nameof(DecomposedMatrixLuData))]
        public void Compute_The_LU_Factorization_Of_A_Matrix(Matrix matrix, Matrix LuMatrixExpected, int[] permutationExpected)
        {
            // Results compared with https://keisan.casio.com/exec/system/15076953047019
            // Act
            Matrix matrixLu = Matrix.Decompose(matrix, out int[] permutation);

            // Assert
            permutation.Should().BeEquivalentTo(permutationExpected);
            matrixLu.Should().BeEquivalentTo(LuMatrixExpected, options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }

        [Theory]
        [MemberData(nameof(SolveMatrixEquation))]
        public void Compute_The_Equation_Between_A_LuMatrix_And_A_Vector(Matrix matrix, Vector3 vector, Vector3 result)
        {
            // Act
            Matrix matrixLu = Matrix.Decompose(matrix, out int[] permutation);
            Vector3 solution = Matrix.Solve(matrixLu, permutation, vector);

            // Assert
            solution.Equals(result).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_An_Exception_If_The_Matrix_Is_Singular()
        {
            // Arrange
            Matrix matrix = new Matrix { new List<double> { 2,4,6 }, new List<double> { 2,0,2 }, new List<double> { 6,8,14 } };
            Vector3 vector = new Vector3 {3, 13, 4};
            int[] pivot = new[] {1, 1, 1};

            // Act
            Func<object> func = () => Matrix.Solve(matrix, pivot, vector);

            // Assert
            func.Should().Throw<Exception>().WithMessage("Matrix is singular.");
        }

        [Fact]
        public void It_Returns_An_Exception_If_The_Decomposition_Value_Dimension_Is_Different_Of_Matrix_Row_Dimension()
        {
            // Arrange
            Matrix matrix = new Matrix {new List<double> {1, 2, 4}, new List<double> {3, 8, 14}, new List<double> {2, 6, 13}};
            Vector3 vector = new Vector3 { 3, 13};
            int[] pivot = new[] { 1, 1, 1 };

            // Act
            Func<object> func = () => Matrix.Solve(matrix, pivot, vector);

            // Assert
            func.Should().Throw<Exception>().WithMessage("The matrix should have the same number of rows as the decomposition b parameter.");
        }

        [Fact]
        public void It_Returns_The_Inverse_Of_A_Matrix()
        {
            // Arrange
            Matrix matrix = new Matrix { new List<double> { 1, 2, 4 }, new List<double> { 3, 8, 14 }, new List<double> { 2, 6, 13 } };
            Matrix matrixExpected = new Matrix
            {
                new List<double> { 3.333333333333334, -0.33333333333333304, -0.6666666666666671 }, 
                new List<double> { -1.8333333333333337, 0.8333333333333331, -0.333333333333333 }, 
                new List<double> { 0.3333333333333334, -0.33333333333333326, 0.3333333333333332 }
            };

            // Act
            Matrix invertedMatrix = Matrix.Inverse(matrix);

            // Assert
            invertedMatrix.Should().BeEquivalentTo(matrixExpected);
        }
    }
}