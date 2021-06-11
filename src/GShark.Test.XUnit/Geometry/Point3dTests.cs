using FluentAssertions;
using GShark.Geometry;
using System;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class Point3dTests
    {
        private readonly ITestOutputHelper _testOutput;

        public Point3dTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Distance_Between_Two_Points()
        {
            //Arrange
            var p1 = new Point3d(15, 3, 6);
            var p2 = new Point3d(-5, -8, 5);
            var p2p1 = p2 - p1;
            //Same as measuring the lenght of a vector between the two points.
            var expectedDistance = p2p1.Length;

            //Act
            var distance = p1.DistanceTo(p2);

            //Assert
            distance.Should().Be(expectedDistance);
        }

        [Fact]
        public void It_Returns_The_Distance_Between_A_Point_And_A_Line()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void It_Returns_The_Linear_Interpolation_Between_Two_Points()
        { 
            //Arrange
           Point3d p1 = new Point3d(0d, 0d, 0d);
            Point3d p2 = new Point3d(10d, 10d, 10d);
            Point3d expectedPoint = new Point3d(5d, 5d, 5d);
            double amount = 0.5;

            // Act
            var result = Point3d.Interpolate(p1, p2, amount);


            // Assert
            result.Equals(expectedPoint).Should().Be(true);
        }

        [Fact]
        public void It_Divides_A_Point3d_By_A_Number()
        {
            // Arrange
            Point3d p = new Point3d(-10, 15, 5);
            Point3d expectedPoint = new Point3d(-5, 7.5, 2.5);

            // Act
            Point3d divisionResult = p / 2;

            // Assert
            divisionResult.Equals(expectedPoint).Should().Be(true);
        }

        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal()
        {
            // Arrange
            Point3d p1 = new Point3d(5.982099, 5.950299, 0);
            Point3d p2 = new Point3d(5.982099, 5.950299, 0);

            // Assert
            (p1 == p2).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Maximum_Coordinate_Of_A_Point()
        {
            //Arrange
            var p1 = new Point3d(12, 4, 3);
            var expected = 12;

            //Act
            var result = p1.MaximumCoordinate;

            //Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void It_Returns_The_Minimum_Coordinate_Of_A_Point()
        {
            //Arrange
            var p1 = new Point3d(12, 4, 3);
            var expected = 3;

            //Act
            var result = p1.MaximumCoordinate;

            //Assert
            result.Should().Be(expected);
        }
    }
}
