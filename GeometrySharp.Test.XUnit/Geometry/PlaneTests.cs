using System;
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
        // ToDo verify this test.
        [Fact]
        public void It_Initializes_A_Plane()
        {
            Vector3 origin = new Vector3 { 5, 5, 0 };

            Plane plane = new Plane(origin, Vector3.ZAxis);

            _testOutput.WriteLine(plane.XAxis.ToString());
            _testOutput.WriteLine(plane.YAxis.ToString());
        }
    }
}
