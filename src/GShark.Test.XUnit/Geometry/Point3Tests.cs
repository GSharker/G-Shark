using FluentAssertions;
using GShark.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class Point3Tests
    {
        private readonly ITestOutputHelper _testOutput;

        public Point3Tests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Distance_Between_Two_Points()
        {
            //Arrange
            var p1 = new Point3(15, 3, 6);
            var p2 = new Point3(-5, -8, 5);
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
            // Arrange
            var line = new Line(new Point3(0, 0, 0), new Point3(30, 45, 0));
            var pt = new Point3(10, 20, 0);
            double distanceExpected = 2.7735009811261464;

            // Act
            double distance = pt.DistanceTo(line);

            // Assert
            distance.Should().Be(distanceExpected);
        }

        [Fact]
        public void It_Checks_If_A_Point_Lies_On_A_Plane()
        {
            // Arrange
            Plane plane = new Plane(new Point3(30, 45, 0), new Point3(30, 45, 0));
            var pt = new Point3(26.565905, 47.289396, 0.0);

            // Assert
            pt.IsOnPlane(plane, 0.001).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Linear_Interpolation_Between_Two_Points()
        {
            //Arrange
            Point3 p1 = new Point3(0d, 0d, 0d);
            Point3 p2 = new Point3(10d, 10d, 10d);
            Point3 expectedPoint = new Point3(5d, 5d, 5d);
            double amount = 0.5;

            // Act
            var result = Point3.Interpolate(p1, p2, amount);

            // Assert
            result.Equals(expectedPoint).Should().Be(true);
        }

        [Fact]
        public void It_Divides_A_Point3d_By_A_Number()
        {
            // Arrange
            Point3 p = new Point3(-10, 15, 5);
            Point3 expectedPoint = new Point3(-5, 7.5, 2.5);

            // Act
            Point3 divisionResult = p / 2;

            // Assert
            divisionResult.Equals(expectedPoint).Should().Be(true);
        }

        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal()
        {
            // Arrange
            Point3 p1 = new Point3(5.982099, 5.950299, 0);
            Point3 p2 = new Point3(5.982099, 5.950299, 0);

            // Assert
            (p1 == p2).Should().BeTrue();
        }
    }
}
