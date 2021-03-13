using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class IntersectionTests
    {
        private readonly ITestOutputHelper _testOutput;
        public IntersectionTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_Two_Planes()
        {
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneYZ.SetOrigin(new Vector3{10,10,5});
            Plane pl2 = Plane.PlaneXZ.SetOrigin(new Vector3 {10, -10, -5});

            Ray intersection0 = Intersect.PlaneToPlane(pl0, pl1);
            Ray intersection1 = Intersect.PlaneToPlane(pl1, pl2);

            intersection0.Position.Should().BeEquivalentTo(new Vector3 {10, 0, 0});
            intersection0.Direction.Should().BeEquivalentTo(new Vector3 { 0, 1, 0 });

            intersection1.Position.Should().BeEquivalentTo(new Vector3 { 10, -10, 0 });
            intersection1.Direction.Should().BeEquivalentTo(new Vector3 { 0, 0, 1 });
        }
    }
}
