using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using GShark.Geometry.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class PolygonTests
    {
        private readonly ITestOutputHelper _testOutput;
        public PolygonTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Point3[] Planar2D => new[]
        {
            new Point3(1d, 1d, 0d), 
            new Point3(10d, 1d, 0d),
            new Point3(2d, 10d, 0d), 
            new Point3(1d, 10d, 0d), 
            new Point3(1d, 1d, 0)
        };

        public static Point3[] NotPlanar2D => new[]
        {
            new Point3(1d, 1d, 5d),
            new Point3(10d, 1d, 0d),
            new Point3(2d, 10d, 0d),
            new Point3(1d, 10d, -5d),
            new Point3(1d, 1d, 0)
        };

        public static Point3[] Planar3D => new[]
        {
            new Point3(74.264416, 36.39316, -1.884313),
            new Point3(79.65881, 22.402983, 1.741763),
            new Point3(97.679126, 13.940616, 3.812853),
            new Point3(100.92443, 30.599893, -0.585116),
            new Point3(78.805261, 45.16886, -4.22451),
            new Point3(74.264416, 36.39316, -1.884313)
        };

        [Fact]
        public void It_Returns_A_Polygon()
        {
            // Act
            Polygon polygon = new Polygon(Planar3D);

            // Assert
            polygon.Should().NotBeEmpty();
            polygon.Segments.Length.Should().Be(5);
        }

        [Fact]
        public void Polygon_Throws_An_Exception_If_The_Collection_Of_Points_Is_Less_Than_3()
        {
            // Arrange
            Point3[] pts = new[]
            {
                new Point3(74.264416, 36.39316, -1.884313), new Point3(79.65881, 22.402983, 1.741763),
            };

            // Act
            Func<Polygon> func = () => new Polygon(pts);

            // Assert
            func.Should().Throw<Exception>().WithMessage("Insufficient points for a Polygon.");
        }

        [Fact]
        public void Polygon_Throws_An_Exception_If_Points_Are_Not_Planar()
        {
            // Act
            Func<Polygon> func = () => new Polygon(NotPlanar2D);

            // Assert
            func.Should().Throw<Exception>().WithMessage("The points must be co-planar.");
        }

        [Fact]
        public void It_Returns_The_Area()
        {
            // Arrange
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            // Act
            double poly2DArea = poly2D.Area;
            double poly3DArea = poly3D.Area;

            // Assert
            poly2DArea.Should().Be(45);
            poly3DArea.Should().BeApproximately(480.580633, GeoSharkMath.MinTolerance);
        }

        [Fact]
        public void It_Returns_The_Centroid_By_Vertices()
        {
            // Arrange
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            Point3 centroid2DExpected = new Point3( 3.5, 5.5, 0.0);
            Point3 centroid3DExpected = new Point3( 86.266409, 29.701102, -0.227864);

            // Act
            Point3 poly2DCentroid = poly2D.CentroidByVertices;
            Point3 poly3DCentroid = poly3D.CentroidByVertices;

            // Assert
            poly2DCentroid.Equals(centroid2DExpected).Should().BeTrue();
            poly3DCentroid.EpsilonEquals(centroid3DExpected, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Centroid_By_Area()
        {
            // Arrange
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            Point3 centroid2DExpected = new Point3( 4.033333, 4.3, 0);
            Point3 centroid3DExpected = new Point3( 87.620479, 29.285305, -0.129984);

            // Act
            Point3 poly2DCentroid = poly2D.CentroidByArea;
            Point3 poly3DCentroid = poly3D.CentroidByArea;

            // Assert
            poly2DCentroid.DistanceTo(centroid2DExpected).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            poly3DCentroid.DistanceTo(centroid3DExpected).Should().BeLessThan(GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Polygon_Transformed_In_NurbsCurve()
        {
            // Arrange
            ICurve poly2D = new Polygon(Planar2D);
            KnotVector knots = poly2D.Knots;

            // Assert
            poly2D.Degree.Should().Be(1);
            for (int i = 1; i < poly2D.Knots.Count - 1; i++)
            {
                Point3 pt = poly2D.PointAt(knots[i]);
                Planar2D[i - 1].Equals(poly2D.PointAt(knots[i])).Should().BeTrue();
            }
        }
    }
}
