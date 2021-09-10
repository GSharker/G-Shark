using FluentAssertions;
using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
            NurbsSurface surfaceCcw = NurbsSurface.CreateFromCorners(p1, p2, p3, p4);
            NurbsSurface surfaceCw = NurbsSurface.CreateFromCorners(p1, p4, p3, p2);
            Point3 evalPtCcw = new Point3(surfaceCcw.PointAt(0.5, 0.5));
            Point3 evalPtCw = new Point3(surfaceCw.PointAt(0.5, 0.5));

            // Assert
            surfaceCcw.Should().NotBeNull();
            surfaceCcw.ControlPointLocations.Count.Should().Be(2);
            surfaceCcw.ControlPointLocations[0].Count.Should().Be(2);
            surfaceCcw.ControlPointLocations[0][1].Equals(p4).Should().BeTrue();
            (evalPtCcw.EpsilonEquals(expectedPt, GSharkMath.MinTolerance) && evalPtCw.EpsilonEquals(expectedPt, GSharkMath.MinTolerance)).Should().BeTrue();
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
            Vector3 normal = surface.EvaluateAt(u, v, EvaluateSurfaceDirection.Normal);

            // Assert
            normal.EpsilonEquals(expectedNormal, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Evaluated_Surface_At_A_Given_U_And_V_Parameter()
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedUDirection = new Vector3(0.985802, 0.152837, 0.069541);
            Vector3 expectedVDirection = new Vector3(0.053937, 0.911792, 0.407096);

            // Act
            Vector3 uDirection = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.U);
            Vector3 vDirection = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.V);

            // Assert
            uDirection.EpsilonEquals(expectedUDirection, GSharkMath.MinTolerance).Should().BeTrue();
            vDirection.EpsilonEquals(expectedVDirection, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 1.901998, 16.685193, 5.913446 })]
        [InlineData(0.1, 0.1, new double[] { -15.044280, 3.808873, 0.968338 })]
        [InlineData(1.0, 1.0, new double[] { 5, 35, 0 })]
        public void It_Returns_A_Normal_Lofted_Surface_By_Opened_Curves(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(NurbsBaseCollection.OpenNurbs());
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 0.625, 17.5, 6.59375 })]
        [InlineData(0.1, 0.1, new double[] { -14.7514, 3.14, 1.63251 })]
        [InlineData(1.0, 1.0, new double[] { 5, 35, 0 })]
        public void It_Returns_A_Loose_Lofted_Surface_By_Opened_Curves(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(NurbsBaseCollection.OpenNurbs(), LoftType.Loose);
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 8.515625, 17.5, 1.890625 })]
        [InlineData(0.1, 0.1, new double[] { -3.9403, 3.14, 6.446595 })]
        [InlineData(1.0, 1.0, new double[] { -2.5, 35, 9 })]
        public void It_Returns_A_Loose_Lofted_Surface_By_Closed_Curves(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(NurbsBaseCollection.ClosedNurbs(), LoftType.Loose);
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_The_Curves_Are_Null()
        {
            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(null);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_There_Are_Null_Curves()
        {
            // Arrange
            List<NurbsBase> crvs = NurbsBaseCollection.OpenNurbs();
            crvs.Add(null);

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("The input set contains null curves.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_Curves_Count_Are_Less_Than_Two()
        {
            // Arrange
            NurbsBase[] crvs = { NurbsBaseCollection.OpenNurbs()[0] };

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_The_All_Curves_Are_Not_Closed_Or_Open()
        {
            // Arrange
            List<NurbsBase> crvs = NurbsBaseCollection.OpenNurbs();
            crvs[1] = crvs[1].Close();

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.CreateLoftedSurface(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("Loft only works if all curves are open, or all curves are closed.");
        }

        [Theory]
        [InlineData(new double[] { 2.60009, 7.69754, 3.408162 }, new double[] { 2.5, 7, 5 })]
        [InlineData(new double[] { 2.511373, 1.994265, 0.887211 }, new double[] { 2.5, 1.5, 2 })]
        [InlineData(new double[] { 8.952827, 2.572942, 0.735217 }, new double[] { 9, 2.5, 1 })]
        [InlineData(new double[] { 5.073733, 4.577509, 1.978153 }, new double[] { 5, 5, 1 })]
        public void Returns_The_Closest_Point_On_The_Surface(double[] expectedPt, double[] testPt)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 pt = new Point3(testPt[0], testPt[1], testPt[2]);
            Point3 expectedClosestPt = new Point3(expectedPt[0], expectedPt[1], expectedPt[2]);

            // Act
            Point3 closestPt = surface.ClosestPoint(pt);

            // Assert
            closestPt.DistanceTo(expectedClosestPt).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Returns_True_If_Two_Surfaces_Are_Equals()
        {
            // Arrange
            NurbsSurface surface0 = NurbsSurfaceCollection.SurfaceFromPoints();
            NurbsSurface surface1 = NurbsSurfaceCollection.SurfaceFromPoints();

            // Assert
            surface0.Equals(surface1).Should().BeTrue();
        }

        [Fact]
        public void Returns_True_If_Surface_Is_Close()
        {
            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(NurbsBaseCollection.ClosedNurbs(), LoftType.Loose);

            // Assert
            surface.IsClosed(SurfaceDirection.V).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { 0.2655, 1, 2.442 })]
        [InlineData(0.5, 0.5, new double[] { 4.0625, 5, 4.0625 })]
        [InlineData(1.0, 1.0, new double[] { 10, 10, 0 })]
        public void Returns_A_Ruled_Surface_Between_Two_Nurbs_Curve(double u, double v, double[] pt1)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt1[0], pt1[1], pt1[2]);
            List<Point3> ptsA = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(0, 0, 5),
                new Point3(5, 0, 5),
                new Point3(5, 0, 0),
                new Point3(10, 0, 0)
            };

            List<Point3> ptsB = new List<Point3>
            {
                new Point3(0, 10, 0),
                new Point3(0, 10, 5),
                new Point3(5, 10, 5),
                new Point3(5, 10, 0),
                new Point3(10, 10, 0)
            };

            NurbsCurve curveA = new NurbsCurve(ptsA, 3);
            NurbsCurve curveB = new NurbsCurve(ptsB, 2);

            // Act
            NurbsSurface ruledSurface = NurbsSurface.CreateRuledSurface(curveA, curveB);
            Point3 pointAt = ruledSurface.PointAt(u, v);

            // Assert
            pointAt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { 0.0225, 1, 2.055 })]
        [InlineData(0.5, 0.5, new double[] { 4.6875, 5, 4.6875 })]
        [InlineData(1.0, 1.0, new double[] { 10, 10, 0 })]
        public void Returns_A_Ruled_Surface_Between_A_Polyline_And_A_Nurbs_Curve(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);
            List<Point3> ptsA = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(0, 0, 5),
                new Point3(5, 0, 5),
                new Point3(5, 0, 0),
                new Point3(10, 0, 0)
            };

            List<Point3> ptsB = new List<Point3>
            {
                new Point3(0, 10, 0),
                new Point3(0, 10, 5),
                new Point3(5, 10, 5),
                new Point3(5, 10, 0),
                new Point3(10, 10, 0)
            };

            PolyLine poly = new PolyLine(ptsA);
            NurbsCurve curveB = new NurbsCurve(ptsB, 2);

            // Act
            NurbsSurface ruledSurface = NurbsSurface.CreateRuledSurface(poly.ToNurbs(), curveB);
            Point3 pointAt = ruledSurface.PointAt(u, v);

            // Assert
            pointAt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }
    }
}