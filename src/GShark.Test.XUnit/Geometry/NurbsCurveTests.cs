using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using GShark.Test.XUnit.Fitting;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class NurbsBaseTests
    {
        private readonly ITestOutputHelper _testOutput;

        public NurbsBaseTests(ITestOutputHelper testOutput)
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
        public void It_Returns_A_NurbsBase()
        {
            // Act
            NurbsBase NurbsBase = NurbsCurveCollection.NurbsBasePlanarExample();

            // Assert
            NurbsBase.Should().NotBeNull();
            NurbsBase.Degree.Should().Be(3);
            NurbsBase.Weights.Should().BeEquivalentTo(CollectionHelpers.RepeatData(1.0, 6));
        }

        [Fact]
        public void It_Returns_A_Closed_NurbsBase()
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
            NurbsBase NurbsBase = new NurbsCurve(controlPts, degree).Close();
            Vector3 ptAt00 = NurbsBase.PointAt(0.75);
            Vector3 ptAt01 = NurbsBase.PointAt(1);

            // Assert
            NurbsBase.ControlPoints.Count.Should().Be(5);
            NurbsBase.ControlPointLocations[1].DistanceTo(NurbsBase.ControlPointLocations[NurbsBase.ControlPointLocations.Count - 1]).Should().BeLessThan(GSharkMath.Epsilon);
            NurbsBase.Knots.Count.Should().Be(8);
            expectedPt00.DistanceTo(ptAt00).Should().BeLessThan(GSharkMath.Epsilon);
            expectedPt01.DistanceTo(ptAt01).Should().BeLessThan(GSharkMath.Epsilon);
        }

        [Fact]
        public void It_Returns_True_If_A_NurbsBase_Is_Closed()
        {
            // Assert
            NurbsCurveCollection.NurbsBaseWithStartingAndEndPointOverlapping().IsClosed().Should().BeTrue();
            NurbsCurveCollection.PeriodicClosedNurbsBase().IsClosed().Should().BeTrue();
        }

        [Fact]
        public void It_Returns_True_If_A_NurbsBase_Is_Periodic()
        {
            // Assert
            NurbsCurveCollection.PeriodicClosedNurbsBase().IsPeriodic().Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_NurbsBase_Evaluated_With_A_List_Of_Weights()
        {
            // Act
            NurbsBase NurbsBase = NurbsCurveCollection.NurbsBasePtsAndWeightsExample();

            // Assert
            NurbsBase.Should().NotBeNull();
            NurbsBase.ControlPoints[2].Should().BeEquivalentTo(new Point4(10, 0, 0, 0.5));
            NurbsBase.ControlPointLocations[2].Should().BeEquivalentTo(new Point3(20, 0, 0));
        }

        [Fact]
        public void It_Creates_A_NurbsBase_From_ControlPoints_And_Degree()
        {
            // Act
            NurbsBase NurbsBase = new NurbsCurve(CurveData.pts, CurveData.degree);

            // Assert
            NurbsBase.Should().NotBeNull();
            NurbsBase.Degree.Should().Be(2);
            NurbsBase.Weights.Should().BeEquivalentTo(CollectionHelpers.RepeatData(1.0, CurveData.pts.Count));
            NurbsBase.Knots.Should().BeEquivalentTo(new KnotVector(CurveData.degree, CurveData.pts.Count));
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_A_Planar_Curve()
        {
            // Arrange
            NurbsBase crv0 = NurbsCurveCollection.NurbsBaseCubicBezierPlanar();
            NurbsBase crv1 = NurbsCurveCollection.NurbsBaseQuadraticBezierPlanar();

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
            NurbsBase crv0 = NurbsCurveCollection.NurbsBase3DExample();
            NurbsBase crv1 = NurbsCurveCollection.NurbsBaseQuadratic3DBezier();

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
            BoundingBox bBox = NurbsCurveCollection.PeriodicClosedNurbsBase().GetBoundingBox();

            // Assert
            bBox.Max.DistanceTo(expectedPtMax).Should().BeLessThan(GSharkMath.MaxTolerance);
            bBox.Min.DistanceTo(expectedPtMin).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Transforms_A_NurbsBase_By_A_Given_Matrix()
        {
            // Arrange
            var curve = NurbsCurveCollection.NurbsBasePlanarExample();
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
        public void It_Returns_A_NurbsBase_With_Clamped_End()
        {
            // Arrange
            NurbsBase curve = NurbsCurveCollection.PeriodicClosedNurbsBase();

            // Act
            NurbsBase curveClamped = curve.ClampEnds();

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
            Plane frame0 = NurbsCurveCollection.NurbsBase3DExample().PerpendicularFrameAt(t0);
            Plane frame1 = NurbsCurveCollection.NurbsBase3DExample().PerpendicularFrameAt(t1);

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
            Vector3 curvature = NurbsCurveCollection.NurbsBase3DExample().CurvatureAt(0.25);

            // Assert
            (curvature.Length - expectedRadiusLength).Should().BeLessThan(GSharkMath.MinTolerance);
            curvature.EpsilonEquals(expectedCurvature, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Curve()
        {
            // Arrange
            NurbsCurve crv = new NurbsCurve(CurveTests.pts, 2);
            double offset = 22.5;

            // Act
            NurbsBase offsetResult = crv.Offset(offset, Plane.PlaneXY);

            // Assert
            for (double i = 0; i <= 1; i += 0.1)
            {
                Point3 pt = offsetResult.PointAt(i);
                Point3 closestPt = crv.ClosestPoint(pt);
                pt.DistanceTo(closestPt).Should().BeApproximately(offset, GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Returns_A_Curve_Parameter_At_A_Given_ChordLength_From_A_Starting_Curve_Parameter()
        {
            //Arrange
            var chordLength = 5.0;
            var expectedParamOnCrv = 0.169836765178962;
            var crv = NurbsCurveCollection.NurbsBase3DExample();
            var startParam = 0;

            //Act
            var param = crv.ParameterAtChordLength(startParam, chordLength);

            //Assert
            param.Equals(expectedParamOnCrv).Should().BeTrue();
        }
    }
}
