using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class NurbsSurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;
        public NurbsSurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_NURBS_Surface_By_Four_Points()
        {
            // Arrange
            Point3 p1 = new Point3(0.0, 0.0, 0.0);
            Point3 p2 = new Point3(10.0, 0.0, 0.0);
            Point3 p3 = new Point3(10.0, 10.0, 2.0);
            Point3 p4 = new Point3(0.0, 10.0, 4.0);

            Point3 expectedPt = new Point3(5.0, 5.0, 1.5);

            // Act
            NurbsSurface surface = NurbsSurface.CreateFromCorners(p1, p2, p3, p4);
            Point3 evalPt = Evaluation.SurfacePointAt(surface, 0.5, 0.5);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(2);
            surface.LocationPoints[0].Count.Should().Be(2);
            surface.LocationPoints[0][1].Equals(p4).Should().BeTrue();
            evalPt.EpsilonEquals(expectedPt, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { -0.020397, -0.392974, 0.919323 })]
        [InlineData(0.5, 0.5, new double[] { 0.091372, -0.395944, 0.913717 })]
        [InlineData(1.0, 1.0, new double[] { 0.507093, -0.169031, 0.845154 })]
        public void It_Returns_The_Surface_Normal_At_A_Given_U_And_V_Parameter(double u, double v, double[] pt)
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedNormal = new Vector3(pt[0], pt[1], pt[2]);

            // Act
            Vector3 normal = surface.Normal(u,v);

            // Assert
            normal.EpsilonEquals(expectedNormal, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        //[Fact]
        //public void It_Returns_The_Surface_Tange_At_A_Given_U_And_V_Parameter_Along_U_Direction()
        //{
        //    var nurbsSurface = BuildTestNurbsSurface();
        //    var res1 = nurbsSurface.TangentAtU(0.5, 0.5).Unitize();

        //    res1.Should().NotBeNullOrEmpty();
        //    res1[0].Should().BeApproximately(0d, 3);
        //    res1[1].Should().BeApproximately(-0.996d, 3);
        //    res1[2].Should().BeApproximately(0.093d, 3);

        //    var res2 = nurbsSurface.TangentAtU(0.2, 0.7).Unitize();
        //    res2.Should().NotBeNullOrEmpty();
        //    res2[0].Should().BeApproximately(0d, 3);
        //    res2[1].Should().BeApproximately(-0.998d, 3);
        //    res2[2].Should().BeApproximately(0.060d, 3);

        //    _testOutput.WriteLine($"Vector1[0.5,0.5]: {res1}\nVector2[0.2,0.7]: {res2}\n");
        //}

        //[Fact]
        //public void It_Returns_The_Surface_Tangent_At_A_Given_U_And_V_Parameter_Along_V_Direction()
        //{
        //    var nurbsSurface = BuildTestNurbsSurface();
        //    var res1 = nurbsSurface.TangentAtV(0.5, 0.5).Unitize();

        //    res1.Should().NotBeNullOrEmpty();
        //    res1[0].Should().BeApproximately(0.996d, 3);
        //    res1[1].Should().BeApproximately(0d, 3);
        //    res1[2].Should().BeApproximately(-0.093d, 3);

        //    var res2 = nurbsSurface.TangentAtV(0.2, 0.7).Unitize();
        //    res2.Should().NotBeNullOrEmpty();
        //    res2[0].Should().BeApproximately(0.992d, 3);
        //    res2[1].Should().BeApproximately(0d, 3);
        //    res2[2].Should().BeApproximately(-0.126d, 3);

        //    _testOutput.WriteLine($"Vector1[0.5,0.5]: {res1}\nVector2[0.2,0.7]: {res2}\n");
        //}
    }
}
