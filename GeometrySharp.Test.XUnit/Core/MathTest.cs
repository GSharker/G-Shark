using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Core
{
    [Trait("Category", "GeoSharpMath")]
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
        public void It_Returns_The_Radians_From_Degree(double degree, double radiansExpected)
        {
            Math.Round(GeoSharpMath.ToRadians(degree), 6).Should().Be(radiansExpected);
        }

        [Theory]
        [InlineData(0.174533, 10)]
        [InlineData(0.558505, 32)]
        [InlineData(0.785398, 45)]
        [InlineData(3.141592, 180)]
        public void It_Returns_The_Degree_From_Radians(double radians, double degreeExpected)
        {
            Math.Round(GeoSharpMath.ToDegrees(radians), 0).Should().Be(degreeExpected);
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
            GeoSharpMath.IsValidDouble(val).Should().Be(expectedResult);
        }
    }
}
