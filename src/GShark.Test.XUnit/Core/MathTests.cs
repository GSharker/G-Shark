using FluentAssertions;
using GShark.Core;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Core
{
    public class MathTests
    {
        private readonly ITestOutputHelper _testOutput;

        public MathTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(10, 0.174533)]
        [InlineData(32, 0.558505)]
        [InlineData(45, 0.785398)]
        [InlineData(180, 3.141593)]
        public void It_Returns_The_Radians_From_Degree(double degree, double radiansExpected)
        {
            System.Math.Round(GSharkMath.ToRadians(degree), 6).Should().Be(radiansExpected);
        }

        [Theory]
        [InlineData(0.174533, 10)]
        [InlineData(0.558505, 32)]
        [InlineData(0.785398, 45)]
        [InlineData(3.141592, 180)]
        public void It_Returns_The_Degree_From_Radians(double radians, double degreeExpected)
        {
            System.Math.Round(GSharkMath.ToDegrees(radians), 0).Should().Be(degreeExpected);
        }

        [Theory]
        [InlineData(0.174533, true)]
        [InlineData(double.NaN, false)]
        [InlineData(double.NegativeInfinity, false)]
        [InlineData(double.PositiveInfinity, false)]
        [InlineData(-1.23432101234321E+308, false)]
        [InlineData(-1.2, true)]
        [InlineData(0.0, true)]
        public void It_Checks_If_A_Double_Is_Valid(double val, bool expectedResult)
        {
            GSharkMath.IsValidDouble(val).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(1, 1, 1)]
        [InlineData(0, 1, 0)]
        [InlineData(3, 1, 3)]
        [InlineData(3, 3, 1)]
        [InlineData(5, 3, 10)]
        [InlineData(5, 4, 5)]
        [InlineData(8, 4, 70)]
        public void It_Returns_A_Binomial_Coefficient(int n, int k, double resultValue)
        {
            // Act
            double valToCheck = GSharkMath.GetBinomial(n, k);

            // Assert
            (System.Math.Abs(valToCheck - resultValue) < GSharkMath.Epsilon).Should().BeTrue();
        }
    }
}
