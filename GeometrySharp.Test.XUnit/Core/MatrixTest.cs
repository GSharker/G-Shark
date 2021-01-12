using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Core
{
    [Trait("Category", "Matrix")]
    public class MatrixTest
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
        public MatrixTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        
        [Fact]
        public void Create_Identity_Matrix()
        {
           int i = 3;
           Matrix.Identity(i).Should().BeEquivalentTo(IdentityMatrix);
        }

        [Fact]
        public void GetTheTransformedVector()
        {
            var homogenizedVec = new Vector3(){-10.0, 20.0, 5.0, 1.0};

            var vecExpected = new Vector3() { -20.0, 40.0, 6.0, 1.0 };

            var transformedVec = Matrix.Dot(TransformationMatrixExample, homogenizedVec);

            transformedVec.Should().BeEquivalentTo(vecExpected);
        }

        [Fact]
        public void MatrixDot_ThrowsAnException_IfTheVectorAndTheMatrix_HaveNotTheSameDimension()
        {
            var vec = new Vector3() { -10.0, 20.0, 5.0 };

            Func<object> funcResult = () => Matrix.Dot(TransformationMatrixExample, vec);

            funcResult.Should().Throw<ArgumentOutOfRangeException>();
        }

    }
}
