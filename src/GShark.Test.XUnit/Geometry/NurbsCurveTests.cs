using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class NurbsCurveTests
    {
        private readonly ITestOutputHelper _testOutput;

        public NurbsCurveTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static (int degree, List<Vector3> pts, KnotVector knots, List<double> weights) CurveData =>
        (
            2,
            new List<Vector3>
            {
                new Vector3 {-10,15,5},
                new Vector3 {10,5,5},
                new Vector3 {20,0,0}
            },
            new KnotVector { 1, 1, 1, 1, 1, 1 },
            new List<double> { 0.5, 0.5, 0.5 }
        );

        [Fact]
        public void It_Returns_A_NurbsCurve()
        {
            // Act
            NurbsCurve nurbsCurve = NurbsCurveCollection.NurbsCurvePlanarExample();

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.Degree.Should().Be(3);
            nurbsCurve.Weights.Should().BeEquivalentTo(Sets.RepeatData(1.0, 6));
        }

        [Fact]
        public void It_Returns_A_Closed_NurbsCurve()
        {
            // Arrange
            int degree = 2;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {4.5,2.5,2.5},
                new Vector3 {5,5,5},
                new Vector3 {0,5,0}
            };
            Vector3 expectedPt00 = new Vector3 {3.25, 3.28125, 1.875};
            Vector3 expectedPt01 = new Vector3 { 4.75, 3.75, 3.75 };

            // Act
            NurbsCurve nurbsCurve = new NurbsCurve(controlPts, degree).Close();
            Vector3 ptAt00 = nurbsCurve.PointAt(0.75);
            Vector3 ptAt01 = nurbsCurve.PointAt(1);

            // Assert
            nurbsCurve.ControlPoints.Count.Should().Be(5);
            nurbsCurve.ControlPoints[1].DistanceTo(nurbsCurve.ControlPoints[^1]).Should().BeLessThan(GeoSharpMath.EPSILON);
            nurbsCurve.Knots.Count.Should().Be(8);
            nurbsCurve.Domain.T0.Should().Be(0.0);
            nurbsCurve.Domain.T1.Should().Be(1.0);
            expectedPt00.DistanceTo(ptAt00).Should().BeLessThan(GeoSharpMath.EPSILON);
            expectedPt01.DistanceTo(ptAt01).Should().BeLessThan(GeoSharpMath.EPSILON);
        }

        [Fact]
        public void It_Returns_True_If_A_NurbsCurve_Is_Closed()
        {
            // Assert
            NurbsCurveCollection.PeriodicClosedNurbsCurve().IsClosed().Should().BeTrue();
            NurbsCurveCollection.NurbsCurveWithStartingAndEndPointOverlapping().IsClosed().Should().BeTrue();
            NurbsCurveCollection.NurbsCurveExample().IsClosed().Should().BeFalse();
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Evaluated_With_A_List_Of_Weights()
        {
            // Act
            NurbsCurve nurbsCurve = NurbsCurveCollection.NurbsCurvePtsAndWeightsExample();

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.HomogenizedPoints[2].Should().BeEquivalentTo(new Vector3 { 10, 0, 0, 0.5 });
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Vector3 { 20, 0, 0 });
        }

        [Fact]
        public void It_Creates_A_NurbsCurve_From_ControlPoints_And_Degree()
        {
            // Act
            NurbsCurve nurbsCurve = new NurbsCurve(CurveData.pts, CurveData.degree);

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.Degree.Should().Be(2);
            nurbsCurve.Weights.Should().BeEquivalentTo(Sets.RepeatData(1.0, CurveData.pts.Count));
            nurbsCurve.Knots.Should().BeEquivalentTo(new KnotVector(CurveData.degree, CurveData.pts.Count));
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
            bBox0.Max.DistanceTo(expectedPtMax0).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
            bBox0.Min.DistanceTo(expectedPtMin0).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);

            bBox1.Max.DistanceTo(expectedPtMax1).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
            bBox1.Min.DistanceTo(expectedPtMin1).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
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
            bBox0.Max.DistanceTo(expectedPtMax0).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
            bBox0.Min.DistanceTo(expectedPtMin0).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);

            bBox1.Max.DistanceTo(expectedPtMax1).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
            bBox1.Min.DistanceTo(expectedPtMin1).Should().BeLessThan(GeoSharpMath.MAX_TOLERANCE);
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
            KnotVector knots = new KnotVector { 0, 0, 1, 1, 2, 2 };

            // Act
            Func<NurbsCurve> curve = () => new NurbsCurve(CurveData.degree, knots, CurveData.pts, CurveData.weights);

            // Assert
            curve.Should().Throw<ArgumentException>()
                .WithMessage("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
        }

        [Fact]
        public void It_Returns_A_Copy_Of_A_NurbsCurve()
        {
            // Arrange
            NurbsCurve nurbsCurve = NurbsCurveCollection.NurbsCurvePlanarExample();

            // Act
            NurbsCurve nurbsCurveCopy = nurbsCurve.Clone();

            // Assert
            nurbsCurveCopy.Should().NotBeNull();
            nurbsCurveCopy.Equals(nurbsCurve).Should().BeTrue();
            nurbsCurveCopy.Should().NotBeSameAs(nurbsCurve); // Checks at reference level are different.
            nurbsCurveCopy.Degree.Should().Be(nurbsCurve.Degree);
            nurbsCurveCopy.Weights.Should().BeEquivalentTo(nurbsCurve.Weights);
        }

        [Fact]
        public void It_Returns_The_Domain_Of_The_Curve()
        {
            // Act
            Interval curveDomain = NurbsCurveCollection.NurbsCurveExample().Domain;

            // Assert
            curveDomain.T0.Should().Be(NurbsCurveCollection.NurbsCurveExample().Knots.First());
            curveDomain.T1.Should().Be(NurbsCurveCollection.NurbsCurveExample().Knots.Last());
        }

        [Fact]
        public void It_Transforms_A_NurbsCurve_By_A_Given_Matrix()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            Transform transform = Transform.Translation(new Vector3 { -10, 20, 0 });

            // Act
            NurbsCurve transformedCurve = curve.Transform(transform);
            Vector3 pt1 = curve.PointAt(0.5);
            Vector3 pt2 = transformedCurve.PointAt(0.5);
            double distanceBetweenPts = System.Math.Round((pt2 - pt1).Length(), 6);

            // Assert
            distanceBetweenPts.Should().Be(22.36068);
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_With_Clamped_End()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.PeriodicClosedNurbsCurve();

            // Act
            NurbsCurve curveClamped = curve.ClampEnds();

            // Assert
            curveClamped.Knots.IsClamped(curveClamped.Degree).Should().BeTrue();
            curveClamped.ControlPoints[0].Select((val, i) => val.Should()
                    .BeApproximately(curveClamped.ControlPoints[^1][i], GeoSharpMath.MAX_TOLERANCE));
            curve.ControlPoints[2].Should().BeEquivalentTo(curveClamped.ControlPoints[2]);
            curve.ControlPoints[^3].Should().BeEquivalentTo(curveClamped.ControlPoints[^3]);
        }
    }
}
