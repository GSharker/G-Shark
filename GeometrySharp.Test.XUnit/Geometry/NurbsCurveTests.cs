using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using GeometrySharp.XUnit.Core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Geometry
{
    [Trait("Category", "NurbsCurve")]
    public class NurbsCurveTests
    {
        private readonly ITestOutputHelper _testOutput;

        public NurbsCurveTests(ITestOutputHelper testOutput)
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

        public static NurbsCurve NurbsCurvePtsAndWeightsExample()
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

        public static NurbsCurve NurbsCurveExample2()
        {
            var knots = new Knot() { 0.0, 0.0, 0.0, 0.0, 0.33, 0.66, 1.0, 1.0, 1.0, 1.0 };
            var degree = 3;
            var controlPts = new List<Vector3>()
            {
                new Vector3() {5,5,0},
                new Vector3() {10, 10, 0},
                new Vector3() {20, 15, 0},
                new Vector3() {35, 15, 0},
                new Vector3() {45, 10, 0},
                new Vector3() {50, 5, 0}
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        [Fact]
        public void It_Returns_A_NurbsCurve()
        {
            var nurbsCurve = NurbsCurveExample2();
            nurbsCurve.Should().NotBeNull();
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Evaluated_With_A_List_Of_Weights()
        {
            var nurbsCurve = NurbsCurvePtsAndWeightsExample();

            nurbsCurve.Should().NotBeNull();
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Vector3() {10, 0, 0, 0.5});
        }

        [Fact]
        public void It_Returns_A_Copied_NurbsCurve()
        {
            var nurbsCurve = NurbsCurveExample2();
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
            NurbsCurveExample().AreControlPointsHomogenized().Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_NurbsCurve_By_A_Given_Matrix()
        {
            var curve = NurbsCurveExample2();
            var matrix = MatrixTest.TransformationMatrixExample;

            var transformedCurve = curve.Transform(matrix);

            var pt1 = curve.PointAt(0.5);
            var pt2 = transformedCurve.PointAt(0.5);

            var distanceBetweenPts = Math.Round((pt2 - pt1).Length(), 6);

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

        // This values have been compered with NurbsPython lib.
        [Theory]
        [InlineData(0.0, new double[] { 0.7071067811865476, 0.7071067811865476, 0 })]
        [InlineData(0.25, new double[] { 0.9325937445592631, 0.3609278426623971, 0 })]
        [InlineData(0.5, new double[] { 0.9999839615943609, -0.005663616693228194, 0 })]
        [InlineData(0.75, new double[] { 0.9295348056537327, -0.36873438282627974, 0 })]
        [InlineData(1.0, new double[] { 0.7071067811865475, -0.7071067811865475, 0 })]
        public void It_Returns_The_Tangent_At_Give_Point(double t, double[] tangentData)
        {
            // Verb test
            var degree = 3;
            var knots = new Knot() {0, 0, 0, 0, 0.5, 1, 1, 1, 1};
            List<Vector3> pts = new List<Vector3>()
            {
                new Vector3(){0, 0, 0},
                new Vector3(){1, 0, 0},
                new Vector3(){2, 0, 0},
                new Vector3(){3, 0, 0},
                new Vector3(){4, 0, 0}
            };
            var weights = new List<double>() {1, 1, 1, 1, 1};
            var crv = new NurbsCurve(degree, knots, pts, weights);
            var tangent = crv.Tangent(0.5);

            tangent.Should().BeEquivalentTo(new Vector3() {3, 0, 0});

            // Custom test
            var tangentToCheck = NurbsCurveExample2().Tangent(t);
            var tangentNormalized = tangentToCheck.Normalized();
            var tangentExpected = new Vector3(tangentData);

            tangentNormalized.Should().BeEquivalentTo(tangentExpected, option => option
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }

        // This values have been compered with NurbsPython lib.
        [Theory]
        [InlineData(0.0, new double[] { 5.0, 5.0, 0 })]
        [InlineData(0.25, new double[] { 16.352766647188133, 12.60271447254918, 0 })]
        [InlineData(0.5, new double[] { 27.64549230676716, 14.691702224146088, 0 })]
        [InlineData(0.75, new double[] { 38.87409413435116, 12.502152087670986, 0 })]
        [InlineData(1.0, new double[] { 50.0, 5.0, 0 })]
        public void It_Returns_A_Point_At_The_Value(double t, double[] pointData)
        {
            var pointToCheck = LinearAlgebra.Dehomogenize(NurbsCurveExample2().PointAt(t));
            var pointExpected = new Vector3(pointData);

            pointToCheck.Should().BeEquivalentTo(pointExpected, option => option
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6))
                .WhenTypeIs<double>());
        }

    }
}
