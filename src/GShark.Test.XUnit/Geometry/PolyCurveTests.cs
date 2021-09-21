using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class PolyCurveTests
    {
        private readonly PolyCurve _polycurve;

        private readonly ITestOutputHelper _testOutput;

        public PolyCurveTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(5, 0, 0),
                new Point3(5, 0, 5),
                new Point3(5, 5, 5),
                new Point3(5, 5, 0)
            };

            #region example
            // Initializes a polycurve from a curve a line and an arc.
            NurbsCurve curve = new NurbsCurve(pts, degree);
            Line line = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));
            Arc arc = Arc.ByStartEndDirection(new Point3(5, 5, -2.5), new Point3(10, 5, -5), new Vector3(0, 0, -1));

            List<NurbsBase> curveList = new List<NurbsBase>();
            curveList.Add(curve);
            curveList.Add(line);
            curveList.Add(arc);
            _polycurve = new PolyCurve(curveList);
            #endregion
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_PolyCurve()
        {
            // Arrange
            double expectedLength = 29.689504;

            // Act            
            var length = _polycurve.Length;

            // Arrange
            length.Should().BeApproximately(expectedLength, GSharkMath.MinTolerance);
        }

        [Theory]
        [InlineData(new double[] { 5, 3.042501, 4.519036 }, 15)]
        [InlineData(new double[] { 5, 5, -1.730175 }, 22)]
        [InlineData(new double[] { 6.118663, 5, -4.895879 }, 25.5)]
        public void It_Returns_A_Point_At_Length(double[] coords, double length)
        {
            // Arrange
            Point3 expectedPoint = new Point3(coords[0], coords[1], coords[2]);

            //Act
            Point3 pt = _polycurve.PointAtLength(length);

            // Assert
            (pt == expectedPoint).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.265154444812697, 15)]
        [InlineData(0.564023377863855, 22)]
        [InlineData(0.797352256245054, 25.5)]
        public void It_Returns_The_Length_At_Parameter(double t, double expectedLength)
        {
            //Act
            double length = _polycurve.LengthAt(t);

            // Assert
            length.Should().BeApproximately(expectedLength, GSharkMath.MinTolerance);
        }

        [Theory]
        [InlineData(23.769824635278, 2)]
        [InlineData(21.269824635278, 1)]
        [InlineData(20, 0)]
        public void It_Returns_The_Segment_At_Length(double length, int expectedSegmentIndex)
		{
            //Arrange
            NurbsBase expectedSegment = _polycurve.Segments[expectedSegmentIndex];

            //Act
            NurbsBase segmentResult = _polycurve.SegmentAtLength(length);

            //Assert
            segmentResult.Should().BeSameAs(expectedSegment);
        }

        
    }
}
