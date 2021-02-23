using Xunit.Abstractions;
using FluentAssertions;
using GeometrySharp.Geometry;
using Xunit;
using GeometrySharp.Core;
using System.Collections.Generic;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class NurbsSurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;
        public NurbsSurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_NurbSurface_By_Four_Points()
        {
            var p1 = new Vector3() { 0.0d, 0.0d, 0.0d };
            var p2 = new Vector3() { 1.0d, 0.0d, 0.0d };
            var p3 = new Vector3() { 1.0d, 1.0d, 1.0d };
            var p4 = new Vector3() { 0.0d, 1.0d, 1.0d };

            Knot knotU = new Knot { 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 1.0d, 1.0d, 1.0d };
            Knot knotV = new Knot { 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 1.0d, 1.0d, 1.0d };

            var nurbsSurface = NurbsSurface.ByFourPoints(p1, p2, p3, p4);
            nurbsSurface.Should().NotBeNull();
            nurbsSurface.DegreeU.Should().Be(3);
            nurbsSurface.DegreeV.Should().Be(3);
            nurbsSurface.KnotsU.Should().BeEquivalentTo(knotU);
            nurbsSurface.KnotsV.Should().BeEquivalentTo(knotV);
        }
    }
}
