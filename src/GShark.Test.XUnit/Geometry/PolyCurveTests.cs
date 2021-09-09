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
        private readonly NurbsCurve _nurbs;
        private readonly Line _line;
        private readonly Arc _arc;

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

            _nurbs = new NurbsCurve(pts, degree);
            _line = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));
            _arc = Arc.ByStartEndDirection(new Point3(5, 5, -2.5), new Point3(10, 5, -5), new Vector3(0, 0, -1));

            _polycurve = new PolyCurve();
            _polycurve.Append(_nurbs);
            _polycurve.Append(_line);
            _polycurve.Append(_arc);
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_PolyCurve()
        {
            // Arrange
            double expectedLength = 30.623806269249716;

            // Act            
            var length = _polycurve.Length;

            // Arrange
            length.Should().BeApproximately(expectedLength, GSharkMath.Epsilon);
        }

        [Theory]
        [InlineData(new double[] { 5, 3.04250104617472, 4.51903625915119 }, 15)]
        [InlineData(new double[] { 5, 5, -1.73017533397891 }, 22)]
        [InlineData(new double[] { 6.00761470775174, 5, -5.51012618975348 }, 26)]
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
    }
}
