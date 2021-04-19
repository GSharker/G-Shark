using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Test.XUnit.Core;
using GeometrySharp.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class NurbsCurveTests
    {
        private readonly ITestOutputHelper _testOutput;

        public NurbsCurveTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static (int degree, List<Vector3> pts, Knot knots, List<double> weights) CurveData =>
        (
            2,
            new List<Vector3>()
            {
                new Vector3(){-10,15,5},
                new Vector3(){10,5,5},
                new Vector3(){20,0,0}
            },
            new Knot() { 1, 1, 1, 1, 1, 1 },
            new List<double>() { 0.5, 0.5, 0.5 }
        );

        [Fact]
        public void It_Returns_A_NurbsCurve()
        {
            var nurbsCurve = NurbsCurveCollection.NurbsCurveExample2();

            nurbsCurve.Should().NotBeNull();
            nurbsCurve.Degree.Should().Be(3);
            nurbsCurve.Weights.Should().BeEquivalentTo(Sets.RepeatData(1.0, 6));
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Evaluated_With_A_List_Of_Weights()
        {
            var nurbsCurve = NurbsCurveCollection.NurbsCurvePtsAndWeightsExample();

            nurbsCurve.Should().NotBeNull();
            nurbsCurve.HomogenizedPoints[2].Should().BeEquivalentTo(new Vector3() { 10, 0, 0, 0.5 });
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Vector3() { 20, 0, 0 });
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_From_ControlPoints_And_Degree()
        {
            var nurbsCurve = new NurbsCurve(CurveData.pts, CurveData.degree);

            nurbsCurve.Should().NotBeNull();
            nurbsCurve.Degree.Should().Be(2);
            nurbsCurve.Weights.Should().BeEquivalentTo(Sets.RepeatData(1.0, CurveData.pts.Count));
            nurbsCurve.Knots.Should().BeEquivalentTo(new Knot(CurveData.degree, CurveData.pts.Count));
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_ControlPoints_Are_Null()
        {
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, CurveData.knots, null, CurveData.weights);

            curve.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Knots_Are_Null()
        {
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, null, CurveData.pts, CurveData.weights);

            curve.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Degree_is_Less_Than_1()
        {
            Func<NurbsCurve> curve = () => new NurbsCurve(0, CurveData.knots, CurveData.pts, CurveData.weights);

            curve.Should().Throw<ArgumentException>()
                .WithMessage("Degree must be greater than 1!");
        }

        // Confirm the relations between degree(p), number of control points(n+1), and the number of knots(m+1).
        // m = p + n + 1
        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Is_Not_Valid_The_Relation_Between_Pts_Degree_Knots()
        {
            Func<NurbsCurve> curve = () => new NurbsCurve(1, CurveData.knots, CurveData.pts, CurveData.weights);

            curve.Should().Throw<ArgumentException>()
                .WithMessage("Number of points + degree + 1 must equal knots length!");
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Knots_Are_Not_Valid()
        {
            var knots = new Knot() { 0, 0, 1, 1, 2, 2 };
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, knots, CurveData.pts, CurveData.weights);

            curve.Should().Throw<ArgumentException>()
                .WithMessage("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
        }

        [Fact]
        public void It_Returns_A_Copied_NurbsCurve()
        {
            var nurbsCurve = NurbsCurveCollection.NurbsCurveExample2();
            var copiedNurbs = nurbsCurve.Clone();

            copiedNurbs.Should().NotBeNull();
            copiedNurbs.Equals(nurbsCurve).Should().BeTrue();
            copiedNurbs.Should().NotBeSameAs(nurbsCurve); // Checks at reference level are different.
            copiedNurbs.Degree.Should().Be(nurbsCurve.Degree);
            copiedNurbs.Weights.Should().BeEquivalentTo(nurbsCurve.Weights);
        }

        [Fact]
        public void It_Returns_The_Domain_Of_The_Curve()
        {
            var curveDomain = NurbsCurveCollection.NurbsCurveExample().Domain;

            curveDomain.Min.Should().Be(NurbsCurveCollection.NurbsCurveExample().Knots.First());
            curveDomain.Max.Should().Be(NurbsCurveCollection.NurbsCurveExample().Knots.Last());
        }

        [Fact]
        public void It_Returns_A_Transformed_NurbsCurve_By_A_Given_Matrix()
        {
            var curve = NurbsCurveCollection.NurbsCurveExample2();
            var matrix = MatrixTests.TransformationMatrixExample;

            var transformedCurve = curve.Transform(matrix);

            var pt1 = curve.PointAt(0.5);
            var pt2 = transformedCurve.PointAt(0.5);

            var distanceBetweenPts = System.Math.Round((pt2 - pt1).Length(), 6);

            distanceBetweenPts.Should().Be(22.383029);
        }
    }
}
