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


        [Fact]
        public void It_Returns_A_NURBS_Lofted_Surface_AllOpenedCurves()
        {
            // Arrange
            List<Point3> pts1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0) };

            List<Point3> pts2 = new List<Point3> { new Point3(-15.0, 10.0, 0.0),
                                                   new Point3(0.0, 10.0, 5.0),
                                                   new Point3(20.0, 10.0, 1.0) };

            List<Point3> pts3 = new List<Point3> { new Point3(-5.0, 25.0, 0.0),
                                                   new Point3(0.0, 25.0, 20.0),
                                                   new Point3(10.0, 25.0, 0.0) };

            List<Point3> pts4 = new List<Point3> { new Point3(-5.0, 35.0, -2.0),
                                                   new Point3(0.0, 35.0, 20.0),
                                                   new Point3(5.0, 35.0, 0.0) };

            NurbsCurve c1 = new NurbsCurve(pts1, 2);
            NurbsCurve c2 = new NurbsCurve(pts2, 2);
            NurbsCurve c3 = new NurbsCurve(pts3, 2);
            NurbsCurve c4 = new NurbsCurve(pts4, 2);

            Point3 expectedPt1 = new Point3( 0.625, 17.5, 6.59375 );
            Point3 expectedPt2 = new Point3( -14.7514, 3.14, 1.63251);
            Point3 expectedPt3 = new Point3( 5, 35, 0 );

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(new List<NurbsCurve> { c1, c2, c3, c4 });
            Point3 evalPt1 = Evaluation.SurfacePointAt(surface, 0.5, 0.5);
            Point3 evalPt2 = Evaluation.SurfacePointAt(surface, 0.1, 0.1);
            Point3 evalPt3 = Evaluation.SurfacePointAt(surface, 1.0, 1.0);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(3);
            surface.LocationPoints[0].Count.Should().Be(4);
            evalPt1.EpsilonEquals(expectedPt1, GeoSharkMath.MinTolerance).Should().BeTrue();
            evalPt2.EpsilonEquals(expectedPt2, GeoSharkMath.MinTolerance).Should().BeTrue();
            evalPt3.EpsilonEquals(expectedPt3, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_NURBS_Lofted_Surface_AllClosedCurves()
        {
            // Arrange
            List<Point3> pts1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0),
                                                   new Point3(-20.0, 0.0, 0.0)};

            List<Point3> pts2 = new List<Point3> { new Point3(-15.0, 10.0, 0.0),
                                                   new Point3(0.0, 10.0, 5.0),
                                                   new Point3(20.0, 10.0, 1.0),
                                                   new Point3(-15.0, 10.0, 0.0)};

            List<Point3> pts3 = new List<Point3> { new Point3(-5.0, 25.0, 0.0),
                                                   new Point3(0.0, 25.0, 20.0),
                                                   new Point3(10.0, 25.0, 0.0),
                                                   new Point3(-5.0, 25.0, 0.0)};

            List<Point3> pts4 = new List<Point3> { new Point3(-5.0, 35.0, -2.0),
                                                   new Point3(0.0, 35.0, 20.0),
                                                   new Point3(5.0, 35.0, 0.0),
                                                   new Point3(-5.0, 35.0, -2.0)};

            NurbsCurve c1 = new NurbsCurve(pts1, 2);
            NurbsCurve c2 = new NurbsCurve(pts2, 2);
            NurbsCurve c3 = new NurbsCurve(pts3, 2);
            NurbsCurve c4 = new NurbsCurve(pts4, 2);

            Point3 expectedPt1 = new Point3(6.5625, 17.5, 6.75);
            Point3 expectedPt2 = new Point3(-11.5051, 3.14, 3.08568);
            Point3 expectedPt3 = new Point3(-5, 35, -2);

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(new List<NurbsCurve> { c1, c2, c3, c4 });
            Point3 evalPt1 = Evaluation.SurfacePointAt(surface, 0.5, 0.5);
            Point3 evalPt2 = Evaluation.SurfacePointAt(surface, 0.1, 0.1);
            Point3 evalPt3 = Evaluation.SurfacePointAt(surface, 1.0, 1.0);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(4);
            surface.LocationPoints[0].Count.Should().Be(4);
            evalPt1.EpsilonEquals(expectedPt1, GeoSharkMath.MinTolerance).Should().BeTrue();
            evalPt2.EpsilonEquals(expectedPt2, GeoSharkMath.MinTolerance).Should().BeTrue();
            evalPt3.EpsilonEquals(expectedPt3, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_Error_InitialCurves_NURBS_Lofted_Surface()
        {
            // Arange
            List<NurbsCurve> crvs = new List<NurbsCurve>();
            List<NurbsCurve> crvs2 = null;

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);
            Func<NurbsSurface> func2 = () => NurbsSurface.CreateLoftedSurface(crvs2);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("Invalid initial curves! You should select at least 2 curves");
            func2.Should().Throw<Exception>()
                         .WithMessage("Invalid initial curves! You should select at least 2 curves");
        }

        [Fact]
        public void It_Returns_Error_AllCurvesShouldBeClosedOrOpened_NURBS_Lofted_Surface()
        {
            // Arrange
            List<Point3> pts1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0) };

            List<Point3> pts2 = new List<Point3> { new Point3(-15.0, 10.0, 0.0),
                                                   new Point3(0.0, 10.0, 5.0),
                                                   new Point3(20.0, 10.0, 1.0),
                                                   new Point3(-15.0, 10.0, 0.0)};

            List<Point3> pts3 = new List<Point3> { new Point3(-5.0, 25.0, 0.0),
                                                   new Point3(0.0, 25.0, 20.0),
                                                   new Point3(10.0, 25.0, 0.0) };

            List<Point3> pts4 = new List<Point3> { new Point3(-5.0, 35.0, -2.0),
                                                   new Point3(0.0, 35.0, 20.0),
                                                   new Point3(5.0, 35.0, 0.0) };

            NurbsCurve crv1 = new NurbsCurve(pts1, 2);
            NurbsCurve crv2 = new NurbsCurve(pts2, 2);
            NurbsCurve crv3 = new NurbsCurve(pts3, 2);
            NurbsCurve crv4 = new NurbsCurve(pts4, 2);

            List<NurbsCurve> crvs = new List<NurbsCurve>() { crv1, crv2, crv3, crv4 };

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            crv1.IsClosed().Should().BeFalse();
            crv2.IsClosed().Should().BeTrue();
            func.Should().Throw<Exception>()
                         .WithMessage("Loft only works if all curves are open, or all curves are closed!");

        }

        [Fact]
        public void It_Returns_Error_CleanInputCurves_NURBS_Lofted_Surface()
        {
            // Arrange
            List<Point3> pts1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0) };

            List<Point3> pts2 = new List<Point3> { new Point3(-15.0, 10.0, 0.0),
                                                   new Point3(0.0, 10.0, 5.0),
                                                   new Point3(20.0, 10.0, 1.0) };

            NurbsCurve crv1 = new NurbsCurve(pts1, 2);
            NurbsCurve crv2 = new NurbsCurve(pts2, 2);
            NurbsCurve crv3 = null;

            List<NurbsCurve> crvs = new List<NurbsCurve>() { crv1, crv2, crv3 };

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(2);
        }

    }
}
