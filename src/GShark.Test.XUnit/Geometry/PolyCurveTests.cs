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
        public void It_Returns_A_Tangent_At_Length()
        {
            // Act
            var v1 = new Vector3(0,0.967518,-0.252801);
            var v2 = new Vector3(0,0,-1);
            var v3 = new Vector3(0.602025,0,-0.798477);

            var vl1 = _polycurve.TangentAtLength(15);
            var vl2 = _polycurve.TangentAtLength(22);
            var vl3 = _polycurve.TangentAtLength(26);

            // Arrange
            vl1.EpsilonEquals(v1, 1e-5).Should().BeTrue();
            vl2.EpsilonEquals(v2, GSharkMath.MinTolerance).Should().BeTrue();
            vl3.EpsilonEquals(v3, GSharkMath.MinTolerance).Should().BeTrue();
            
            _testOutput.WriteLine(string.Format("{0} on the nurbs", vl1));
            _testOutput.WriteLine(string.Format("{0} on the line", vl2));
            _testOutput.WriteLine(string.Format("{0} on the arc", vl3));
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

        [Fact]
        public void It_Returns_A_Tangent_At_Parameter()
        {
            // Act
            var v1 = new Vector3(0, 0.967518, -0.252801);
            var v2 = new Vector3(0, 0, -1);
            var v3 = new Vector3(0.602025, 0, -0.798477);

            double t0 = 0.265154;
            double t1 = 0.564023;
            double t2 = 0.80376;

            var vp1 = _polycurve.TangentAt(t0);
            var vp2 = _polycurve.TangentAt(t1);
            var vp3 = _polycurve.TangentAt(t2);

            // Arrange
            vp1.EpsilonEquals(v1, 1e-4).Should().BeTrue();
            vp2.EpsilonEquals(v2, 1e-4).Should().BeTrue();
            vp3.EpsilonEquals(v3, 1e-4).Should().BeTrue();

            _testOutput.WriteLine(string.Format("{0} on the nurbs", vp1));
            _testOutput.WriteLine(string.Format("{0} on the line", vp2));
            _testOutput.WriteLine(string.Format("{0} on the arc", vp3));
        }

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            // Act
            var p1 = new Point3(-0.852901, 2.957569, 1.405093);
            var p2 = new Point3(5.110799,4.817776,-1.224014);
            var p3 = new Point3(7.15482,4.790861,-6.554605);
            var cp1 = new Point3(0.326161, 2.36256, 2.371569);
            var cp2= new Point3(5,5,-1.224014);
            var cp3 = new Point3(7.127977,5,-6.592858);

            var pl1 = _polycurve.ClosestPoint(p1);
            var pl2 = _polycurve.ClosestPoint(p2);
            var pl3 = _polycurve.ClosestPoint(p3);

            // Arrange
            pl1.DistanceTo(cp1).Should().BeLessOrEqualTo(1e-4);
            pl2.DistanceTo(cp2).Should().BeLessOrEqualTo(1e-4);
            pl3.DistanceTo(cp3).Should().BeLessOrEqualTo(1e-4);

            _testOutput.WriteLine(string.Format("Closest Point to {0} is {1} - Distance: {2}", p1, pl1, pl1.DistanceTo(p1)));
            _testOutput.WriteLine(string.Format("Closest Point to {0} is {1} - Distance: {2}", p2, pl2, pl2.DistanceTo(p2)));
            _testOutput.WriteLine(string.Format("Closest Point to {0} is {1} - Distance: {2}", p3, pl3, pl3.DistanceTo(p3)));
 
        }
    }
}
