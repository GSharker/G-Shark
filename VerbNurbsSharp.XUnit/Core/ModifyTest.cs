using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Evaluation;
using VerbNurbsSharp.Geometry;
using VerbNurbsSharp.XUnit.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
	[Trait("Category", "Modify")]
	public class ModifyTest
	{
		private readonly ITestOutputHelper _testOutput;

		public ModifyTest(ITestOutputHelper testOutput)
		{
			_testOutput = testOutput;
		}

		[Fact]
		public void Check_CurveKnotRefine()
		{
			//arrange
			NurbsCurve inputCurve = null;
			NurbsCurve expectedCurve = null;
			//act
			//assert
			Assert.Equal(expectedCurve, inputCurve);
		}

        [Fact]
        public void TransformNurbsCurve_UsingAMatrix()
        {
            var curve = NurbsCurveTest.NurbsCurveExample();
            var mat = new Matrix() {
                new List<double>{1.0, 0.0, 0.0, -10.0 },
                new List<double>{0.0, 1.0, 0.0, 20.0 },
                new List<double>{0.0, 0.0, 1.0, 1.0 },
                new List<double>{0.0, 0.0, 0.0, 1.0 }
            };

			var expectedControlPts = new List<Vector>()
            {
                new Vector(){-20.0, 35.0, 6.0, 1.0 },
                new Vector(){0.0, 25.0, 6.0, 1.0 },
                new Vector(){10.0, 20.0, 1.0, 1.0 },
			};

            var resultedCurve = Modify.RationalCurveTransform(curve, mat);

            resultedCurve.ControlPoints.Should().BeEquivalentTo(expectedControlPts);
        }

        [Fact]
        public void CurveKnotRefine()
        {
            var degree = 3;
            var knots = new KnotArray(){ 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5};
            var newKnots = Sets.RepeatData(2.5, 1);

            var controlPts = new List<Vector>();
            for (int i = 0; i <= knots.Count - 3 - 2; i++)
            {
                controlPts.Add(new Vector(){i,0.0,0.0});
            }

            var curve = new NurbsCurve(degree, knots, controlPts);
            var curveAfterRefine = Modify.CurveKnotRefine(curve, newKnots);

            _testOutput.WriteLine(curveAfterRefine.ToString());
        }
    }
}
