using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
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

        public static (int degree, List<Point3> pts, KnotVector knots, List<double> weights) CurveData =>
        (
            2,
            new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
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
            List<Point3> controlPts = new List<Point3>
            {
                new Point3 (4.5,2.5,2.5),
                new Point3 (5,5,5),
                new Point3 (0,5,0)
            };
            Point3 expectedPt00 = new Point3(3.25, 3.28125, 1.875);
            Point3 expectedPt01 = new Point3(4.75, 3.75, 3.75);

            // Act
            NurbsCurve nurbsCurve = new NurbsCurve(controlPts, degree).Close();
            Vector3 ptAt00 = nurbsCurve.PointAt(0.75);
            Vector3 ptAt01 = nurbsCurve.PointAt(1);

            // Assert
            nurbsCurve.ControlPoints.Count.Should().Be(5);
            nurbsCurve.ControlPointLocations[1].DistanceTo(nurbsCurve.ControlPointLocations[nurbsCurve.ControlPointLocations.Count - 1]).Should().BeLessThan(GSharkMath.Epsilon);
            nurbsCurve.Knots.Count.Should().Be(8);
            nurbsCurve.Domain.T0.Should().Be(0.0);
            nurbsCurve.Domain.T1.Should().Be(1.0);
            expectedPt00.DistanceTo(ptAt00).Should().BeLessThan(GSharkMath.Epsilon);
            expectedPt01.DistanceTo(ptAt01).Should().BeLessThan(GSharkMath.Epsilon);
        }

        [Fact]
        public void It_Returns_True_If_A_NurbsCurve_Is_Closed()
        {
            // Assert
            NurbsCurveCollection.NurbsCurveWithStartingAndEndPointOverlapping().IsClosed().Should().BeTrue();
            NurbsCurveCollection.PeriodicClosedNurbsCurve().IsClosed().Should().BeTrue();
        }

        [Fact]
        public void It_Returns_True_If_A_NurbsCurve_Is_Periodic()
        {
            // Assert
            NurbsCurveCollection.PeriodicClosedNurbsCurve().IsPeriodic().Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Evaluated_With_A_List_Of_Weights()
        {
            // Act
            NurbsCurve nurbsCurve = NurbsCurveCollection.NurbsCurvePtsAndWeightsExample();

            // Assert
            nurbsCurve.Should().NotBeNull();
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Point4(10, 0, 0, 0.5));
            nurbsCurve.ControlPointLocations[2].Should().BeEquivalentTo(new Point3(20, 0, 0));
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
        public void It_Returns_The_Bounding_Box_Of_A_Planar_Curve()
        {
            // Arrange
            NurbsCurve crv0 = NurbsCurveCollection.NurbsCurveCubicBezierPlanar();
            NurbsCurve crv1 = NurbsCurveCollection.NurbsCurveQuadraticBezierPlanar();

            var expectedPtMin0 = new Point3(0, 0, 0);
            var expectedPtMax0 = new Point3(2, 0.444444, 0);

            var expectedPtMin1 = new Point3(-10, 0, 0);
            var expectedPtMax1 = new Point3(20, 15, 5);

            // Act
            BoundingBox bBox0 = crv0.GetBoundingBox();
            BoundingBox bBox1 = crv1.GetBoundingBox();

            // Assert
            bBox0.Max.DistanceTo(expectedPtMax0).Should().BeLessThan(GSharkMath.MaxTolerance);
            bBox0.Min.DistanceTo(expectedPtMin0).Should().BeLessThan(GSharkMath.MaxTolerance);

            bBox1.Max.DistanceTo(expectedPtMax1).Should().BeLessThan(GSharkMath.MaxTolerance);
            bBox1.Min.DistanceTo(expectedPtMin1).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_A_3D_Nurbs_Curve()
        {
            // Arrange
            NurbsCurve crv0 = NurbsCurveCollection.NurbsCurve3DExample();
            NurbsCurve crv1 = NurbsCurveCollection.NurbsCurveQuadratic3DBezier();

            var expectedPtMin0 = new Point3(0, 0.5555556, 0);
            var expectedPtMax0 = new Point3(4.089468, 5, 5);

            var expectedPtMin1 = new Point3(0, 2.5, 0);
            var expectedPtMax1 = new Point3(4.545455, 5, 3.333333);

            // Act
            BoundingBox bBox0 = crv0.GetBoundingBox();
            BoundingBox bBox1 = crv1.GetBoundingBox();

            // Assert
            bBox0.Max.DistanceTo(expectedPtMax0).Should().BeLessThan(GSharkMath.MaxTolerance);
            bBox0.Min.DistanceTo(expectedPtMin0).Should().BeLessThan(GSharkMath.MaxTolerance);

            bBox1.Max.DistanceTo(expectedPtMax1).Should().BeLessThan(GSharkMath.MaxTolerance);
            bBox1.Min.DistanceTo(expectedPtMin1).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_A_Periodic_Curve()
        {
            // Arrange 
            Point3 expectedPtMin = new Point3(0, 0.208333, 0.208333);
            Point3 expectedPtMax = new Point3(4.354648, 5, 3.333333);

            // Act
            BoundingBox bBox = NurbsCurveCollection.PeriodicClosedNurbsCurve().GetBoundingBox();

            // Assert
            bBox.Max.DistanceTo(expectedPtMax).Should().BeLessThan(GSharkMath.MaxTolerance);
            bBox.Min.DistanceTo(expectedPtMin).Should().BeLessThan(GSharkMath.MaxTolerance);
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
            var curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            var transform = Transform.Translation(new Vector3(-10, 20, 0));

            // Act
            var transformedCurve = curve.Transform(transform);
            var pt1 = curve.PointAt(0.5);
            var pt2 = transformedCurve.PointAt(0.5);
            var distanceBetweenPts = System.Math.Round((pt2 - pt1).Length, 6);

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
            curveClamped.ControlPoints[0]
                .EpsilonEquals(curveClamped.ControlPoints[curveClamped.ControlPoints.Count - 1], GSharkMath.MaxTolerance)
                .Should().BeTrue();
            curve.ControlPoints[2].Should().BeEquivalentTo(curveClamped.ControlPoints[2]);
            curve.ControlPoints[curve.ControlPoints.Count - curveClamped.Degree].Should().BeEquivalentTo(curveClamped.ControlPoints[curve.ControlPoints.Count - curveClamped.Degree]);
        }

        [Fact]
        public void It_Returns_A_Perpendicular_Frame_At_Given_Parameter()
        {
            // Arrange
            double t0 = 0.2;
            double t1 = 0.75;

            Point3 expectedPlaneOrigin0 = new Point3(0.784, 1.16, 1.16);
            Point3 expectedPlaneOrigin1 = new Point3(3.96875, 3.59375, 2.96875);

            Vector3 expectedXDir0 = new Vector3(0.889878, 0.322581, 0.322581);
            Vector3 expectedXDir1 = new Vector3(-0.690371, -0.162782, -0.704905);

            // Act
            Plane frame0 = NurbsCurveCollection.NurbsCurve3DExample().FrameAt(t0);
            Plane frame1 = NurbsCurveCollection.NurbsCurve3DExample().FrameAt(t1);

            // Assert
            frame0.Origin.EpsilonEquals(expectedPlaneOrigin0, GSharkMath.MinTolerance).Should().BeTrue();
            frame1.Origin.EpsilonEquals(expectedPlaneOrigin1, GSharkMath.MinTolerance).Should().BeTrue();

            frame0.XAxis.IsParallelTo(expectedXDir0).Should().NotBe(0);
            frame1.XAxis.IsParallelTo(expectedXDir1).Should().NotBe(0);
        }

        [Fact]
        public void It_Returns_The_Curvature_Vector_At_The_Given_Parameter()
        {
            // Arrange
            double expectedRadiusLength = 1.469236;
            Vector3 expectedCurvature = new Vector3(1.044141, 0.730898, 0.730898);

            // Act
            Vector3 curvature = NurbsCurveCollection.NurbsCurve3DExample().CurvatureAt(0.25);

            // Assert
            (curvature.Length - expectedRadiusLength).Should().BeLessThan(GSharkMath.MinTolerance);
            curvature.EpsilonEquals(expectedCurvature, GSharkMath.MinTolerance).Should().BeTrue();
        }
    }
}
