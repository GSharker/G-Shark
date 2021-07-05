using FluentAssertions;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using GShark.Core;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class BoundingBoxTests
    {
        private readonly ITestOutputHelper _testOutput;

        public BoundingBoxTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxCollections), MemberType = typeof(BoundingBoxCollection))]
        public void It_Creates_A_BoundingBox_From_Points(Point3[] pts, Point3 min, Point3 max)
        {
            // Act
            var bBox = new BoundingBox(pts);

            // Assert
            _testOutput.WriteLine(bBox.ToString());
            bBox.Should().NotBeNull();
            bBox.Min.Should().BeEquivalentTo(min);
            bBox.Max.Should().BeEquivalentTo(max);
        }

        [Fact]
        public void It_Creates_An_Aligned_BoundingBox()
        {
            // Arrange
            Plane orientedPlane = Plane.PlaneXY.Rotate(GeoSharkMath.ToRadians(30));
            var expectedMin = new Point3( 45.662928, 59.230957, -4.22451);
            var expectedMax = new Point3( 77.622297, 78.520011, 3.812853);

            // Act
            var bBox = new BoundingBox(BoundingBoxCollection.BoundingBox3D(), orientedPlane);

            // Assert
            _testOutput.WriteLine(bBox.ToString());
            bBox.Should().NotBeNull();
            bBox.Min.DistanceTo(expectedMin).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            bBox.Max.DistanceTo(expectedMax).Should().BeLessThan(GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_True_If_A_Point_Is_Contained_Into_TheBoundingBox()
        {
            // Arrange
            var conteinedPt = new Point3(2.5, 4.5, 0.0);
            var bBox = new BoundingBox(BoundingBoxCollection.BoundingBox2D());

            // Act
            var containsResult = bBox.Contains(conteinedPt, false);

            // Assert
            containsResult.Should().BeTrue();
        }

        [Fact]
        public void It_Returns_False_IfAPoint_Is_Outside_TheBoundingBox()
        {
            // Arrange
            var externalPt = new Point3(12.4, 5.0, 0.0);
            var bBox = new BoundingBox(BoundingBoxCollection.BoundingBox2D());

            // Act
            var containsResult = bBox.Contains(externalPt, false);

            // Assert
            containsResult.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxIntersections), MemberType = typeof(BoundingBoxCollection))]
        public void It_Returns_True_If_TwoBoundingBoxes_Intersect(Point3[] ptsBBox1, Point3[] ptsBBox2, bool result)
        {
            // Arrange
            var bBox1 = new BoundingBox(ptsBBox1);
            var bBox2 = new BoundingBox(ptsBBox2);

            // Act
            var intersectionResult = BoundingBox.AreOverlapping(bBox1, bBox2, 0.0);

            // Assert
            intersectionResult.Should().Be(result);
        }

        [Fact]
        public void It_Returns_BooleanUnion_Between_Two_BoundingBoxes()
        {
            // Arrange
            var pt1 = new Point3(5d, 5d, 0);
            var pt2 = new Point3(-15d, -13d, -5);
            var pts = new List<Point3> {pt1, pt2};
            var pMax = new Point3(10, 10, 0);
            var bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBox2D());
            var bBox2 = new BoundingBox(pts);

            // Act
            var bBoxResult = bBox1.Union(bBox2);

            // Assert
            bBoxResult.IsValid.Should().BeTrue();
            bBoxResult.Max.Should().BeEquivalentTo(pMax);
            bBoxResult.Min.Should().BeEquivalentTo(pt2);
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxIntersectionsUnset),
            MemberType = typeof(BoundingBoxCollection))]
        public void Intersect_Returns_UnsetBBox_If_OneOfTheTwoBBoxes_IsNotInitialized_OrNotIntersection(
            Point3[] bBoxPts, BoundingBox bBox2)
        {
            // Arrange
            var bBox1 = new BoundingBox(bBoxPts);

            // Act
            var bBoxIntersect = bBox1.Intersect(bBox2);

            // Assert
            bBoxIntersect.IsValid.Should().BeFalse();
            bBoxIntersect.Max.Equals(Vector3.Unset);
            bBoxIntersect.Min.Equals(Vector3.Unset);
        }

        [Fact]
        public void Intersect_Returns_BBox_As_Intersection_Of_Two_BBoxes()
        {
            // Arrange
            var pt1 = new Point3(5d, 5d, 0);
            var pt2 = new Point3(15d, 15d, 0);
            var pts2 = new List<Point3> {pt1, pt2};
            var bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBox2D());
            var bBox2 = new BoundingBox(pts2);

            // Act
            var bBoxResult = bBox1.Intersect(bBox2);

            // Assert
            bBoxResult.IsValid.Should().BeTrue();
            bBoxResult.Max.Should().BeEquivalentTo(bBox1.Max);
            bBoxResult.Min.Should().BeEquivalentTo(bBox2.Min);
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxAxisLength), MemberType = typeof(BoundingBoxCollection))]
        public void It_Returns_ACollection_Of_GetAxisLength(Point3[] pts, int index, double length)
        {
            // Arrange
            var bBox = new BoundingBox(pts);

            // Act
            var lengthResult = bBox.GetAxisLength(index);

            // Assert
            lengthResult.Should().Be(length);
        }

        [Fact]
        public void It_Returns_TheLongestAxis()
        {
            // Arrange
            var bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxWithZValue());

            // Act
            var longestAxis = bBox.GetLongestAxis();

            // Assert
            longestAxis.Should().Be(1);
        }

        [Fact]
        public void Union_Returns_ValidBoundingBox_If_Other_IsNotValid()
        {
            // Arrange
            var bBox1 = BoundingBox.Unset;
            var bBox2 = new BoundingBox(BoundingBoxCollection.BoundingBox2D());

            // Act
            var bBoxResult = BoundingBox.Union(bBox1, bBox2);

            // Assert
            bBoxResult.Should().Be(bBox2);
        }

        [Fact]
        public void It_Returns_A_BoundingBox_In_Ascending_Way()
        {
            // Arrange
            var bBox = new BoundingBox(new Point3(15,15,0), new Point3(5,0,0));

            // Act
            var bBoxMadeValid = bBox.MakeItValid();

            // Assert
            bBoxMadeValid.Min.Should().BeEquivalentTo(bBox.Max);
            bBoxMadeValid.Max.Should().BeEquivalentTo(bBox.Min);
        }
    }
}
