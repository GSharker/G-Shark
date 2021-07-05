using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using Xunit;
using Xunit.Abstractions;
using Plane = GShark.Geometry.Plane;

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
                new object[] { Vector.Unset, false},
            };

        public static IEnumerable<object[]> VectorLengths =>
            new List<object[]>
            {
                new object[] { new Vector { -18d, -21d, -17d }, 32.46536616149585},
                new object[] { Vector.Unset, 0.0},
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

        public static TheoryData<Vector> NotValidVectorUnitized =>
            new TheoryData<Vector>
            {
                Vector.Unset,
                new Vector{ 0, 0, 0 },
            };

        [Fact]
        public void It_Returns_The_Radian_Angle_Between_Two_Vectors()
        {
            // Arrange
            Vector v1 = new Vector { 20d, 0d, 0d };
            Vector v2 = new Vector { -10d, 15d, 0d };

            // Act
            double angle = Vector.AngleBetween(v1, v2);

            // Assert
            angle.Should().Be(2.1587989303424644);
        }

        [Fact]
        public void It_Returns_The_Linear_Interpolation_Between_Two_Vectors()
        {
            // Arrange
            Vector v1 = new Vector { 0d, 0d, 0d };
            Vector v2 = new Vector { 10d, 10d, 10d };

            // Act
            double amount = 0.5;

            // Assert
            Vector.Lerp(v1, v2, amount).Should().BeEquivalentTo(new Vector { 5d, 5d, 5d });
        }

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
        public void It_Returns_The_Cross_Product_Between_Two_Vectors()
        {
            // Arrange
            Vector v1 = new Vector { -10d, 5d, 10d };
            Vector v2 = new Vector { 10d, 15d, 5d };
            Vector crossProductExpected = new Vector { -125d, 150d, -200d };

            // Act
            Vector crossProduct = Vector.Cross(v1, v2);

            // Assert
            crossProduct.Should().BeEquivalentTo(crossProductExpected);
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
            var vec2D = Vector.Zero2d(3,3);

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
            var expectedVec = new Vector{ -1.428571, 2.142857, 0.714286 };

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
        public void DistanceTo_Throws_An_Exception_If_The_Two_Vector_Have_Different_Length()
        {
            // Arrange
            var vec1 = new Vector { -10, 15, 5 };
            var vec2 = new Vector { 10, 15 };

            // Act
            Func<object> funcResult = () => vec1.DistanceTo(vec2);

            // Assert
            funcResult.Should().Throw<Exception>().WithMessage("The two list doesn't match in length.");
        }

        [Fact]
        public void It_Returns_The_DistanceTo_TwoVectors()
        {
            // Arrange
            var vec1 = new Vector { -20, 15, 5 };
            var vec2 = new Vector { 10, 0, 15 };

            // Act
            var distance = vec1.DistanceTo(vec2);

            // Assert
            distance.Should().Be(35);
        }

        [Fact]
        public void It_Checks_If_Vectors_Are_Perpendicular()
        {
            // Arrange
            var vec = new Vector { -7, 10, -5 };
            var other1 = new Vector { 10, 7, 0 };
            var other2 = Vector.YAxis;

            // Assert
            vec.IsPerpendicularTo(other1).Should().BeTrue();
            vec.IsPerpendicularTo(other2).Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Perpendicular_Vector()
        {
            // Arrange
            var vectorGuide = new Vector { -7, 10, -5 };
            var vector = new Vector { 0, 1, 0 };
            var vectorExpected = new Vector { 10, 7, 0 };

            // Act
            var perVector = vector.PerpendicularTo(vectorGuide);

            // Assert
            perVector.Equals(vectorExpected).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Perpendicular_Vector_Given_Three_Points()
        {
            // Arrange
            var vector = new Vector { -7, 10, -5 };
            var pt1 = new Vector { 3,-1, 2 };
            var pt2 = new Vector { 1, -1, -3 };
            var pt3 = new Vector { 4, -3, 1 };
            var vectorExpected = new Vector { -10, -7, 4 };

            // Act
            var perVector = vector.PerpendicularTo(pt1, pt2, pt3);

            // Assert
            perVector.Equals(vectorExpected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(NotValidVectorUnitized))]
        public void Unitize_Throws_An_Error_If_Invalid_Vector_Or_Zero_Length(Vector vector)
        {
            // Act
            Func<Vector> func = vector.Unitize;

            // Assert
            func.Should().Throw<Exception>().WithMessage("An invalid or zero length vector cannot be unitized.");
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

        [Theory]
        [InlineData(-0.0000125, new [] {-7.0, 10.0, -5.0})]
        [InlineData(0.0, new [] { -7.0, 10.0, -5.0 })]
        [InlineData(12.5, new [] { -7.454672, 10.649531, -2.239498 })]
        [InlineData(450, new [] { -2.867312, 4.09616, 12.206556 })]
        public void It_Returns_A_Rotated_Vector_By_An_Angle(double angle, double[] vectorExpected)
        {
            // Arrange
            var vector = new Vector { -7, 10, -5 };
            var axis = new Vector { 10, 7, 0 };
            var radiance = GeoSharkMath.ToRadians(angle);

            // Act
            var vectorRot = vector.Rotate(axis, radiance);

            // Assert
            vectorRot.Should().BeEquivalentTo(vectorExpected, options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }

        [Theory]
        [InlineData(new[] {11.5, 0.0, 0.0}, new[] {10.3, 0.0, 0.0}, 1)]
        [InlineData(new[] {-7.0, 10.0, -5.0}, new[] {7.0, 15.0, 0.0}, 0)]
        [InlineData(new[] {7.0, 0.0, 0.0}, new[] {-7.0, 0.0, 0.0}, -1)]
        public void It_Checks_If_Two_Vectors_Are_Parallel(double[] v1, double[] v2, int result)
        {
            // Arrange
            var vec1 = new Vector(v1);
            var vec2 = new Vector(v2);

            // Assert
            vec1.IsParallelTo(vec2).Should().Be(result);
        }

        [Fact]
        public void It_Returns_A_Point_And_A_HomogenizedPoint_Transformed()
        {
            // Arrange
            var pt1 = new Point3(5,5,0);
            var pt2 = new Point4(5,5,0,0.2);
            var pt1Expected = new Point3(15, 15, 0);
            var pt2Expected = new Point4(7, 7, 0, 0.2);
            var transform = Transform.Translation(new Vector3d(10, 10, 0));

            // Act
            var pt1Translated = pt1.Transform(transform);
            var pt2Translated = pt2.Transform(transform);

            // Assert
            pt1Translated.Equals(pt1Expected);
            pt2Translated.Equals(pt2Expected);
        }
    }
}
