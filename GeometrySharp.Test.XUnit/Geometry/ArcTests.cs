using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class ArcTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ArcTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void Initializes_An_Arc()
        {
            double angle = GeoSharpMath.ToRadians(40);

            Arc arc = new Arc(Plane.PlaneXY, 15, angle);

            arc.Should().NotBeNull();
            arc.Length.Should().BeApproximately(10.471976, GeoSharpMath.MAXTOLERANCE);
            arc.Center.Should().BeEquivalentTo(Plane.PlaneXY.Origin);
            arc.Radius.Should().Be(15);
            arc.Angle.Should().BeApproximately(0.698132, GeoSharpMath.MAXTOLERANCE);
        }
    }
}
