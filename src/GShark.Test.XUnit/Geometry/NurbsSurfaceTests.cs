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

        //private NurbsSurface BuildTestNurbsSurface()
        //{
        //    int degreeU = 3;
        //    int degreeV = 3;
        //    KnotVector knotsU = new KnotVector() { 0, 0, 0, 0, 1, 1, 1, 1 };
        //    KnotVector knotsV = new KnotVector() { 0, 0, 0, 0, 1, 1, 1, 1 };

        //    List<Vector3> u1 = new List<Vector3>()
        //    {
        //        new Vector3() { 0d, 0d, 0d },
        //        new Vector3() { 10d, 0d, 0d },
        //        new Vector3() { 20d, 0d, 0d },
        //        new Vector3() { 30d, 0d, 0d }
        //    };
        //    List<Vector3> u2 = new List<Vector3>()
        //    {
        //        new Vector3() { 0d, -10d, 0d },
        //        new Vector3() { 10d, -10d, 10d },
        //        new Vector3() { 20d, -10d, 0d },
        //        new Vector3() { 30d, -10d, 0d }
        //    };
        //    List<Vector3> u3 = new List<Vector3>()
        //    {
        //        new Vector3() { 0d, -20d, 0d },
        //        new Vector3() { 10d, -20d, 0d },
        //        new Vector3() { 20d, -20d, 0d },
        //        new Vector3() { 30d, -20d, 0d }
        //    };
        //    List<Vector3> u4 = new List<Vector3>()
        //    {
        //        new Vector3() { 0d, -30d, 0d },
        //        new Vector3() { 10d, -30d, 0d },
        //        new Vector3() { 20d, -30d, 0d },
        //        new Vector3() { 30d, -30d, 0d }
        //    };
        //    List<List<Vector3>> controlPoints = new List<List<Vector3>>()
        //    {
        //        u1, u2, u3, u4
        //    };

        //    return new NurbsSurface(degreeU, degreeV, knotsU, knotsV, controlPoints);
        //}

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
            NurbsSurface surface = NurbsSurface.ByFourPoints(p1, p2, p3, p4);
            Point3 evalPt = Evaluation.SurfacePointAt(surface, 0.5, 0.5);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(2);
            surface.LocationPoints[0].Count.Should().Be(2);
            surface.LocationPoints[0][1].Equals(p4).Should().BeTrue();
            evalPt.EpsilonEquals(expectedPt, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        //[Fact]
        //public void It_Returns_The_Surface_Normal_At_A_Given_U_And_V_Parameter()
        //{
        //    var nurbsSurface = BuildTestNurbsSurface();
        //    var res1 = nurbsSurface.Normal(0.5, 0.5).Unitize();
        //    res1.Should().NotBeNullOrEmpty();
        //    res1[0].Should().BeApproximately(0.093d, 3);
        //    res1[1].Should().BeApproximately(-0.093d, 3);
        //    res1[2].Should().BeApproximately(0.991d, 3);

        //    var res2 = nurbsSurface.Normal(0.2, 0.7).Unitize();
        //    res2.Should().NotBeNullOrEmpty();
        //    res2[0].Should().BeApproximately(0.125d, 3);
        //    res2[1].Should().BeApproximately(0.060d, 3);
        //    res2[2].Should().BeApproximately(0.990d, 3);

        //    _testOutput.WriteLine($"Vector1[0.5,0.5]: {res1}\nVector2[0.2,0.7]: {res2}\n");

        //}

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
