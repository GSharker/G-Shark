using System.Linq;
using FluentAssertions;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class LineTests
    {
        private readonly ITestOutputHelper _testOutput;
        public LineTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_Line()
        {
            var p1 = new Vector3 {-0.913, 1.0, 4.68};
            var p2 = new Vector3 {6.363, 10.0, 7.971};
            var l = new Line(p1, p2);
            
            l.Should().NotBeNull();
            l.Start.All(p1.Contains).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_Line()
        {
            var p1 = new Vector3 { -0.913, 1.0, 4.68 };
            var p2 = new Vector3 { 6.363, 10.0, 7.971 };
            var l = new Line(p1, p2);
            double expectedLength = 12.03207;

            l.Length.Should().BeApproximately(expectedLength, 5);
        }

        [Fact]
        public void It_Returns_The_Line_Direction()
        {
            var p1 = new Vector3 { 0, 0, 0 };
            var p2 = new Vector3 { 5, 0, 0 };
            var l = new Line(p1, p2);
            var expectedDirection = new Vector3 { 1, 0, 0 };

            l.Direction.Should().BeEquivalentTo(expectedDirection);
        }

        [Fact]
        public void It_Returns_The_ClosestPoint()
        {
            var line = new Line(new Vector3{ 0, 0, 0 }, new Vector3{ 30, 45, 0 });
            var pt = new Vector3{ 10, 20, 0 };
            var expectedPt = new Vector3{ 12.30769230769231, 18.461538461538463, 0 };

            var closestPt = line.ClosestPoint(pt);

            closestPt.Should().BeEquivalentTo(expectedPt);
        }
    }
}
