using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using GeometrySharp.XUnit.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Evaluation
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
        public void It_Returns_A_Transformed_NurbsCurve_Using_A_Matrix()
        {
            var curve = NurbsCurveTests.NurbsCurveExample();
            var mat = new Matrix() {
                new List<double>{1.0, 0.0, 0.0, -10.0 },
                new List<double>{0.0, 1.0, 0.0, 20.0 },
                new List<double>{0.0, 0.0, 1.0, 1.0 },
                new List<double>{0.0, 0.0, 0.0, 1.0 }
            };

			var expectedControlPts = new List<Vector3>()
            {
                new Vector3(){-20.0, 35.0, 6.0, 1.0 },
                new Vector3(){0.0, 25.0, 6.0, 1.0 },
                new Vector3(){10.0, 20.0, 1.0, 1.0 },
			};

            var resultedCurve = Modify.RationalCurveTransform(curve, mat);

            resultedCurve.ControlPoints.Should().BeEquivalentTo(expectedControlPts);
        }

        [Theory]
        [InlineData(2.5 ,1)]
        [InlineData(2.5, 2)]
        [InlineData(2.5, 3)]
        [InlineData(2.5, 4)]
        [InlineData(0.5, 1)]
        [InlineData(0.5, 2)]
        [InlineData(0.5, 3)]
        [InlineData(0.5, 4)]
        [InlineData(3.0, 1)]
        [InlineData(3.0, 2)]
        public void It_Refines_The_Curve_Knot(double val, int insertion)
        {
            var degree = 3;
            var knots = new Knot(){ 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5};

            var newKnots = new List<double>();
            for (int i = 0; i < insertion; i++)
                newKnots.Add(val);

            var controlPts = new List<Vector3>();
            for (int i = 0; i <= knots.Count - degree - 2; i++)
                controlPts.Add(new Vector3(){i,0.0,0.0});

            var curve = new NurbsCurve(degree, knots, controlPts);
            var curveAfterRefine = Modify.CurveKnotRefine(curve, newKnots);

            _testOutput.WriteLine(curveAfterRefine.ToString());
            _testOutput.WriteLine(knots.Count.ToString());
            _testOutput.WriteLine(curveAfterRefine.Knots.Count.ToString());

            (knots.Count + insertion).Should().Be(curveAfterRefine.Knots.Count);
            (controlPts.Count + insertion).Should().Be(curveAfterRefine.ControlPoints.Count);

            // ToDo add this part of the test
            /*
            var p0 = verb.eval.Eval.curvePoint( crv, 2.5);
		    var p1 = verb.eval.Eval.curvePoint( after, 2.5);

		    p0[0].should.be.approximately(p1[0], verb.core.GeoSharpMath.TOLERANCE);
		    p0[1].should.be.approximately(p1[1], verb.core.GeoSharpMath.TOLERANCE);
		    p0[2].should.be.approximately(p1[2], verb.core.GeoSharpMath.TOLERANCE);
            */
        }
    }
}
