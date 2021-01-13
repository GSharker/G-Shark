using System;
using FluentAssertions;
using GeometrySharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Core
{
    [Trait("Category", "Knot")]
    public class KnotTest
    {
        [Theory]
        [InlineData(0,12)]
        [InlineData(4,0)]
        [InlineData(-1, 0)]
        public void Knot_Throws_An_Exception_If_Degree_Or_Number_Of_ControlPts_Are_Less_Than_Or_Zero(int degree, int numberOfControlPts)
        {
            Func<Knot> funcResult = () => new Knot(degree, numberOfControlPts);

            funcResult.Should().Throw<Exception>().WithMessage("Input values must be positive and different than zero.");
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Knot()
        {
            var degree = 4;
            var ctrlPts = 12;
            var resultExpected = new Knot(){ 0.0, 0.0, 0.0, 0.0, 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0, 1.0, 1.0, 1.0, 1.0};

            var generateKnots = new Knot(degree, ctrlPts);

            generateKnots.Should().BeEquivalentTo(resultExpected);
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Knot_Unclamped()
        {
            var degree = 3;
            var ctrlPts = 5;
            var resultExpected = new Knot() { 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0 };

            var generateKnots = new Knot(degree, ctrlPts, false);

            generateKnots.Should().BeEquivalentTo(resultExpected);
        }

        [Theory]
        [InlineData(new double[] {0, 0, 1, 2, 3, 4, 4}, 4, 12, false)]
        [InlineData(new double[] {5, 3, 6, 5, 4, 5, 6}, 3, 3, false)]
        [InlineData(new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0, 1.0, 1.0, 1.0, 1.0 }, 4, 12, true)]
        public void It_Checks_Are_Valid_Knots(double[] knots, int degree, int ctrlPts, bool expectedResult)
        {
            var knotsArray = new Knot(knots);

            knotsArray.AreValid(degree, ctrlPts).Should().Be(expectedResult);
        }

        [Fact]
        public void It_Normalizes_Knot()
        {
            var knots = new Knot(){ -5, -5, -3, -2, 2, 3, 5, 5 };
            var knotsExpected = new Knot(){ 0.0, 0.0, 0.2, 0.3, 0.7, 0.8, 1.0, 1.0 };

            knots.Normalize();

            knots.Should().BeEquivalentTo(knotsExpected);
        }

        [Theory]
        [InlineData(4, 2.5)]
        [InlineData(3, 1)]
        [InlineData(3, 1.5)]
        [InlineData(7, 4.9)]
        [InlineData(7, 10)]
        [InlineData(7, 5)]
        [InlineData(2, 0)]
        [InlineData(2, -1)]
        public void It_Returns_The_KnotSpan_Given_A_Parameter(int expectedValue, double parameter)
        {
            var knotVector = new Knot() { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
            var degree = 2;

            var result = Knot.Span(knotVector.Count - degree - 2, 2, parameter, knotVector);

            result.Should().Be(expectedValue);
        }
    }
}
