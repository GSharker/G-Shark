using FluentAssertions;
using GeometrySharp.Geometry;
using System.Collections.Generic;
using Xunit;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class RayTests
    {
        public static IEnumerable<object[]> PointAlongTheRay =>
            new List<object[]>
            {
                new object[] { new Vector3{ 3.0930734141595426, 11.54653670707977, 6.726731646460115 }, 15},
                new object[] { new Vector3{ -27.457431218879393, -3.7287156094396963, 14.364357804719848 }, -20}
            };

        [Theory]
        [MemberData(nameof(PointAlongTheRay))]
        public void It_Returns_An_Point_Along_The_Ray(Vector3 expected, double amplitude)
        {
            // Arrange
            Ray ray = new Ray(new Vector3{ -10, 5, 10 }, new Vector3{ 20, 10, -5 });

            // Act
            Vector3 pointAlongTheRay = ray.OnRay(amplitude);

            // Assert
            pointAlongTheRay.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            // Arrange
            Ray ray = new Ray(new Vector3 { 0, 0, 0 }, new Vector3 { 30, 45, 0 });
            Vector3 pt = new Vector3 { 10, 20, 0 };
            Vector3 expectedPt = new Vector3 { 12.30769230769231, 18.461538461538463, 0 };

            // Act
            Vector3 closestPt = ray.ClosestPoint(pt);

            // Assert
            closestPt.Should().BeEquivalentTo(expectedPt);
        }

        [Fact]
        public void It_Returns_The_Distance_To_A_Point()
        {
            // Arrange
            Ray ray = new Ray(new Vector3() { 0, 0, 0 }, new Vector3() { 30, 45, 0 });
            Vector3 pt = new Vector3() { 10, 20, 0 };
            const double distanceExpected = 2.7735009811261464;

            // Act
            double distance = ray.DistanceTo(pt);

            // Assert
            distance.Should().Be(distanceExpected);
        }

        [Fact]
        public void It_Returns_A_Point_At_The_T_Parameter()
        {
            // Arrange
            Ray ray = new Ray(new Vector3 { 0, 0, 0 }, new Vector3 { -7, 10, -5 });

            // Act
            Vector3 pt = ray.PointAt(1.25);

            // Assert
            pt.Should().BeEquivalentTo(new Vector3 { -8.75, 12.5, -6.25 });
        }
    }
}
