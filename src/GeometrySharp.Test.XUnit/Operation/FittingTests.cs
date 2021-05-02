using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class FittingTests
    {
        private readonly ITestOutputHelper _testOutput;

        public FittingTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void t()
        {
            List<Vector3> pts = new List<Vector3>
            {
                new Vector3 {0, 0, 0},
                new Vector3 {3, 4, 0},
                new Vector3 {-1, 4, 0},
                new Vector3 {-4, 0, 0},
                new Vector3 {-4, -3, 0},
            };

            var c = Fitting.InterpolatedCurve(pts, 3);

            _testOutput.WriteLine(c.ToString());
        }
    }
}
