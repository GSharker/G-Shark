using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class TessellationTests
    {
        private readonly ITestOutputHelper _testOutput;

        public TessellationTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void RegularSample_Returns_Points_Equal_The_Number_Of_Samples_Required()
        {
            // Arrange
            int degree = 2;
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 1, 1 };
            List<double> weights1 = new List<double> { 1, 1, 1 };
            List<double> weights2 = new List<double> { 1, 1, 2 };
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(1, 0, 0),
                new Point3(1, 1, 0),
                new Point3(0, 2, 0)
            };

            NurbsCurve curve1 = new NurbsCurve(degree, knots, controlPts, weights1);
            NurbsCurve curve2 = new NurbsCurve(degree, knots, controlPts, weights2);

            // Act
            (List<double> tvalues, List<Point3> pts) curveLength1 = Tessellation.CurveRegularSample(curve1, 10);
            (List<double> tvalues, List<Point3> pts) curveLength2 = Tessellation.CurveRegularSample(curve2, 10);

            // Assert
            for (int i = 0; i < curveLength1.pts.Count; i++)
            {
                _testOutput.WriteLine($"tVal -> {curveLength1.tvalues[i]} - Pts -> {curveLength1.pts[i]}");
                _testOutput.WriteLine($"tVal -> {curveLength2.tvalues[i]} - Pts -> {curveLength2.pts[i]}");
            }

            curveLength1.pts.Count.Should().Be(curveLength2.pts.Count).And.Be(10);
            curveLength1.tvalues.Count.Should().Be(curveLength2.tvalues.Count).And.Be(10);
        }

        [Fact]
        public void Return_Adaptive_Sample_Subdivision_Of_A_Nurbs()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveQuadratic3DBezier();

            // Act
            (List<double> tValues, List<Point3> pts) result0 = Tessellation.CurveAdaptiveSample(curve, 1.0);
            (List<double> tValues, List<Point3> pts) result1 = Tessellation.CurveAdaptiveSample(curve, 0.01);

            // Arrange
            result0.Should().NotBeNull();
            result1.Should().NotBeNull();
            result0.pts.Count.Should().BeLessThan(result1.pts.Count);
            result0.tValues[0].Should().Be(result1.tValues[0]).And.Be(0.0);
            result0.tValues[result0.tValues.Count - 1].Should().Be(result1.tValues[result1.tValues.Count - 1]).And.Be(1.0);

            double prev = double.MinValue;
            foreach (var t in result1.tValues)
            {
                t.Should().BeGreaterThan(prev);
                t.Should().BeInRange(0.0, 1.0);
                prev = t;
            }
        }

        [Fact]
        public void AdaptiveSample_Returns_The_ControlPoints_If_Curve_Has_Grade_One()
        {
            // Arrange
            List<Point3> controlPts = NurbsCurveCollection.NurbsCurvePlanarExample().LocationPoints;
            NurbsCurve curve = new NurbsCurve(controlPts, 1);

            // Act
            (List<double> tValues, List<Point3> pts) = Tessellation.CurveAdaptiveSample(curve, 0.1);

            // Assert
            tValues.Count.Should().Be(pts.Count).And.Be(6);
            pts.Should().BeEquivalentTo(controlPts);
        }

        [Fact]
        public void Return_Adaptive_Sample_Subdivision_Of_A_Line()
        {
            // Arrange
            Point3 p1 = new Point3(0, 0, 0);
            Point3 p2 = new Point3(10, 0, 0);
            Line ln = new Line(p1, p2);

            // Act
            (List<double> tValues, List<Point3> pts) result = Tessellation.CurveAdaptiveSample(ln);

            // Arrange
            result.pts.Count.Should().Be(result.tValues.Count).And.Be(2);
            result.pts[0].DistanceTo(p1).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            result.pts[1].DistanceTo(p2).Should().BeLessThan(GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void Return_Adaptive_Sample_Subdivision_Of_A_Polyline()
        {
            // Arrange
            var p1 = new Point3( 0, 0, 0);
            var p2 = new Point3( 10, 10, 0);
            var p3 = new Point3( 14, 20, 0);
            var p4 = new Point3( 10, 32, 4);
            var p5 = new Point3( 12, 16, 22);
            List<Point3> pts = new List<Point3> { p1, p2, p3, p4, p5 };
            Polyline poly = new Polyline(pts);

            // Act
            (List<double> tValues, List<Point3> pts) result = Tessellation.CurveAdaptiveSample(poly);

            // Arrange
            result.pts.Count.Should().Be(result.tValues.Count).And.Be(5);
            result.pts[0].DistanceTo(p1).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            result.pts[result.pts.Count - 1].DistanceTo(p5).Should().BeLessThan(GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void AdaptiveSample_Use_MaxTolerance_If_Tolerance_Is_Set_Less_Or_Equal_To_Zero()
        {
            // Act
            (List<double> tValues, List<Point3> pts) = Tessellation.CurveAdaptiveSample(NurbsCurveCollection.NurbsCurvePlanarExample(), 0.0);

            // Assert
            tValues.Should().NotBeEmpty();
            pts.Should().NotBeEmpty();
        }
    }
}
