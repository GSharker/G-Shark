using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Vector3d = GShark.Geometry.Vector3d;

namespace GShark.Test.XUnit.Geometry
{
    public class Vector3dTests
    {
        private readonly ITestOutputHelper _testOutput;

        public Vector3dTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Radian_Angle_Between_Two_Vectors()
        {
            // Arrange
            Vector3d v1 = new Vector3d(20d, 0d, 0d);
            Vector3d v2 = new Vector3d(-10d, 15d, 0d);

            // Act
            double angle = Vector3d.VectorAngle(v1, v2);

            // Assert
            angle.Should().Be(2.1587989303424644);
        }

        [Fact]
        public void It_Returns_The_Radian_Angle_Between_Two_Vectors_On_A_Plane()
        {
            throw new NotImplementedException();
            //// Arrange
            //Vector3d v1 = new Vector3d(20d, 0d, 0d);
            //Vector3d v2 = new Vector3d(-10d, 15d, 0d);

            //// Act
            //double angle = Vector3d.VectorAngle(v1, v2);

            //// Assert
            //angle.Should().Be(2.1587989303424644);
        }

        [Fact]
        public void It_Returns_A_Reversed_Vector()
        {
            // Arrange
            Vector3d v1 = new Vector3d(20d, 0d, 0d);
            Vector3d vectorExpected = new Vector3d(-20d, 0d, 0d);

            // Act
            v1.Reverse();

            // Assert
            v1.Should().BeEquivalentTo(vectorExpected);
        }

        [Fact]
        public void It_Checks_If_Vectors_Are_Valid_Or_Not()
        {
            //Arrange
            Vector3d v1 = new Vector3d(20d, -10d, 0d);
            Vector3d v2 = Vector3d.Unset;
            //Act
            bool result1 = v1.IsValid;
            bool result2 = v2.IsValid;

            // Assert
            result1.Should().Be(true);
            result2.Should().Be(false);
        }

        [Fact]
        public void It_Returns_The_Cross_Product_Between_Two_Vectors()
        {
            // Arrange
            Vector3d v1 = new Vector3d(-10d, 5d, 10d);
            Vector3d v2 = new Vector3d(10d, 15d, 5d);
            Vector3d crossProductExpected = new Vector3d(-125d, 150d, -200d);

            // Act
            Vector3d crossProduct = Vector3d.CrossProduct(v1, v2);

            // Assert
            crossProduct.Should().BeEquivalentTo(crossProductExpected);
        }

        [Fact]
        public void It_Returns_The_Dot_Product_Of_Two_Vectors()
        {
            // Arrange
            Vector3d v1 = new Vector3d(-10d, 5d, 10d);
            Vector3d v2 = new Vector3d(10d, 15d, 5d);

            // Act
            double dotProduct = Vector3d.DotProduct(v1, v2);

            // Assert
            dotProduct.Should().Be(25);
        }

        [Fact]
        public void It_Returns_The_Squared_Length_Of_A_Vector()
        {
            
            // Arrange
            Vector3d v1 = new Vector3d(10d, 15d, 5d);

            // Act
            double squareLength = v1.SquareLength;

            // Assert
            squareLength.Should().Be(350);
        }

        [Fact]
        public void It_Returns_The_Length_Of_A_Vector()
        {
            //Arrange
            Vector3d vector1 = new Vector3d(-18d, -21d, -17d);
            double expectedLength1 = 32.46536616149586;
            Vector3d vector2 = Vector3d.Unset;
            double expectedLength2 = 0.0;
            Vector3d vector3 = new Vector3d(0, 0, 0);
            double expectedLength3 = 0.0;

            // Act
            double result1 = vector1.Length;
            double result2 = vector2.Length;
            double result3 = vector3.Length;

            // Assert
            result1.Should().Be(expectedLength1);
            result2.Should().Be(expectedLength2);
            result3.Should().Be(expectedLength3);
        }

        [Fact]
        public void It_Returns_A_Zero_Vector()
        {
            //Arrange
            Vector3d expected = new Vector3d(0, 0, 0);

            // Act
            Vector3d result = Vector3d.Zero;

            // Assert
            result.Equals(expected).Should().Be(true);
        }


        [Fact]
        public void It_Returns_An_Amplified_Vector()
        {
            //Arrange
            int amplitude = 10;
            Vector3d vec1 = new Vector3d(3, 4, 0);
            Vector3d vec1AmplifiedByAmplitude = new Vector3d(6, 8, 0);

            // Act
            Vector3d amplifiedVector = vec1.Amplify(amplitude);
            
            // Assert
            amplifiedVector.EpsilonEquals(vec1AmplifiedByAmplitude, GeoSharpMath.MaxTolerance).Should().Be(true);
        }

        [Fact]
        public void It_Returns_The_Sum_Of_Two_Vectors()
        {
            // Arrange
            Vector3d vec1 = new Vector3d(20, 0, 0);
            Vector3d vec2 = new Vector3d(-10, 15, 5);
            Vector3d expectedVec = new Vector3d(10, 15, 5);

            // Assert
            (vec1 + vec2).Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Returns_The_Subtraction_Of_Two_Vectors()
        {
            // Arrange
            #region example
            Vector3d vec1 = new Vector3d(20, 0, 0);
            Vector3d vec2 = new Vector3d(-10, 15, 5);
            #endregion
            Vector3d expectedVec = new Vector3d(30, -15, -5);

            //Act
            Vector3d result = vec1 - vec2;

            // Assert
            result.Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Multiplies_A_Vector_By_A_Number()
        {
            // Arrange
            Vector3d vector = new Vector3d(-10, 15, 5);
            Vector3d expectedVec = new Vector3d(-70, 105, 35);

            //Act
            Vector3d result = vector * 7;

            // Assert
            result.Equals(expectedVec).Should().Be(true);
        }

        [Fact]
        public void It_Divides_A_Vector_By_A_Number()
        {
            // Arrange
            Vector3d vec = new Vector3d( -10, 15, 5);
            Vector3d expectedVec = new Vector3d(-5, 7.5, 2.5);

            // Act
            Vector3d divisionResult = vec / 2;

            // Assert
            divisionResult.Equals(expectedVec).Should().Be(true);
        }

        [Fact]
        public void It_Returns_True_If_Vectors_Are_Equal()
        {
            // Arrange
            Vector3d vec1 = new Vector3d (5.982099, 5.950299, 0);
            Vector3d vec2 = new Vector3d (5.982099, 5.950299, 0);

            // Assert
            (vec1 == vec2).Should().BeTrue();
        }

        [Fact]
        public void It_Checks_If_Vectors_Are_Perpendicular()
        {
            // Arrange
            Vector3d vec = new Vector3d(-7, 10, -5);
            Vector3d other1 = new Vector3d(10, 7, 0);
            Vector3d other2 = Vector3d.YAxis;

            // Assert
            vec.IsPerpendicularTo(other1).Should().BeTrue();
            vec.IsPerpendicularTo(other2).Should().BeFalse();
        }


        [Fact]
        public void It_Returns_The_Perpendicular_Vector()
        {
            // Arrange
            Vector3d vectorGuide = new Vector3d(-7, 10, -5);
            Vector3d vector = new Vector3d(0, 1, 0);
            Vector3d vectorExpected = new Vector3d(10, 7, 0);

            // Act
            Vector3d perpVector = vector.PerpendicularTo(vectorGuide);

            // Assert
            perpVector.Equals(vectorExpected).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Perpendicular_Vector_Given_Three_Points()
        {
            // Arrange
            Vector3d vector = new Vector3d(-7, 10, -5);
            Point3d pt1 = new Point3d(3, -1, 2);
            Point3d pt2 = new Point3d(1, -1, -3);
            Point3d pt3 = new Point3d(4, -3, 1);
            Vector3d vectorExpected = new Vector3d(-10, -7, 4);

            // Act
            Vector3d perVector = vector.PerpendicularTo(pt1, pt2, pt3);

            // Assert
            perVector.Equals(vectorExpected).Should().BeTrue();
        }

        [Fact]
        public void Unitize_Throws_An_Error_If_Vector_Or_Zero_Length()
        {
            //Arrange
            Vector3d vectorZero = new Vector3d(0, 0, 0);
            Vector3d vectorUnset = Vector3d.Unset;

            // Act
            Vector3d result1 = vectorZero.Unitize();
            Vector3d result2 = vectorUnset.Unitize();

            // Assert
            result1.Equals(Vector3d.Unset).Should().Be(true);
            result2.Equals(Vector3d.Unset).Should().Be(true);
        }

        [Fact]
        public void It_Returns_A_Unitized_Vector()
        {
            // Arrange
            Vector3d vector = new Vector3d(-7, 10, -5);
            Vector3d expectedVector = vector * (1/vector.Length);

            // Act
            Vector3d unitizedVector = vector.Unitize();

            // Assert
            unitizedVector.Equals(expectedVector).Should().Be(true);
        }

        [Fact]
        public void It_Returns_A_Vector_Rotated_By_An_Angle()
        {
            // Arrange
            Vector3d vector = new Vector3d(-7, 10, -5);
            double rotationAngle1 = GeoSharpMath.ToRadians(-0.0000125);
            Vector3d expectedResult1 = new Vector3d(-7.0, 10.0, -5.0);
            double rotationAngle2 = GeoSharpMath.ToRadians(0.0);
            Vector3d expectedResult2 = new Vector3d(-7.0, 10.0, -5.0);
            double rotationAngle3 = GeoSharpMath.ToRadians(12.5);
            Vector3d expectedResult3 = new Vector3d(-7.454672, 10.649531, -2.239498);
            double rotationAngle4 = GeoSharpMath.ToRadians(450);
            Vector3d expectedResult4 = new Vector3d(-2.867312, 4.09616, 12.206556);

            Vector3d axis = new Vector3d(10, 7, 0);

            // Act
            Vector3d result1 = vector.Rotate(axis, rotationAngle1);
            Vector3d result2 = vector.Rotate(axis, rotationAngle2);
            Vector3d result3 = vector.Rotate(axis, rotationAngle3);
            Vector3d result4 = vector.Rotate(axis, rotationAngle4);

            // Assert
            result1.EpsilonEquals(expectedResult1, 1e-6).Should().Be(true);
            result2.EpsilonEquals(expectedResult2, 1e-6).Should().Be(true);
            result3.EpsilonEquals(expectedResult3, 1e-6).Should().Be(true);
            result4.EpsilonEquals(expectedResult4, 1e-6).Should().Be(true);
        }

        [Fact]
        public void It_Checks_If_Two_Vectors_Are_Parallel(/*double[] v4, double[] v2, int result*/)
        {
            // Arrange
            Vector3d vec1 = new Vector3d(11.5, 0.0, 0.0);
            Vector3d vec2 = new Vector3d(11.5, 0.0, 0.0);
            Vector3d vec3 = new Vector3d(-7.0, 10.0, -5.0);
            Vector3d vec4 = new Vector3d(7.0, 15.0, 0.0);
            Vector3d vec5 = new Vector3d(7.0, 0.0, 0.0);
            Vector3d vec6 = new Vector3d(-7.0, 0.0, 0.0);
            int expectedVec1Vec2 = 1;
            int expectedVec3Vec4 = 0;
            int expectedVec5Vec6 = -1;

            // Assert
            vec1.IsParallelTo(vec2).Should().Be(expectedVec1Vec2);
            vec3.IsParallelTo(vec4).Should().Be(expectedVec3Vec4);
            vec5.IsParallelTo(vec6).Should().Be(expectedVec5Vec6);
        }

        [Fact]
        public void It_Returns_The_Maximum_Coordinate_Of_A_Vector()
        {
            //Arrange
            var v1 = new Vector3d(12, 4, 3);
            var expected = 12;

            //Act
            var result = v1.MaximumCoordinate;

            //Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void It_Returns_The_Minimum_Coordinate_Of_A_Vector()
        {
            //Arrange
            var v1 = new Vector3d(12, 4, 3);
            var expected = 3;

            //Act
            var result = v1.MinimumCoordinate;

            //Assert
            result.Should().Be(expected);
        }
    }
}
