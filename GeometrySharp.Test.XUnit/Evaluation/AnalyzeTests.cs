using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Test.XUnit.Data;
using verb.eval;
using Xunit;
using Xunit.Abstractions;
using Analyze = GeometrySharp.Evaluation.Analyze;

namespace GeometrySharp.Test.XUnit.Evaluation
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
    }
}