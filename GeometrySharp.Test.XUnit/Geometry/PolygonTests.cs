using System;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class PolygonTests
    {
        private readonly ITestOutputHelper _testOutput;
        public PolygonTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Vector3[] Planar2D => new[]
        {
            new Vector3 {1d, 1d, 0d}, new Vector3 {10d, 1d, 0d},
            new Vector3 {2d, 10d, 0d}, new Vector3 {1d, 10d, 0d}, new Vector3 {1d, 1d, 0d}
        };

        public static Vector3[] NotPlanar2D => new[]
        {
            new Vector3 {1d, 1d, 5d}, new Vector3 {10d, 1d, 0d},
            new Vector3 {2d, 10d, 0d}, new Vector3 {1d, 10d, -5d}, new Vector3 {1d, 1d, 0d}
        };

        public static Vector3[] Planar3D => new[]
        {
            new Vector3 {74.264416, 36.39316, -1.884313}, new Vector3 {79.65881, 22.402983, 1.741763},
            new Vector3 {97.679126, 13.940616, 3.812853}, new Vector3 {100.92443, 30.599893, -0.585116},
            new Vector3 {78.805261, 45.16886, -4.22451}, new Vector3 {74.264416, 36.39316, -1.884313}
        };

        [Fact]
        public void It_Returns_A_Polygon()
        {
            Polygon polygon = new Polygon(Planar3D);

            polygon.Should().NotBeEmpty();
            polygon.Segments().Length.Should().Be(5);
        }

        [Fact]
        public void Polygon_Throws_An_Exception_If_The_Collection_Of_Points_Is_Less_Than_3()
        {
            Vector3[] pts = new[]
            {
                new Vector3 {74.264416, 36.39316, -1.884313}, new Vector3 {79.65881, 22.402983, 1.741763},
            };

            Func<Polygon> func = () => new Polygon(pts);

            func.Should().Throw<Exception>().WithMessage("Insufficient points for a Polygon.");
        }

        [Fact]
        public void Polygon_Throws_An_Exception_If_Points_Are_Not_Planar()
        {
            Func<Polygon> func = () => new Polygon(NotPlanar2D);

            func.Should().Throw<Exception>().WithMessage("The points must be co-planar.");
        }

        [Fact]
        public void It_Returns_The_Area()
        {
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            double poly2DArea = poly2D.Area;
            double poly3DArea = poly3D.Area;

            poly2DArea.Should().Be(45);
            poly3DArea.Should().BeApproximately(480.580633, GeoSharpMath.MINTOLERANCE);
        }

        [Fact]
        public void It_Returns_The_Centroid_By_Vertices()
        {
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            Vector3 Centroid2DCheck = new Vector3 { 3.5, 5.5, 0.0 };
            Vector3 Centroid3DCheck = new Vector3 { 86.266409, 29.701102, -0.227864 };

            Vector3 poly2DCentroid = poly2D.CentroidByVertices;
            Vector3 poly3DCentroid = poly3D.CentroidByVertices;

            poly2DCentroid.Equals(Centroid2DCheck).Should().BeTrue();
            poly3DCentroid.IsEqualRoundingDecimal(Centroid3DCheck, 5).Should().BeTrue();
        }
    }
}
