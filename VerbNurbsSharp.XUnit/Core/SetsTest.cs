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
        private readonly ITestOutputHelper _testOutput;

        public SetsTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> RangesToBeDefined =>
            new List<object[]>
            {
                new object[] { new Interval(0, 1), 4},
                new object[] { new Interval(2, 30), 3},
                new object[] { new Interval(-10, 30), 6},
                new object[] { new Interval(1, 10), 0}
            };

        public static IEnumerable<object[]> SetOfNumbersAndTheSetDimension =>
            new List<object[]>
            {
                new object[] { new List<double>(){ 10, 99, 87, 45, 67, 43, 45, 33, 21, 7, 65, 98 }, 92},
                new object[] { new Vector3(){ 14, -12, 7, 0, -5, -8, 17, -11, 19 }, 31},
                new object[] { new List<double>(){ 2.7, 3.5, 4.9, 5.1, 8.3 }, 5.6000000000000005 }
            };

        public static IEnumerable<object[]> DataToRepeat =>
            new List<object[]>
            {
                new object[] { 10, 7},
                new object[] { new Vector3(){ 14, -12, 7}, 5},
                new object[] { 2.7, 8 },
                new object[] { 1.0, 0 }
            };

        [Theory]
        [MemberData(nameof(RangesToBeDefined))]
        public void GetARangeOfNumbers(Interval interval, int step)
        {
            var range = Sets.Range(interval, step);

            var st = string.Join(',', range);
            _testOutput.WriteLine(st);

            range.Should().NotBeNull();
            range.Should().BeEquivalentTo(new List<double>(range));
        }

        [Theory]
        [MemberData(nameof(RangesToBeDefined))]
        public void GetAListOfEquallySpaceNumbers(Interval interval, int step)
        {
            var linearSpace = Sets.LinearSpace(interval, step);

            var st = string.Join(',', linearSpace);
            _testOutput.WriteLine(st);

            linearSpace.Should().NotBeNull();
            linearSpace.Should().BeEquivalentTo(new List<double>(linearSpace));
        }

        [Fact]
        public void GetARangeOfPositiveNumber_SteppingOfOne()
        {
            var range = Sets.Range(12);

            var st = string.Join(',', range);
            _testOutput.WriteLine(st);

            range.Should().NotBeNull();
            range.Should().BeEquivalentTo(new List<double>(range));

        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void RangeThrowAnException_IfTheValueIsNegativeOrZero(int maxValue)
        {
            Func<object> resultFunction = () => Sets.Range(maxValue);
            resultFunction.Should().Throw<Exception>().WithMessage("Max value range can not be negative or zero.");
        }

        [Theory]
        [InlineData(2, 4, 8)]
        [InlineData(-3, 10, 10)]
        [InlineData(0, 1, 10)]
        public void GetASeriesOfNumber(double start, double step, int count)
        {
            var series = Sets.Span(start, step, count);

            var st = string.Join(',', series);
            _testOutput.WriteLine(st);

            series.Should().NotBeNull();
            series.Should().BeEquivalentTo(new List<double>(series));
        }

        [Theory]
        [InlineData(-3, 10, 0)]
        [InlineData(0, 10, -1)]
        public void SeriesThrowAnException_IfTheValueIsNegativeOrZero(double start, double step, int count)
        {
            Func<object> resultFunction = () => Sets.Span(start, step, count);
            resultFunction.Should().Throw<Exception>().WithMessage("Count can not be negative or zero.");
        }

        [Theory]
        [MemberData(nameof(SetOfNumbersAndTheSetDimension))]
        public void GetTheDimension_OfASetOfNumbers(IList<double> set, double expectedRange)
        {
            Sets.Dimension(set).Should().Be(expectedRange);
        }

        [Theory]
        [MemberData(nameof(DataToRepeat))]
        public void GetASet_OfRepeatedData_OfASpecificLength(object data, int length)
        {
            var repeatedData = Sets.RepeatData(data, length);

            var resultConcat = string.Join(",", repeatedData);
            _testOutput.WriteLine(resultConcat);

            repeatedData.Should().HaveCount(length);
            repeatedData.Should().BeEquivalentTo(new List<object>(repeatedData));
        }

        [Fact]
        public void RepeatThrowAnException_IfTheValueIsNegativeOrZero()
        {
            Func<object> resultFunction = () => Sets.RepeatData(5, -1);
            resultFunction.Should().Throw<Exception>().WithMessage("Length can not be negative.");
        }

        [Theory]
        [InlineData(new double[] { 3, 5, 7, 9, 11 }, new double[] { 5, 6, 7 }, new double[] { 3, 8, 11 })]
        [InlineData(new double[] { }, new double[] { 5, 6, 7 }, new double[] { 5, 6, 7 })]
        [InlineData(new double[] { 3, 5, 7, 9, 11 }, new double[] { }, new double[] { 3, 5, 7, 9, 11 })]
        public void Return_ASetUnion_FromTwoCollectionsOfNumbers(double[] set1, double[] set2, double[] setExpected)
        {
            var setSub = Sets.SetUnion(set1, set2);
            var resultConcat = string.Join(",", setSub);
            _testOutput.WriteLine(resultConcat);

            setExpected.Should().BeEquivalentTo(setExpected);
        }

        [Theory]
        [InlineData(new double[]{ 3, 5, 7 }, new double[] { 5, 6 }, new double[] { 3, 7 })]
        [InlineData(new double[] { 3, 5, 7, 9, 11 }, new double[] {}, new double[] { 3, 5, 7, 9, 11 })]
        public void Return_ASetDifference_FromTwoCollectionsOfNumbers(double[] set1, double[] set2, double[] setExpected)
        {
            var setSub = Sets.SetDifference(set1, set2);
            var resultConcat = string.Join(",", setSub);
            _testOutput.WriteLine(resultConcat);

            setExpected.Should().BeEquivalentTo(setExpected);
        }

        [Fact]
        public void SetDifference_ThrowAnException_IfTheFirstCollection_IsEmpty()
        {
            var set1 = new List<double>();
            var set2 = new List<double>() { 3, 5, 7, 9, 11 };

            Func<object> resultFunction = () => Sets.SetDifference(set1, set2);

            resultFunction.Should().Throw<Exception>("Set difference can't be computed, the first set is empty.");
        }
    }
}
