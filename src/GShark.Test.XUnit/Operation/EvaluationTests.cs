using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;
using GShark.Operation.Utilities;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class EvaluationTests
    {
        private readonly ITestOutputHelper _testOutput;

        public EvaluationTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Tests_A_Basic_Function()
        {
            // Arrange
            int degree = 2;
            int span = 4;
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };

            // Act
            List<double> result1 = Evaluation.BasisFunction(degree, knots, span, 2.5);
            List<double> result2 = Evaluation.BasisFunction(degree, knots, 2.5);

            // Assert
            result1.Should().BeEquivalentTo(result2);
            result1.Count.Should().Be(3);
            result1[0].Should().Be(0.125);
            result1[1].Should().Be(0.75);
            result1[2].Should().Be(0.125);
        }

        [Theory]
        [InlineData(0.0, new[] { 5.0, 5.0, 0.0 })]
        [InlineData(0.3, new[] { 18.5, 13.33625, 0.0 })]
        [InlineData(0.5, new[] { 27.5, 14.6875, 0.0 })]
        [InlineData(0.6, new[] { 32.0, 14.35, 0.0 })]
        [InlineData(1.0, new[] { 50.0, 5.0, 0.0 })]
        public void It_Returns_A_Point_At_A_Given_Parameter(double parameter, double[] result)
        {
            // Arrange
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new (5,5,0),
                new (10, 10, 0),
                new (20, 15, 0),
                new (35, 15, 0),
                new (45, 10, 0),
                new (50, 5, 0)
            };
            NurbsCurve curve = new NurbsCurve(pts, degree);

            // Act
            Point3 pt = Evaluation.CurvePointAt(curve, parameter);

            // Assert
            pt[0].Should().BeApproximately(result[0], 0.001);
            pt[1].Should().BeApproximately(result[1], 0.001);
        }

        [Fact]
        public void It_Returns_Extrema_Values()
        {
            // Arrange
            List<Point3> pts = new List<Point3>
            {
                new (330, 592, 0),
                new (330, 557, 0),
                new (315, 522, 0),
                new (315, 485, 0)
            };
            NurbsCurve curve = new NurbsCurve(pts, 3);

            // Act
            Extrema e = Evaluation.ComputeExtrema(curve);

            // Assert
            e.Values.Count.Should().Be(3);
            e.Values[0].Should().Be(0);
            e.Values[1].Should().Be(0.5);
            e.Values[2].Should().Be(1);
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { 1.0, 1.0, 0.38 })]
        [InlineData(0.5, 0.5, new double[] { 5.0, 5.0, 1.5 })]
        [InlineData(1.0, 1.0, new double[] { 10.0, 10.0, 2.0 })]
        public void It_Returns_A_Point_On_Surface_At_A_Given_U_And_V_Parameter(double u, double v, double[] pt)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.QuadrilateralSurface();
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            Point3 evalPt = new Point3(Evaluation.SurfacePointAt(surface, u, v));

            // Assert
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_Derivatives_For_A_Rational_Surface()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.QuadrilateralSurface();
            Vector3 zeroDerivative = new Vector3(3, 5, 1.7); // 0th derivative with respect to U and V
            Vector3 expectedDerivativeU = new Vector3(0, 10, 3.4);
            Vector3 expectedDerivativeV = new Vector3(10, 0, -1);

            // Act
            Vector3[,] derivatives = Evaluation.RationalSurfaceDerivatives(surface, 0.3, 0.5, 1);

            // Assert
            derivatives[0, 0].Equals(zeroDerivative).Should().BeTrue();
            derivatives[0, 1].Equals(expectedDerivativeU).Should().BeTrue();
            derivatives[1, 0].Equals(expectedDerivativeV).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Derive_Basic_Function_Given_NI()
        {
            // Arrange
            // Values and formulas from The Nurbs Book p.69 & p.72
            int degree = 2;
            int span = 4;
            int order = 2;
            double parameter = 2.5;
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
            double[,] expectedResult = new double[,] { { 0.125, 0.75, 0.125 }, { -0.5, 0.0, 0.5 }, { 1.0, -2.0, 1.0 } };

            // Act
            List<Vector> resultToCheck = Evaluation.DerivativeBasisFunctionsGivenNI(span, parameter, degree, order, knots);

            // Assert
            resultToCheck[0][0].Should().BeApproximately(expectedResult[0, 0], GSharkMath.MaxTolerance);
            resultToCheck[0][1].Should().BeApproximately(expectedResult[0, 1], GSharkMath.MaxTolerance);
            resultToCheck[0][2].Should().BeApproximately(expectedResult[0, 2], GSharkMath.MaxTolerance);

            resultToCheck[1][0].Should().BeApproximately(expectedResult[1, 0], GSharkMath.MaxTolerance);
            resultToCheck[1][1].Should().BeApproximately(expectedResult[1, 1], GSharkMath.MaxTolerance);
            resultToCheck[1][2].Should().BeApproximately(expectedResult[1, 2], GSharkMath.MaxTolerance);

            resultToCheck[2][0].Should().BeApproximately(expectedResult[2, 0], GSharkMath.MaxTolerance);
            resultToCheck[2][1].Should().BeApproximately(expectedResult[2, 1], GSharkMath.MaxTolerance);
            resultToCheck[2][2].Should().BeApproximately(expectedResult[2, 2], GSharkMath.MaxTolerance);

            resultToCheck.Count.Should().Be(order + 1);
            resultToCheck[0].Count.Should().Be(degree + 1);
        }

        [Fact]
        public void It_Returns_The_Result_Of_A_Curve_Derivatives()
        {
            // Arrange
            int degree = 3;
            int parameter = 0;
            int numberDerivs = 2;
            List<Point3> pts = new List<Point3>
            {
                new (10, 0, 0),
                new (20, 10, 0),
                new (30, 20, 0),
                new (50, 50, 0)
            };

            NurbsCurve curve = new NurbsCurve(pts, degree);

            // Act
            List<Point4> p = Evaluation.CurveDerivatives(curve, parameter, numberDerivs);

            // Assert
            p[0][0].Should().Be(10);
            p[0][1].Should().Be(0);
            (p[1][0] / p[1][1]).Should().Be(1);
        }

        [Fact]
        public void It_Returns_The_Result_Of_A_Rational_Curve_Derivatives()
        {
            // Consider the quadratic rational Bezier circular arc.
            // Example at page 126.
            // Arrange
            int degree = 2;
            List<double> weight = new List<double> { 1, 1, 2 };
            List<Point3> pts = new List<Point3>()
            {
                new Point3(1, 0, 0),
                new Point3(1, 1, 0),
                new Point3(0, 1, 0)
            };
            NurbsCurve curve = new NurbsCurve(pts, weight, degree);
            int derivativesOrder = 2;

            // Act
            List<Vector3> resultToCheck = Evaluation.RationalCurveDerivatives(curve, 0, derivativesOrder);

            // Assert
            resultToCheck[0][0].Should().Be(1);
            resultToCheck[0][1].Should().Be(0);

            resultToCheck[1][0].Should().Be(0);
            resultToCheck[1][1].Should().Be(2);

            resultToCheck[2][0].Should().Be(-4);
            resultToCheck[2][1].Should().Be(0);

            List<Vector3> resultToCheck2 = Evaluation.RationalCurveDerivatives(curve, 1, derivativesOrder);

            resultToCheck2[0][0].Should().Be(0);
            resultToCheck2[0][1].Should().Be(1);

            resultToCheck2[1][0].Should().Be(-1);
            resultToCheck2[1][1].Should().Be(0);

            resultToCheck2[2][0].Should().Be(1);
            resultToCheck2[2][1].Should().Be(-1);

            List<Vector3> resultToCheck3 = Evaluation.RationalCurveDerivatives(curve, 0, 3);

            resultToCheck3[3][0].Should().Be(0);
            resultToCheck3[3][1].Should().Be(-12);

            List<Vector3> resultToCheck4 = Evaluation.RationalCurveDerivatives(curve, 1, 3);

            resultToCheck4[3][0].Should().Be(0);
            resultToCheck4[3][1].Should().Be(3);
        }

        [Theory]
        [InlineData(0.0, new double[] { 0.707107, 0.707107, 0.0 })]
        [InlineData(0.25, new double[] { 0.931457, 0.363851, 0.0 })]
        [InlineData(0.5, new double[] { 1.0, 0.0, 0.0 })]
        [InlineData(0.75, new double[] { 0.931457, -0.363851, 0 })]
        [InlineData(1.0, new double[] { 0.707107, -0.707107, 0.0 })]
        public void It_Returns_The_Tangent_At_Given_Point(double t, double[] tangentData)
        {
            // Arrange
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new (0, 0, 0),
                new (1, 0, 0),
                new (2, 0, 0),
                new (3, 0, 0),
                new (4, 0, 0)
            };
            List<double> weights = new List<double> { 1, 1, 1, 1, 1 };
            NurbsCurve curve = new NurbsCurve(pts, weights, degree);
            Vector3 tangentExpectedLinearCurve = new Vector3(3, 0, 0);
            Vector3 tangentExpectedPlanarCurve = new Vector3(tangentData[0], tangentData[1], tangentData[2]);

            // Act
            // Act on a linear nurbs curve.
            Vector3 tangentLinearCurve = Evaluation.RationalCurveTangent(curve, 0.5);
            var tangentPlanarCurve = Evaluation.RationalCurveTangent(NurbsBaseCollection.NurbsPlanarExample(), t);
            Vector3 tangentNormalized = tangentPlanarCurve.Unitize();

            // Assert
            tangentLinearCurve.Should().BeEquivalentTo(tangentExpectedLinearCurve);
            tangentNormalized.EpsilonEquals(tangentExpectedPlanarCurve, GSharkMath.MaxTolerance).Should().BeTrue();
        }
    }
}
