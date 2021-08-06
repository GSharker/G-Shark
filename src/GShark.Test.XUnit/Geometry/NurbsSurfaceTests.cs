using FluentAssertions;
using FluentAssertions.Specialized;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Enum;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class NurbsSurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;
        public NurbsSurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_NURBS_Surface_By_Four_Points()
        {
            // Arrange
            Point3 p1 = new Point3(0.0, 0.0, 0.0);
            Point3 p2 = new Point3(10.0, 0.0, 0.0);
            Point3 p3 = new Point3(10.0, 10.0, 2.0);
            Point3 p4 = new Point3(0.0, 10.0, 4.0);

            Point3 expectedPt = new Point3(5.0, 5.0, 1.5);

            // Act
            NurbsSurface surface = NurbsSurface.CreateFromCorners(p1, p2, p3, p4);
            Point3 evalPt = Evaluation.SurfacePointAt(surface, 0.5, 0.5);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(2);
            surface.LocationPoints[0].Count.Should().Be(2);
            surface.LocationPoints[0][1].Equals(p4).Should().BeTrue();
            evalPt.EpsilonEquals(expectedPt, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { -0.020397, -0.392974, 0.919323 })]
        [InlineData(0.5, 0.5, new double[] { 0.091372, -0.395944, 0.913717 })]
        [InlineData(1.0, 1.0, new double[] { 0.507093, -0.169031, 0.845154 })]
        public void It_Returns_The_Surface_Normal_At_A_Given_U_And_V_Parameter(double u, double v, double[] pt)
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedNormal = new Vector3(pt[0], pt[1], pt[2]);

            // Act
            Vector3 normal = surface.EvaluateAt(u, v, SurfaceDirection.Normal);

            // Assert
            normal.EpsilonEquals(expectedNormal, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Evaluated_Surface_At_A_Given_U_And_V_Parameter()
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedUDirection = new Vector3(0.985802, 0.152837, 0.069541);
            Vector3 expectedVDirection = new Vector3(0.053937, 0.911792, 0.407096);

            // Act
            Vector3 uDirection = surface.EvaluateAt(0.3, 0.5, SurfaceDirection.U);
            Vector3 vDirection = surface.EvaluateAt(0.3, 0.5, SurfaceDirection.V);

            // Assert
            uDirection.EpsilonEquals(expectedUDirection, GeoSharkMath.MinTolerance).Should().BeTrue();
            vDirection.EpsilonEquals(expectedVDirection, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 1.901998, 16.685193, 5.913446 })]
        [InlineData(0.1, 0.1, new double[] { -15.044280, 3.808873, 0.968338})]
        [InlineData(1.0, 1.0, new double[] { 5, 35, 0 })]
        public void It_Returns_A_Normal_Lofted_Surface_By_Opened_Curves(double u, double v, double[] pt)
        {
            // Arrange
            List<NurbsCurve> crvs = NurbsCurveCollection.OpenCurves();
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(crvs, 3, LoftType.Normal);
            Point3 evalPt = Evaluation.SurfacePointAt(surface, u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 0.625, 17.5, 6.59375 })]
        [InlineData(0.1, 0.1, new double[] { -14.7514, 3.14, 1.63251 })]
        [InlineData(1.0, 1.0, new double[] { 5, 35, 0 })]
        public void It_Returns_A_Loose_Lofted_Surface_By_Opened_Curves(double u, double v, double[] pt)
        {
            // Arrange
            List<NurbsCurve> crvs = NurbsCurveCollection.OpenCurves();
            Point3 expectedPt = new Point3( pt[0], pt[1], pt[2] );

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(crvs, 3, LoftType.Loose);
            Point3 evalPt = Evaluation.SurfacePointAt(surface, u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_The_Curves_Are_Empty_Or_Null()
        {
            // Arange
            List<NurbsCurve> crvs = new List<NurbsCurve>();
            List<NurbsCurve> crvs2 = null;

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);
            Func<NurbsSurface> func2 = () => NurbsSurface.CreateLoftedSurface(crvs2);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
            func2.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_The_All_Curves_Are_Not_Closed_Or_Open()
        {
            // Arrange
            List<NurbsCurve> crvs = NurbsCurveCollection.OpenCurves();
            crvs[1] = crvs[1].Close();

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            crvs[0].IsClosed().Should().BeFalse();
            crvs[1].IsPeriodic().Should().BeTrue();
            func.Should().Throw<Exception>()
                         .WithMessage("Loft only works if all curves are open, or all curves are closed!");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_After_Cleaning_Curves()
        {
            // Arrange
            List<Point3> pts1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0) };

            List<Point3> pts2 = new List<Point3> { new Point3(-15.0, 10.0, 0.0),
                                                   new Point3(0.0, 10.0, 5.0),
                                                   new Point3(20.0, 10.0, 1.0) };

            NurbsCurve crv1 = new NurbsCurve(pts1, 2);
            NurbsCurve crv2 = null;

            List<NurbsCurve> crvs = new List<NurbsCurve>() { crv1, crv2 };

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
        }
    }
}
