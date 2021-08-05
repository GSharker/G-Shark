using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
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
            curveLength.Should().BeApproximately(expectedLength, GeoSharkMath.MaxTolerance);
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
                double t = Analyze.BezierCurveParamAtLength(curve, sumLengths, GeoSharkMath.MaxTolerance);

                double segmentLength = Analyze.BezierCurveLength(curve, t);

                // Assert
                t.Should().BeApproximately(tValuesExpected[i], GeoSharkMath.MinTolerance);
                segmentLength.Should().BeApproximately(sumLengths, GeoSharkMath.MinTolerance);

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
            (List<double> tvalues, List<Point3> pts) samples = Tessellation.CurveRegularSample(curve, 10000);

            double length = 0.0;
            for (int j = 0; j < samples.pts.Count - 1; j++)
                length += (samples.pts[j + 1] - samples.pts[j]).Length;

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
            t.Should().BeApproximately(tValExpected, GeoSharkMath.MaxTolerance);
            pt.EpsilonEquals(expectedPt, GeoSharkMath.MaxTolerance).Should().BeTrue();
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
    }
}
