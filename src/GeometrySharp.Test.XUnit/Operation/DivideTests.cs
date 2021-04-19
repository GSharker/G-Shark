using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using GeometrySharp.Test.XUnit.Data;
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
            var degree = 3;
            var controlPts = new List<Vector3>()
            {
                new Vector3(){2,2,0},
                new Vector3(){4,12,0},
                new Vector3(){7,12,0},
                new Vector3(){15,2,0}
            };
            var knots = new Knot(degree, controlPts.Count);

            var curve = new NurbsCurve(degree, knots, controlPts);
            var curves = Divide.SplitCurve(curve, parameter);

            for (int i = 0; i < degree + 1; i++)
            {
                var d = curves[0].Knots.Count - (degree + 1);
                curves[0].Knots[d + i].Should().BeApproximately(parameter, GeoSharpMath.MAXTOLERANCE);
            }

            for (int i = 0; i < degree + 1; i++)
            {
                var d = 0;
                curves[1].Knots[d + i].Should().BeApproximately(parameter, GeoSharpMath.MAXTOLERANCE);
            }

            curves.Should().HaveCount(2);

            _testOutput.WriteLine(curves[0].ToString());
            _testOutput.WriteLine(curves[1].ToString());
        }

        [Fact]
        public void RationalCurveByDivisions_Returns_The_Curve_Divided_By_A_Count()
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();
            // Values from Rhino.
            var tValuesExpected = new[] { 0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1 };
            var steps = 7;

            var divisions = Divide.CurveByCount(curve, steps);

            divisions.tValues.Count.Should().Be(divisions.lengths.Count).And.Be(steps + 1);
            for (int i = 0; i < steps; i++)
                divisions.tValues[i].Should().BeApproximately(tValuesExpected[i], GeoSharpMath.MINTOLERANCE);
        }

        [Fact]
        public void RationalCurveByEqualLength_Returns_The_Curve_Divided_By_A_Length()
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();
            // Values from Rhino.
            var tValuesExpected = new[] { 0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1 };

            var steps = 7;
            var length = curve.Length() / steps;

            var divisions = Divide.CurveByLength(curve, length);

            divisions.tValues.Count.Should().Be(divisions.lengths.Count).And.Be(steps + 1);
            for (int i = 0; i < steps; i++)
                divisions.tValues[i].Should().BeApproximately(tValuesExpected[i], GeoSharpMath.MINTOLERANCE);
        }
    }
}
