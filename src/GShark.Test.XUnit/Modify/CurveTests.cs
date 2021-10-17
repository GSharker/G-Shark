using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Modify
{
    public class CurveTests
    {
        private readonly ITestOutputHelper _testOutput;

        public CurveTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(0.3, 1)]
        [InlineData(0.3, 2)]
        [InlineData(0.3, 3)]
        [InlineData(0.3, 4)]
        [InlineData(0.45, 1)]
        [InlineData(0.45, 2)]
        [InlineData(0.45, 3)]
        [InlineData(0.45, 4)]
        [InlineData(0.7, 1)]
        [InlineData(0.7, 2)]
        public void It_Refines_The_Curve_Knot(double val, int insertion)
        {
            // Arrange
            int degree = 3;

            List<double> newKnots = new List<double>();
            for (int i = 0; i < insertion; i++)
                newKnots.Add(val);

            List<Point3> pts = new List<Point3>();
            for (int i = 0; i <= 12 - degree - 2; i++)
                pts.Add(new Point3(i, 0.0, 0.0));

            NurbsCurve curve = new NurbsCurve(pts, degree);

            // Act
            NurbsBase curveAfterRefine = KnotVector.Refine(curve, newKnots);
            Point3 p0 = curve.PointAt(2.5);
            Point3 p1 = curveAfterRefine.PointAt(2.5);

            // Assert
            (curve.Knots.Count + insertion).Should().Be(curveAfterRefine.Knots.Count);
            (pts.Count + insertion).Should().Be(curveAfterRefine.ControlPointLocations.Count);
            (p0 == p1).Should().BeTrue();
        }

        [Fact]
        public void It_Decomposes_The_Curve_Of_Degree_Three_Into_Bezier_Curve_Segments()
        {
            // Arrange
            int degree = 3;

            List<Point3> pts = new List<Point3>();
            for (int i = 0; i <= 12 - degree - 2; i++)
                pts.Add(new Point3(i, 0.0, 0.0));

            NurbsCurve curve = new NurbsCurve(pts, degree);

            // Act
            List<NurbsBase> curvesAfterDecompose = curve.DecomposeIntoBeziers();

            // Assert
            curvesAfterDecompose.Count.Should().Be(5);
            foreach (NurbsBase bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Point3 pt0 = bezierCurve.PointAt(t);
                Point3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length;

                pt0_pt1.Should().BeApproximately(0.0, GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Decomposes_The_Curve_Of_Degree_Two_Into_Bezier_Curve_Segments()
        {
            // Arrange
            int degree = 2;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(4, 0, 0),
                new Point3(5, 5, 5),
                new Point3(0, 5, 0),
            };
            NurbsCurve curve = new NurbsCurve(controlPts, degree);

            // Act
            List<NurbsBase> curvesAfterDecompose = curve.DecomposeIntoBeziers();

            // Assert
            curvesAfterDecompose.Count.Should().Be(3);
            foreach (NurbsBase bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Point3 pt0 = bezierCurve.PointAt(t);
                Point3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length;

                pt0_pt1.Should().BeApproximately(0.0, GSharkMath.MaxTolerance);
            }
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public void It_Returns_A_Curve_Where_Degree_Is_Elevated_From_2_To_Elevated_Degree_Value(int finalDegree)
        {
            // Arrange
            List<Point3> pts = new List<Point3>
            {
                new Point3(5.2, 5.2, 5),
                new Point3(5.4, 4.8, 0),
                new Point3(5.2, 5.2, -5),
            };
            int degree = 2;
            NurbsCurve curve = new NurbsCurve(pts, degree);
            Point3 ptOnCurve = curve.PointAt(0.5);

            // Act
            NurbsBase elevatedDegreeCurve = curve.ElevateDegree(finalDegree);
            Point3 ptOnElevatedDegreeCurve = elevatedDegreeCurve.PointAt(0.5);

            // Assert
            elevatedDegreeCurve.Degree.Should().Be(finalDegree);
            ptOnElevatedDegreeCurve.DistanceTo(ptOnCurve).Should().BeLessThan(GSharkMath.MinTolerance);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void It_Returns_A_Curve_Where_Degree_Is_Elevated_From_1_To_Elevated_Degree_Value(int finalDegree)
        {
            // Arrange
            List<Point3> pts = new List<Point3>
            {
                new Point3(0.0, 0.0, 1.0),
                new Point3(7.0, 3.0, -10),
                new Point3(5.2, 5.2, -5),
            };
            int degree = 1;
            NurbsCurve curve = new NurbsCurve(pts, degree);
            Point3 ptOnCurve = curve.PointAt(0.5);

            // Act
            NurbsBase elevatedDegreeCurve = curve.ElevateDegree(finalDegree);
            Point3 ptOnElevatedDegreeCurve = elevatedDegreeCurve.PointAt(0.5);

            // Assert
            elevatedDegreeCurve.Degree.Should().Be(finalDegree);
            ptOnElevatedDegreeCurve.DistanceTo(ptOnCurve).Should().BeLessThan(GSharkMath.MinTolerance);
        }

        [Fact]
        public void It_Returns_A_Curve_Where_Degree_Is_Reduced_From_5_To_4()
        {
            // Arrange
            // Followed example Under C1 constrain condition https://www.hindawi.com/journals/mpe/2016/8140427/tab1/
            List<Point3> pts = new List<Point3>
            {
                new Point3(-5.0, 0.0, 0.0),
                new Point3(-7.0, 2.0, 0.0),
                new Point3(-3.0, 5.0, 0.0),
                new Point3(2.0, 6.0, 0.0),
                new Point3(5.0, 3.0, 0.0),
                new Point3(3.0, 0.0, 0.0)
            };
            int degree = 5;
            double tolerance = 10e-2;
            NurbsCurve curve = new NurbsCurve(pts, degree);
            Point3 ptOnCurve0 = curve.PointAt(0.5);
            Point3 ptOnCurve1 = curve.PointAt(0.25);

            // Act
            NurbsBase reducedCurve = curve.ReduceDegree(tolerance);
            Point3 ptOnReducedDegreeCurve0 = reducedCurve.PointAt(0.5);
            Point3 ptOnReducedDegreeCurve1 = reducedCurve.PointAt(0.25);

            // Assert
            reducedCurve.Degree.Should().Be(degree - 1);

            ptOnCurve0.DistanceTo(ptOnReducedDegreeCurve0).Should().BeLessThan(GSharkMath.MinTolerance);
            ptOnCurve1.DistanceTo(ptOnReducedDegreeCurve1).Should().BeLessThan(tolerance);
        }
    }
}

