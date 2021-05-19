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
    }
}
