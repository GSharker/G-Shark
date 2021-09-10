using FluentAssertions;
using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Evaluate
{
    public class SurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;

        public SurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { 1.0, 1.0, 0.38 })]
        [InlineData(0.5, 0.5, new double[] { 5.0, 5.0, 1.5 })]
        [InlineData(1.0, 1.0, new double[] { 10.0, 10.0, 2.0 })]
        public void It_Returns_A_Point_On_Surface_At_A_Given_U_And_V_Parameter(double u, double v, double[] pt)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.QuadrilateralSurface();
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Evaluation_Of_A_Surface_At_The_Given_Parameter()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.QuadrilateralSurface();
            Vector3 expectedNormal = new Vector3(0.094255, -0.320469, 0.942557);
            Vector3 expectedDerivativeU = new Vector3(0.995037, 0.0, -0.099503);
            Vector3 expectedDerivativeV = new Vector3(0.0, 0.946772, 0.321902);

            // Act
            Vector3 evaluationNormal = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.Normal);
            Vector3 evaluationU = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.U);
            Vector3 evaluationV = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.V);

            // Assert
            (evaluationNormal.IsParallelTo(expectedNormal)).Should().Be(1);
            (evaluationU.IsParallelTo(expectedDerivativeU)).Should().Be(1);
            (evaluationV.IsParallelTo(expectedDerivativeV)).Should().Be(1);
        }
    }
}
