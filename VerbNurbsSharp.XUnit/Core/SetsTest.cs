using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
    [Trait("Category", "Sets")]
    public class SetsTest
    {
        public readonly ITestOutputHelper _OutputHelper;

        public SetsTest(ITestOutputHelper outputHelper)
        {
            _OutputHelper = outputHelper;
        }

        public static IEnumerable<object[]> DefineRanges =>
            new List<object[]>
            {
                new object[] { new Interval(2, 30), 3},
                new object[] { new Interval(-10, 30), 6},
                new object[] { new Interval(1, 10), 0}
            };

        [Theory]
        [MemberData(nameof(DefineRanges))]
        public void GetARangeOfNumbers(Interval interval, int step)
        {
            var range = Sets.Range(interval, step);

            var st = string.Join('-', range);
            _OutputHelper.WriteLine(st);

            range.Should().NotBeNull();
            range.Should().BeEquivalentTo(new List<double>(range));
        }

        [Fact]
        public void GetARangeOfPositiveNumber_SteppingOfOne()
        {
            var range = Sets.Range(12);

            var st = string.Join('-', range);
            _OutputHelper.WriteLine(st);

            range.Should().NotBeNull();
            range.Should().BeEquivalentTo(new List<double>(range));

        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void RangeThrowAnException_IfTheValueIsNegativeOrZero(int maxValue)
        {
            Func<object> resultFunction = () => Sets.Range(maxValue);
            resultFunction.Should().Throw<Exception>().WithMessage("Zero or negative value is not accepted");
        }

        [Theory]
        [InlineData(2, 4, 8)]
        [InlineData(-3, 10, 10)]
        [InlineData(0, 1, 10)]
        public void GetASeriesOfNumber(double start, double step, int count)
        {
            var series = Sets.Span(start, step, count);

            var st = string.Join('-', series);
            _OutputHelper.WriteLine(st);

            series.Should().NotBeNull();
            series.Should().BeEquivalentTo(new List<double>(series));
        }

        [Theory]
        [InlineData(-3, 10, 0)]
        [InlineData(0, 10, -1)]
        public void SeriesThrowAnException_IfTheValueIsNegativeOrZero(double start, double step, int count)
        {
            Func<object> resultFunction = () => Sets.Span(start, step, count);
            resultFunction.Should().Throw<Exception>().WithMessage("Count as zero or negative is not accepted");
        }
    }
}
