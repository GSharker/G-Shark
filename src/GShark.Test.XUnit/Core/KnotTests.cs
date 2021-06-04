using System;
using FluentAssertions;
using GShark.Core;
using Xunit;

namespace GShark.Test.XUnit.Core
{
    public class KnotTests
    {
        [Theory]
        [InlineData(0,12)]
        [InlineData(4,0)]
        [InlineData(-1, 0)]
        public void Knot_Throws_An_Exception_If_Degree_Or_Number_Of_ControlPts_Are_Less_Than_Or_Zero(int degree, int numberOfControlPts)
        {
            Func<KnotVector> funcResult = () => new KnotVector(degree, numberOfControlPts);

            funcResult.Should().Throw<Exception>().WithMessage("Input values must be positive and different than zero.");
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Knot()
        {
            var degree = 4;
            var ctrlPts = 12;
            var resultExpected = new KnotVector(){ 0.0, 0.0, 0.0, 0.0, 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0, 1.0, 1.0, 1.0, 1.0};

            var generateKnots = new KnotVector(degree, ctrlPts);

            generateKnots.Should().BeEquivalentTo(resultExpected);
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Knot_Unclamped()
        {
            var degree = 3;
            var ctrlPts = 5;
            var resultExpected = new KnotVector() { 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0 };

            var generateKnots = new KnotVector(degree, ctrlPts, false);

            generateKnots.Should().BeEquivalentTo(resultExpected);
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0, 1, 1, 1 }, 2, 3, true)]
        [InlineData(new double[] { 0, 0, 0, 0.5, 1, 1, 1 }, 2, 4, true)]
        [InlineData(new double[] { 0, 0, 1, 1, 1 }, 2, 2, false)]
        [InlineData(new double[] { 0, 0, 0.5, 1, 1, 1 }, 2, 4, false)]
        [InlineData(new double[] { 0, 0, 0, 1, 1, 2 }, 2, 3, false)]
        [InlineData(new double[] { 0, 0, 0, 0.5, 1, 1, 2 }, 2, 4, false)]
        [InlineData(new double[] { 0, 0, 0, 0.5, 0.25, 1, 1, 1 }, 2, 5, false)]
        [InlineData(new double[] { 2, 2, 2, 3, 4, 4, 4 }, 2, 4, true)]
        [InlineData(new double[] { 0, 0, 1, 2, 3, 4, 4 }, 4, 12, false)]
        [InlineData(new double[] { 5, 3, 6, 5, 4, 5, 6 }, 3, 3, false)]
        // [InlineData(new double[] { 0, 0, 0, 0.5, 0.25, 1, 1, 1 }, 2, true)]  Check for periodic knots
        public void It_Checks_If_The_Knots_Are_Valid(double[] knots, int degree, int ctrlPts, bool expectedResult)
        {
            var knot = new KnotVector(knots);
            knot.IsValid(degree, ctrlPts).Should().Be(expectedResult);
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
            var knotVector = new KnotVector() { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
            var degree = 2;

            var result = knotVector.Span(knotVector.Count - degree - 2, 2, parameter);

            result.Should().Be(expectedValue);
        }

        [Fact]
        public void KnotMultiplicity_Throws_An_Exception_If_Index_Out_Of_Scope()
        {
            var knots = new KnotVector() { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3 };

            Func<object> funcResult = () => knots.MultiplicityByIndex(12);

            funcResult.Should().Throw<Exception>()
                .WithMessage("Input values must be in the dimension of the knot set.");
        }

        [Theory]
        [InlineData(0, 4)]
        [InlineData(4, 2)]
        [InlineData(6, 3)]
        [InlineData(9, 1)]
        [InlineData(10, 1)]
        [InlineData(11, 3)]
        public void KnotMultiplicity_Returns_Knot_Multiplicity_At_The_Given_Index(int index, int result)
        {
            var knots = new KnotVector() { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3, 4, 4, 4 };

            var knotMult = knots.MultiplicityByIndex(index);

            knotMult.Should().Be(result);
        }

        [Fact]
        public void Multiplicities_Returns_A_Dictionary_Of_Knot_Values_And_Multiplicity()
        {
            var knotsValue = new double[] {0, 1, 2, 3, 3.3, 4};
            var multiplicityResult = new int[] {4, 2, 3, 1, 1, 3};
            var count = 0;

            var knots = new KnotVector() { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3, 4, 4, 4 };

            var multiplicities = knots.Multiplicities();

            multiplicities.Keys.Count.Should().Be(knotsValue.Length);

            foreach (var (key, value) in multiplicities)
            {
                key.Should().Be(knotsValue[count]);
                value.Should().Be(multiplicityResult[count]);
                count += 1;
            }
        }

        [Fact]
        public void It_Returns_A_Normalized_Knot_Vectors()
        {
            var knots = new KnotVector() { -5, -5, -3, -2, 2, 3, 5, 5 };
            var knotsExpected = new KnotVector() { 0.0, 0.0, 0.2, 0.3, 0.7, 0.8, 1.0, 1.0 };

            var normalizedKnots = KnotVector.Normalize(knots);

            normalizedKnots.Should().BeEquivalentTo(knotsExpected);
        }

        [Fact]
        public void It_Throws_An_Exception_If_Input_Knot()
        {
            var knots = new KnotVector();

            Func<KnotVector> func = () => KnotVector.Normalize(knots);

            func.Should().Throw<Exception>().WithMessage("Input knot vector cannot be empty");
        }

        [Fact]
        public void It_Returns_The_Reversed_Knot_Vectors()
        {
            var knots = new KnotVector { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3, 4, 4, 4 };
            var knotsExpected = new KnotVector {0, 0, 0, 0.7000000000000002, 1, 2, 2, 2, 3, 3, 4, 4, 4, 4};

            var reversedKnots = KnotVector.Reverse(knots);

            reversedKnots.Should().BeInAscendingOrder();
            reversedKnots.Should().BeEquivalentTo(knotsExpected);
        }
    }
}
