using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;
using GeometrySharp.Operation;
using GeometrySharp.Test.XUnit.Data;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class DivideTests
    {
        private readonly ITestOutputHelper _testOutput;

        public DivideTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(0.25)]
        [InlineData(0.5)]
        [InlineData(0.75)]
        public void It_Returns_Two_Curves_Splitting_One_Curve(double parameter)
        {
            // Arrange
            int degree = 3;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {2,2,0},
                new Vector3 {4,12,0},
                new Vector3 {7,12,0},
                new Vector3 {15,2,0}
            };
            Knot knots = new Knot(degree, controlPts.Count);
            NurbsCurve curve = new NurbsCurve(degree, knots, controlPts);

            // Act
            List<ICurve> curves = Divide.SplitCurve(curve, parameter);

            // Assert
            curves.Should().HaveCount(2);

            for (int i = 0; i < degree + 1; i++)
            {
                int d = curves[0].Knots.Count - (degree + 1);
                curves[0].Knots[d + i].Should().BeApproximately(parameter, GeoSharpMath.MAXTOLERANCE);
            }

            for (int i = 0; i < degree + 1; i++)
            {
                int d = 0;
                curves[1].Knots[d + i].Should().BeApproximately(parameter, GeoSharpMath.MAXTOLERANCE);
            }
        }

        [Fact]
        public void RationalCurveByDivisions_Returns_The_Curve_Divided_By_A_Count()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            double[] tValuesExpected = new[] { 0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1 };
            int steps = 7;

            // Act
            (List<double> tValues, List<double> lengths) divisions = Divide.CurveByCount(curve, steps);

            // Assert
            divisions.tValues.Count.Should().Be(divisions.lengths.Count).And.Be(steps + 1);
            for (int i = 0; i < steps; i++)
                divisions.tValues[i].Should().BeApproximately(tValuesExpected[i], GeoSharpMath.MINTOLERANCE);
        }

        [Fact]
        public void RationalCurveByEqualLength_Returns_The_Curve_Divided_By_A_Length()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            double[] tValuesExpected = new[] { 0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1 };

            int steps = 7;
            double length = curve.Length() / steps;

            // Act
            (List<double> tValues, List<double> lengths) divisions = Divide.CurveByLength(curve, length);

            // Assert
            divisions.tValues.Count.Should().Be(divisions.lengths.Count).And.Be(steps + 1);
            for (int i = 0; i < steps; i++)
                divisions.tValues[i].Should().BeApproximately(tValuesExpected[i], GeoSharpMath.MINTOLERANCE);
        }

        [Fact]
        public void Return_Adaptive_Sample_Subdivision_Of_A_Line()
        {
            // Arrange
            Vector3 p1 = new Vector3 { 0, 0, 0 };
            Vector3 p2 = new Vector3 { 10, 0, 0 };
            Line ln = new Line(p1, p2);

            // Act
            (List<double> tValues, List<Vector3> pts) result = Divide.CurveAdaptiveSample(ln);

            // Arrange
            result.pts.Count.Should().Be(result.tValues.Count).And.Be(2);
            result.pts[0].DistanceTo(p1).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            result.pts[1].DistanceTo(p2).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void Return_Adaptive_Sample_Subdivision_Of_A_Polyline()
        {
            // Arrange
            Vector3 p1 = new Vector3 { 0, 0, 0 };
            Vector3 p2 = new Vector3 { 10, 10, 0 };
            Vector3 p3 = new Vector3 { 14, 20, 0 };
            Vector3 p4 = new Vector3 { 10, 32, 4 };
            Vector3 p5 = new Vector3 { 12, 16, 22 };
            List<Vector3> pts = new List<Vector3> { p1, p2, p3, p4, p5 };
            Polyline poly = new Polyline(pts);

            // Act
            (List<double> tValues, List<Vector3> pts) result = Divide.CurveAdaptiveSample(poly);

            // Arrange
            result.pts.Count.Should().Be(result.tValues.Count).And.Be(5);
            result.pts[0].DistanceTo(p1).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            result.pts[^1].DistanceTo(p5).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void Return_Adaptive_Sample_Subdivision_Of_A_Nurbs()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveQuadratic3DBezier();

            // Act
            (List<double> tValues, List<Vector3> pts) result0 = Divide.CurveAdaptiveSample(curve, 1.0);
            (List<double> tValues, List<Vector3> pts) result1 = Divide.CurveAdaptiveSample(curve, 0.01);

            // Arrange
            result0.Should().NotBeNull();
            result1.Should().NotBeNull();
            result0.pts.Count.Should().BeLessThan(result1.pts.Count);
            result0.tValues[0].Should().Be(result1.tValues[0]).And.Be(0.0);
            result0.tValues[^1].Should().Be(result1.tValues[^1]).And.Be(1.0);

            double prev = double.MinValue;
            foreach (var t in result1.tValues)
            {
                t.Should().BeGreaterThan(prev);
                t.Should().BeInRange(0.0, 1.0);
                prev = t;
            }
        }
    }
}
