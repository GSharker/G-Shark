using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Vector3 = GShark.Geometry.Vector3;

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
            Vector3 v1 = new Vector3(20d, 0d, 0d);
            Vector3 v2 = new Vector3(-10d, 15d, 0d);

            // Act
            double angle = Vector3.VectorAngle(v1, v2);

            // Assert
            angle.Should().Be(2.1587989303424644);
        }

        [Fact]
        public void It_Returns_The_Radian_Angle_Between_Two_Vectors_On_A_Plane()
        {
            // Arrange
            Vector3 v1 = new Vector3(1, 0, 0);
            Vector3 v2 = new Vector3(3, 0, 4);
            var expectedAngle = System.Math.Atan2(4,3);

            // Act
            double reflexAngle;
            double angle = Vector3.VectorAngleOnPlane(v1, v2, Plane.PlaneZX, out reflexAngle);

            // Assert
            angle.Should().BeApproximately(expectedAngle, GeoSharkMath.Epsilon);
            reflexAngle.Should().BeApproximately(2 * Math.PI - expectedAngle, GeoSharkMath.Epsilon);
        }

        [Fact]
        public void It_Returns_A_Reversed_Vector()
        {
            // Arrange
            Vector3 v1 = new Vector3(20d, 0d, 0d);
            Vector3 vectorExpected = new Vector3(-20d, 0d, 0d);

            // Act
            var reversedVector = v1.Reverse();

            // Assert
            reversedVector.Should().BeEquivalentTo(vectorExpected);
        }

        [Fact]
        public void It_Checks_If_Vectors_Are_Valid_Or_Not()
        {
            //Arrange
            Vector3 v1 = new Vector3(20d, -10d, 0d);
            Vector3 v2 = Vector3.Unset;
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
            Vector3 v1 = new Vector3(-10d, 5d, 10d);
            Vector3 v2 = new Vector3(10d, 15d, 5d);
            Vector3 crossProductExpected = new Vector3(-125d, 150d, -200d);

            // Act
            Vector3 crossProduct = Vector3.CrossProduct(v1, v2);

            // Assert
            crossProduct.Should().BeEquivalentTo(crossProductExpected);
        }

        [Fact]
        public void It_Returns_The_Dot_Product_Of_Two_Vectors()
        {
            // Arrange
            Vector3 v1 = new Vector3(-10d, 5d, 10d);
            Vector3 v2 = new Vector3(10d, 15d, 5d);

            // Act
            double dotProduct = Vector3.DotProduct(v1, v2);

            // Assert
            dotProduct.Should().Be(25);
        }

        [Fact]
        public void It_Returns_The_Squared_Length_Of_A_Vector()
        {
            
            // Arrange
            Vector3 v1 = new Vector3(10d, 15d, 5d);

            // Act
            double squareLength = v1.SquareLength;

            // Assert
            squareLength.Should().Be(350);
        }

        [Fact]
        public void It_Returns_The_Length_Of_A_Vector()
        {
            //Arrange
            Vector3 vector1 = new Vector3(-18d, -21d, -17d);
            double expectedLength1 = 32.46536616149586;
            Vector3 vector2 = Vector3.Unset;
            double expectedLength2 = 0.0;
            Vector3 vector3 = new Vector3(0, 0, 0);
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
            Vector3 expected = new Vector3(0, 0, 0);

            // Act
            Vector3 result = Vector3.Zero;

            // Assert
            result.Equals(expected).Should().Be(true);
        }


        [Fact]
        public void It_Returns_An_Amplified_Vector()
        {
            //Arrange
            int amplitude = 10;
            Vector3 vec1 = new Vector3(3, 4, 0);
            Vector3 vec1AmplifiedByAmplitude = new Vector3(6, 8, 0);

            // Act
            Vector3 amplifiedVector = vec1.Amplify(amplitude);
            
            // Assert
            amplifiedVector.EpsilonEquals(vec1AmplifiedByAmplitude, GeoSharkMath.MaxTolerance).Should().Be(true);
        }

        [Fact]
        public void It_Returns_The_Sum_Of_Two_Vectors()
        {
            // Arrange
            Vector3 vec1 = new Vector3(20, 0, 0);
            Vector3 vec2 = new Vector3(-10, 15, 5);
            Vector3 expectedVec = new Vector3(10, 15, 5);

            // Assert
            (vec1 + vec2).Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Returns_The_Subtraction_Of_Two_Vectors()
        {
            // Arrange
            #region example
            Vector3 vec1 = new Vector3(20, 0, 0);
            Vector3 vec2 = new Vector3(-10, 15, 5);
            #endregion
            Vector3 expectedVec = new Vector3(30, -15, -5);

            //Act
            Vector3 result = vec1 - vec2;

            // Assert
            result.Should().BeEquivalentTo(expectedVec);
        }

        [Fact]
        public void It_Multiplies_A_Vector_By_A_Number()
        {
            // Arrange
            Vector3 vector = new Vector3(-10, 15, 5);
            Vector3 expectedVec = new Vector3(-70, 105, 35);

            //Act
            Vector3 result = vector * 7;

            // Assert
            result.Equals(expectedVec).Should().Be(true);
        }

        [Fact]
        public void It_Divides_A_Vector_By_A_Number()
        {
            // Arrange
            Vector3 vec = new Vector3( -10, 15, 5);
            Vector3 expectedVec = new Vector3(-5, 7.5, 2.5);

            // Act
            Vector3 divisionResult = vec / 2;

            // Assert
            divisionResult.Equals(expectedVec).Should().Be(true);
        }

        [Fact]
        public void It_Returns_True_If_Vectors_Are_Equal()
        {
            // Arrange
            Vector3 vec1 = new Vector3 (5.982099, 5.950299, 0);
            Vector3 vec2 = new Vector3 (5.982099, 5.950299, 0);

            // Assert
            (vec1 == vec2).Should().BeTrue();
        }

        [Fact]
        public void It_Checks_If_Vectors_Are_Perpendicular()
        {
            // Arrange
            Vector3 vec = new Vector3(-7, 10, -5);
            Vector3 other1 = new Vector3(10, 7, 0);
            Vector3 other2 = Vector3.YAxis;

            // Assert
            vec.IsPerpendicularTo(other1).Should().BeTrue();
            vec.IsPerpendicularTo(other2).Should().BeFalse();
        }


        [Fact]
        public void It_Returns_The_Perpendicular_Vector()
        {
            // Arrange
            Vector3 vectorGuide = new Vector3(-7, 10, -5);
            Vector3 vector = new Vector3(0, 1, 0);
            Vector3 vectorExpected = new Vector3(10, 7, 0);

            // Act
            Vector3 perpVector = vector.PerpendicularTo(vectorGuide);

            // Assert
            perpVector.Equals(vectorExpected).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Perpendicular_Vector_Given_Three_Points()
        {
            // Arrange
            Vector3 vector = new Vector3(-7, 10, -5);
            Point3 pt1 = new Point3(3, -1, 2);
            Point3 pt2 = new Point3(1, -1, -3);
            Point3 pt3 = new Point3(4, -3, 1);
            Vector3 vectorExpected = new Vector3(-10, -7, 4);

            // Act
            Vector3 perVector = vector.PerpendicularTo(pt1, pt2, pt3);

            // Assert
            perVector.Equals(vectorExpected).Should().BeTrue();
        }

        [Fact]
        public void Unitize_Returns_UnsetValue_If_Vector_Is_Unset_Or_Zero_Length()
        {
            //Arrange
            Vector3 vectorZero = new Vector3(0, 0, 0);
            Vector3 vectorUnset = Vector3.Unset;

            // Act
            Vector3 result1 = vectorZero.Unitize();
            Vector3 result2 = vectorUnset.Unitize();

            // Assert
            result1.Equals(Vector3.Unset).Should().Be(true);
            result2.Equals(Vector3.Unset).Should().Be(true);
        }

        [Fact]
        public void It_Returns_A_Unitized_Vector()
        {
            // Arrange
            Vector3 vector = new Vector3(-7, 10, -5);
            Vector3 expectedVector = vector * (1/vector.Length);

            // Act
            Vector3 unitizedVector = vector.Unitize();

            // Assert
            unitizedVector.Equals(expectedVector).Should().Be(true);
        }

        [Fact]
        public void It_Returns_A_Vector_Rotated_By_An_Angle()
        {
            // Arrange
            Vector3 vector = new Vector3(-7, 10, -5);
            double rotationAngle1 = GeoSharkMath.ToRadians(-0.0000125);
            Vector3 expectedResult1 = new Vector3(-7.0, 10.0, -5.0);
            double rotationAngle2 = GeoSharkMath.ToRadians(0.0);
            Vector3 expectedResult2 = new Vector3(-7.0, 10.0, -5.0);
            double rotationAngle3 = GeoSharkMath.ToRadians(12.5);
            Vector3 expectedResult3 = new Vector3(-7.454672, 10.649531, -2.239498);
            double rotationAngle4 = GeoSharkMath.ToRadians(450);
            Vector3 expectedResult4 = new Vector3(-2.867312, 4.09616, 12.206556);

            Vector3 axis = new Vector3(10, 7, 0);

            // Act
            Vector3 result1 = vector.Rotate(axis, rotationAngle1);
            Vector3 result2 = vector.Rotate(axis, rotationAngle2);
            Vector3 result3 = vector.Rotate(axis, rotationAngle3);
            Vector3 result4 = vector.Rotate(axis, rotationAngle4);

            // Assert
            result1.EpsilonEquals(expectedResult1, 1e-6).Should().Be(true);
            result2.EpsilonEquals(expectedResult2, 1e-6).Should().Be(true);
            result3.EpsilonEquals(expectedResult3, 1e-6).Should().Be(true);
            result4.EpsilonEquals(expectedResult4, 1e-6).Should().Be(true);
        }

        [Fact]
        public void It_Checks_If_Two_Vectors_Are_Parallel()
        {
            // Arrange
            Vector3 vec1 = new Vector3(11.5, 0.0, 0.0);
            Vector3 vec2 = new Vector3(11.5, 0.0, 0.0);
            Vector3 vec3 = new Vector3(-7.0, 10.0, -5.0);
            Vector3 vec4 = new Vector3(7.0, 15.0, 0.0);
            Vector3 vec5 = new Vector3(7.0, 0.0, 0.0);
            Vector3 vec6 = new Vector3(-7.0, 0.0, 0.0);
            int expectedVec1Vec2 = 1;
            int expectedVec3Vec4 = 0;
            int expectedVec5Vec6 = -1;

            // Assert
            vec1.IsParallelTo(vec2).Should().Be(expectedVec1Vec2);
            vec3.IsParallelTo(vec4).Should().Be(expectedVec3Vec4);
            vec5.IsParallelTo(vec6).Should().Be(expectedVec5Vec6);
        }
    }
}
