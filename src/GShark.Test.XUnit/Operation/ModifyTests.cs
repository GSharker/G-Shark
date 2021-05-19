using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
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
        [InlineData(2.5, 1)]
        [InlineData(2.5, 2)]
        [InlineData(2.5, 3)]
        [InlineData(2.5, 4)]
        [InlineData(0.5, 1)]
        [InlineData(0.5, 2)]
        [InlineData(0.5, 3)]
        [InlineData(0.5, 4)]
        [InlineData(3.0, 1)]
        [InlineData(3.0, 2)]
        public void It_Refines_The_Curve_Knot(double val, int insertion)
        {
            // Arrange
            int degree = 3;
            Knot knots = new Knot { 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 };

            List<double> newKnots = new List<double>();
            for (int i = 0; i < insertion; i++)
                newKnots.Add(val);

            List<Vector3> controlPts = new List<Vector3>();
            for (int i = 0; i <= knots.Count - degree - 2; i++)
                controlPts.Add(new Vector3 { i, 0.0, 0.0 });

            ICurve curve = new NurbsCurve(degree, knots, controlPts);

            // Act
            ICurve curveAfterRefine = Modify.CurveKnotRefine(curve, newKnots);

            // Assert
            (knots.Count + insertion).Should().Be(curveAfterRefine.Knots.Count);
            (controlPts.Count + insertion).Should().Be(curveAfterRefine.ControlPoints.Count);

            Vector3 p0 = curve.PointAt(2.5);
            Vector3 p1 = curveAfterRefine.PointAt(2.5);

            p0[0].Should().BeApproximately(p1[0], GeoSharpMath.MAXTOLERANCE);
            p0[1].Should().BeApproximately(p1[1], GeoSharpMath.MAXTOLERANCE);
            p0[2].Should().BeApproximately(p1[2], GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Decomposes_The_Curve_Of_Degree_Three_Into_Bezier_Curve_Segments()
        {
            // Arrange
            int degree = 3;
            Knot knots = new Knot { 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 };

            List<Vector3> controlPts = new List<Vector3>();
            for (int i = 0; i <= knots.Count - degree - 2; i++)
                controlPts.Add(new Vector3 { i, 0.0, 0.0 });

            ICurve curve = new NurbsCurve(degree, knots, controlPts);

            // Act
            List<ICurve> curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            // Assert
            curvesAfterDecompose.Count.Should().Be(5);
            foreach (ICurve bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Vector3 pt0 = bezierCurve.PointAt(t);
                Vector3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length();

                pt0_pt1.Should().BeApproximately(0.0, GeoSharpMath.MAXTOLERANCE);
            }
        }

        [Fact]
        public void It_Decomposes_The_Curve_Of_Degree_Two_Into_Bezier_Curve_Segments()
        {
            // Arrange
            int degree = 2;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {0, 5, 5},
                new Vector3 {0, 0, 0},
                new Vector3 {4, 0, 0},
                new Vector3 {5, 5, 5},
                new Vector3 {0, 5, 0},
            };
            NurbsCurve curve = new NurbsCurve(controlPts, degree);

            // Act
            List<ICurve> curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            // Assert
            curvesAfterDecompose.Count.Should().Be(3);
            foreach (ICurve bezierCurve in curvesAfterDecompose)
            {
                double t = bezierCurve.Knots[0];
                Vector3 pt0 = bezierCurve.PointAt(t);
                Vector3 pt1 = curve.PointAt(t);

                double pt0_pt1 = (pt0 - pt1).Length();

                pt0_pt1.Should().BeApproximately(0.0, GeoSharpMath.MAXTOLERANCE);
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

            Vector3 pt0 = curve.PointAt(0.0);
            Vector3 pt1 = crvRev1.PointAt(1.0);

            // Assert
            pt0.Should().BeEquivalentTo(pt1);
            curve.Equals(crvRev2).Should().BeTrue();
            // Checks at reference level are different.
            curve.Should().NotBeSameAs(crvRev2);
        }
    }
}
