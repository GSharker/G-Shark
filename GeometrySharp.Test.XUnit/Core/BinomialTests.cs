using FluentAssertions;
using GeometrySharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Core
{
    public class BinomialTests
    {
        private readonly ITestOutputHelper _testOutput;

        public BinomialTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
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
        public void It_Returns_A_Binomial_Coefficient(int n, int k, double valResult)
        {
            var valToCheck = Binomial.Get(n, k);
            _testOutput.WriteLine(valToCheck.ToString());
            (System.Math.Abs(valToCheck - valResult) < GeoSharpMath.EPSILON).Should().BeTrue();
        }
    }
}
