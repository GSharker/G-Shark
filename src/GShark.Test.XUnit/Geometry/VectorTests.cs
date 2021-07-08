using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class VectorTests
    {
        private readonly ITestOutputHelper _testOutput;

        public VectorTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> ValidateVectors =>
            new List<object[]>
            {
                new object[] { new Vector { 20d, -10d, 0d }, true},
            };

        public static IEnumerable<object[]> VectorLengths =>
            new List<object[]>
            {
                new object[] { new Vector { -18d, -21d, -17d }, 32.46536616149585},
                new object[] { new Vector { -0d, 0d, 0d }, 0.0}
            };

        public static TheoryData<Vector, double, Vector> AmplifiedVectors =>
            new TheoryData<Vector, double, Vector>
            {
                { new Vector{ 5, 5, 0 }, 0, new Vector{ 0, 0, 0 }},
                { new Vector{ 10, 10, 0 }, 15, new Vector{ 10.606602,10.606602,0 }},
                { new Vector{ 20, 15, 0 }, 33, new Vector{ 26.4,19.8,0 }},
                { new Vector{ 35, 15, 0 }, 46, new Vector{ 42.280671,18.120288,0 }}
            };

        [Fact]
        public void It_Returns_A_Reversed_Vector()
        {
            // Arrange
            Vector v1 = new Vector { 20d, 0d, 0d };
            Vector vectorExpected = new Vector { -20d, 0d, 0d };

            // Act
            Vector reversedVector = Vector.Reverse(v1);

            // Assert
            reversedVector.Should().BeEquivalentTo(vectorExpected);
        }

        [Theory]
        [MemberData(nameof(ValidateVectors))]
        public void It_Checks_If_Vectors_Are_Valid_Or_Not(Vector v, bool expected)
        {
            // Assert
            v.IsValid().Should().Be(expected);
        }

        [Fact]
        public void It_Returns_The_Dot_Product_Between_Two_Vectors()
        {
            // Arrange
            Vector v1 = new Vector { -10d, 5d, 10d };
            Vector v2 = new Vector { 10d, 15d, 5d };

            // Act
            double dotProduct = Vector.Dot(v1, v2);

            // Assert
            dotProduct.Should().Be(25);
        }

        [Fact]
        public void It_Returns_The_Squared_Length_Of_A_Vector()
        {
            // Arrange
            Vector v1 = new Vector { 10d, 15d, 5d };

            // Act
            double squaredLength = v1.SquaredLength();

            // Assert
            squaredLength.Should().Be(350);
        }

        [Theory]
        [MemberData(nameof(VectorLengths))]
        public void It_Returns_The_Length_Of_A_Vector(Vector v, double expectedLength)
        {
            // Act
            double length = v.Length();

            // Assert
            length.Should().Be(expectedLength);
        }

        [Fact]
        public void It_Returns_Normalized_Vector()
        {
            // Arrange
            Vector v1 = new Vector { -18d, -21d, -17d };
            Vector normalizedExpected = new Vector() { -0.5544369932703277, -0.6468431588153823, -0.5236349380886428 };

            // Act
            Vector normalizedVector = v1.Unitize();

            // Assert
            normalizedVector.Should().Equal(normalizedExpected);
        }

        [Fact]
        public void It_Returns_A_Zero1d_Vector()
        {
            // Act
            Vector vec1D = Vector.Zero1d(4);

            // Assert
            vec1D.Should().HaveCount(4);
            vec1D.Select(val => val.Should().Be(0.0));
        }

        [Fact]
        public void It_Returns_A_Zero2d_Vector()
        {
            // Act
            var vec2D = Vector.Zero2d(3, 3);

            // Assert
            vec2D.Should().HaveCount(3);
            vec2D.Select(val => val.Should().HaveCount(3));
            vec2D.Select(val => val.Should().Contain(0.0));
        }

        [Fact]
        public void It_Returns_A_Zero3d_Vector()
        {
            // Act
            var vec3D = Vector.Zero3d(3, 3, 4);

            // Assert
            vec3D.Should().HaveCount(3);
            vec3D.Select(val => val.Should().HaveCount(4));
            vec3D.Select(val => val.Select(x => x.Should().Contain(0.0)));
        }

        [Theory]
        [MemberData(nameof(AmplifiedVectors))]
        public void It_Returns_An_Amplified_Vector(Vector vector, double amplitude, Vector expected)
        {
            // Act
            var amplifiedVector = vector.Amplify(amplitude);

            // Assert
            // https://stackoverflow.com/questions/36782975/fluent-assertions-approximately-compare-a-classes-properties
            amplifiedVector.Should().BeEquivalentTo(expected, options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }

        [Fact]
        public void It_Returns_The_Addiction_Between_Two_Vectors()
        {
            // Arrange
            var vec1 = new Vector { 20, 0, 0 };
            var vec2 = new Vector { -10, 15, 5 };
            var expectedVec = new Vector { 10, 15, 5 };

            // Assert
            (vec1 + vec2).Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Returns_The_Subtraction_Between_Two_Vectors()
        {
            // Arrange
            #region example
            var vec1 = new Vector { 20, 0, 0 };
            var vec2 = new Vector { -10, 15, 5 };
            #endregion
            var expectedVec = new Vector { 30, -15, -5 };

            // Assert
            (vec1 - vec2).Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Returns_The_Multiplication_Between_Two_Vectors()
        {
            // Arrange
            var vec = new Vector { -10, 15, 5 };
            var expectedVec = new Vector { -70, 105, 35 };

            // Assert
            (vec * 7).Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void Multiply_Between_Vector_And_Matrix_Throws_An_Exception_If_Vector_And_Matrix_Are_Not_Compatible()
        {
            // Arrange
            var vec = new Vector { -10, 15, 5 };
            var matrix = new Matrix { new List<double> { 7, 8 }, new List<double> { 9, 10 } };

            // Act
            Func<Vector> func = () => vec * matrix;

            // Assert
            func.Should().Throw<Exception>().WithMessage("Non-conformable matrix and vector");
        }

        [Fact]
        public void It_Returns_A_Vector_Transform_By_A_Matrix()
        {
            // Arrange
            var vec = new Vector { 1, 3 };
            var matrix = new Matrix { new List<double> { 1, 0 }, new List<double> { 0, -1 } };
            var expectedVec = new Vector { 1, -3 };

            // Act
            var transformedVector = vec * matrix;

            // Assert
            transformedVector.Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Returns_The_Division_Between_Two_Vectors()
        {
            // Arrange
            var vec = new Vector { -10, 15, 5 };
            var expectedVec = new Vector { -1.428571, 2.142857, 0.714286 };

            // Act
            var divisionResult = vec / 7;

            // Assert
            divisionResult.Select((val, i) => System.Math.Round(val, 6).Should().Be(expectedVec[i]));
        }

        [Fact]
        public void It_Returns_True_If_Vectors_Are_Equal()
        {
            // Arrange
            var vec1 = new Vector { 5.982099, 5.950299, 0 };
            var vec2 = new Vector { 5.982099, 5.950299, 0 };

            // Assert
            (vec1 == vec2).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Unitized_Vector()
        {
            // Arrange
            var vector = new Vector { -7, 10, -5 };
            var vectorExpected = new Vector { -0.530669, 0.758098, -0.379049 };

            // Act
            var unitizedVector = vector.Unitize();

            // Assert
            unitizedVector.Should().BeEquivalentTo(vectorExpected, options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }
    }
}
