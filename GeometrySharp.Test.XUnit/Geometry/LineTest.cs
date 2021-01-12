using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Geometry
{
    public class LineTest
    {
        private readonly ITestOutputHelper _testOutput;
        public LineTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Trait("Category", "Line")]
        [Fact]
        public void Create_Line_By_Two_Points()
        {
            var l = new Line(new Vector3 { -0.913, 1.0, 4.68 }, new Vector3 { 6.363, 10.0, 7.971 });
            double dynL = 12.03207;
            l.Length.Should().BeApproximately(dynL, 5);
        }
    }
}
