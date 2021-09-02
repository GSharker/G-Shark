using System.Collections.Generic;
using FluentAssertions;
using GShark.Core;
using Xunit;

namespace GShark.Test.XUnit.Core
{
    public class IntervalTests
    {
        public static IEnumerable<object[]> RangesToBeDefined =>
            new List<object[]>
            {
                new object[] { new Interval(0, 1), 4},
                new object[] { new Interval(2, 30), 3},
                new object[] { new Interval(-10, 30), 6},
                new object[] { new Interval(1, 10), 0}
            };

        [Fact]
        public void It_Returns_An_Interval()
        {
            Interval interval = new Interval(-10, 20);

            interval.Should().NotBeNull();
            interval.T1.Should().Be(20);
            interval.T0.Should().Be(-10);
        }

        [Theory]
        [InlineData(0, -10)]
        [InlineData(0.25, -2.5)]
        [InlineData(0.5, 5)]
        [InlineData(0.75, 12.5)]
        [InlineData(1, 20)]
        public void It_Returns_The_Value_At_The_Given_Normalized_Parameter(double normalizeParam, double valueExpected)
        {
            Interval interval = new Interval(-10, 20);

            double valueResult = interval.ParameterAt(normalizeParam);

            valueResult.Should().Be(valueExpected);
        }

        [Fact]
        public void It_Returns_The_Interval_Length()
        {
            Interval interval = new Interval(1.5, 15.3);

            double intervalLength = interval.Length;

            intervalLength.Should().Be(13.8);
        }

        [Fact]
        public void It_Returns_The_Mid_Value_Of_The_Interval()
        {
            Interval interval = new Interval(1.5, 15.3);

            double intervalMidVal = interval.Mid;

            intervalMidVal.Should().Be(8.4);
        }

        [Fact]
        public void It_Returns_True_If_Interval_Is_Decreasing()
        {
            //Arrange

            //Act
            var interval = new Interval(5.5, 0.2);

            //Assert
            interval.IsDecreasing.Should().Be(true);
            interval.IsIncreasing.Should().Be(false);
            interval.IsSingleton.Should().Be(false);
        }

        [Fact]
        public void It_Returns_True_If_Interval_Is_Increasing()
        {
            //Arrange

            //Act
            var interval = new Interval(0.2, 0.5);

            //Assert
            interval.IsDecreasing.Should().Be(false);
            interval.IsIncreasing.Should().Be(true);
            interval.IsSingleton.Should().Be(false);
        }

        [Fact]
        public void It_Returns_True_If_Interval_Is_Singleton()
        {
            //Arrange

            //Act
            var interval = new Interval(0.2, 0.2);

            //Assert
            interval.IsDecreasing.Should().Be(false);
            interval.IsIncreasing.Should().Be(false);
            interval.IsSingleton.Should().Be(true);
        }

        [Fact]
        public void It_Returns_The_Maximum_Value_In_The_Interval()
        {
            //Arrange
            var expectedMaxValue = 0.5;

            //Act
            var interval = new Interval(expectedMaxValue, 0.2);

            //Assert
            interval.Max.Should().Be(0.5);
        }

        [Fact]
        public void It_Returns_The_Minimum_Value_In_The_Interval()
        {
            //Arrange
            var expectedMinValue = -0.5;

            //Act
            var interval = new Interval(expectedMinValue, 0.2);

            //Assert
            interval.Min.Should().Be(-0.5);
        }

        [Theory]
        [MemberData(nameof(RangesToBeDefined))]
        public void It_Divides_An_Interval_In_Equal_Parts(Interval interval, int step)
        {
            // Arrange
            IList<double> linearSpace = Interval.Divide(interval, step);

            // Act
            _ = string.Join(',', linearSpace);

            // Assert
            linearSpace.Should().NotBeNull();
            linearSpace.Should().BeEquivalentTo(new List<double>(linearSpace));
        }
    }
}
