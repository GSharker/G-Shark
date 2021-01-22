using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.XUnit.Core;
using Xunit;
using Xunit.Abstractions;
using Math = System.Math;

namespace GeometrySharp.XUnit.Geometry
{
    [Trait("Category", "NurbsCurve")]
    public class NurbsCurveTest
    {
        private readonly ITestOutputHelper _testOutput;

        public NurbsCurveTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static NurbsCurve NurbsCurveExample()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>()
            {
                new Vector3(){-10,15,5},
                new Vector3(){10,5,5},
                new Vector3(){20,0,0}
            };
            Knot knots = new Knot() { 1, 1, 1, 1, 1, 1 };

            return new NurbsCurve(degree, knots, pts);
        }

        public static NurbsCurve NurbsCurveHomogenizedPtsExample()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>()
            {
                new Vector3(){-10,15,5},
                new Vector3(){10,5,5},
                new Vector3(){20,0,0}
            };
            Knot knots = new Knot() { 1, 1, 1, 1, 1, 1 };
            var weights = new List<double>() { 0.5, 0.5, 0.5 };

            return new NurbsCurve(degree, knots, pts, weights);
        }

        [Fact]
        public void It_Returns_A_NurbsCurve()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>()
            {
                new Vector3(){-10,15,5},
                new Vector3(){10,5,5},
                new Vector3(){20,0,0}
            };
            Knot knots = new Knot() {1, 1, 1};

            var nurbsCurve = new NurbsCurve(degree, knots, pts);

            nurbsCurve.Should().NotBeNull();
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Evaluated_With_A_List_Of_Weights()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>()
            {
                new Vector3(){-10,15,5},
                new Vector3(){10,5,5},
                new Vector3(){20,0,0}
            };
            Knot knots = new Knot() { 1, 1, 1 };
            var weights = new List<double>() {0.5, 0.5, 0.5};

            var nurbsCurve = new NurbsCurve(degree, knots, pts, weights);

            nurbsCurve.Should().NotBeNull();
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Vector3() {10, 0, 0, 0.5});
        }

        [Fact]
        public void It_Returns_A_Copied_NurbsCurve()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>()
            {
                new Vector3(){-10,15,5},
                new Vector3(){10,5,5},
                new Vector3(){20,0,0}
            };
            Knot knots = new Knot() { 1, 1, 1, 1, 1, 1};

            var nurbsCurve = new NurbsCurve(degree, knots, pts);
            var copiedNurbs = nurbsCurve.Clone();

            copiedNurbs.Should().NotBeNull();
            copiedNurbs.Equals(nurbsCurve).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Domain_Of_The_Curve()
        {
            var curveDomain = NurbsCurveExample().Domain();

            curveDomain.Min.Should().Be(NurbsCurveExample().Knots.First());
            curveDomain.Max.Should().Be(NurbsCurveExample().Knots.Last());
        }

        [Fact]
        public void It_Checks_If_The_Control_Points_Are_Homogenized()
        {
            NurbsCurveExample().AreControlPointsHomogenized().Should().BeFalse();
            NurbsCurveHomogenizedPtsExample().AreControlPointsHomogenized().Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_NurbsCurve_By_A_Given_Matrix()
        {
            var curve = NurbsCurveExample();
            var matrix = MatrixTest.TransformationMatrixExample;

            var transformedCurve = curve.Transform(matrix);
            var demoPts = LinearAlgebra.Dehomogenize1d(transformedCurve.ControlPoints);

            var distanceBetweenPts =
                Math.Round((demoPts[0] - curve.ControlPoints[0]).Length(), 6);

            distanceBetweenPts.Should().Be(22.383029);
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(3.5)]
        public void It_Splits_A_Curve_Returning_Two_Curves(double parameter)
        {
            // Arrange
            var degree = 3;
            var knots = new Knot() { 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 };
            var controlPts = new List<Vector3>();
            for (int i = 0; i <= knots.Count - 3 - 2; i++)
            {
                controlPts.Add(new Vector3() { i, 0.0, 0.0 });
            }
            var weights = Sets.RepeatData(1.0, controlPts.Count);
            var curve = new NurbsCurve(degree, knots, controlPts, weights);

            // Act
            var splitCurves = curve.Split(parameter);

            // Assert
            for (var i = 0; i < degree + 1; i++)
            {
                var d = splitCurves[0].Knots.Count- (degree + 1);
                splitCurves[0].Knots[d + i].Should().BeApproximately(parameter, GeoSharpMath.TOLERANCE);
            }

            for (var i = 0; i < degree + 1; i++)
            {
                var d = 0;
                splitCurves[1].Knots[d + i].Should().BeApproximately(parameter, GeoSharpMath.TOLERANCE);
            }

            splitCurves.Should().HaveCount(2);
            splitCurves[0].ControlPoints.Last().Should().BeEquivalentTo(splitCurves[1].ControlPoints.First());
        }
    }
}
