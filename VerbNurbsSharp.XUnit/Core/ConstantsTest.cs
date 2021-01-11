using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
    [Trait("Category", "Constants")]
    public class ConstantsTest
    {
        private readonly ITestOutputHelper _testOutput;

        public ConstantsTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(10, 0.174533)]
        [InlineData(32, 0.558505)]
        [InlineData(45, 0.785398)]
        [InlineData(180, 3.141593)]
        public void GetRadiansFromDegree(double degree, double radiansExpected)
        {
            Math.Round(Constants.ToRadians(degree), 6).Should().Be(radiansExpected);
        }

        [Theory]
        [InlineData(0.174533, 10)]
        [InlineData(0.558505, 32)]
        [InlineData(0.785398, 45)]
        [InlineData(3.141592, 180)]
        public void GetDegreeFromRadians(double radians, double degreeExpected)
        {
            Math.Round(Constants.ToDegrees(radians), 0).Should().Be(degreeExpected);
        }

        [Theory]
        [InlineData(0.174533, true)]
        [InlineData(double.NaN, false)]
        [InlineData(double.NegativeInfinity, false)]
        [InlineData(double.PositiveInfinity, false)]
        [InlineData(-1.23432101234321E+308, false)]
        [InlineData(-1.2, true)]
        [InlineData(0.0, true)]
        public void CheckIfADoubleIsValid(double val, bool expectedResult)
        {
            Constants.IsValidDouble(val).Should().Be(expectedResult);
        }

        [Fact]
        public void GetANewSetOfNumbers_AddingOneSetToAnother()
        {
            var set1 = new List<double>() { 20, 0, 0 };
            var set2 = new List<double>() { -10, 15, 5 };
            var expectedSet = new List<double>() { 10, 15, 5 };

            Constants.Addition(set1, set2).Should().BeEquivalentTo(expectedSet);
        }

        [Fact]
        public void GetANewSetOfNumbers_SubtractingOneSetFromAnother()
        {
            var set1 = new List<double>() { 20, 0, 0 };
            var set2 = new List<double>() { -10, 15, 5 };
            var expectedSet = new List<double>() { 30, -15, -5 };

            Constants.Subtraction(set1, set2).Should().BeEquivalentTo(expectedSet);
        }

        [Fact]
        public void GetANewSetOfNumbers_MultiplyingTheSetForAScalarValue()
        {
            var set1 = new List<double>() { -10, 15, 5 };
            var expectedSet = new List<double>() { -70, 105, 35 };

            Constants.Multiplication(set1, 7).Should().BeEquivalentTo(expectedSet);
        }

        [Fact]
        public void GetANewSetOfNumbers_DividingTheSetForAScalarValue()
        {
            var set1 = new List<double>() { -10, 15, 5 };
            var expectedSet = new List<double>() { -1.428571, 2.142857, 0.714286 };

            var divisionResult = Constants.Division(set1, 7);

            divisionResult.Select((val, i) => Math.Round(val, 6).Should().Be(expectedSet[i]));
        }
    }
}
