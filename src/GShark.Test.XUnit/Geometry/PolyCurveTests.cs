using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class PolyCurveTests
    {
        private PolyCurve _polycurve;
        private NurbsCurve _nurbs;
        private Line _line;
        private Arc _arc;

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
            _arc = Arc.ByStartEndDirection(new Point3(5, 5, -2.5), new Point3(10, 5, -7.5), new Vector3(0, 0, -1));            
        }

        [Fact]
        public void It_Returns_A_PolyCurve()
        {
            // Arrange
            int numberOfExpectedSegments = 1;

            // Act
            _polycurve = new PolyCurve(_nurbs);

            // Arrange
            _polycurve.Should().NotBeNull();
            _polycurve.SegmentCount.Should().Be(numberOfExpectedSegments);
        }
    }
}
