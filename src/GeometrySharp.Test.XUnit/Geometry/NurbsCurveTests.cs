using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Test.XUnit.Core;
using GeometrySharp.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Operation;
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
            new List<Vector3>
            {
                new Vector3 {-10,15,5},
                new Vector3 {10,5,5},
                new Vector3 {20,0,0}
            },
            new Knot { 1, 1, 1, 1, 1, 1 },
            new List<double> { 0.5, 0.5, 0.5 }
        );

        [Fact]
        public void It_Returns_A_NurbsCurve()
        {
            // Act
            var nurbsCurve = NurbsCurveCollection.NurbsCurvePlanarExample();

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.Degree.Should().Be(3);
            nurbsCurve.Weights.Should().BeEquivalentTo(Sets.RepeatData(1.0, 6));
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Evaluated_With_A_List_Of_Weights()
        {
            // Act
            var nurbsCurve = NurbsCurveCollection.NurbsCurvePtsAndWeightsExample();

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.HomogenizedPoints[2].Should().BeEquivalentTo(new Vector3 { 10, 0, 0, 0.5 });
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Vector3 { 20, 0, 0 });
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_From_ControlPoints_And_Degree()
        {
            // Act
            var nurbsCurve = new NurbsCurve(CurveData.pts, CurveData.degree);

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.Degree.Should().Be(2);
            nurbsCurve.Weights.Should().BeEquivalentTo(Sets.RepeatData(1.0, CurveData.pts.Count));
            nurbsCurve.Knots.Should().BeEquivalentTo(new Knot(CurveData.degree, CurveData.pts.Count));
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_ControlPoints_Are_Null()
        {
            // Act
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, CurveData.knots, null, CurveData.weights);

            // Assert
            curve.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Knots_Are_Null()
        {
            // Act
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, null, CurveData.pts, CurveData.weights);

            // Assert
            curve.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_A_Planar_Curve()
        {
            // Arrange
            NurbsCurve crv0 = NurbsCurveCollection.NurbsCurveCubicBezierPlanar();
            NurbsCurve crv1 = NurbsCurveCollection.NurbsCurveQuadraticBezierPlanar();

            Vector3 expectedPtMin0 = new Vector3 { 0, 0, 0 };
            Vector3 expectedPtMax0 = new Vector3 { 2, 0.444444, 0 };

            Vector3 expectedPtMin1 = new Vector3 { -10, 0, 0 };
            Vector3 expectedPtMax1 = new Vector3 { 20, 15, 5 };

            // Act
            BoundingBox bBox0 = crv0.BoundingBox;
            BoundingBox bBox1 = crv1.BoundingBox;

            // Assert
            bBox0.Max.DistanceTo(expectedPtMax0).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            bBox0.Min.DistanceTo(expectedPtMin0).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);

            bBox1.Max.DistanceTo(expectedPtMax1).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            bBox1.Min.DistanceTo(expectedPtMin1).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_A_3D_Nurbs_Curve()
        {
            // Arrange
            NurbsCurve crv0 = NurbsCurveCollection.NurbsCurve3DExample();
            NurbsCurve crv1 = NurbsCurveCollection.NurbsCurveQuadratic3DBezier();

            Vector3 expectedPtMin0 = new Vector3 { 0, 0.5555556, 0 };
            Vector3 expectedPtMax0 = new Vector3 { 4.089468, 5, 5 };

            Vector3 expectedPtMin1 = new Vector3 { 0, 2.5, 0 };
            Vector3 expectedPtMax1 = new Vector3 { 4.545455, 5, 3.333333 };

            // Act
            BoundingBox bBox0 = crv0.BoundingBox;
            BoundingBox bBox1 = crv1.BoundingBox;

            // Assert
            bBox0.Max.DistanceTo(expectedPtMax0).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            bBox0.Min.DistanceTo(expectedPtMin0).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);

            bBox1.Max.DistanceTo(expectedPtMax1).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            bBox1.Min.DistanceTo(expectedPtMin1).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Degree_is_Less_Than_1()
        {
            // Act
            Func<NurbsCurve> curve = () => new NurbsCurve(0, CurveData.knots, CurveData.pts, CurveData.weights);

            // Assert
            curve.Should().Throw<ArgumentException>()
                .WithMessage("Degree must be greater than 1!");
        }

        // Confirm the relations between degree(p), number of control points(n+1), and the number of knots(m+1).
        // m = p + n + 1
        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Is_Not_Valid_The_Relation_Between_Pts_Degree_Knots()
        {
            // Act
            Func<NurbsCurve> curve = () => new NurbsCurve(1, CurveData.knots, CurveData.pts, CurveData.weights);

            // Assert
            curve.Should().Throw<ArgumentException>()
                .WithMessage("Number of points + degree + 1 must equal knots length!");
        }

        [Fact]
        public void NurbsCurve_Throws_An_Exception_If_Knots_Are_Not_Valid()
        {
            // Arrange
            var knots = new Knot { 0, 0, 1, 1, 2, 2 };

            // Act
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, knots, CurveData.pts, CurveData.weights);

            // Assert
            curve.Should().Throw<ArgumentException>()
                .WithMessage("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
        }

        [Fact]
        public void It_Returns_A_Copied_NurbsCurve()
        {
            // Arrange
            var nurbsCurve = NurbsCurveCollection.NurbsCurvePlanarExample();
            
            // Act
            var copiedNurbs = nurbsCurve.Clone();

            // Assert
            copiedNurbs.Should().NotBeNull();
            copiedNurbs.Equals(nurbsCurve).Should().BeTrue();
            copiedNurbs.Should().NotBeSameAs(nurbsCurve); // Checks at reference level are different.
            copiedNurbs.Degree.Should().Be(nurbsCurve.Degree);
            copiedNurbs.Weights.Should().BeEquivalentTo(nurbsCurve.Weights);
        }

        [Fact]
        public void It_Returns_The_Domain_Of_The_Curve()
        {
            // Act
            var curveDomain = NurbsCurveCollection.NurbsCurveExample().Domain;

            // Assert
            curveDomain.Min.Should().Be(NurbsCurveCollection.NurbsCurveExample().Knots.First());
            curveDomain.Max.Should().Be(NurbsCurveCollection.NurbsCurveExample().Knots.Last());
        }

        [Fact]
        public void It_Returns_A_Transformed_NurbsCurve_By_A_Given_Matrix()
        {
            // Arrange
            var curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            var transform = Transform.Translation(new Vector3 { -10, 20, 0 });

            // Act
            var transformedCurve = curve.Transform(transform);
            var pt1 = curve.PointAt(0.5);
            var pt2 = transformedCurve.PointAt(0.5);
            var distanceBetweenPts = System.Math.Round((pt2 - pt1).Length(), 6);

            // Assert
            distanceBetweenPts.Should().Be(22.36068);
        }
    }
}
