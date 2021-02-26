using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class PolylineTests
    {
        private readonly ITestOutputHelper _testOutput;
        public PolylineTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        public static Vector3[] ExamplePts => new[] { new Vector3 { 5, 0, 0 }, new Vector3 { 15, 15, 0 }, new Vector3 { 20, 5, 0 }, new Vector3 { 30, 10, 0 } };

        [Fact]
        public void It_Returns_A_Polyline()
        {
            Polyline polyline = new Polyline(ExamplePts);

            polyline.Count.Should().Be(ExamplePts.Length);
            polyline[0].Should().BeEquivalentTo(ExamplePts[0]);
        }

        [Fact]
        public void Polyline_Throws_An_Exception_If_Vertex_Count_Is_Less_Than_Two()
        {
            Vector3[] pts = new[] {new Vector3 {5, 0, 0}};

            Func<Polyline> func = () => new Polyline(pts);

            func.Should().Throw<Exception>().WithMessage("Insufficient points for a polyline.");
        }

        [Fact]
        public void It_Returns_A_Polyline_Removing_Short_Segments()
        {
            Vector3[] pts = new[] { new Vector3 { 5, 5, 0 }, new Vector3 { 5, 10, 0 }, new Vector3 { 5, 10, 0 }, 
                new Vector3 { 15,12,0 }, new Vector3 { 20,-20,0 }, new Vector3 { 20, -20, 0 } };

            Vector3[] ptsExpected = new[] { new Vector3 { 5, 5, 0 }, new Vector3 { 5, 10, 0 }, new Vector3 { 15,12,0 }, new Vector3 { 20,-20,0 }};

            Polyline polyline = new Polyline(pts);

            polyline.Should().BeEquivalentTo(ptsExpected);
            _testOutput.WriteLine(polyline.ToString());
        }
    }
}
