using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Evaluation;
using VerbNurbsSharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
    [Trait("Category", "Divide")]
    public class DivideTest
    {
        private readonly ITestOutputHelper _testOutput;

        public DivideTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(0.25)]
        [InlineData(0.5)]
        [InlineData(0.75)]
        public void CurveSplit(double cubicSplit)
        {
            var degree = 3;
            var controlPts = new List<Vector>()
            {
                new Vector(){2,2,0},
                new Vector(){4,12,0},
                new Vector(){7,12,0},
                new Vector(){15,2,0}
            };
            var knots = new KnotArray(degree, controlPts.Count);

            var curve = new NurbsCurve(degree, knots, controlPts);
            var curves = Divide.CurveSplit(curve, cubicSplit);

            for (int i = 0; i < degree + 1; i++)
            {
                var d = curves[0].Knots.Count - (degree + 1);
                curves[0].Knots[d + i].Should().BeApproximately(cubicSplit, Constants.TOLERANCE);
            }

            for (int i = 0; i < degree + 1; i++)
            {
                var d = 0;
                curves[1].Knots[d + i].Should().BeApproximately(cubicSplit, Constants.TOLERANCE);
            }

            curves.Should().HaveCount(2);

            _testOutput.WriteLine(curves[0].ToString());
            _testOutput.WriteLine(curves[1].ToString());
        }
    }
}
