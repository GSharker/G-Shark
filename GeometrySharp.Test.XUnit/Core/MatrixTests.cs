using System;
using System.Collections.Generic;
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
        public static readonly Matrix IdentityMatrix = new Matrix()
        {
            new List<double> { 1, 0, 0 },
            new List<double> { 0, 1, 0 },
            new List<double> { 0, 0, 1 }
        };

        public static readonly Matrix TransformationMatrixExample = new Matrix()
        {
            new List<double>{1.0, 0.0, 0.0, -10.0 },
            new List<double>{0.0, 1.0, 0.0, 20.0 },
            new List<double>{0.0, 0.0, 1.0, 1.0 },
            new List<double>{0.0, 0.0, 0.0, 1.0 }
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
    }
}
