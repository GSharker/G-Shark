using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Interfaces;
using GShark.Operation;
using GShark.Test.XUnit.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class OffsetTests
    {
        private readonly ITestOutputHelper _testOutput;
        public OffsetTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Circle()
        {
            // Arrange
            Circle cl = new Circle(Plane.PlaneXY, 13);
            double offset = -5;

            // Act
            Circle offsetResult = Offset.Circle(cl, offset);

            // Assert
            offsetResult.Plane.Origin.Should().BeEquivalentTo(cl.Plane.Origin);
            (offsetResult.Radius - offset).Should().Be(cl.Radius);
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Open_Polyline()
        {
            // Arrange
            Polyline pl = new Polyline(new PolylineTests().ExamplePts);
            double offset = 5;

            // Act
            Polyline offsetResult = Offset.Polyline(pl, offset, Plane.PlaneXY);

            // Assert
            (offsetResult[0].DistanceTo(pl[0]) - offset).Should().BeLessThan(GSharkMath.MaxTolerance);
            (offsetResult[offsetResult.Count - 1].DistanceTo(pl[pl.Count - 1]) - offset).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Theory]
        [InlineData(0,7.071068)]
        [InlineData(1, 12.205361)]
        [InlineData(2, 5.481013)]
        [InlineData(3, 7.071068)]
        [InlineData(4, 7.071068)]
        public void Returns_The_Offset_Of_A_Polygon(int vertex, double expectedDistance)
        {
            // Arrange
            Polygon polygon = new Polygon(PolygonTests.Planar2D);
            double offset = 5;

            Polyline offsetPolygon = Offset.Polyline(polygon, offset, Plane.PlaneXY);

            // Assert
            polygon[vertex].DistanceTo(offsetPolygon[vertex]).Should()
                .BeApproximately(expectedDistance, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Curve()
        {
            // Arrange
            NurbsCurve crv = new NurbsCurve(FittingTests.pts, 2);
            double offset = 22.5;

            // Act
            ICurve offsetResult = Offset.Curve(crv, offset, Plane.PlaneXY);

            // Assert
            for (double i = 0; i <= 1; i += 0.1)
            {
                Point3 pt = offsetResult.PointAt(i);
                Point3 closestPt = crv.ClosestPoint(pt);
                pt.DistanceTo(closestPt).Should().BeApproximately(offset, GSharkMath.MaxTolerance);
            }
        }
    }
}
