using FluentAssertions;
using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using System;
using System.Linq;
using GShark.Geometry.Interfaces;
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
            Line line = new Line(pt1, Vector3d.XAxis, 15);
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
            line.Start.Equals(startPoint).Should().BeTrue();
            line.End.Equals(endPoint).Should().BeTrue();
            line.Length.Equals(startPoint.DistanceTo(endPoint));
        }

        [Fact]
        public void It_Throws_An_Exception_If_Inputs_Are_Not_Valid_Or_Equals()
        {
            // Arrange
            Point3 pt = new Point3(5, 5, 0);

            // Act
            Func<Line> func0 = () => new Line(pt, pt);
            Func<Line> func1 = () => new Line(pt, Vector3d.Unset);

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
            Vector3d expectedDirection = new Vector3d(1, 0, 0);
            Point3 expectedEndPoint = new Point3(lineLength, 0, 0);

            // Act
            Line line1 = new Line(startPoint, Vector3d.XAxis, lineLength);
            
            //ToDo this should give an error since it is the direction which should be negated/reversed. Length should always be > 0.
            Line line2 = new Line(startPoint, Vector3d.XAxis, -lineLength);

            // Assert
            line1.Length.Should().Be(line2.Length).And.Be(lineLength);
            line1.Start.Should().BeEquivalentTo(startPoint);

            line1.Direction.Should().BeEquivalentTo(expectedDirection);
            line1.End.Should().BeEquivalentTo(expectedEndPoint);

            //ToDo Review assertion.
            //line2.Direction.Should().BeEquivalentTo(Vector.Reverse(expectedDirection));
            //line2.End.Should().BeEquivalentTo(Vector.Reverse(expectedEndPoint));
        }

        [Fact]
        public void It_Throws_An_Exception_If_Length_Is_Zero()
        {
            // Arrange
            Point3 startPoint = new Point3(0, 0, 0);
            // Act
            Func<Line> func = () => new Line(startPoint, Vector3d.XAxis, 0);

            // Assert
            //ToDo Length should always be > 0.
            func.Should().Throw<Exception>().WithMessage("Length must not be 0.0");
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
            Vector3d expectedDirection = new Vector3d(0.5547, 0.83205, 0);

            // Act
            Vector3d dir = _exampleLine.Direction;

            // Assert
            dir.EpsilonEquals(expectedDirection, 1e-5).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_ClosestPoint()
        {
            // Arrange
            Point3 pt = new Point3 ( 5, 8, 0);
            Point3 expectedPt = new Point3(8.692308, 5.538462, 0);

            // Act
            Point3 closestPt = _exampleLine.ClosestPoint(pt);

            // Assert
            closestPt.EpsilonEquals(expectedPt, 1e-6).Should().BeTrue();
        }

        [Fact]
        public void PointAt_Throw_An_Exception_If_Parameter_Outside_The_Curve_Domain()
        {
            // Act
            Func<Point3> func = () => _exampleLine.PointAt(2);

            // Assert
            func.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Parameter is outside the domain 0.0 to 1.0 *");
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
           ptEvaluated.EpsilonEquals(expectedPt, GeoSharkMath.Epsilon).Should().BeTrue();
            
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
            parameter.Should().BeApproximately(expectedParam, GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Flipped_Line()
        {
            // Act
            Line flippedLine = _exampleLine.Flip();

            // Assert
            flippedLine.Start.Equals(_exampleLine.End).Should().BeTrue();
            flippedLine.End.Equals(_exampleLine.Start).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_An_Extend_Line()
        {
            // Act
            Line extendedLine = _exampleLine.Extend(0, -5);

            // Assert
            extendedLine.Length.Should().BeApproximately(13.027756, GeoSharkMath.MaxTolerance);
            extendedLine.Start.Should().BeEquivalentTo(_exampleLine.Start);
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
        //ToDo Create Transform data to represent various canonical transformations to be used in transform tests on all objects.
        public void It_Translates_A_Line()
        {
            // Arrange
            Transform transform = Transform.Translation(new Vector3d(10, 10, 0));

            // Act
            Line transformedLine = _exampleLine.Transform(transform);

            // Assert
            transformedLine.Start.Should().BeEquivalentTo(new Point3(15, 10, 0));
            transformedLine.End.Should().BeEquivalentTo(new Point3(25, 25, 0));
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Data_From_The_Line()
        {
            // Arrange
            ICurve line = _exampleLine;

            // Assert
            line.ControlPoints.Count.Should().Be(2);
            line.Degree.Should().Be(1);
        }
    }
}
