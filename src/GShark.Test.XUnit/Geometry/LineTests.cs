using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class LineTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Line _exampleLine;
        public LineTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            #region example
            // Initializes a line by start and end point.
            Point3 pt1 = new Point3(5, 0, 0);
            Point3 pt2 = new Point3(15, 15, 0);
            _exampleLine = new Line(pt1, pt2);

            // Initializes a line by a starting point a direction and a length.
            Line line = new Line(pt1, Vector3.XAxis, 15);
            #endregion
        }

        [Fact]
        public void It_Creates_A_Line()
        {
            // Arrange
            Point3 startPoint = new Point3(5, 0, 0);
            Point3 endPoint = new Point3(10, 0, 0);

            // Act
            Line line = new Line(startPoint, endPoint);

            // Assert
            line.Should().NotBeNull();
            line.StartPoint.Equals(startPoint).Should().BeTrue();
            line.EndPoint.Equals(endPoint).Should().BeTrue();
            line.Length.Equals(startPoint.DistanceTo(endPoint));
        }

        [Fact]
        public void It_Throws_An_Exception_If_Inputs_Are_Not_Valid_Or_Equals()
        {
            // Arrange
            Point3 pt = new Point3(5, 5, 0);

            // Act
            Func<Line> func0 = () => new Line(pt, pt);
            Func<Line> func1 = () => new Line(pt, Vector3.Unset);

            // Assert
            func0.Should().Throw<Exception>();
            func1.Should().Throw<Exception>();
        }

        [Fact]
        public void It_Creates_A_Line_By_Starting_Point_Direction_And_Length()
        {
            // Arrange
            Point3 startPoint = new Point3(0, 0, 0);
            int lineLength = 15;
            Vector3 expectedDirection = new Vector3(1, 0, 0);
            Point3 expectedEndPointLine1 = new Point3(lineLength, 0, 0);
            Point3 expectedEndPointLine2 = new Point3(-lineLength, 0, 0);

            // Act
            Line line1 = new Line(startPoint, Vector3.XAxis, lineLength);
            Line line2 = new Line(startPoint, Vector3.XAxis.Reverse(), lineLength);

            // Assert
            line1.Length.Should().Be(line2.Length).And.Be(lineLength);
            line1.StartPoint.Should().BeEquivalentTo(startPoint);

            line1.Direction.Should().BeEquivalentTo(expectedDirection);
            line1.EndPoint.Should().BeEquivalentTo(expectedEndPointLine1);

            line2.Direction.Should().BeEquivalentTo(expectedDirection.Reverse());
            line2.EndPoint.Should().BeEquivalentTo(expectedEndPointLine2);
        }

        [Fact]
        public void It_Throws_An_Exception_If_Length_Is_Zero()
        {
            // Arrange
            Point3 startPoint = new Point3(0, 0, 0);
            // Act
            Func<Line> func = () => new Line(startPoint, Vector3.XAxis, 0);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_Line()
        {
            // Arrange
            double expectedLength = 18.027756;

            // Act
            Line line = _exampleLine;

            // Assert
            line.Length.Should().BeApproximately(expectedLength, 5);
        }

        [Fact]
        public void It_Returns_The_Line_Direction()
        {
            // Arrange
            Vector3 expectedDirection = new Vector3(0.5547, 0.83205, 0);

            // Act
            Vector3 dir = _exampleLine.Direction;

            // Assert
            dir.EpsilonEquals(expectedDirection, 1e-5).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_ClosestPoint()
        {
            // Arrange
            Point3 pt = new Point3(5, 8, 0);
            Point3 expectedPt = new Point3(8.692308, 5.538462, 0);

            // Act
            Point3 closestPt = _exampleLine.ClosestPoint(pt);

            // Assert
            closestPt.EpsilonEquals(expectedPt, 1e-6).Should().BeTrue();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void It_Returns_A_Point_On_The_Line_At_A_Given_Length(double len)
        {
            //Arrange
            var line =  new Line(new Point3(0,0,0), new Point3(10,0,0));
            var expectedPoint = line.StartPoint + line.Direction * len;

            //Act
            var pt = line.PointAtLength(len);

            //Assert
            pt.Equals(expectedPoint).Should().BeTrue();
        }

        [Fact]
        public void PointAt_Returns_End_Point_If_Parameter_Is_Greater_Than_The_Curve_Domain()
        {
            // Act
            var pt = _exampleLine.PointAt(2);

            // Assert
            pt.Equals(_exampleLine.EndPoint).Should().BeTrue();
        }

        [Fact]
        public void PointAt_Returns_Start_Point_If_Parameter_Is_Less_Than_The_Curve_Domain()
        {
            // Act
            var pt = _exampleLine.PointAt(-1);

            // Assert
            pt.Equals(_exampleLine.StartPoint).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Length_At_Specified_Parameter()
        {
            //Arrange
            var line = new Line(new Point3(0,0,0), new Point3(10,0,0));
            var parameter = 0.5;
            var expectedLength = 5;

            //Act
            var length = line.LengthAt(parameter);

            //Assert
            length.Equals(expectedLength).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.0, new[] { 5.0, 0, 0 })]
        [InlineData(0.15, new[] { 6.5, 2.25, 0 })]
        [InlineData(0.36, new[] { 8.6, 5.4, 0 })]
        [InlineData(0.85, new[] { 13.5, 12.75, 0 })]
        [InlineData(1.0, new[] { 15.0, 15.0, 0 })]
        public void It_Returns_The_Evaluated_Point_At_The_Given_Parameter(double t, double[] ptExpected)
        {
            //Arrange
            var expectedPt = new Point3(ptExpected[0], ptExpected[1], ptExpected[2]);

            // Act
            Point3 ptEvaluated = _exampleLine.PointAt(t);

            // Assert
            ptEvaluated.EpsilonEquals(expectedPt, GSharkMath.Epsilon).Should().BeTrue();

        }

        [Theory]
        [InlineData(0.323077, new[] { 5.0, 7.0, 0 })]
        [InlineData(0.338462, new[] { 7.0, 6.0, 0 })]
        [InlineData(0.415385, new[] { 5.0, 9.0, 0 })]
        public void It_Returns_The_Parameter_On_The_Line_Closest_To_The_Point(double expectedParam, double[] pts)
        {
            // Arrange
            Point3 pt = new Point3(pts[0], pts[1], pts[2]);

            // Act
            double parameter = _exampleLine.ClosestParameter(pt);

            // Assert
            parameter.Should().BeApproximately(expectedParam, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Flipped_Line()
        {
            // Act
            Line flippedLine = _exampleLine.Flip();

            // Assert
            flippedLine.StartPoint.Equals(_exampleLine.EndPoint).Should().BeTrue();
            flippedLine.EndPoint.Equals(_exampleLine.StartPoint).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_An_Extend_Line()
        {
            // Act
            Line extendedLine = _exampleLine.Extend(0, -5);

            // Assert
            extendedLine.Length.Should().BeApproximately(13.027756, GSharkMath.MaxTolerance);
            extendedLine.StartPoint.Should().BeEquivalentTo(_exampleLine.StartPoint);
        }

        [Fact]
        public void It_Checks_If_Two_Lines_Are_Equals()
        {
            // Act
            Line lineFlip = _exampleLine.Flip();
            Line lineFlippedBack = lineFlip.Flip();

            // Assert
            lineFlip.Equals(lineFlippedBack).Should().BeFalse();
            lineFlippedBack.Equals(_exampleLine).Should().BeTrue();
        }

        [Fact]
        public void It_Translates_A_Line()
        {
            // Arrange
            Transform transform = Transform.Translation(new Vector3(10, 10, 0));

            // Act
            Line transformedLine = _exampleLine.Transform(transform);

            // Assert
            transformedLine.StartPoint.Should().BeEquivalentTo(new Point3(15, 10, 0));
            transformedLine.EndPoint.Should().BeEquivalentTo(new Point3(25, 25, 0));
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Form_Of_A_Line()
        {
            // Arrange
            var line = _exampleLine;

            //Act
            var nurbsLine = line.ToNurbs();

            // Assert
            nurbsLine.ControlPointLocations.Count.Should().Be(2);
            nurbsLine.ControlPointLocations[0].Equals(line.StartPoint).Should().BeTrue();
            nurbsLine.ControlPointLocations[1].Equals(line.EndPoint).Should().BeTrue();
            nurbsLine.Degree.Should().Be(1);
        }
    }
}
