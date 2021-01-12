using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using Xunit;
using Xunit.Abstractions;
using Math = VerbNurbsSharp.Core.Math;

namespace VerbNurbsSharp.XUnit.Core
{
    [Trait("Category", "Math")]
    public class MathTest
    {
        private readonly ITestOutputHelper _testOutput;

        public MathTest(ITestOutputHelper testOutput)
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
            System.Math.Round(Math.ToRadians(degree), 6).Should().Be(radiansExpected);
        }

        [Theory]
        [InlineData(0.174533, 10)]
        [InlineData(0.558505, 32)]
        [InlineData(0.785398, 45)]
        [InlineData(3.141592, 180)]
        public void GetDegreeFromRadians(double radians, double degreeExpected)
        {
            System.Math.Round(Math.ToDegrees(radians), 0).Should().Be(degreeExpected);
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
            Math.IsValidDouble(val).Should().Be(expectedResult);
        }
    }
}
