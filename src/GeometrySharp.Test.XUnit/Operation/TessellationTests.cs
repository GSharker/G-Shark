using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using GeometrySharp.Test.XUnit.Data;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
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
            Knot knots = new Knot { 0, 0, 0, 1, 1, 1 };
            List<double> weights1 = new List<double> { 1, 1, 1 };
            List<double> weights2 = new List<double> { 1, 1, 2 };
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {1, 0, 0},
                new Vector3 {1, 1, 0},
                new Vector3 {0, 2, 0}
            };

            NurbsCurve curve1 = new NurbsCurve(degree, knots, controlPts, weights1);
            NurbsCurve curve2 = new NurbsCurve(degree, knots, controlPts, weights2);

            // Act
            (List<double> tvalues, List<Vector3> pts) curveLength1 = Tessellation.RegularSample(curve1, 10);
            (List<double> tvalues, List<Vector3> pts) curveLength2 = Tessellation.RegularSample(curve2, 10);

            // Assert
            for (int i = 0; i < curveLength1.pts.Count; i++)
            {
                _testOutput.WriteLine($"tVal -> {curveLength1.tvalues[i]} - Pts -> {curveLength1.pts[i]}");
                _testOutput.WriteLine($"tVal -> {curveLength2.tvalues[i]} - Pts -> {curveLength2.pts[i]}");
            }

            curveLength1.pts.Count.Should().Be(curveLength2.pts.Count).And.Be(10);
            curveLength1.tvalues.Count.Should().Be(curveLength2.tvalues.Count).And.Be(10);
            curveLength1.pts.Select((pt, i) => pt.Count.Should().Be(curveLength2.pts[i].Count).And.Be(3));
        }

        [Fact]
        public void AdaptiveSample_Returns_Points_Sampling_The_Domain_With_Respect_Local_Curvature()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();

            // Act
            (List<double> tValues, List<Vector3> pts) adaptiveSample = Tessellation.AdaptiveSample(curve, 0.1);

            // Assert
            _testOutput.WriteLine($"{adaptiveSample.pts.Count}");

            for (int i = 0; i < adaptiveSample.pts.Count; i++)
            {
                _testOutput.WriteLine($"tVal -> {adaptiveSample.tValues[i]} - Pts -> {adaptiveSample.pts[i]}");
            }

            adaptiveSample.tValues.Count.Should().Be(adaptiveSample.pts.Count).And.Be(17);
            adaptiveSample.pts[0].Should().BeEquivalentTo(curve.ControlPoints[0]);
            adaptiveSample.pts[^1].Should().BeEquivalentTo(curve.ControlPoints[^1]);
        }

        [Fact]
        public void AdaptiveSample_Returns_The_ControlPoints_If_Curve_Has_Grade_One()
        {
            // Arrange
            List<Vector3> controlPts = NurbsCurveCollection.NurbsCurvePlanarExample().ControlPoints;
            NurbsCurve curve = new NurbsCurve(controlPts, 1);

            // Act
            (List<double> tValues, List<Vector3> pts) = Tessellation.AdaptiveSample(curve, 0.1);

            // Assert
            tValues.Count.Should().Be(pts.Count).And.Be(6);
            pts.Select((pt, i) => pt.Should().BeEquivalentTo(controlPts[i]));
        }

        [Fact]
        public void AdaptiveSample_Use_MaxTolerance_If_Tolerance_Is_Set_Less_Or_Equal_To_Zero()
        {
            // Act
            (List<double> tValues, List<Vector3> pts) = Tessellation.AdaptiveSample(NurbsCurveCollection.NurbsCurvePlanarExample(), 0.0);

            // Assert
            tValues.Should().NotBeEmpty();
            pts.Should().NotBeEmpty();
        }
    }
}
