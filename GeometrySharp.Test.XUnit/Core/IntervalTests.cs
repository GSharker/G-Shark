using FluentAssertions;
using GeometrySharp.Core;
using Xunit;

namespace GeometrySharp.Test.XUnit.Core
{
    public class IntervalTests
    {
        [Fact]
        public void It_Returns_A_Interval()
        {
            var interval = new Interval(-10, 20);

            interval.Should().NotBeNull();
            interval.Max.Should().Be(20);
            interval.Min.Should().Be(-10);
        }

        [Theory]
        [InlineData(0, -10)]
        [InlineData(0.25, -2.5)]
        [InlineData(0.5, 5)]
        [InlineData(0.75, 12.5)]
        [InlineData(1, 20)]
        public void It_Returns_The_Value_At_The_Give_Normalized_Parameter(double normalizeParam, double valueExpected)
        {
            var interval = new Interval(-10, 20);

            var valueResult = interval.ParameterAt(normalizeParam);

            valueResult.Should().Be(valueExpected);
        }
    }
}
