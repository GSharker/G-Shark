using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using GeometrySharp.Test.XUnit.Data;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class AnalyzeTests
    {
        private readonly ITestOutputHelper _testOutput;

        public AnalyzeTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void RationalBezierCurveArcLength_Returns_The_Approximated_Length()
        {
            var degree = 3;
            var knots1 = new Knot() { 0, 0, 0, 0, 1, 1, 1, 1 };
            var knots2 = new Knot() { 1, 1, 1, 1, 4, 4, 4, 4 };
            var controlPts = new List<Vector3>()
            {
                new Vector3() {0, 0, 0},
                new Vector3() {0.5, 0, 0},
                new Vector3() {2.5, 0, 0},
                new Vector3() {3, 0, 0}
            };

            var curve1 = new NurbsCurve(degree, knots1, controlPts);
            var curve2 = new NurbsCurve(degree, knots2, controlPts);

            var curveLength1 = Analyze.RationalBezierCurveLength(curve1, 1);
            var curveLength2 = Analyze.RationalBezierCurveLength(curve2, 4);

            curveLength1.Should().BeApproximately(3.0, GeoSharpMath.MAXTOLERANCE);
            curveLength2.Should().BeApproximately(3.0, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void RationalBezierCurveParamAtLength_Returns_Parameters_At_Passed_Lengths()
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();
            // Values from Rhino.
            var tValuesExpected = new[] {0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1};

            var steps = 7;
            var length = curve.Length() / steps;
            var sumLengths = 0.0;

            for (int i = 0; i < steps + 1; i++)
            {
                var t = Analyze.RationalBezierCurveParamAtLength(curve, sumLengths, GeoSharpMath.MAXTOLERANCE);

                var segmentLength = Analyze.RationalBezierCurveLength(curve, t);

                t.Should().BeApproximately(tValuesExpected[i], GeoSharpMath.MINTOLERANCE);
                segmentLength.Should().BeApproximately(sumLengths, GeoSharpMath.MINTOLERANCE);

                sumLengths += length;
            }
        }

        // This value has been compared with Rhino.
        [Fact]
        public void It_Returns_The_Length_Of_The_Curve()
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();

            var crvLength = Analyze.RationalCurveArcLength(curve);
            var samples = Tessellation.RegularSample(curve, 10000);

            var length = 0.0;
            for (int j = 0; j < samples.pts.Count - 1; j++)
                length += (samples.pts[j + 1] - samples.pts[j]).Length();

            _testOutput.WriteLine(crvLength.ToString());
            _testOutput.WriteLine(length.ToString());

            crvLength.Should().BeApproximately(length, 1e-3);
        }

        // These values have been compared with Rhino.
        [Theory]
        [InlineData(new double[] { 5, 7, 0 }, new double[] { 5.982099, 5.950299, 0 }, 0.021824)]
        [InlineData(new double[] { 12, 10, 0 }, new double[] { 11.781824, 10.364244, 0 }, 0.150707)]
        [InlineData(new double[] { 21, 17, 0 }, new double[] { 21.5726, 14.101932, 0 }, 0.36828)]
        [InlineData(new double[] { 32, 15, 0 }, new double[] { 31.906562, 14.36387, 0 }, 0.597924)]
        [InlineData(new double[] { 41, 8, 0 }, new double[] { 42.554645, 10.750437, 0 }, 0.834548)]
        [InlineData(new double[] { 50, 5, 0 }, new double[] { 50, 5, 0 }, 1.0)]
        public void It_Returns_The_Closest_Point_And_The_Parameter_t(double[] ptToCheck, double[] ptExpected, double tValExpected)
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();
            var ptHomogenized = Analyze.RationalCurveClosestPoint(curve, ptToCheck.ToVector(), out var t);
            var pt = LinearAlgebra.PointDehomogenizer(ptHomogenized);

            _testOutput.WriteLine(pt.ToString());
            _testOutput.WriteLine(t.ToString());

            t.Should().BeApproximately(tValExpected, GeoSharpMath.MAXTOLERANCE);
            // https://stackoverflow.com/questions/36782975/fluent-assertions-approximately-compare-a-classes-properties
            pt.Should().BeEquivalentTo(ptExpected.ToVector(), options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, GeoSharpMath.MAXTOLERANCE))
                .WhenTypeIs<double>());
        }

        // These values have been compared with Rhino.
        [Theory]
        [InlineData(0, 0)]
        [InlineData(15, 0.278127)]
        [InlineData(33, 0.672164)]
        [InlineData(46, 0.928308)]
        [InlineData(50.334675, 1)]
        public void RationalCurveParameterAtLength_Returns_Parameter_t_At_The_Given_Length(double segmentLength, double tValueExpected)
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();

            var t = Analyze.RationalCurveParameterAtLength(curve, segmentLength);

            t.Should().BeApproximately(tValueExpected, 1e-5);
        }
    }
}
