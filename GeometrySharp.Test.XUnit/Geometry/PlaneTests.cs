using System;
using System.Linq;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class PlaneTests
    {
        private readonly ITestOutputHelper _testOutput;

        public PlaneTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Plane BasePlane => new Plane(new Vector3 {5, 0, 0}, new Vector3 {10, 15, 0});
        public static Plane BasePlaneByPoints => new Plane(new Vector3 { 20, 20, 0 }, new Vector3 { 5, 5, 0 }, new Vector3 { -5, 10, 0 });

        [Fact]
        public void It_Initializes_A_Plane()
        {
            Vector3 origin = new Vector3 {5, 5, 0};
            Vector3 dir = new Vector3 {-10, -15, 0};

            Plane plane = new Plane(origin, dir);

            plane.XAxis.IsEqualRoundingDecimal(new Vector3 {-0.83205, 0.5547, 0}, 6).Should().BeTrue();
            plane.YAxis.IsEqualRoundingDecimal(new Vector3 {0, 0, -1}, 6).Should().BeTrue();
            plane.ZAxis.IsEqualRoundingDecimal(new Vector3 {-0.5547, -0.83205, 0}, 6).Should().BeTrue();
            plane.Origin.Equals(origin).Should().BeTrue();
        }

        [Fact]
        public void It_Trows_An_Exception_If_The_Three_Point_Are_Collinear()
        {
            Vector3 pt1 = new Vector3 {5, 0, 0};
            Vector3 pt2 = new Vector3 {10, 0, 0};
            Vector3 pt3 = new Vector3 {15, 0, 0};

            Func<Plane> func = () => new Plane(pt1, pt2, pt3);

            func.Should().Throw<Exception>()
                .WithMessage("Plane cannot be created, the tree points must not be collinear");
        }

        [Fact]
        public void It_Creates_A_Plane_By_Tree_Points()
        {
            Vector3 pt1 = new Vector3 {20, 20, 0};
            Vector3 pt2 = new Vector3 {5, 5, 0};
            Vector3 pt3 = new Vector3 {-5, 10, 0};

            Plane plane = new Plane(pt1, pt2, pt3);

            plane.Origin.Equals(pt1).Should().BeTrue();
            plane.XAxis.IsEqualRoundingDecimal(new Vector3 {-0.707107, -0.707107, 0}, 6).Should().BeTrue();
            plane.YAxis.IsEqualRoundingDecimal(new Vector3 {-0.707107, 0.707107, 0}, 6).Should().BeTrue();
            plane.ZAxis.IsEqualRoundingDecimal(new Vector3 {0, 0, -1}, 6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            Plane plane = BasePlane;
            Vector3 pt = new Vector3 {7, 7, 3};

            Vector3 closestPt = plane.ClosestPoint(pt, out double distance);

            closestPt.IsEqualRoundingDecimal(new Vector3 {3.153846, 1.230769, 3}, 6).Should().BeTrue();
            distance.Should().BeApproximately(6.933752, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_A_Flipped_Plane()
        {
            Plane plane = BasePlane;
            Plane flippedPlane = plane.Flip();

            flippedPlane.XAxis.Equals(plane.YAxis).Should().BeTrue();
            flippedPlane.YAxis.Equals(plane.XAxis).Should().BeTrue();
            flippedPlane.ZAxis.Equals(Vector3.Reverse(plane.ZAxis)).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_Plane()
        {
            Vector3 pt1 = new Vector3 { 20, 20, 0 };
            Vector3 pt2 = new Vector3 { 5, 5, 0 };
            Vector3 pt3 = new Vector3 { -5, 10, 0 };
            Plane plane = new Plane(pt1, pt2, pt3);
            Transform translation = Transform.Translation(new Vector3 { 10, 15, 0 });
            Transform rotation = Transform.Rotation(GeoSharpMath.ToRadians(30), new Vector3 {0, 0, 0});

            Transform combinedTransformations = translation.Combine(rotation);

            Plane transformedPlane = plane.Transform(combinedTransformations);

            transformedPlane.Origin.IsEqualRoundingDecimal(new Vector3 {17.320508, 42.320508, 0}, 6).Should().BeTrue();
            transformedPlane.ZAxis.IsEqualRoundingDecimal(new Vector3 { 0, 0, -1 }, 6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Rotated_Plane()
        {
            Plane plane = BasePlaneByPoints;

            Plane rotatedPlane = plane.Rotate(GeoSharpMath.ToRadians(30));

            rotatedPlane.XAxis.IsEqualRoundingDecimal(new Vector3 { -0.965926, -0.258819, 0 }, 6).Should().BeTrue();
            rotatedPlane.YAxis.IsEqualRoundingDecimal(new Vector3 { -0.258819, 0.965926, 0 }, 6).Should().BeTrue();
            rotatedPlane.ZAxis.IsEqualRoundingDecimal(new Vector3 { 0, 0, -1 }, 6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Plane_Aligned_To_A_Given_Vector()
        {
            Plane plane = BasePlaneByPoints;

            Plane alignedPlane = plane.Align(Vector3.XAxis);

            alignedPlane.XAxis.IsEqualRoundingDecimal(Vector3.XAxis, 6).Should().BeTrue();
            alignedPlane.YAxis.IsEqualRoundingDecimal(new Vector3 { 0.0, -1.0, 0.0 }, 6).Should().BeTrue();
            alignedPlane.ZAxis.IsEqualRoundingDecimal(new Vector3 { 0.0, 0.0, -1.0 }, 6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Plane_With_A_New_Origin()
        {
            Plane plane = BasePlaneByPoints;
            Vector3 newOrigin = new Vector3 { 50, 60, 5 };

            Plane translatedPlane = plane.SetOrigin(newOrigin);

            translatedPlane.Origin.Should().BeEquivalentTo(newOrigin);
            translatedPlane.Normal.Should().BeEquivalentTo(plane.Normal);
        }

        [Fact]
        public void It_Returns_A_Plane_Which_Fits_Through_A_Set_Of_Points()
        {
            Vector3[] pts = new[]
            {
                new Vector3 {74.264416, 36.39316, -1.884313}, new Vector3 {79.65881, 22.402983, 1.741763},
                new Vector3 {97.679126, 13.940616, 3.812853}, new Vector3 {100.92443, 30.599893, -0.585116},
                new Vector3 {78.805261, 45.16886, -4.22451}, new Vector3 {74.264416, 36.39316, -1.884313}
            };
            Vector3 originCheck = new Vector3 {86.266409, 29.701102, -0.227864};
            Vector3 normalCheck = new Vector3 { 0.008012, 0.253783, 0.967228 };

            Plane fitPlane = Plane.FitPlane(pts, out _);

            fitPlane.Origin.IsEqualRoundingDecimal(originCheck, 5).Should().BeTrue();
            fitPlane.Normal.IsEqualRoundingDecimal(normalCheck, 5).Should().BeTrue();
        }
    }
}
