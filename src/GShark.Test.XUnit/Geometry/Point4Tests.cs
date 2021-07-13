using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class Point4Tests
    {
        private readonly ITestOutputHelper _testOutput;

        public Point4Tests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_Transformed_Point()
        {
            //Arrange
            var testPoint = new Point4(5, 10, 0, 0.5);
            var expectedPoint = new Point4(15, 0, 0, 0.5);
            var translation = Transform.Translation(new Vector3(10, 0, 0));

            //Act
            var xFormedPoint = testPoint.Transform(translation);

            //Assert
            xFormedPoint.EpsilonEquals(expectedPoint, GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal()
        {
            // Arrange
            var p1 = new Point4(5.982099, 5.950299, 0, 1);
            var p2 = new Point4(5.982099, 5.950299, 0, 1);

            // Assert
            (p1 == p2).Should().BeTrue();
        }
    }
}
