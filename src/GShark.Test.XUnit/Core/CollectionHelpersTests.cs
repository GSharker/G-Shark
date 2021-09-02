using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Core
{
    public class CollectionHelpersTests
    {
        private readonly ITestOutputHelper _testOutput;

        public CollectionHelpersTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> DataToRepeat =>
            new List<object[]>
            {
                new object[] { 10, 7},
                new object[] { new Vector(){ 14, -12, 7}, 5},
                new object[] { 2.7, 8 },
                new object[] { 1.0, 0 }
            };

        [Theory]
        [MemberData(nameof(DataToRepeat))]
        public void It_Returns_A_Set_Of_Repeated_Data_Of_A_Specific_Length(object data, int length)
        {
            // Act 
            List<object> repeatedData = CollectionHelpers.RepeatData(data, length);

            // Assert
            repeatedData.Should().HaveCount(length);
            repeatedData.Should().BeEquivalentTo(new List<object>(repeatedData));
        }

        [Fact]
        public void RepeatData_Throws_An_Exception_If_The_Value_Is_Negative_Or_Zero()
        {
            // Act 
            Func<object> resultFunction = () => CollectionHelpers.RepeatData(5, -1);

            // Assert
            resultFunction.Should().Throw<Exception>().WithMessage("Length can not be negative.");
        }

       
        [Fact]
        public void It_Transposes_A_2D_List_Of_Points()
        {
            // Arrange
            List<List<Point3>> pts = new List<List<Point3>>
            {
                new()
                {
                    new Point3( 0d, -10d, 0),
                    new Point3( 10d, -10d, 10)
                },
                new()
                {
                    new Point3( 0d, -30d, 0),
                    new Point3( 10d, -30d, 0)

                },
                new()
                {
                    new Point3( 0d, 0d, 50),
                    new Point3( 10d, 0d, 0)
                }
            };

            // Act
            List<List<Point3>> transposedPointData = CollectionHelpers.Transpose2DArray(pts);

            // Assert
            transposedPointData.Count.Should().Be(2);
            transposedPointData[0].Count.Should().Be(3);
            transposedPointData[0][2].Should().BeEquivalentTo(pts[2][0]);
            transposedPointData[1][2].Should().BeEquivalentTo(pts[2][1]);
        }
    }
}