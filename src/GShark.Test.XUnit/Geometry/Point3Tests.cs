using System.Collections.Generic;
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
        public void If_Check_Project_A_Point_On_A_Plane()
        {
            // Arrange
            Point3 point = new Point3(10, 20, -5);
            Point3 planeOrigin = new Point3(0, 10, 0);
            Vector3 direction = new Vector3(0, 1, 0);
            Plane plane = new Plane(planeOrigin, direction);
            var expectedPt = new Point3(10, 10, -5);

            // Act
            var result = point.ProjectToPlan(plane);

            // Assert
            result.Should().BeEquivalentTo(expectedPt);
            result.Equals(expectedPt).Should().Be(true);
        }

        [Fact]
        public void It_Check_Point_Is_On_Line()
        {
            //Arrange
            Point3 p1 = new Point3(0, 5, 0);
            Point3 p2 = new Point3(0, 20, 0);
            Point3 p3 = new Point3(0, 0, 0);
            Line line1 = new Line(new Point3(0,0,0), new Point3(0,10,0));
            // Act
            bool isOnLine = p2.IsOnLine(line1);
            bool isOnLine1 = p1.IsOnLine(line1);
            bool isOnLine2 = p3.IsOnLine(line1);
            // Assert
            Assert.False(isOnLine);
            Assert.True(isOnLine1);
            Assert.True(isOnLine2);
            
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
        public void It_Returns_Centroid_Of_Collection_Points()
        {
            // Arrange
            Point3 p1 = new Point3(1, 2, 3);
            Point3 p2 = new Point3(4, 5, 6);
            Point3 p3 = new Point3(7, 8, 9);
            Point3 p4 = new Point3(10, 11, 12);
            List<Point3> point3s = new List<Point3> { p1, p2, p3, p4 };
            //Act
            Point3 centroid = Point3.Centroid(point3s);
            Point3 expectedCentroid = new Point3(5.5, 6.5, 7.5);
            // Assert
            centroid.Equals(expectedCentroid).Should().Be(true);
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

        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal_Null_Case1()
        {
            // Arrange
            Point3 p1 = new Point3(5.982099, 5.950299, 0);
            Point3 p2 = null;

            // Assert
            (p1 == p2).Should().BeFalse();
        }

        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal_Null_Case2()
        {
            // Arrange
            Point3 p1 = null;
            Point3 p2 = new Point3(5.982099, 5.950299, 0);

            // Assert
            (p1 == p2).Should().BeFalse();
        }
        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal_Null_Case3()
        {
            // Arrange
            Point3 p1 = null;
            Point3 p2 = null;

            // Assert
            (p1 == p2).Should().BeFalse();
        }

        [Fact]
        public void It_Returns_Whether_A_Point_Is_Inside_Outside_Or_Coincident_With_A_Polygon()
        {
            //Arrange
            var testPointOutside = new Point3(-156.409788517205, -69.8134382323643, 0);
            var testPointInside = new Point3(26.5133684383793, 24.5045165536549, 31.5968021722952);
            var testPointCoincident = new Line(
                    new Point3(-27.1829592472304, -12.3049979552339, 59.1652925745504),
                    new Point3(-40.9982936339814, 20.739935073677, 34.4162859330554)).MidPoint;

            var testPolygon = new GShark.Geometry.Polygon(new Point3[] {
                    new Point3(-27.1829592472304,-12.3049979552339,59.1652925745504),
                    new Point3(-40.9982936339814,20.739935073677,34.4162859330554),
                    new Point3(19.082346606075,37.5838503530052,21.8010335204645),
                    new Point3(49.9258838416119,19.3255605082373,35.4755819371661),
                    new Point3(34.18282837764,-33.7777754487285,75.2473319096853)
            });

            //Act
            var pointOutside = testPointOutside.InPolygon(testPolygon);
            var pointInside = testPointInside.InPolygon(testPolygon);
            var pointCoincident = testPointCoincident.InPolygon(testPolygon);

            //Assert
            pointOutside.Should().Be(-1);
            pointInside.Should().Be(1);
            pointCoincident.Should().Be(0);
        }
    }
}
