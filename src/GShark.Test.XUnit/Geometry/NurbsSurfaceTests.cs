//using FluentAssertions;
//using GShark.Core;
//using GShark.Geometry;
//using System.Collections.Generic;
//using Xunit;
//using Xunit.Abstractions;

//namespace GShark.Test.XUnit.Geometry
//{
//    public class NurbsSurfaceTests
//    {
//        private readonly ITestOutputHelper _testOutput;
//        public NurbsSurfaceTests(ITestOutputHelper testOutput)
//        {
//            _testOutput = testOutput;
//        }

//        private NurbsSurface BuildTestNurbsSurface()
//        {
//            int degreeU = 3;
//            int degreeV = 3;
//            KnotVector knotsU = new KnotVector() { 0, 0, 0, 0, 1, 1, 1, 1 };
//            KnotVector knotsV = new KnotVector() { 0, 0, 0, 0, 1, 1, 1, 1 };

//            List<Vector3> u1 = new List<Vector3>()
//            {
//                new Vector3() { 0d, 0d, 0d },
//                new Vector3() { 10d, 0d, 0d },
//                new Vector3() { 20d, 0d, 0d },
//                new Vector3() { 30d, 0d, 0d }
//            };
//            List<Vector3> u2 = new List<Vector3>()
//            {
//                new Vector3() { 0d, -10d, 0d },
//                new Vector3() { 10d, -10d, 10d },
//                new Vector3() { 20d, -10d, 0d },
//                new Vector3() { 30d, -10d, 0d }
//            };
//            List<Vector3> u3 = new List<Vector3>()
//            {
//                new Vector3() { 0d, -20d, 0d },
//                new Vector3() { 10d, -20d, 0d },
//                new Vector3() { 20d, -20d, 0d },
//                new Vector3() { 30d, -20d, 0d }
//            };
//            List<Vector3> u4 = new List<Vector3>()
//            {
//                new Vector3() { 0d, -30d, 0d },
//                new Vector3() { 10d, -30d, 0d },
//                new Vector3() { 20d, -30d, 0d },
//                new Vector3() { 30d, -30d, 0d }
//            };
//            List<List<Vector3>> controlPoints = new List<List<Vector3>>()
//            {
//                u1, u2, u3, u4
//            };

//            return new NurbsSurface(degreeU, degreeV, knotsU, knotsV, controlPoints);
//        }

//        [Fact]
//        public void It_Returns_A_NurbSurface_By_Four_Points()
//        {
//            var p1 = new Vector3() { 0.0d, 0.0d, 0.0d };
//            var p2 = new Vector3() { 1.0d, 0.0d, 0.0d };
//            var p3 = new Vector3() { 1.0d, 1.0d, 1.0d };
//            var p4 = new Vector3() { 0.0d, 1.0d, 1.0d };

//            KnotVector knotU = new KnotVector { 0.0d, 0.0d, 1.0d, 1.0d };
//            KnotVector knotV = new KnotVector { 0.0d, 0.0d, 1.0d, 1.0d };

//            var nurbsSurface = NurbsSurface.ByFourPoints(p1, p2, p3, p4);
//            nurbsSurface.Should().NotBeNull();
//            nurbsSurface.DegreeU.Should().Be(1);
//            nurbsSurface.DegreeV.Should().Be(1);
//            nurbsSurface.KnotsU.Should().BeEquivalentTo(knotU);
//            nurbsSurface.KnotsV.Should().BeEquivalentTo(knotV);
//        }

//        [Fact]
//        public void It_Returns_The_Surface_Normal_At_A_Given_U_And_V_Parameter()
//        {
//            var nurbsSurface = BuildTestNurbsSurface();
//            var res1 = nurbsSurface.Normal(0.5, 0.5).Unitize();
//            res1.Should().NotBeNullOrEmpty();
//            res1[0].Should().BeApproximately(0.093d, 3);
//            res1[1].Should().BeApproximately(-0.093d, 3);
//            res1[2].Should().BeApproximately(0.991d, 3);

//            var res2 = nurbsSurface.Normal(0.2, 0.7).Unitize();
//            res2.Should().NotBeNullOrEmpty();
//            res2[0].Should().BeApproximately(0.125d, 3);
//            res2[1].Should().BeApproximately(0.060d, 3);
//            res2[2].Should().BeApproximately(0.990d, 3);

//            _testOutput.WriteLine($"Vector1[0.5,0.5]: {res1}\nVector2[0.2,0.7]: {res2}\n");

//        }

//        [Fact]
//        public void It_Returns_The_Surface_Tange_At_A_Given_U_And_V_Parameter_Along_U_Direction()
//        {
//            var nurbsSurface = BuildTestNurbsSurface();
//            var res1 = nurbsSurface.TangentAtU(0.5, 0.5).Unitize();
            
//            res1.Should().NotBeNullOrEmpty();
//            res1[0].Should().BeApproximately(0d, 3);
//            res1[1].Should().BeApproximately(-0.996d, 3);
//            res1[2].Should().BeApproximately(0.093d, 3);

//            var res2 = nurbsSurface.TangentAtU(0.2, 0.7).Unitize();
//            res2.Should().NotBeNullOrEmpty();
//            res2[0].Should().BeApproximately(0d, 3);
//            res2[1].Should().BeApproximately(-0.998d, 3);
//            res2[2].Should().BeApproximately(0.060d, 3);

//            _testOutput.WriteLine($"Vector1[0.5,0.5]: {res1}\nVector2[0.2,0.7]: {res2}\n");
//        }

//        [Fact]
//        public void It_Returns_The_Surface_Tangent_At_A_Given_U_And_V_Parameter_Along_V_Direction()
//        {
//            var nurbsSurface = BuildTestNurbsSurface();
//            var res1 = nurbsSurface.TangentAtV(0.5, 0.5).Unitize();

//            res1.Should().NotBeNullOrEmpty();
//            res1[0].Should().BeApproximately(0.996d, 3);
//            res1[1].Should().BeApproximately(0d, 3);
//            res1[2].Should().BeApproximately(-0.093d, 3);

//            var res2 = nurbsSurface.TangentAtV(0.2, 0.7).Unitize();
//            res2.Should().NotBeNullOrEmpty();
//            res2[0].Should().BeApproximately(0.992d, 3);
//            res2[1].Should().BeApproximately(0d, 3);
//            res2[2].Should().BeApproximately(-0.126d, 3);

//            _testOutput.WriteLine($"Vector1[0.5,0.5]: {res1}\nVector2[0.2,0.7]: {res2}\n");
//        }
//    }
//}
