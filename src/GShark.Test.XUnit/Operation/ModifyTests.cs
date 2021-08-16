using System;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class ModifyTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ModifyTests(ITestOutputHelper testOutput)
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

            ICurve curve = new NurbsCurve(pts, degree);

            // Act
            ICurve curveAfterRefine = Modify.CurveKnotRefine(curve, newKnots);

            // Assert
            (curve.Knots.Count + insertion).Should().Be(curveAfterRefine.Knots.Count);
            (pts.Count + insertion).Should().Be(curveAfterRefine.LocationPoints.Count);

            Point3 p0 = curve.PointAt(2.5);
            Point3 p1 = curveAfterRefine.PointAt(2.5);

            p0[0].Should().BeApproximately(p1[0], GeoSharkMath.MaxTolerance);
            p0[1].Should().BeApproximately(p1[1], GeoSharkMath.MaxTolerance);
            p0[2].Should().BeApproximately(p1[2], GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Decomposes_The_Curve_Of_Degree_Three_Into_Bezier_Curve_Segments()
        {
            // Arrange
            int degree = 3;

            List<Point3> pts = new List<Point3>();
            for (int i = 0; i <= 12 - degree - 2; i++)
                pts.Add(new Point3(i, 0.0, 0.0));

            ICurve curve = new NurbsCurve(pts, degree);

            // Act
            List<ICurve> curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            // Assert
            curvesAfterDecompose.Count.Should().Be(5);
            foreach (ICurve bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Point3 pt0 = bezierCurve.PointAt(t);
                Point3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length;

                pt0_pt1.Should().BeApproximately(0.0, GeoSharkMath.MaxTolerance);
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
            List<ICurve> curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            // Assert
            curvesAfterDecompose.Count.Should().Be(3);
            foreach (ICurve bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Point3 pt0 = bezierCurve.PointAt(t);
                Point3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length;

                pt0_pt1.Should().BeApproximately(0.0, GeoSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Reverses_The_Curve()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveCubicBezierPlanar();

            // Act
            ICurve crvRev1 = Modify.ReverseCurve(curve);
            ICurve crvRev2 = Modify.ReverseCurve(crvRev1);

            Point3 pt0 = curve.PointAt(0.0);
            Point3 pt1 = crvRev1.PointAt(1.0);

            // Assert
            pt0.Should().BeEquivalentTo(pt1);
            curve.Equals(crvRev2).Should().BeTrue();
            // Checks at reference level are different.
            curve.Should().NotBeSameAs(crvRev2);
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
            ICurve elevatedDegreeCurve = Modify.ElevateDegree(curve, finalDegree);
            Point3 ptOnElevatedDegreeCurve = elevatedDegreeCurve.PointAt(0.5);

            // Assert
            elevatedDegreeCurve.Degree.Should().Be(finalDegree);
            ptOnElevatedDegreeCurve.DistanceTo(ptOnCurve).Should().BeLessThan(GeoSharkMath.MinTolerance);
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
            ICurve elevatedDegreeCurve = Modify.ElevateDegree(curve, finalDegree);
            Point3 ptOnElevatedDegreeCurve = elevatedDegreeCurve.PointAt(0.5);

            // Assert
            elevatedDegreeCurve.Degree.Should().Be(finalDegree);
            ptOnElevatedDegreeCurve.DistanceTo(ptOnCurve).Should().BeLessThan(GeoSharkMath.MinTolerance);
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
            ICurve reducedCurve = Modify.ReduceDegree(curve, tolerance);
            Point3 ptOnReducedDegreeCurve0 = reducedCurve.PointAt(0.5);
            Point3 ptOnReducedDegreeCurve1 = reducedCurve.PointAt(0.25);

            // Assert
            reducedCurve.Degree.Should().Be(degree - 1);

            ptOnCurve0.DistanceTo(ptOnReducedDegreeCurve0).Should().BeLessThan(GeoSharkMath.MinTolerance);
            ptOnCurve1.DistanceTo(ptOnReducedDegreeCurve1).Should().BeLessThan(tolerance);
        }

        [Fact]
        public void JoinCurve_Throw_An_Exception_If_The_Number_Of_Curves_Is_Insufficient()
        {
            // Arrange
            ICurve[] curves = {NurbsCurveCollection.NurbsCurvePlanarExample()};

            // Act
            Func<object> func = () => Modify.JoinCurve(curves);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Fact]
        public void JoinCurve_Throw_An_Exception_If_Curves_Are_Close_Enough_To_Be_Joined()
        {
            // Arrange
            ICurve[] curves = { NurbsCurveCollection.NurbsCurvePlanarExample(), NurbsCurveCollection.NurbsCurveQuadratic3DBezier() };

            // Act
            Func<object> func = () => Modify.JoinCurve(curves);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Fact]
        public void Returns_A_Curve_Joining_Multiple_Curves()
        {
            // Arrange
            int degree = 3;
            List<Point3> pts0 = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(5, 0, 0),
                new Point3(5, 0, 5),
                new Point3(5, 5, 5),
                new Point3(5, 5, 0)
            };

            List<Point3> pts1 = new List<Point3>
            {
                new Point3(5, 5, -2.5),
                new Point3(5, 5, -7.5),
                new Point3(10, 5, -7.5)
            };

            NurbsCurve curve0 = new NurbsCurve(pts0, degree);
            Line ln = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));
            NurbsCurve curve1 = new NurbsCurve(pts1, 2);
            ICurve[] curves = {curve0, ln, curve1};

            // Act
            var joinedCurve = Modify.JoinCurve(curves);

            // Arrange

        }
    }
}
