﻿using FluentAssertions;
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

        //private NurbsSurface ConstructNurbsSurface()
        //{
        //    int degreeU = 3;
        //    int degreeV = 3;
        //    KnotVector knotsU = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };
        //    KnotVector knotsV = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };

        //    List<Point3> u1 = new List<Point3>
        //    {
        //        new (0d, 0d, 0d),
        //        new (10d, 0d, 0d),
        //        new (20d, 0d, 0d),
        //        new (30d, 0d, 0d)
        //    };
        //    List<Point3> u2 = new List<Point3>
        //    {
        //        new (0d, -10d, 0d),
        //        new (10d, -10d, 10d),
        //        new (20d, -10d, 0d),
        //        new (30d, -10d, 0d)
        //    };
        //    List<Point3> u3 = new List<Point3>
        //    {
        //        new (0d, -20d, 0d),
        //        new (10d, -20d, 0d),
        //        new (20d, -20d, 0d),
        //        new (30d, -20d, 0d)
        //    };
        //    List<Point3> u4 = new List<Point3>
        //    {
        //        new (0d, -30d, 0d),
        //        new (10d, -30d, 0d),
        //        new (20d, -30d, 0d),
        //        new (30d, -30d, 0d)
        //    };
        //    List<List<Point3>> controlPoints = new List<List<Point3>>
        //    {
        //        u1, u2, u3, u4
        //    };

        //    return new NurbsSurface(degreeU, degreeV, knotsU, knotsV, controlPoints);
        //}

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
        [InlineData(0.3, new[] { 18.617, 13.377, 0.0 })]
        [InlineData(0.5, new[] { 27.645, 14.691, 0.0 })]
        [InlineData(0.6, new[] { 32.143, 14.328, 0.0 })]
        [InlineData(1.0, new[] { 50.0, 5.0, 0.0 })]
        public void It_Returns_A_Point_At_A_Given_Parameter(double parameter, double[] result)
        {
            // Arrange
            KnotVector knots = new KnotVector { 0.0, 0.0, 0.0, 0.0, 0.33, 0.66, 1.0, 1.0, 1.0, 1.0 };
            int degree = 3;
            List<Point3> controlPts = new List<Point3>
            {
                new (5,5,0),
                new (10, 10, 0),
                new (20, 15, 0),
                new (35, 15, 0),
                new (45, 10, 0),
                new (50, 5, 0)
            };
            NurbsCurve curve = new NurbsCurve(degree, knots, controlPts);

            // Act
            Point3 pt = Evaluation.CurvePointAt(curve, parameter);

            // Assert
            pt[0].Should().BeApproximately(result[0], 0.001);
            pt[1].Should().BeApproximately(result[1], 0.001);
        }

        //[Fact]
        //public void It_Returns_A_Point_On_Four_Points_Surface_At_A_Given_U_And_V_Parameter()
        //{
        //    // Arrange
        //    Point3 p1 = new Point3(6.292d, -3.297d, -1.311d);
        //    Point3 p2 = new Point3(4.599d, 4.910d, 5.869d);
        //    Point3 p3 = new Point3(-8.032d, -8.329d, -0.556d);
        //    Point3 p4 = new Point3(-7.966d, 7.580d, 5.366d);

        //    // Act
        //    NurbsSurface nurbsSurface = NurbsSurface.ByFourPoints(p1, p2, p3, p4);

        //    // Assert
        //    nurbsSurface.Should().NotBeNull();
        //    Point3 pt = Evaluation.SurfacePointAt(nurbsSurface, 0.5, 0.5);

        //    pt[0].Should().BeApproximately(-1.27675, 0.00001);
        //    pt[1].Should().BeApproximately(0.216, 0.00001);
        //    pt[2].Should().BeApproximately(2.342, 0.00001);
        //}

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

        //[Fact]
        //public void It_Returns_A_Point_On_Surface_At_A_Given_U_And_V_Parameter()
        //{
        //    // Arrange
        //    List<Point3> u1 = new List<Point3>
        //    {
        //        new (0d, 0d, 50),
        //        new (10d, 0d, 0),
        //        new (20d, 0d, 0),
        //        new (30d, 0d, 0)
        //    };
        //    List<Point3> u2 = new List<Point3>
        //    {
        //        new (0d, -10d, 0),
        //        new (10d, -10d, 10),
        //        new (20d, -10d, 10),
        //        new (30d, -10d, 0)
        //    };
        //    List<Point3> u3 = new List<Point3>
        //    {
        //        new (0d, -20d, 0),
        //        new (10d, -20d, 10),
        //        new (20d, -20d, 10),
        //        new (30d, -20d, 0)
        //    };
        //    List<Point3> u4 = new List<Point3>
        //    {
        //        new (0d, -30d, 0),
        //        new (10d, -30d, 0),
        //        new (20d, -30d, 0),
        //        new (30d, -30d, 0)
        //    };
        //    List<List<Point3>> controlPoints = new List<List<Point3>>
        //    {
        //        u1, u2, u3, u4
        //    };
        //    int degreeU = 3; int degreeV = degreeU;
        //    KnotVector knotsU = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };
        //    KnotVector knotsV = knotsU;

        //    // Act
        //    NurbsSurface nurbsSurface = new NurbsSurface(degreeU, degreeV, knotsU, knotsV, controlPoints);
        //    Point3 pt1 = Evaluation.SurfacePointAt(nurbsSurface, 0, 0);

        //    // Assert
        //    //ToDo Break out into arrange as expected results.
        //    pt1[0].Should().BeApproximately(u1[0][0], 0.00001);
        //    pt1[1].Should().BeApproximately(u1[0][1], 0.00001);
        //    pt1[2].Should().BeApproximately(u1[0][2], 0.00001);

        //    Point3 ptMid = Evaluation.SurfacePointAt(nurbsSurface, 0.5, 0.5);
        //    ptMid[0].Should().BeApproximately(15d, 0.00001);
        //    ptMid[1].Should().BeApproximately(-15d, 0.00001);
        //    ptMid[2].Should().BeApproximately(6.40625d, 0.00001);
        //}

        //[Fact]
        //public void It_Return_Surface_Derivatives_At_Given_NM()
        //{
        //    // Arrange
        //    NurbsSurface nurbsSurface = ConstructNurbsSurface();
        //    int n = 3;
        //    int m = 3;
        //    int numDers = 1;

        //    // Act
        //    var res = Evaluation.SurfaceDerivativesGivenNM(nurbsSurface, n, m, 0, 0, numDers);

        //    // Assert
        //    // 0th derivative with respect to u & v
        //    res[0][0][0].Should().Be(0d);
        //    res[0][0][1].Should().Be(0d);
        //    res[0][0][2].Should().Be(0d);

        //    // d/du
        //    (res[0][1][0] / res[0][1][0]).Should().Be(1d);
        //    res[0][1][2].Should().Be(0d);

        //    // d/dv
        //    res[1][0][0].Should().Be(0d);
        //    res[1][0][1].Should().Be(-30d);
        //    res[1][0][2].Should().Be(0d);

        //    // dd/dudv
        //    res[1][1][0].Should().Be(0d);
        //    res[1][1][1].Should().Be(0d);
        //    res[1][1][2].Should().Be(0d);

        //}

        //[Fact]
        //public void It_Return_Surface_Derivatives()
        //{
        //    // Arrange
        //    NurbsSurface nurbsSurface = ConstructNurbsSurface();
        //    int numDers = 1;

        //    // Act
        //    List<List<Vector3>> res = Evaluation.SurfaceDerivatives(nurbsSurface, 0, 0, numDers);

        //    // Assert
        //    // 0th derivative with respect to u & v
        //    res[0][0][0].Should().Be(0d);
        //    res[0][0][1].Should().Be(0d);
        //    res[0][0][2].Should().Be(0d);

        //    // d/du
        //    (res[0][1][0] / res[0][1][0]).Should().Be(1d);
        //    res[0][1][2].Should().Be(0d);

        //    // d/dv
        //    res[1][0][0].Should().Be(0d);
        //    res[1][0][1].Should().Be(-30d);
        //    res[1][0][2].Should().Be(0d);

        //    // dd/dudv
        //    res[1][1][0].Should().Be(0d);
        //    res[1][1][1].Should().Be(0d);
        //    res[1][1][2].Should().Be(0d);

        //}

        //[Fact]
        //public void It_Return_Surface_IsoCurve_At_A_Given_Parameter_Along_A_Given_Direction()
        //{
        //    //ToDo Reorganise sections. 
        //    // Arrange
        //    NurbsSurface nurbsSurface = ConstructNurbsSurface();
        //    double t = 0.2;
        //    double v = 0.3;

        //    // Act
        //    ICurve res = Evaluation.SurfaceIsoCurve(nurbsSurface, 0.2);

        //    // Assert
        //    Point3 p1 = Evaluation.CurvePointAt(res, 0.5);
        //    p1[0].Should().BeApproximately(6d, 5);
        //    p1[1].Should().BeApproximately(-15d, 5);
        //    p1[2].Should().BeApproximately(-1.44d, 5);

        //    Point3 p2 = Evaluation.CurvePointAt(res, 0.2);
        //    p2[0].Should().BeApproximately(6d, 5);
        //    p2[1].Should().BeApproximately(-6d, 5);
        //    p2[2].Should().BeApproximately(-1.47456d, 5);

        //    ICurve res1 = Evaluation.SurfaceIsoCurve(nurbsSurface, t, false);
        //    Point3 p3 = Evaluation.CurvePointAt(res1, v);
        //    p3[0].Should().BeApproximately(9d, 3);
        //    p3[1].Should().BeApproximately(-6d, 3);
        //    p3[2].Should().BeApproximately(1.69344d, 3);
        //}



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
            List<Vector3> resultToCheck = Evaluation.DerivativeBasisFunctionsGivenNI(span, parameter, degree, order, knots);

            // Assert
            resultToCheck[0][0].Should().BeApproximately(expectedResult[0, 0], GeoSharkMath.MaxTolerance);
            resultToCheck[0][1].Should().BeApproximately(expectedResult[0, 1], GeoSharkMath.MaxTolerance);
            resultToCheck[0][2].Should().BeApproximately(expectedResult[0, 2], GeoSharkMath.MaxTolerance);

            resultToCheck[1][0].Should().BeApproximately(expectedResult[1, 0], GeoSharkMath.MaxTolerance);
            resultToCheck[1][1].Should().BeApproximately(expectedResult[1, 1], GeoSharkMath.MaxTolerance);
            resultToCheck[1][2].Should().BeApproximately(expectedResult[1, 2], GeoSharkMath.MaxTolerance);

            resultToCheck[2][0].Should().BeApproximately(expectedResult[2, 0], GeoSharkMath.MaxTolerance);
            resultToCheck[2][1].Should().BeApproximately(expectedResult[2, 1], GeoSharkMath.MaxTolerance);
            resultToCheck[2][2].Should().BeApproximately(expectedResult[2, 2], GeoSharkMath.MaxTolerance);

            resultToCheck.Count.Should().Be(order + 1);
            resultToCheck[0].Count.Should().Be(degree + 1);
        }

        [Fact]
        public void It_Returns_The_Result_Of_A_Curve_Derivatives()
        {
            // Arrange
            int degree = 3;
            int parameter = 0;
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };
            int numberDerivs = 2;
            List<Point3> controlPts = new List<Point3>
            {
                new (10, 0, 0),
                new (20, 10, 0),
                new (30, 20, 0),
                new (50, 50, 0)
            };

            NurbsCurve curve = new NurbsCurve(degree, knots, controlPts);

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
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 1, 1 };
            List<double> weight = new List<double> { 1, 1, 2 };
            List<Point3> controlPts = new List<Point3>()
            {
                new Point3(1, 0, 0),
                new Point3(1, 1, 0),
                new Point3(0, 1, 0)
            };
            NurbsCurve curve = new NurbsCurve(degree, knots, controlPts, weight);
            int derivativesOrder = 2;

            // Act
            List<Vector3d> resultToCheck = Evaluation.RationalCurveDerivatives(curve, 0, derivativesOrder);

            // Assert
            resultToCheck[0][0].Should().Be(1);
            resultToCheck[0][1].Should().Be(0);

            resultToCheck[1][0].Should().Be(0);
            resultToCheck[1][1].Should().Be(2);

            resultToCheck[2][0].Should().Be(-4);
            resultToCheck[2][1].Should().Be(0);

            List<Vector3d> resultToCheck2 = Evaluation.RationalCurveDerivatives(curve, 1, derivativesOrder);

            resultToCheck2[0][0].Should().Be(0);
            resultToCheck2[0][1].Should().Be(1);

            resultToCheck2[1][0].Should().Be(-1);
            resultToCheck2[1][1].Should().Be(0);

            resultToCheck2[2][0].Should().Be(1);
            resultToCheck2[2][1].Should().Be(-1);

            List<Vector3d> resultToCheck3 = Evaluation.RationalCurveDerivatives(curve, 0, 3);

            resultToCheck3[3][0].Should().Be(0);
            resultToCheck3[3][1].Should().Be(-12);

            List<Vector3d> resultToCheck4 = Evaluation.RationalCurveDerivatives(curve, 1, 3);

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
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 0.5, 1, 1, 1, 1 };
            List<Point3> pts = new List<Point3>
            {
                new (0, 0, 0),
                new (1, 0, 0),
                new (2, 0, 0),
                new (3, 0, 0),
                new (4, 0, 0)
            };
            List<double> weights = new List<double> { 1, 1, 1, 1, 1 };
            NurbsCurve curve = new NurbsCurve(degree, knots, pts, weights);
            Vector3d tangentExpectedLinearCurve = new Vector3d(3, 0, 0);
            Vector3d tangentExpectedPlanarCurve = new Vector3d(tangentData[0], tangentData[1], tangentData[2]);

            // Act
            // Act on a linear nurbs curve.
            Vector3d tangentLinearCurve = Evaluation.RationalCurveTangent(curve, 0.5);
            var tangentPlanarCurve = Evaluation.RationalCurveTangent(NurbsCurveCollection.NurbsCurvePlanarExample(), t);
            Vector3d tangentNormalized = tangentPlanarCurve.Unitize();

            // Assert
            tangentLinearCurve.Should().BeEquivalentTo(tangentExpectedLinearCurve);
            tangentNormalized.EpsilonEquals(tangentExpectedPlanarCurve, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }
    }
}
