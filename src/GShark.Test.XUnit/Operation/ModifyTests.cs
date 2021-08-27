using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Transactions;
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

            NurbsCurve curve = new NurbsCurve(pts, degree);

            // Act
            NurbsCurve curveAfterRefine = Modify.CurveKnotRefine(curve, newKnots);

            // Assert
            (curve.Knots.Count + insertion).Should().Be(curveAfterRefine.Knots.Count);
            (pts.Count + insertion).Should().Be(curveAfterRefine.ControlPointLocations.Count);

            Point3 p0 = curve.PointAt(2.5);
            Point3 p1 = curveAfterRefine.PointAt(2.5);

            p0[0].Should().BeApproximately(p1[0], GSharkMath.MaxTolerance);
            p0[1].Should().BeApproximately(p1[1], GSharkMath.MaxTolerance);
            p0[2].Should().BeApproximately(p1[2], GSharkMath.MaxTolerance);
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
            List<NurbsCurve> curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            // Assert
            curvesAfterDecompose.Count.Should().Be(5);
            foreach (NurbsCurve bezierCurve in curvesAfterDecompose)
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
            List<NurbsCurve> curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            // Assert
            curvesAfterDecompose.Count.Should().Be(3);
            foreach (NurbsCurve bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Point3 pt0 = bezierCurve.PointAt(t);
                Point3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length;

                pt0_pt1.Should().BeApproximately(0.0, GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Reverses_The_Curve()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveCubicBezierPlanar();

            // Act
            NurbsCurve crvRev1 = Modify.ReverseCurve(curve);
            NurbsCurve crvRev2 = Modify.ReverseCurve(crvRev1);

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
            NurbsCurve elevatedDegreeCurve = Modify.ElevateDegree(curve, finalDegree);
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
            NurbsCurve elevatedDegreeCurve = Modify.ElevateDegree(curve, finalDegree);
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
            NurbsCurve reducedCurve = Modify.ReduceDegree(curve, tolerance);
            Point3 ptOnReducedDegreeCurve0 = reducedCurve.PointAt(0.5);
            Point3 ptOnReducedDegreeCurve1 = reducedCurve.PointAt(0.25);

            // Assert
            reducedCurve.Degree.Should().Be(degree - 1);

            ptOnCurve0.DistanceTo(ptOnReducedDegreeCurve0).Should().BeLessThan(GSharkMath.MinTolerance);
            ptOnCurve1.DistanceTo(ptOnReducedDegreeCurve1).Should().BeLessThan(tolerance);
        }

        [Fact]
        public void JoinCurve_Throw_An_Exception_If_The_Number_Of_Curves_Is_Insufficient()
        {
            // Arrange
            NurbsCurve[] curves = { NurbsCurveCollection.NurbsCurvePlanarExample() };

            // Act
            Func<object> func = () => Modify.JoinCurves(curves);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Fact]
        public void JoinCurve_Throw_An_Exception_If_Curves_Are_Close_Enough_To_Be_Joined()
        {
            // Arrange
            NurbsCurve[] curves = { NurbsCurveCollection.NurbsCurvePlanarExample(), NurbsCurveCollection.NurbsCurveQuadratic3DBezier() };

            // Act
            Func<object> func = () => Modify.JoinCurves(curves);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Fact]
        public void Returns_A_Curve_Joining_Different_Types_Of_Curves()
        {
            // Arrange
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

            NurbsCurve curve = new NurbsCurve(pts, degree);
            Line ln = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));
            Arc arc = Arc.ByStartEndDirection(new Point3(5, 5, -2.5), new Point3(10, 5, -7.5), new Vector3(0, 0, -1));
            NurbsCurve[] curves = { curve, ln.ToNurbs(), arc.ToNurbs() };

            Point3 expectedPt1 = new Point3(5, 3.042501, 4.519036);
            Point3 expectedPt2 = new Point3(5, 5, -1.230175);
            Point3 expectedPt3 = new Point3(7.075482, 5, -6.555514);

            // Act
            NurbsCurve joinedCurve = Modify.JoinCurves(curves);

            double t0 = joinedCurve.ParameterAtLength(15);
            double t1 = joinedCurve.ParameterAtLength(21.5);
            double t2 = joinedCurve.ParameterAtLength(27.5);

            Point3 pt1 = joinedCurve.PointAt(t0);
            Point3 pt2 = joinedCurve.PointAt(t1);
            Point3 pt3 = joinedCurve.PointAt(t2);

            // Arrange
            pt1.DistanceTo(expectedPt1).Should().BeLessThan(GSharkMath.MinTolerance);
            pt2.DistanceTo(expectedPt2).Should().BeLessThan(GSharkMath.MinTolerance);
            pt3.DistanceTo(expectedPt3).Should().BeLessThan(GSharkMath.MinTolerance);
        }

        [Fact]
        public void Returns_A_Curve_Joining_Polylines_And_Lines()
        {
            // Arrange
            var poly = new Polyline(new List<Point3>
            {
                new (0, 5, 5),
                new (0, 0, 0),
                new (5, 0, 0),
                new (5, 0, 5),
                new (5, 5, 5),
                new (5, 5, 0)
            });

            Line ln = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));

            Point3 expectedPt1 = new Point3(0, 2.0, 2.0);
            Point3 expectedPt2 = new Point3(2.5, 0, 0);
            Point3 expectedPt3 = new Point3(5, 5.0, 2.5);

            // Act
            NurbsCurve joinedCurve = Modify.JoinCurves(new List<NurbsCurve> { poly.ToNurbs(), ln.ToNurbs() });
            Point3 pt1 = joinedCurve.PointAt(0.1);
            Point3 pt2 = joinedCurve.PointAt(0.25);
            Point3 pt3 = joinedCurve.PointAt(0.75);

            // Arrange
            joinedCurve.Degree.Should().Be(1);
            pt1.Equals(expectedPt1).Should().BeTrue();
            pt2.Equals(expectedPt2).Should().BeTrue();
            pt3.Equals(expectedPt3).Should().BeTrue();
        }
    }
}

