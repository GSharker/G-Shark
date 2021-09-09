using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;
using System.Collections.Generic;
using GShark.Intersection;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class IntersectionTests
    {
        private readonly ITestOutputHelper _testOutput;
        public IntersectionTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_Two_Planes()
        {
            // Arrange
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneYZ.SetOrigin(new Point3(10, 10, 5));
            Plane pl2 = Plane.PlaneZX.SetOrigin(new Point3(10, -10, -5));

            // Act
            bool intersection0 = Intersect.PlanePlane(pl0, pl1, out Line lineIntersect0);
            bool intersection1 = Intersect.PlanePlane(pl1, pl2, out Line lineIntersect1);

            // Assert
            intersection0.Should().BeTrue();
            lineIntersect0.StartPoint.Should().BeEquivalentTo(new Point3(10, 0, 0));
            lineIntersect0.Direction.Should().BeEquivalentTo(new Vector3(0, 1, 0));

            intersection1.Should().BeTrue();
            lineIntersect1.StartPoint.Should().BeEquivalentTo(new Point3(10, -10, 0));
            lineIntersect1.Direction.Should().BeEquivalentTo(new Vector3(0, 0, 1));
        }

        [Fact]
        public void PlanePlane_Is_False_If_Planes_Are_Parallel()
        {
            // Arrange
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneXY.SetOrigin(new Point3(10, 10, 5));

            // Act
            bool intersection = Intersect.PlanePlane(pl0, pl1, out _);

            // Assert
            intersection.Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Intersection_Point_Between_A_Segment_And_A_Plane()
        {
            // Arrange
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));
            Line ln0 = new Line(new Point3(0, 0, 0), new Point3(20, 20, 10));
            Line ln1 = new Line(new Point3(0, 0, 0), new Point3(5, 5, 10));

            // Act
            bool intersection0 = Intersect.LinePlane(ln0, pl, out Point3 pt0, out _);
            bool intersection1 = Intersect.LinePlane(ln1, pl, out Point3 pt1, out _);

            // Assert
            intersection0.Should().BeTrue();
            intersection1.Should().BeTrue();
            pt0.Equals(new Point3(10, 10, 5)).Should().BeTrue();
            pt1.Equals(new Point3(10, 10, 20)).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 10, 0 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 0, 10 })]
        [InlineData(new double[] { 10, 20, 5 }, new double[] { 10, 20, 10 })]
        public void LinePlane_Is_False_If_Line_Is_Parallel(double[] startPt, double[] endPt)
        {
            // Arrange
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));
            Line ln = new Line(new Point3(startPt[0], startPt[1], startPt[2]), new Point3(endPt[0], endPt[1], endPt[2]));

            // Act
            bool intersection = Intersect.LinePlane(ln, pl, out _, out _);

            // Assert
            intersection.Should().BeFalse();
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 3, 3, 2 }, new double[] { 4.565217, 5, 4.782609 }, new double[] { 5.217391, 5.217391, 3.478261 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 20, 0 }, new double[] { -1, 5, 2 }, new double[] { 0, 5, 0 })]
        [InlineData(new double[] { -5, 5, 0 }, new double[] { 5, 5, 0 }, new double[] { -5, 5, 0 }, new double[] { -5, 5, 0 })]
        public void It_Returns_The_Intersection_Points_Between_Segments(double[] startPt, double[] endPt, double[] output0, double[] output1)
        {
            // Arrange
            Line ln0 = new Line(new Point3(-5, 5, 0), new Point3(5, 5, 5));
            Line ln1 = new Line(new Point3(startPt[0], startPt[1], startPt[2]), new Point3(endPt[0], endPt[1], endPt[2]));
            Point3 ptCheck0 = new Point3(output0[0], output0[1], output0[2]);
            Point3 ptCheck1 = new Point3(output1[0], output1[1], output1[2]);

            // Act
            _ = Intersect.LineLine(ln0, ln1, out Point3 pt0, out Point3 pt1, out _, out _);

            // Assert
            pt0.EpsilonEquals(ptCheck0, GSharkMath.MaxTolerance).Should().BeTrue();
            pt1.EpsilonEquals(ptCheck1, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void LineLine_Is_False_If_Segments_Are_Parallel()
        {
            // Arrange
            Line ln0 = new Line(new Point3(5, 0, 0), new Point3(5, 5, 0));
            Line ln1 = new Line(new Point3(0, 0, 0), new Point3(0, 5, 0));

            // Act
            bool intersection = Intersect.LineLine(ln0, ln1, out _, out _, out _, out _);

            // Assert
            intersection.Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Intersection_Points_Between_A_Polyline_And_A_Plane()
        {
            // Arrange
            Point3[] pts = new[]
            {
                new Point3(-1.673787, -0.235355, 14.436008),
                new Point3(13.145523, 6.066452, 0),
                new Point3(2.328185, 22.89864, 0),
                new Point3(18.154088, 30.745098, 7.561387),
                new Point3(18.154088, 12.309505, 7.561387)
            };

            Point3[] intersectionChecks = new[]
            {
                new Point3(10, 4.728841, 3.064164),
                new Point3(10, 10.961005, 0),
                new Point3(10, 26.702314, 3.665482)
            };

            PolyLine poly = new PolyLine(pts);
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));

            // Act
            var intersections = Intersect.PolylinePlane(poly, pl);

            // Assert
            intersections.Count.Should().Be(intersectionChecks.Length);
            for (int i = 0; i < intersectionChecks.Length; i++)
            {
                intersections[i].EpsilonEquals(intersectionChecks[i], GSharkMath.MaxTolerance).Should().BeTrue();
            }
        }

        [Fact]
        public void It_Returns_The_Intersection_Points_Between_A_Circle_And_A_Line()
        {
            // Arrange
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));
            Circle cl = new Circle(pl, 20);
            Line ln0 = new Line(new Point3(10, 29.769674, 17.028815), new Point3(10, 37.594559, 24.680781));
            Line ln1 = new Line(new Point3(10, 40, 25), new Point3(10, 40, 17));

            Point3[] intersectionChecks = new[]
            {
                new Point3( 10, 33.00596, 20.193584),
                new Point3( 10, 4.51962, -7.663248)
            };

            Point3 intersectionCheck = new Point3(10, 40, 5);

            // Act
            _ = Intersect.LineCircle(cl, ln0, out Point3[] pts0);
            _ = Intersect.LineCircle(cl, ln1, out Point3[] pts1);

            // Assert
            for (int i = 0; i < intersectionChecks.Length; i++)
            {
                pts0[i].EpsilonEquals(intersectionChecks[i], GSharkMath.MaxTolerance).Should().BeTrue();
            }

            pts1[0].EpsilonEquals(intersectionCheck, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void CircleLine_Intersection_Returns_False_If_No_Intersections_Are_Computed()
        {
            // Arrange
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));
            Circle cl = new Circle(pl, 20);
            Line ln = new Line(new Point3(-15, 45, 17), new Point3(15, 45, 25));

            // Act
            bool intersection = Intersect.LineCircle(cl, ln, out _);

            // Assert
            intersection.Should().BeFalse();
        }

        [Fact]
        public void PlaneCircle_Is_False_If_Planes_Are_Parallel_Or_Intersection_Plane_Misses_The_Circle()
        {
            // Arrange
            Plane pl0 = Plane.PlaneYZ;
            Plane pl1 = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));
            Plane pl2 = Plane.PlaneZX.SetOrigin(new Point3(10, -10, -5));
            Circle cl = new Circle(pl1, 20);

            // Act
            bool intersection0 = Intersect.PlaneCircle(pl0, cl, out _);
            bool intersection1 = Intersect.PlaneCircle(pl2, cl, out _);

            // Assert
            intersection0.Should().BeFalse();
            intersection1.Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Intersection_Points_Between_A_Plane_And_A_Circle()
        {
            // Arrange
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(10, 20, 5));
            Circle cl = new Circle(pl, 20);
            Plane plSec = new Plane(new Point3(10, 10, 10), new Point3(10, 20, 25));

            Point3[] intersectionChecks = new[]
            {
                new Point3( 10, 34.04646, -9.237168),
                new Point3( 10, 3.026711, 15.578632)
            };

            // Act
            _ = Intersect.PlaneCircle(plSec, cl, out Point3[] pts);

            // Assert
            for (int i = 0; i < intersectionChecks.Length; i++)
            {
                pts[i].EpsilonEquals(intersectionChecks[i], GSharkMath.MaxTolerance).Should().BeTrue();
            }
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_Two_Planar_Lines()
        {
            // Arrange
            int crvDegree0 = 1;
            var crvPts0 = new List<Point3> { new Point3(0, 0, 0), new Point3(2, 0, 0) };

            int crvDegree1 = 1;
            var crvPts1 = new List<Point3> { new Point3(0.5, 0.5, 0), new Point3(0.5, -1.5, 0) };

            NurbsCurve crv0 = new NurbsCurve(crvPts0, crvDegree0);
            NurbsCurve crv1 = new NurbsCurve(crvPts1, crvDegree1);

            // Act
            List<CurvesIntersectionResult> intersection = Intersect.CurveCurve(crv0, crv1, GSharkMath.MaxTolerance);

            // Assert
            intersection.Count.Should().Be(1);
            intersection[0].ParameterA.Should().BeApproximately(0.25, GSharkMath.MaxTolerance);
            intersection[0].ParameterB.Should().BeApproximately(0.25, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_A_Curve_And_Line_Planar()
        {
            // Arrange
            var p1 = new Point3(0.0, 0.0, 0.0);
            var p2 = new Point3(2.0, 0.0, 0.0);
            Line ln = new Line(p1, p2);

            int crvDegree1 = 2;
            var crvPts1 = new List<Point3> { new Point3(0.5, 0.5, 0), new Point3(0.7, 0, 0), new Point3(0.5, -1.5, 0) };
            NurbsCurve crv = new NurbsCurve(crvPts1, crvDegree1);

            // Act
            List<CurvesIntersectionResult> intersection = Intersect.CurveLine(crv, ln);

            // Assert
            _testOutput.WriteLine(intersection[0].ToString());
            intersection.Count.Should().Be(1);
            intersection[0].PointA.DistanceTo(intersection[0].PointB).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_A_Planar_Curves()
        {
            // Arrange
            int crvDegree0 = 2;
            var crvPts0 = new List<Point3> { new Point3(0, 0, 0), new Point3(0.5, 0.1, 0), new Point3(2, 0, 0) };

            int crvDegree1 = 2;
            var crvPts1 = new List<Point3> { new Point3(0.5, 0.5, 0), new Point3(0.7, 0, 0), new Point3(0.5, -1.5, 0) };

            NurbsCurve crv0 = new NurbsCurve(crvPts0, crvDegree0);
            NurbsCurve crv1 = new NurbsCurve(crvPts1, crvDegree1);

            // Act
            List<CurvesIntersectionResult> intersection = Intersect.CurveCurve(crv0, crv1);

            // Assert
            _testOutput.WriteLine(intersection[0].ToString());
            intersection.Count.Should().Be(1);
            intersection[0].PointA.DistanceTo(intersection[0].PointB).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Intersections_Between_A_Planar_Curves()
        {
            // Arrange
            int degree = 3;
            List<Point3> crvPts0 = new List<Point3>
            {
                new Point3( -5, 0, 0), new Point3( 10, 0, 0), new Point3( 10, 10, 0),
                new Point3(0, 10, 0) , new Point3(5, 5, 0)
            };

            List<Point3> crvPts1 = new List<Point3>
            {
                new Point3( -5, 0, 0), new Point3(5, -1, 0), new Point3(10, 5, 0),
                new Point3( 3, 10, 0), new Point3(5, 12, 0)
            };

            NurbsCurve crv0 = new NurbsCurve(crvPts0, degree);
            NurbsCurve crv1 = new NurbsCurve(crvPts1, degree);

            // Act
            List<CurvesIntersectionResult> intersections = Intersect.CurveCurve(crv0, crv1);

            // Assert
            intersections.Count.Should().Be(3);
            foreach (CurvesIntersectionResult intersection in intersections)
            {
                _testOutput.WriteLine(intersection.ToString());
                intersection.PointA.DistanceTo(intersection.PointB).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Returns_The_Intersections_Between_A_3D_Curves()
        {
            // Arrange
            int degree = 3;
            List<Point3> crvPts0 = new List<Point3>
            {
                new Point3( 0, 0, 0), new Point3 (5, 2.5, 5), new Point3 (5, 5, 0),
                new Point3( 7.5, 10, 5), new Point3 (10, 10, 0)
            };

            List<Point3> crvPts1 = new List<Point3>
            {
                new Point3( 2.225594, 1.226218, 2.01283), new Point3( 8.681402, 4.789645, 5.010206), new Point3(6.181402, 4.789645, 0.010206),
                new Point3( 1.181402, 7.289645, 5.010206), new Point3(8.496731, 9.656647, 2.348212)
            };

            NurbsCurve crv0 = new NurbsCurve(crvPts0, degree);
            NurbsCurve crv1 = new NurbsCurve(crvPts1, degree);

            // Act
            List<CurvesIntersectionResult> intersections = Intersect.CurveCurve(crv0, crv1);

            // Assert
            intersections.Count.Should().Be(3);
            foreach (CurvesIntersectionResult intersection in intersections)
            {
                _testOutput.WriteLine(intersection.ToString());
                intersection.PointA.DistanceTo(intersection.PointB).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }

        [Theory]
        [InlineData(0.25)]
        [InlineData(0.75)]
        [InlineData(1.0)]
        [InlineData(1.75)]
        public void It_Returns_The_Intersections_Between_A_Planar_Curve_And_A_Plane(double xValue)
        {
            // Arrange
            int crvDegree = 2;
            var crvCtrPts = new List<Point3> { new Point3(0, 0, 0), new Point3(0.5, 0.5, 0), new Point3(2, 0, 0) };
            NurbsCurve crv = new NurbsCurve(crvCtrPts, crvDegree);
            Plane pl = Plane.PlaneYZ.SetOrigin(new Point3(xValue, 0.0, 0.0));

            // Act
            List<CurvePlaneIntersectionResult> intersections = Intersect.CurvePlane(crv, pl);
            var ptOnPlane = pl.PointAt(intersections[0].Coordinate.U, intersections[0].Coordinate.V);

            // Assert
            _testOutput.WriteLine(intersections[0].ToString());
            intersections.Count.Should().Be(1);
            intersections[0].Point.DistanceTo(ptOnPlane).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Intersections_Between_A_Curve3D_And_A_Rotated_Plane()
        {
            // Arrange
            int degree = 3;
            List<Point3> crvCtrPts = new List<Point3>
            {
                new Point3( 2.225594, 1.226218, 2.01283), new Point3( 8.681402, 4.789645, 5.010206), new Point3(6.181402, 4.789645, 0.010206),
                new Point3( 1.181402, 7.289645, 5.010206), new Point3(8.496731, 9.656647, 2.348212)
            };
            NurbsCurve crv = new NurbsCurve(crvCtrPts, degree);

            Transform xForm = Transform.Rotation(0.15, new Point3(0.0, 0.0, 0.0));
            Plane pln = Plane.PlaneYZ;
            var testPln = pln.SetOrigin(new Point3(6, 0.0, 0.0));
            var testPlnXformed = testPln.Transform(xForm);

            // Act
            List<CurvePlaneIntersectionResult> intersections = Intersect.CurvePlane(crv, testPlnXformed);

            // Assert
            intersections.Count.Should().Be(3);
            foreach (CurvePlaneIntersectionResult curveIntersectionResult in intersections)
            {
                _testOutput.WriteLine(curveIntersectionResult.ToString());
                var ptOnPlane = testPlnXformed.PointAt(curveIntersectionResult.Coordinate.U, curveIntersectionResult.Coordinate.V);
                curveIntersectionResult.Point.DistanceTo(ptOnPlane).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Returns_The_SelfIntersections_Of_A_Curve3D()
        {
            // Arrange
            int degree = 3;
            List<Point3> crvCtrPts = new List<Point3>
            {
                new Point3(0,5,5), new Point3(2.5,0,0), new Point3(5,5,2.5),
                new Point3(2.5,5,5), new Point3(0,0,0)
            };
            NurbsCurve crv = new NurbsCurve(crvCtrPts, degree);

            // Act
            List<CurvesIntersectionResult> intersections = Intersect.CurveSelf(crv);
            var ptAt = crv.PointAt(intersections[0].ParameterB);

            // Assert
            _testOutput.WriteLine(intersections[0].ToString());
            intersections.Count.Should().Be(1);
            intersections[0].PointA.DistanceTo(ptAt).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_SelfIntersections_Of_A_Planar_Curve()
        {
            // Arrange
            int degree = 3;
            List<Point3> crvCtrPts = new List<Point3>
            {
                new Point3(0,5,0), new Point3(2.5,0,0), new Point3(5,2.5,0),
                new Point3(2.5,5,0), new Point3(0,0,0), new Point3(5,0,0), new Point3(2.5,5,0)
            };
            NurbsCurve crv = new NurbsCurve(crvCtrPts, degree);

            // Act
            List<CurvesIntersectionResult> intersections = Intersect.CurveSelf(crv);

            // Assert
            intersections.Count.Should().Be(3);
            foreach (CurvesIntersectionResult curveIntersectionResult in intersections)
            {
                _testOutput.WriteLine(curveIntersectionResult.ToString());
                var ptAt = crv.PointAt(curveIntersectionResult.ParameterB);
                curveIntersectionResult.PointA.DistanceTo(ptAt).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }
    }
}

