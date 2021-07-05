using System.Collections.Generic;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
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
        public void Returns_The_Offset_Of_A_Line()
        {
            // Arrange
            Line ln = new Line(new Point3(5, 0, 0), new Point3(0, 5, 0));
            double offset = 12;

            // Act
            Line offsetResult = Offset.Line(ln, offset, Plane.PlaneXY);

            // Assert
            (offsetResult.Start.DistanceTo(ln.Start) - offset).Should().BeLessThan(GeoSharkMath.MaxTolerance);
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
            (offsetResult[0].DistanceTo(pl[0]) - offset).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            (offsetResult[^1].DistanceTo(pl[^1]) - offset).Should().BeLessThan(GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Polygon()
        {
            // Arrange
            Polygon pl = new Polygon(PolygonTests.Planar2D);
            double offset = 5;

            Polyline offsetResult = Offset.Polyline(pl, offset, Plane.PlaneXY);

            // Assert
            offsetResult[0].DistanceTo(offsetResult[^1]).Should().Be(0.0);
            Point3 pt = offsetResult.PointAt(0.5);
            Point3 closestPt = pl.ClosestPoint(pt);
            pt.DistanceTo(closestPt).Should().BeApproximately(offset, GeoSharkMath.MinTolerance);
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
                pt.DistanceTo(closestPt).Should().BeApproximately(offset, GeoSharkMath.MinTolerance);
            }
        }
    }
}
