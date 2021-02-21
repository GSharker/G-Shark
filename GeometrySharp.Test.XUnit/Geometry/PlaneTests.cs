using System;
using FluentAssertions;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class PlaneTests
    {
        private readonly ITestOutputHelper _testOutput;
        public PlaneTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Initializes_A_Plane()
        {
            Vector3 origin = new Vector3 { 5, 5, 0 };
            Vector3 dir = new Vector3 { -10, -15, 0 };

            Plane plane = new Plane(origin, dir);

            plane.XAxis.IsEqualRoundingDecimal(new Vector3 { -0.83205, 0.5547, 0 }, 6).Should().BeTrue();
            plane.YAxis.IsEqualRoundingDecimal(new Vector3 { 0, 0, -1 }, 6).Should().BeTrue();
            plane.ZAxis.IsEqualRoundingDecimal(new Vector3 { -0.5547, -0.83205, 0 }, 6).Should().BeTrue();
            plane.Origin.Equals(origin).Should().BeTrue();
        }
    }
}
