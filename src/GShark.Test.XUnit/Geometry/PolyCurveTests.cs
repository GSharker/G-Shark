using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
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

            _polycurve = new PolyCurve();
            _polycurve.Append(_nurbs);
            _polycurve.Append(_line);
            _polycurve.Append(_arc);
        }

        [Fact]
        public void It_Returns_A_PolyCurve_With_Three_Segments()
        {
            // Arrange
            int numberOfExpectedSegments = 3;

            // Act            

            // Arrange
            _polycurve.Should().NotBeNull();
            _polycurve.SegmentCount.Should().Be(numberOfExpectedSegments);
        }

        [Fact]
        public void It_Returns_If_PolyCurve_Is_Closed()
        {
            // Arrange

            // Act            

            // Arrange
            _polycurve.Should().NotBeNull();
            _polycurve.IsClosed.Should().BeFalse();

            var closedPolyCurve = _polycurve;
            closedPolyCurve.Close();
            closedPolyCurve.IsClosed.Should().BeTrue();
            closedPolyCurve.SegmentCount.Should().Be(4);
        }


        [Fact]
        public void It_Returns_A_PolyCurve_Start_And_End_Points()
        {
            // Arrange
            var startPoint = _nurbs.PointAt(0.0);
            var endPoint = _arc.EndPoint;

            // Act            

            // Arrange
            _polycurve.StartPoint.Should().Be(startPoint);
            _polycurve.EndPoint.Should().Be(endPoint);
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_PolyCurve()
        {
            // Arrange
            var nested = new PolyCurve(_polycurve);

            // Act            
            var length = _polycurve.Length;
            var lengthNested = nested.Length;

            // Arrange
            length.Should().BeApproximately(30.623806269249716, GSharkMath.Epsilon);
            lengthNested.Should().BeApproximately(30.623806269249716, GSharkMath.Epsilon);

            _testOutput.WriteLine(string.Format("Length: {0}", length));
        }


        [Fact]
        public void It_Returns_A_Point_At_Length()
        {
            // Act
            var p1 = new Point3(5.0, 3.042501, 4.519036);
            var p2 = new Point3(5.0, 5.0, -1.730175);
            var p3 = new Point3(6.007615, 5.0, -5.510126);

            var pl1 = _polycurve.PointAtLength(15);
            var pl2 = _polycurve.PointAtLength(22);
            var pl3 = _polycurve.PointAtLength(26);

            // Arrange
            pl1.DistanceTo(p1).Should().BeLessOrEqualTo(1e-4);
            pl2.DistanceTo(p2).Should().BeLessOrEqualTo(1e-4);
            pl3.DistanceTo(p3).Should().BeLessOrEqualTo(1e-4);

            _testOutput.WriteLine(string.Format("Point 1 on the nurbs: {0} - Deviation: {1}", pl1, pl1.DistanceTo(p1)));
            _testOutput.WriteLine(string.Format("Point 2 on the line: {0} - Deviation: {1}", pl2, pl2.DistanceTo(p2)));
            _testOutput.WriteLine(string.Format("Point 3 on the arc: {0} - Deviation: {1}", pl3, pl3.DistanceTo(p3)));
        }

        [Fact]
        public void It_Returns_A_Point_At_Parameter()
        {
            // Act
            var p1 = new Point3(5.0, 3.042501, 4.519036);
            var p2 = new Point3(5.0, 5.0, -1.730175);
            var p3 = new Point3(6.007615, 5.0, -5.510126);

            double t0 = 0.265154;
            double t1 = 0.564023;
            double t2= 0.80376;

            var pl1 = _polycurve.PointAt(t0);
            var pl2 = _polycurve.PointAt(t1);
            var pl3 = _polycurve.PointAt(t2);

            // Arrange
            pl1.DistanceTo(p1).Should().BeLessOrEqualTo(1e-4);
            pl2.DistanceTo(p2).Should().BeLessOrEqualTo(1e-4);
            pl3.DistanceTo(p3).Should().BeLessOrEqualTo(1e-4);

            _testOutput.WriteLine(string.Format("Point at t={0} on the nurbs: {1} - Deviation: {2}", t0, pl1, pl1.DistanceTo(p1)));
            _testOutput.WriteLine(string.Format("Point at t={0} on the line: {1} - Deviation: {2}", t1, pl2, pl2.DistanceTo(p2)));
            _testOutput.WriteLine(string.Format("Point at t={0} on the arc: {1} - Deviation: {2}", t2, pl3, pl3.DistanceTo(p3)));
        }
    }
}
