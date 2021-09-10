using FluentAssertions;
using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Analyze
{
    public class SurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;

        public SurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(0.204157623157292, 0.716170472509343, new double[] { 2.5, 7, 5 })]
        [InlineData(0.237211551442712, 0.154628316784507, new double[] { 2.5, 1.5, 2 })]
        [InlineData(0.910119163727208, 0.229417610613794, new double[] { 9, 2.5, 1 })]
        [InlineData(0.50870054333679, 0.360138133269618, new double[] { 5, 5, 1 })]
        public void It_Returns_Parameter_U_V_Of_A_Closest_Point(double u, double v, double[] testPt)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 pt = new Point3(testPt[0], testPt[1], testPt[2]);
            (double u, double v) expectedUV = (u, v);

            // Act
            var closestParameter = surface.ClosestParameter(pt);

            // Assert
            (closestParameter.U - expectedUV.u).Should().BeLessThan(GSharkMath.MaxTolerance);
            (closestParameter.V - expectedUV.v).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Returns_The_Surface_Isocurve_At_U_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 expectedPt = new Point3(3.591549, 10, 4.464789);

            // Act
            NurbsBase Isocurve = surface.IsoCurve(0.3, SurfaceDirection.U);

            // Assert
            Isocurve.ControlPointLocations[1].DistanceTo(expectedPt).Should().BeLessThan(GSharkMath.MinTolerance);
        }

        [Fact]
        public void Returns_The_Surface_Isocurve_At_V_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 expectedPt = new Point3(5, 4.615385, 2.307692);
            Point3 expectedPtAt = new Point3(5, 3.913043, 1.695652);

            // Act
            NurbsBase Isocurve = surface.IsoCurve(0.3, SurfaceDirection.V);
            Point3 ptAt = Isocurve.PointAt(0.5);

            // Assert
            Isocurve.ControlPointLocations[1].DistanceTo(expectedPt).Should().BeLessThan(GSharkMath.MinTolerance);
            ptAt.DistanceTo(expectedPtAt).Should().BeLessThan(GSharkMath.MinTolerance);
        }
    }
}
