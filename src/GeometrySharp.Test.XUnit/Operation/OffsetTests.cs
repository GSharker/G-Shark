using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;
using GeometrySharp.Operation;
using GeometrySharp.Test.XUnit.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
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
            Line ln = new Line(new Vector3 {5, 0, 0}, new Vector3 {0, 5, 0});
            double offset = 12;

            // Act
            Line offsetResult = Offset.Line(ln, offset, Plane.PlaneXY);

            // Assert
            (offsetResult.Start.DistanceTo(ln.Start) - offset).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Circle()
        {
            // Arrange
            Circle cl = new Circle(Plane.PlaneXY, 13);
            double offset = - 5;

            // Act
            Circle offsetResult = Offset.Circle(cl, offset);

            // Assert
            offsetResult.Plane.Origin.Should().BeEquivalentTo(cl.Plane.Origin);
            (offsetResult.Radius - offset).Should().Be(cl.Radius);
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Polyline()
        {
            // Arrange
            Polyline pl = new Polyline(PolylineTests.ExamplePts);
            double offset = -5;

            // Act
            Polyline offsetResult = Offset.Polyline(pl, offset, Plane.PlaneXY);

            // Assert
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
                Vector3 pt = offsetResult.PointAt(i);
                Vector3 closestPt = crv.ClosestPt(pt);
                pt.DistanceTo(closestPt).Should().BeApproximately(offset, GeoSharpMath.MINTOLERANCE);
            }
        }
    }
}
