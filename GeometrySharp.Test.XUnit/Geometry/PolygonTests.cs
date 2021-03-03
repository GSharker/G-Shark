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
        public static Vector3[] Planar2D => new[] { new Vector3 { 1d,1d,0d }, new Vector3 { 10d, 1d, 0d },
            new Vector3 { 2d, 10d, 0d }, new Vector3 { 1d, 10d, 0d }, new Vector3 { 1d, 1d, 0d } };

        public static Vector3[] Planar3D => new[] { new Vector3 { 70.471955, 29.063798, 0.070192 }, new Vector3 { 96.272861, 10.35717, 4.764732 },
            new Vector3 { 103.439444, 28.619067, -0.086218 }, new Vector3 { 79.853746, 47.195176, -4.764863 }, new Vector3 { 70.471955, 29.063798, 0.070192 } };

        [Fact]
        public void It_Returns_The_Area()
        {
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            double poly2DArea = poly2D.Area;
            double poly3DArea = poly3D.Area;

            poly2DArea.Should().Be(45);
            poly3DArea.Should().BeApproximately(624.027814, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_The_Centroid_By_Vertices()
        {
            Polygon poly2D = new Polygon(Planar2D);
            Polygon poly3D = new Polygon(Planar3D);

            Vector3 Centroid2DCheck = new Vector3 { 3.5, 5.5, 0.0 };
            Vector3 Centroid3DCheck = new Vector3 { 87.509502, 28.808803, -0.004039 };

            Vector3 poly2DCentroid = poly2D.CentroidByVertices;
            Vector3 poly3DCentroid = poly3D.CentroidByVertices;

            poly2DCentroid.Equals(Centroid2DCheck).Should().BeTrue();
            poly3DCentroid.IsEqualRoundingDecimal(Centroid3DCheck, 6).Should().BeTrue();
        }
    }
}
