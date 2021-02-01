using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using GeometrySharp.Test.XUnit.Data;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Evaluation
{
    public class ModifyTests
	{
		private readonly ITestOutputHelper _testOutput;

		public ModifyTests(ITestOutputHelper testOutput)
		{
			_testOutput = testOutput;
		}

        [Fact]
        public void It_Returns_A_Transformed_NurbsCurve_Using_A_Matrix()
        {
            var curve = NurbsCurveCollection.NurbsCurveExample();
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

            resultedCurve.HomogenizedPoints.Should().BeEquivalentTo(expectedControlPts);
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

            (knots.Count + insertion).Should().Be(curveAfterRefine.Knots.Count);
            (controlPts.Count + insertion).Should().Be(curveAfterRefine.ControlPoints.Count);

            var p0 = curve.PointAt(2.5);
            var p1 = curveAfterRefine.PointAt(2.5);

            p0[0].Should().BeApproximately(p1[0], GeoSharpMath.MAXTOLERANCE);
            p0[1].Should().BeApproximately(p1[1], GeoSharpMath.MAXTOLERANCE);
            p0[2].Should().BeApproximately(p1[2], GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Decomposes_The_Curve_Into_Bezier_Curve_Segments()
        {
            var degree = 3;
            var knots = new Knot() { 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 };

            var controlPts = new List<Vector3>();
            for (int i = 0; i <= knots.Count - degree - 2; i++)
                controlPts.Add(new Vector3() { i, 0.0, 0.0 });

            var curve = new NurbsCurve(degree, knots, controlPts);
            var curvesAfterDecompose = Modify.DecomposeCurveIntoBeziers(curve);

            curvesAfterDecompose.Count.Should().Be(5);

            foreach (var bezierCurve in curvesAfterDecompose)
            {
                var t = bezierCurve.Knots[0];
                var pt0 = bezierCurve.PointAt(t);
                var pt1 = curve.PointAt(t);

                var pt0_pt1 = (pt0 - pt1).Length();

                pt0_pt1.Should().BeApproximately(0.0, GeoSharpMath.MAXTOLERANCE);
            }
        }
    }
}
