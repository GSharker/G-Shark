using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using GeometrySharp.Test.XUnit.Data;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
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
            Knot knots1 = new Knot { 0, 0, 0, 0, 1, 1, 1, 1 };
            Knot knots2 = new Knot { 1, 1, 1, 1, 4, 4, 4, 4 };
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {0, 0, 0},
                new Vector3 {0.5, 0, 0},
                new Vector3 {2.5, 0, 0},
                new Vector3 {3, 0, 0}
            };

            NurbsCurve curve1 = new NurbsCurve(degree, knots1, controlPts);
            NurbsCurve curve2 = new NurbsCurve(degree, knots2, controlPts);
            double expectedLength = 3.0;

            // Act
            double curveLength1 = Analyze.BezierCurveLength(curve1, 1);
            double curveLength2 = Analyze.BezierCurveLength(curve2, 4);

            // Assert
            curveLength1.Should().BeApproximately(expectedLength, GeoSharpMath.MAXTOLERANCE);
            curveLength2.Should().BeApproximately(expectedLength, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void RationalBezierCurveParamAtLength_Returns_Parameters_At_Passed_Lengths()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveExample2();
            double[] tValuesExpected = new[] { 0, 0.122941, 0.265156, 0.420293, 0.579707, 0.734844, 0.877059, 1 };

            int steps = 7;
            double length = curve.Length() / steps;
            double sumLengths = 0.0;

            for (int i = 0; i < steps + 1; i++)
            {
                // Act
                double t = Analyze.BezierCurveParamAtLength(curve, sumLengths, GeoSharpMath.MAXTOLERANCE);

                double segmentLength = Analyze.BezierCurveLength(curve, t);

                // Assert
                t.Should().BeApproximately(tValuesExpected[i], GeoSharpMath.MINTOLERANCE);
                segmentLength.Should().BeApproximately(sumLengths, GeoSharpMath.MINTOLERANCE);

                sumLengths += length;
            }
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_Curve()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveExample2();

            // Act
            double crvLength = Analyze.CurveLength(curve);
            (List<double> tvalues, List<Vector3> pts) samples = Tessellation.RegularSample(curve, 10000);

            double length = 0.0;
            for (int j = 0; j < samples.pts.Count - 1; j++)
                length += (samples.pts[j + 1] - samples.pts[j]).Length();

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
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveExample2();
            
            // Act
            Vector3 ptHomogenized = Analyze.CurveClosestPoint(curve, ptToCheck.ToVector(), out double t);
            Vector3 pt = LinearAlgebra.PointDehomogenizer(ptHomogenized);

            // Assert
            t.Should().BeApproximately(tValExpected, GeoSharpMath.MAXTOLERANCE);
            // https://stackoverflow.com/questions/36782975/fluent-assertions-approximately-compare-a-classes-properties
            pt.Should().BeEquivalentTo(ptExpected.ToVector(), options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, GeoSharpMath.MAXTOLERANCE))
                .WhenTypeIs<double>());
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
            NurbsCurve curve = NurbsCurveCollection.NurbsCurveExample2();

            // Act
            double t = Analyze.CurveParameterAtLength(curve, segmentLength);

            // Assert
            t.Should().BeApproximately(tValueExpected, 1e-5);
        }
    }
}
