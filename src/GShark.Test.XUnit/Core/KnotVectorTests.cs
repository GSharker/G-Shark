using FluentAssertions;
using GShark.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace GShark.Test.XUnit.Core
{
    public class KnotVectorTests
    {
        [Theory]
        [InlineData(0, 12)]
        [InlineData(4, 0)]
        [InlineData(-1, 0)]
        [InlineData(3, 2)]
        public void KnotVector_Throws_An_Exception_If_Degree_Or_Number_Of_ControlPts_Are_Less_Than_Or_Zero(int degree, int numberOfControlPts)
        {
            // Act
            Func<KnotVector> funcResult = () => new KnotVector(degree, numberOfControlPts);

            // Assert
            funcResult.Should().Throw<Exception>();
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Clamped_Knot()
        {
            // Arrange
            int degree = 4;
            int ctrlPts = 12;
            KnotVector resultExpected = new KnotVector { 0.0, 0.0, 0.0, 0.0, 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0, 1.0, 1.0, 1.0, 1.0 };
            
            // Act
            KnotVector knots = new KnotVector(degree, ctrlPts);

            // Assert
            knots.Should().BeEquivalentTo(resultExpected);
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Unclamped_Knot()
        {
            // Arrange
            int degree = 3;
            int ctrlPts = 5;
            KnotVector resultExpected = new KnotVector { 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0 };

            // Act
            KnotVector knots = new KnotVector(degree, ctrlPts, false);

            // Assert
            knots.Should().BeEquivalentTo(resultExpected);
        }

        [Fact]
        public void It_Generates_An_Equal_Space_Periodic_Knot()
        {
            // Arrange
            int degree = 2;
            int ctrlPts = 5;
            KnotVector resultExpected = new KnotVector { -0.666667, -0.333333, 0, 0.333333, 0.666667, 1, 1.333333, 1.666667 };

            // Act
            KnotVector knots = KnotVector.CreateUniformPeriodicKnotVector(degree, ctrlPts);

            // Assert
            for (int i = 0; i < knots.Count; i++)
            {
                (resultExpected[i] - knots[i]).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
            }
        }

        [Theory]
        [InlineData(0, 12)]
        [InlineData(4, 0)]
        [InlineData(-1, 0)]
        [InlineData(3, 2)]
        [InlineData(1, 3)]
        public void Periodic_KnotVector_Throws_An_Exception_If_Are_Not_Valid_Inputs(int degree, int numberOfControlPts)
        {
            // Are identifies as not valid inputs when:
            // Degree and control points count is less than 2.
            // Degree is bigger than the control points count.
            // Act
            Func<KnotVector> funcResult = () => KnotVector.CreateUniformPeriodicKnotVector(degree, numberOfControlPts);

            // Assert
            funcResult.Should().Throw<Exception>();
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
        [InlineData(new double[] { 0.0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1.0 }, 3, 5, true)] // Unclamped
        [InlineData(new double[] { -0.666, -0.333, 0, 0.333, 0.666, 1, 1.333, 1.666 }, 2, 5, true)] // Periodic
        public void It_Checks_If_The_Knots_Are_Valid(double[] knots, int degree, int ctrlPts, bool expectedResult)
        {
            // Act
            KnotVector knot = new KnotVector(knots);

            // Assert
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
            // Arrange
            KnotVector knotVector = new KnotVector { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
            int degree = 2;

            // Act
            int result = knotVector.Span(knotVector.Count - degree - 2, 2, parameter);

            // Assert
            result.Should().Be(expectedValue);
        }

        [Fact]
        public void KnotMultiplicity_Throws_An_Exception_If_Index_Out_Of_Scope()
        {
            // Arrange
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3 };

            // Act
            Func<object> funcResult = () => knots.MultiplicityByIndex(12);

            // Assert
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
            // Arrange
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3, 4, 4, 4 };

            // Act
            int knotMult = knots.MultiplicityByIndex(index);

            // Assert
            knotMult.Should().Be(result);
        }

        [Fact]
        public void Multiplicities_Returns_A_Dictionary_Of_Knot_Values_And_Multiplicity()
        {
            // Arrange
            double[] knotsValue = new double[] { 0, 1, 2, 3, 3.3, 4 };
            int[] multiplicityResult = new int[] { 4, 2, 3, 1, 1, 3 };
            int count = 0;
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3, 4, 4, 4 };

            // Act
            Dictionary<double, int> multiplicities = knots.Multiplicities();

            // Assert
            multiplicities.Keys.Count.Should().Be(knotsValue.Length);

            foreach ((double key, int value) in multiplicities)
            {
                key.Should().Be(knotsValue[count]);
                value.Should().Be(multiplicityResult[count]);
                count += 1;
            }
        }

        [Fact]
        public void It_Returns_A_Normalized_Knot_Vectors()
        {
            // Arrange
            KnotVector knots = new KnotVector { -5, -5, -3, -2, 2, 3, 5, 5 };
            KnotVector knotsExpected = new KnotVector { 0.0, 0.0, 0.2, 0.3, 0.7, 0.8, 1.0, 1.0 };

            // Act
            KnotVector normalizedKnots = KnotVector.Normalize(knots);

            // Assert
            normalizedKnots.Should().BeEquivalentTo(knotsExpected);
        }

        [Fact]
        public void It_Throws_An_Exception_If_Input_Knot()
        {
            // Assert
            KnotVector knots = new KnotVector();

            // Act
            Func<KnotVector> func = () => KnotVector.Normalize(knots);

            // Arrange
            func.Should().Throw<Exception>().WithMessage("Input knot vector cannot be empty");
        }

        [Fact]
        public void It_Returns_The_Reversed_Knot_Vectors()
        {
            // Assert
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3.3, 4, 4, 4 };
            KnotVector knotsExpected = new KnotVector { 0, 0, 0, 0.7000000000000002, 1, 2, 2, 2, 3, 3, 4, 4, 4, 4 };

            // Act
            KnotVector reversedKnots = KnotVector.Reverse(knots);

            // Arrange
            reversedKnots.Should().BeInAscendingOrder();
            reversedKnots.Should().BeEquivalentTo(knotsExpected);
        }
    }
}
