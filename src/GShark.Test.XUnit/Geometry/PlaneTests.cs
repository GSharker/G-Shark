using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class PlaneTests
    {
        private readonly ITestOutputHelper _testOutput;

        public PlaneTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Plane BasePlane => new Plane(new Point3(5, 0, 0 ), new Point3(10, 15, 0));
        public static Plane BasePlaneByPoints => new Plane(new Point3( 20, 20, 0 ), new Point3( 5, 5, 0), new Point3( -5, 10, 0));

        [Fact]
        public void It_Initializes_A_Plane()
        {
            // Arrange
            var origin = new Point3(5, 5, 0);
            var dir = new Point3(-10, -15, 0);

            // Act
            Plane plane = new Plane(origin, dir);

            // Assert
            plane.XAxis.EpsilonEquals(new Vector3(-0.83205, 0.5547, 0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            plane.YAxis.EpsilonEquals(new Vector3(0, 0, -1), GeoSharkMath.MaxTolerance).Should().BeTrue();
            plane.ZAxis.EpsilonEquals(new Vector3(-0.5547, -0.83205, 0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            plane.Origin.Equals(origin).Should().BeTrue();
        }

        [Fact]
        public void It_Trows_An_Exception_If_The_Three_Point_Are_Collinear()
        {
            // Arrange
            var pt1 = new Point3(5, 0, 0);
            var pt2 = new Point3(10, 0, 0);
            var pt3 = new Point3(15, 0, 0);

            // Act
            Func<Plane> func = () => new Plane(pt1, pt2, pt3);

            // Assert
            func.Should().Throw<Exception>()
                .WithMessage("Plane cannot be created, the tree points must not be collinear");
        }

        [Fact]
        public void It_Creates_A_Plane_By_Tree_Points()
        {
            // Arrange
            var pt1 = new Point3(20, 20, 0);
            var pt2 = new Point3(5, 5, 0);
            var pt3 = new Point3(-5, 10, 0);

            // Act
            Plane plane = new Plane(pt1, pt2, pt3);

            // Assert
            plane.Origin.Equals(pt1).Should().BeTrue();
            plane.XAxis.EpsilonEquals(new Vector3(-0.707107, -0.707107, 0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            plane.YAxis.EpsilonEquals(new Vector3(-0.707107, 0.707107, 0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            plane.ZAxis.EpsilonEquals(new Vector3(0, 0, -1), GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            // Arrange
            Plane plane = BasePlane;
            var pt = new Point3(7, 7, 3);
            double expectedDistance = 6.933752;

            // Act
            var closestPt = plane.ClosestPoint(pt, out double distance);

            // Assert
            closestPt.EpsilonEquals(new Point3(3.153846, 1.230769, 3), GeoSharkMath.MaxTolerance).Should().BeTrue();
            System.Math.Abs(distance).Should().BeApproximately(expectedDistance, GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Flipped_Plane()
        {
            // Arrange
            var plane = Plane.PlaneXY;
            var expectedPlane = new Plane(Plane.PlaneXY.Origin, -plane.XAxis, plane.YAxis);

            // Act
            Plane flippedPlane = plane.Flip();

            // Assert
            flippedPlane.Equals(expectedPlane).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_Plane()
        {
            // Arrange
            var pt1 = new Point3( 20, 20, 0);
            var pt2 = new Point3( 5, 5, 0);
            var pt3 = new Point3( -5, 10, 0);
            Plane plane = new Plane(pt1, pt2, pt3);
            Transform translation = Transform.Translation(new Point3( 10, 15, 0));
            Transform rotation = Transform.Rotation(GeoSharkMath.ToRadians(30), new Point3(0, 0, 0));
            var expectedOrigin = new Point3(17.320508, 42.320508, 0);
            var expectedZAxis = new Vector3( 0, 0, -1);

            // Act
            Transform combinedTransformations = translation.Combine(rotation);
            Plane transformedPlane = plane.Transform(combinedTransformations);

            // Assert
            transformedPlane.Origin.EpsilonEquals(expectedOrigin, GeoSharkMath.MaxTolerance).Should().BeTrue();
            transformedPlane.ZAxis.EpsilonEquals(expectedZAxis, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Rotated_Plane()
        {
            // Arrange
            Plane plane = BasePlaneByPoints;

            // Act
            Plane rotatedPlane = plane.Rotate(GeoSharkMath.ToRadians(30));

            // Assert
            rotatedPlane.XAxis.EpsilonEquals(new Vector3( -0.965926, -0.258819, 0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            rotatedPlane.YAxis.EpsilonEquals(new Vector3( -0.258819, 0.965926, 0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            rotatedPlane.ZAxis.EpsilonEquals(new Vector3( 0, 0, -1), GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Plane_Aligned_To_A_Given_Vector()
        {
            // Arrange
            Plane plane = BasePlaneByPoints;

            // Act
            Plane alignedPlane = plane.Align(Vector3.XAxis);

            // Assert
            alignedPlane.XAxis.EpsilonEquals(Vector3.XAxis, GeoSharkMath.MaxTolerance).Should().BeTrue();
            alignedPlane.YAxis.EpsilonEquals(new Vector3( 0.0, -1.0, 0.0), GeoSharkMath.MaxTolerance).Should().BeTrue();
            alignedPlane.ZAxis.EpsilonEquals(new Vector3( 0.0, 0.0, -1.0), GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Plane_With_A_New_Origin()
        {
            // Arrange
            Plane plane = BasePlaneByPoints;
            var newOrigin = new Point3( 50, 60, 5);

            // Act
            Plane translatedPlane = plane.SetOrigin(newOrigin);

            // Assert
            translatedPlane.Origin.EpsilonEquals(newOrigin, GeoSharkMath.MaxTolerance).Should().BeTrue();
            translatedPlane.Normal.EpsilonEquals(plane.Normal, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Plane_Which_Fits_Through_A_Set_Of_Points()
        {
            // Arrange
            var pts = new List<Point3>()
            {
                new (74.264416, 36.39316, -1.884313), new (79.65881, 22.402983, 1.741763),
                new (97.679126, 13.940616, 3.812853), new (100.92443, 30.599893, -0.585116),
                new (78.805261, 45.16886, -4.22451), new (74.264416, 36.39316, -1.884313)
            };
            var originCheck = new Point3(86.266409, 29.701102, -0.227864);
            var normalCheck = new Point3( 0.008012, 0.253783, 0.967228);

            // Act
            Plane fitPlane = Plane.FitPlane(pts, out _);

            // Assert
            fitPlane.Origin.EpsilonEquals(originCheck, GeoSharkMath.MaxTolerance).Should().BeTrue();
            fitPlane.Normal.EpsilonEquals(normalCheck, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }
    }
}
