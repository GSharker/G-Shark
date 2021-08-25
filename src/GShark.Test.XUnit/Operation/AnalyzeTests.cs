using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Enum;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using GShark.Geometry.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class AnalyzeTests
    {
        private readonly ITestOutputHelper _testOutput;

        public AnalyzeTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void RationalBezierCurveArcLength_Returns_The_Approximated_Length()
        {
            // Arrange
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(0.5, 0, 0),
                new Point3(2.5, 0, 0),
                new Point3(3, 0, 0)
            };

            NurbsCurve curve = new NurbsCurve(pts, degree);
            double expectedLength = 3.0;

            // Act
            double curveLength = Analyze.BezierCurveLength(curve, 1);

            // Assert
            curveLength.Should().BeApproximately(expectedLength, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void RationalBezierCurveParamAtLength_Returns_Parameters_At_Passed_Lengths()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            double[] tValuesExpected = new[] { 0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1 };

            int steps = 7;
            double length = curve.Length() / steps;
            double sumLengths = 0.0;

            for (int i = 0; i < steps + 1; i++)
            {
                // Act
                double t = Analyze.BezierCurveParamAtLength(curve, sumLengths, GSharkMath.MaxTolerance);

                double segmentLength = Analyze.BezierCurveLength(curve, t);

                // Assert
                t.Should().BeApproximately(tValuesExpected[i], GSharkMath.MaxTolerance);
                segmentLength.Should().BeApproximately(sumLengths, GSharkMath.MaxTolerance);

                sumLengths += length;
            }
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_Curve()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();

            // Act
            double crvLength = Analyze.CurveLength(curve);
            var (_, pts) = Tessellation.CurveRegularSample(curve, 10000);

            double length = 0.0;
            for (int j = 0; j < pts.Count - 1; j++)
                length += (pts[j + 1] - pts[j]).Length;

            // Assert
            crvLength.Should().BeApproximately(length, 1e-3);
        }

        [Theory]
        [InlineData(new double[] { 5, 7, 0 }, new double[] { 5.982099, 5.950299, 0 }, 0.021824)]
        [InlineData(new double[] { 12, 10, 0 }, new double[] { 11.781824, 10.364244, 0 }, 0.150707)]
        [InlineData(new double[] { 21, 17, 0 }, new double[] { 21.5726, 14.101932, 0 }, 0.36828)]
        [InlineData(new double[] { 32, 15, 0 }, new double[] { 31.906562, 14.36387, 0 }, 0.597924)]
        [InlineData(new double[] { 41, 8, 0 }, new double[] { 42.554645, 10.750437, 0 }, 0.834548)]
        [InlineData(new double[] { 50, 5, 0 }, new double[] { 50, 5, 0 }, 1.0)]
        public void It_Returns_The_Closest_Point_And_The_Parameter_t(double[] ptToCheck, double[] ptExpected, double tValExpected)
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            Point3 testPt = new Point3(ptToCheck[0], ptToCheck[1], ptToCheck[2]);
            Point3 expectedPt = new Point3(ptExpected[0], ptExpected[1], ptExpected[2]);

            // Act
            var pt = Analyze.CurveClosestPoint(curve, testPt, out double t);

            // Assert
            t.Should().BeApproximately(tValExpected, GSharkMath.MaxTolerance);
            pt.EpsilonEquals(expectedPt, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(15, 0.278127)]
        [InlineData(33, 0.672164)]
        [InlineData(46, 0.928308)]
        [InlineData(50.334675, 1)]
        public void RationalCurveParameterAtLength_Returns_Parameter_t_At_The_Given_Length(double segmentLength, double tValueExpected)
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();

            // Act
            double t = Analyze.CurveParameterAtLength(curve, segmentLength);

            // Assert
            t.Should().BeApproximately(tValueExpected, 1e-5);
        }

        [Theory]
        [InlineData(0.204157623157292, 0.716170472509343, new double[] { 2.5, 7, 5 })]
        [InlineData(0.237211551442712, 0.154628316784507, new double[] { 2.5, 1.5, 2 })]
        [InlineData(0.910119163727208, 0.229417610613794, new double[] { 9, 2.5, 1 })]
        [InlineData(0.50870054333679, 0.360138133269618, new double[] { 5, 5, 1 })]
        public void RationalSurfaceClosestParam_Returns_Parameters_U_V_Of_A_Closest_Point(double u, double v, double[] testPt)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 pt = new Point3(testPt[0], testPt[1], testPt[2]);
            (double u, double v) expectedUV = (u, v);

            // Act
            var closestParameter = Analyze.SurfaceClosestParameter(surface, pt);

            // Assert
            (closestParameter.u - expectedUV.u).Should().BeLessThan(GSharkMath.MaxTolerance);
            (closestParameter.v - expectedUV.v).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Returns_The_Surface_Isocurve_At_U_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 expectedPt = new Point3(3.591549, 10, 4.464789);

            // Act
            ICurve Isocurve = Analyze.Isocurve(surface, 0.3, SurfaceDirection.U);

            // Assert
            Isocurve.ControlPointLocations[1].DistanceTo(expectedPt).Should().BeLessThan(GSharkMath.MinTolerance);
        }

        [Fact]
        public void Returns_The_Surface_Isocurve_At_V_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 expectedPt = new Point3(5, 4.615385, 2.307692);
            Point3 expectedPtAt = new Point3(5, 3.913043, 1.695652);

            // Act
            ICurve Isocurve = Analyze.Isocurve(surface, 0.3, SurfaceDirection.V);
            Point3 ptAt = Isocurve.PointAt(0.5);

            // Assert
            Isocurve.ControlPointLocations[1].DistanceTo(expectedPt).Should().BeLessThan(GSharkMath.MinTolerance);
            ptAt.DistanceTo(expectedPtAt).Should().BeLessThan(GSharkMath.MinTolerance);
        }


        [Fact]
        public void It_Returns_The_Curvature_Vector_At_The_Given_Parameter()
        {
            // Arrange
            double expectedRadiusLength = 1.469236;
            Vector3 expectedCurvature = new Vector3(1.044141, 0.730898, 0.730898);

            // Act
            Vector3 curvature = Analyze.Curvature(NurbsCurveCollection.NurbsCurve3DExample(), 0.25);

            // Assert
            (curvature.Length - expectedRadiusLength).Should().BeLessThan(GSharkMath.MinTolerance);
            curvature.EpsilonEquals(expectedCurvature, GSharkMath.MinTolerance).Should().BeTrue();
        }
    }
}
