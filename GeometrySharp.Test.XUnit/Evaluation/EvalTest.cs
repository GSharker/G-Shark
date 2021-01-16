using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Evaluation
{
    [Trait("Category", "Eval")]
    public class EvalTest
    {
        private readonly ITestOutputHelper _testOutput;

        public EvalTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Tests_A_Basic_Function()
        {
            var degree = 2;
            var span = 4;
            var knots = new Knot() {0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5};

            var result1 = Eval.BasicFunction(degree, knots, span, 2.5);
            var result2 = Eval.BasicFunction(degree, knots,2.5);

            result1.Should().BeEquivalentTo(result2);
            result1.Count.Should().Be(3);
            result1[0].Should().Be(0.125);
            result1[1].Should().Be(0.75);
            result1[2].Should().Be(0.125);
        }

        [Theory]
        [InlineData(0.0, new double[] {5.0,5.0,0.0})]
        [InlineData(0.3, new double[] { 18.617, 13.377, 0.0 })]
        [InlineData(0.5, new double[] { 27.645, 14.691, 0.0 })]
        [InlineData(0.6, new double[] { 32.143, 14.328, 0.0 })]
        [InlineData(1.0, new double[] { 50.0, 5.0, 0.0 })]
        public void It_Returns_A_Point_On_A_Curve_At_A_Given_Parameter(double parameter, double[] result)
        {
            var knots = new Knot() { 0.0, 0.0, 0.0, 0.0, 0.33, 0.66, 1.0, 1.0, 1.0, 1.0 };
            var degree = 3;
            var controlPts = new List<Vector3>()
            {
                new Vector3() {5,5,0},
                new Vector3() {10, 10, 0},
                new Vector3() {20, 15, 0},
                new Vector3() {35, 15, 0},
                new Vector3() {45, 10, 0},
                new Vector3() {50, 5, 0}
            };
            var curve = new NurbsCurve(degree, knots, controlPts);

            var pt = Eval.CurvePointAt(curve, parameter);

            pt[0].Should().BeApproximately(result[0], 0.001);
            pt[1].Should().BeApproximately(result[1], 0.001);
        }

        [Fact]
        public void It_Returns_A_Derive_Basic_Function_Given_Two_Parameters()
        {
            // Arrange
            // Values and formulas from The Nurbs Book p.69 & p.72
            var degree = 2;
            var span = 4;
            var order = 2;
            var parameter = 2.5;
            var knots = new Knot(){ 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
            var expectedResult = new double[,] {{0.125, 0.75, 0.125}, {-0.5, 0.0, 0.5}, {1.0, -2.0, 1.0}};

            // Act
            var resultToCheck = Eval.DerivativeBasisFunctionsGivenTwoN(span, parameter, degree, order, knots);

            // Assert
            resultToCheck[0][0].Should().BeApproximately(expectedResult[0, 0], GeoSharpMath.TOLERANCE);
            resultToCheck[0][1].Should().BeApproximately(expectedResult[0, 1], GeoSharpMath.TOLERANCE);
            resultToCheck[0][2].Should().BeApproximately(expectedResult[0, 2], GeoSharpMath.TOLERANCE);

            resultToCheck[1][0].Should().BeApproximately(expectedResult[1, 0], GeoSharpMath.TOLERANCE);
            resultToCheck[1][1].Should().BeApproximately(expectedResult[1, 1], GeoSharpMath.TOLERANCE);
            resultToCheck[1][2].Should().BeApproximately(expectedResult[1, 2], GeoSharpMath.TOLERANCE);

            resultToCheck[2][0].Should().BeApproximately(expectedResult[2, 0], GeoSharpMath.TOLERANCE);
            resultToCheck[2][1].Should().BeApproximately(expectedResult[2, 1], GeoSharpMath.TOLERANCE);
            resultToCheck[2][2].Should().BeApproximately(expectedResult[2, 2], GeoSharpMath.TOLERANCE);

            resultToCheck.Count.Should().Be(order + 1);
            resultToCheck[0].Count.Should().Be(degree + 1);
        }

        [Fact]
        public void It_Returns_The_Result_Of_A_Curve_Derivatives()
        {
            var degree = 3;
            var parameter = 0;
            var knots = new Knot() { 0, 0, 0, 0, 1, 1, 1, 1 };
            var numberDerivs = 2;
            var controlPts = new List<Vector3>()
            {
                new Vector3() {10, 0, 0},
                new Vector3() {20, 10, 0},
                new Vector3() {30, 20, 0},
                new Vector3() {50, 50, 0}
            };

            var curve = new NurbsCurve(degree, knots, controlPts);

            var p = Eval.CurveDerivatives(curve, parameter, numberDerivs);

            p[0][0].Should().Be(10);
            p[0][1].Should().Be(0);
            (p[1][0] / p[1][1]).Should().Be(1);
        }
    }
}
