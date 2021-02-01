using System;
using System.Collections.Generic;
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
        public void It_Creates_An_Identity_Matrix()
        {
           int i = 3;
           Matrix.Identity(i).Should().BeEquivalentTo(IdentityMatrix);
        }

        [Fact]
        public void It_Returns_A_Transformed_Vector()
        {
            var homogenizedVec = new Vector3(){-10.0, 20.0, 5.0, 1.0};

            var vecExpected = new Vector3() { -20.0, 40.0, 6.0, 1.0 };

            var transformedVec = Matrix.Dot(TransformationMatrixExample, homogenizedVec);

            transformedVec.Should().BeEquivalentTo(vecExpected);
        }

        [Fact]
        public void Dot_Throws_An_Exception_If_The_Vector_And_The_Matrix_Have_Not_The_Same_Dimension()
        {
            var vec = new Vector3() { -10.0, 20.0, 5.0 };

            Func<object> funcResult = () => Matrix.Dot(TransformationMatrixExample, vec);

            funcResult.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
