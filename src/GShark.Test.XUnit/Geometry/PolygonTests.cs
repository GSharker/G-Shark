using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Linq;
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
            polygon.Segments.Count.Should().Be(5);
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
            poly3DArea.Should().BeApproximately(480.580630021221, GSharkMath.MinTolerance);
        }

        [Fact]
        public void It_Returns_The_Centroid_By_Vertices()
        {
            // Arrange
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            Point3 centroid2DExpected = new Point3(3.5, 5.5, 0.0);
            Point3 centroid3DExpected = new Point3(86.266409, 29.701102, -0.227864);

            // Act
            Point3 poly2DCentroid = poly2D.CentroidByVertices;
            Point3 poly3DCentroid = poly3D.CentroidByVertices;

            // Assert
            poly2DCentroid.Equals(centroid2DExpected).Should().BeTrue();
            poly3DCentroid.EpsilonEquals(centroid3DExpected, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Centroid_By_Area()
        {
            // Arrange
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            Point3 centroid2DExpected = new Point3(4.033333, 4.3, 0);
            Point3 centroid3DExpected = new Point3(87.620479, 29.285305, -0.129984);

            // Act
            Point3 poly2DCentroid = poly2D.CentroidByArea;
            Point3 poly3DCentroid = poly3D.CentroidByArea;

            // Assert
            poly2DCentroid.DistanceTo(centroid2DExpected).Should().BeLessThan(GSharkMath.MaxTolerance);
            poly3DCentroid.DistanceTo(centroid3DExpected).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Polygon_Transformed_In_NurbsBase()
        {
            // Arrange
            PolyLine polygon = new Polygon(Planar2D);
            double lengthSum = 0.0;

            // Act
            NurbsBase polygonCurve = polygon.ToNurbs();

            // Assert
            polygonCurve.Degree.Should().Be(1);
            polygonCurve.ControlPointLocations[0]
                .EpsilonEquals(polygonCurve.ControlPointLocations.Last(), GSharkMath.MinTolerance).Should().BeTrue();
            for (int i = 0; i < polygon.SegmentsCount; i++)
            {
                lengthSum += polygon.Segments[i].Length;
                polygon[i + 1].EpsilonEquals(polygonCurve.PointAtLength(lengthSum), GSharkMath.MaxTolerance).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(0, 7.071068)]
        [InlineData(1, 12.205361)]
        [InlineData(2, 5.481013)]
        [InlineData(3, 7.071068)]
        [InlineData(4, 7.071068)]
        public void Returns_The_Offset_Of_A_Polygon(int vertex, double expectedDistance)
        {
            // Arrange
            Polygon polygon = new Polygon(PolygonTests.Planar2D);
            double offset = 5;

            PolyLine offsetPolygon = polygon.Offset(offset, Plane.PlaneXY);

            // Assert
            polygon[vertex].DistanceTo(offsetPolygon[vertex]).Should()
                .BeApproximately(expectedDistance, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Rectangle()
        {
            // Arrange
            double xDimension = 20;
            double yDimension = 10;
            Point3 expectedPoint = new Point3(0.0, -10.0, -5.0);

            // Act
            Polygon rectangle = Polygon.Rectangle(Plane.PlaneYZ, xDimension, yDimension);

            // Assert
            rectangle.Count.Should().Be(5);
            rectangle[0].DistanceTo(expectedPoint).Should().BeLessThan(GSharkMath.MaxTolerance);
            rectangle[0].DistanceTo(rectangle[1]).Should().Be(xDimension);
            rectangle[1].DistanceTo(rectangle[2]).Should().Be(yDimension);
        }

        [Fact]
        public void It_Returns_A_Regular_Polygon()
        {
            // Arrange
            int numberOfSegments = 5;
            double radius = 15;
            Plane pl = Plane.PlaneYZ;

            // Act
            Polygon polygon = Polygon.RegularPolygon(pl, radius, numberOfSegments);

            // Assert
            polygon.Count.Should().Be(numberOfSegments + 1);
            polygon[0].DistanceTo(pl.Origin).Should().Be(radius);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1.5)]
        public void RegularPolygon_Throw_An_Exception_If_The_Radius_Is_Less_Or_Equal_To_Zero(double radius)
        {
            // Act
            Func<Polygon> func = () => Polygon.RegularPolygon(Plane.PlaneYZ, radius, 5);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(0)]
        public void RegularPolygon_Throw_An_Exception_If_Number_Of_Sides_Is_Less_Than_Three(int numberOfSegments)
        {
            // Act
            Func<Polygon> func = () => Polygon.RegularPolygon(Plane.PlaneYZ, 10.0, numberOfSegments);

            // Assert
            func.Should().Throw<Exception>();
        }
    }
}
