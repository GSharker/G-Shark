using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using GeometrySharp.Test.XUnit.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Evaluation
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
            var degree = 2;
            var knots = new Knot() { 0, 0, 0, 1, 1, 1};
            var weights1 = new List<double>(){ 1, 1, 1 };
            var weights2 = new List<double>() { 1, 1, 2 };
            var controlPts = new List<Vector3>()
            {
                new Vector3() {1, 0, 0},
                new Vector3() {1, 1, 0},
                new Vector3() {0, 2, 0}
            };

            var curve1 = new NurbsCurve(degree, knots, controlPts, weights1);
            var curve2 = new NurbsCurve(degree, knots, controlPts, weights2);

            var curveLength1 = Tessellation.RegularSample(curve1, 10);
            var curveLength2 = Tessellation.RegularSample(curve2, 10);

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
            var curve = NurbsCurveTests.NurbsCurveExample2();

            var adaptiveSample = Tessellation.AdaptiveSample(curve, 0.1);

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
            var controlPts = NurbsCurveTests.NurbsCurveExample2().ControlPoints;
            var curve = new NurbsCurve(controlPts, 1);

            var (tValues, pts) = Tessellation.AdaptiveSample(curve, 0.1);

            tValues.Count.Should().Be(pts.Count).And.Be(6);
            pts.Select((pt, i) => pt.Should().BeEquivalentTo(controlPts[i]));
        }
    }
}
