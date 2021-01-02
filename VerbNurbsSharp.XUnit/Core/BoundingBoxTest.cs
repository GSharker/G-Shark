using System;
using System.Collections.Generic;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharpTest
{
    public class BoundingBoxTest
    {
        private readonly ITestOutputHelper _testOutput;

        public BoundingBoxTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Trait("Category", "BoundingBox")]
        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxCollections), MemberType = typeof(BoundingBoxCollection))]
        public void It_Create_A_BoundingBox_From_Points(List<Point> pts, Point min, Point max)
        {
            BoundingBox bBox = new BoundingBox(pts);

            _testOutput.WriteLine(bBox.ToString());
            bBox.Should().NotBeNull();
            bBox.Min.Should().BeEquivalentTo(min);
            bBox.Max.Should().BeEquivalentTo(max);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void ReturnTrue_IfAPoint_Is_Contained_Into_TheBoundingBox()
        {
            Point conteinedPt = new Point() {2.5, 4.5, 0.0};

            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            bool containsResult = bBox.Contains(conteinedPt, false);

            containsResult.Should().BeTrue();
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void ReturnFalse_IfAPoint_Is_Outside_TheBoundingBox()
        {
            Point externalPt = new Point() { 12.4, 5.0, 0.0 };

            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            bool containsResult = bBox.Contains(externalPt, false);

            containsResult.Should().BeFalse();
        }

        [Trait("Category", "BoundingBox")]
        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxIntersections), MemberType = typeof(BoundingBoxCollection))]
        public void ReturnTrue_If_TwoBoundingBoxes_Intersect(List<Point> ptsBBox1, List<Point> ptsBBox2, bool result)
        {
            BoundingBox bBox1 = new BoundingBox(ptsBBox1);
            BoundingBox bBox2 = new BoundingBox(ptsBBox2);

            bool intersectionResult = BoundingBox.AreOverlapping(bBox1, bBox2, 0.0);

            intersectionResult.Should().Be(result);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void IntersectReturns_UnsetBBox_If_OneOfTheTwoBBoxes_IsNotInitialized()
        {
            BoundingBox bBox1 = BoundingBox.Unset;
            BoundingBox bBox2 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            BoundingBox bBoxResult = BoundingBox.Intersect(bBox1, bBox2);

            bBoxResult.IsValid.Should().BeFalse();
            bBoxResult.Max.Should().BeEquivalentTo(Point.Unset);
            bBoxResult.Min.Should().BeEquivalentTo(Point.Unset);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void IntersectReturns_UnsetBBox_If_ThereIsNotIntersection()
        {
            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(BoundingBoxCollection.NegativeBoundingBox());

            BoundingBox bBoxResult = BoundingBox.Intersect(bBox1, bBox2);

            bBoxResult.IsValid.Should().BeFalse();
            bBoxResult.Max.Should().BeEquivalentTo(Point.Unset);
            bBoxResult.Min.Should().BeEquivalentTo(Point.Unset);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void IntersectReturns_BBox_As_Intersection_Of_Two_BBoxes()
        {
            Point pt1 = new Point() { 5d, 5d, 0d };
            Point pt2 = new Point() { 15d, 15d, 0d };
            List<Point> pts2 = new List<Point>() { pt1, pt2};

            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(pts2);
            BoundingBox bBoxResult = bBox1.Intersect(bBox2);

            bBoxResult.IsValid.Should().BeTrue();
            bBoxResult.Max.Should().BeEquivalentTo(bBox1.Max);
            bBoxResult.Min.Should().BeEquivalentTo(bBox2.Min);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void Return_A_BBox_NotInitialized()
        {
            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            bBox.Clear();

            bBox.IsValid.Should().BeFalse();
        }

        [Trait("Category", "BoundingBox")]
        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxAxisLength), MemberType = typeof(BoundingBoxCollection))]
        public void Return_ACollection_Of_GetAxisLength(List<Point> pts, int index, double length)
        {
            BoundingBox bBox = new BoundingBox(pts);

            double lengthResult = bBox.GetAxisLength(index);

            lengthResult.Should().Be(length);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void Return_TheLongestAxis()
        {
            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxWithZValue());

            int longestAxis = bBox.GetLongestAxis();

            longestAxis.Should().Be(1);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void Return_ABooleanUnion_BetweenTwo_BoundingBoxes()
        {
            Point pt1 = new Point() { 5d, 5d, 0d };
            Point pt2 = new Point() { -15d, -13d, -5d };
            List<Point> pts = new List<Point>() { pt1, pt2 };
            Point pMax = new Point() {10, 10, 0};

            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(pts);
            BoundingBox bBoxResult = bBox1.Union(bBox2);

            _testOutput.WriteLine(bBoxResult.ToString());
            bBoxResult.IsValid.Should().BeTrue();
            bBoxResult.Max.Should().BeEquivalentTo(pMax);
            bBoxResult.Min.Should().BeEquivalentTo(pt2);
        }

        [Trait("Category", "BoundingBox")]
        [Fact]
        public void Union_Returns_TheValidBoundingBox_IfOther_IsNotValid()
        {
            BoundingBox bBox1 = BoundingBox.Unset;
            BoundingBox bBox2 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            BoundingBox bBoxResult = BoundingBox.Union(bBox1, bBox2);

            bBoxResult.Should().Be(bBox2);
        }
    }
}
