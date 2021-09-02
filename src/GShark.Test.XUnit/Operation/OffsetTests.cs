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
