using System.Collections.Generic;
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
        [InlineData(new double[] { 5, 7, 0 }, 0.021824)]
        [InlineData(new double[] { 12, 10, 0 }, 0.150707)]
        [InlineData(new double[] { 22, 17, 0 }, 0.387993)]
        [InlineData(new double[] { 32, 15, 0 }, 0.597924)]
        [InlineData(new double[] { 41, 8, 0 }, 0.834548)]
        [InlineData(new double[] { 50, 5, 0 }, 1.0)]
        public void It_Returns_The_T_Parameter_Of_The_Closest_Point(double[] ptToCheck, double tValExpected)
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();
            var tVal = Analyze.RationalCurveClosestParameter(curve, ptToCheck.ToVector());

            tVal.Should().BeApproximately(tValExpected, GeoSharpMath.MAXTOLERANCE);
        }
    }
}