using System;
using FluentAssertions;
using GeometrySharp.Core;
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

        public static Plane BasePlane => new Plane(new Vector3 {5, 0, 0}, new Vector3 {10, 15, 0});

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

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            Plane plane = BasePlane;
            Vector3 pt = new Vector3{7,7,3};

            Vector3 closestPt = plane.ClosestPoint(pt, out double distance);

            closestPt.IsEqualRoundingDecimal(new Vector3 {3.153846, 1.230769, 3}, 6).Should().BeTrue();
            distance.Should().BeApproximately(6.933752, GeoSharpMath.MAXTOLERANCE);
        }
    }
}
