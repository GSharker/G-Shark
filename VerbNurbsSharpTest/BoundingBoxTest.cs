using System;
using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxCollections), MemberType = typeof(BoundingBoxCollection))]
        public void It_Create_A_BoundingBox_From_Points(List<Point> pts, Point min, Point max)
        {
            BoundingBox bBox = new BoundingBox(pts);

            _testOutput.WriteLine(bBox.ToString());
            Assert.NotNull(bBox);
            Assert.Equal(min, bBox.Min);
            Assert.Equal(max, bBox.Max);
        }

        [Fact]
        public void It_Test_If_A_Point_Is_Contained_Into_The_BoundingBox()
        {
            Point conteinedPt = new Point() {2.5, 4.5, 0.0};

            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            Assert.False(bBox.Contains(conteinedPt, false));
        }

        [Fact]
        public void It_Test_If_A_Point_Is_Outside_The_BoundingBox()
        {
            Point conteinedPt = new Point() { 12.4, 5.0, 0.0 };

            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            Assert.False(bBox.Contains(conteinedPt, false));
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxIntersections), MemberType = typeof(BoundingBoxCollection))]
        public void ReturnTrue_If_TwoBoundingBoxes_Intersect(List<Point> ptsBBox1, List<Point> ptsBBox2, bool result)
        {
            BoundingBox bBox1 = new BoundingBox(ptsBBox1);
            BoundingBox bBox2 = new BoundingBox(ptsBBox2);
            bool intersectionResult = BoundingBox.AreOverlapping(bBox1, bBox2, 0.0);
            Assert.Equal(result, intersectionResult);
        }

        [Fact]
        public void IntersectReturn_UnsetBBox_If_OneOfTheTwoBBoxes_IsNotInitialized()
        {
            BoundingBox bBox1 = BoundingBox.Unset;
            BoundingBox bBox2 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            BoundingBox bBoxResult = BoundingBox.Intersect(bBox1, bBox2);

            Assert.False(bBoxResult.IsValid);
            Assert.Equal(Point.Unset,bBoxResult.Min);
            Assert.Equal(Point.Unset, bBoxResult.Max);
        }

        [Fact]
        public void IntersectReturn_UnsetBBox_If_ThereIsNotIntersection()
        {
            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(BoundingBoxCollection.NegativeBoundingBox());

            BoundingBox bBoxResult = BoundingBox.Intersect(bBox1, bBox2);

            Assert.False(bBoxResult.IsValid);
            Assert.Equal(Point.Unset, bBoxResult.Min);
            Assert.Equal(Point.Unset, bBoxResult.Max);
        }

        [Fact]
        public void IntersectReturn_BBox_As_Intersection_Of_Two_BBoxes()
        {
            Point pt1 = new Point() { 5d, 5d, 0d };
            Point pt2 = new Point() { 15d, 15d, 0d };
            List<Point> pts2 = new List<Point>() { pt1, pt2};

            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(pts2);

            BoundingBox bBoxResult = BoundingBox.Intersect(bBox1, bBox2);
            Assert.True(bBoxResult.IsValid);
            Assert.Equal(bBoxResult.Min, bBox2.Min);
            Assert.Equal(bBoxResult.Max, bBox1.Max);
        }

        [Fact]
        public void Return_A_BBox_NotInitialized()
        {
            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            bBox.Clear();

            Assert.False(bBox.IsValid);
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxAxisLength), MemberType = typeof(BoundingBoxCollection))]
        public void Return_ACollection_Of_GetAxisLength(List<Point> pts, int index, double length)
        {
            BoundingBox bBox = new BoundingBox(pts);
            double lengthResult = bBox.GetAxisLength(index);
            Assert.Equal(length, lengthResult);
        }

        [Fact]
        public void Return_TheLongestAxis()
        {
            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxWithZValue());
            int longestAxis = bBox.GetLongestAxis();
            Assert.Equal(1,longestAxis);
        }

        [Fact]
        public void Return_ABooleanUnion_BetweenTwo_BoundingBoxes()
        {
            Point pt1 = new Point() { 5d, 5d, 0d };
            Point pt2 = new Point() { -15d, -13d, -5d };
            List<Point> pts2 = new List<Point>() { pt1, pt2 };
            Point pMax = new Point() {10, 10, 0};

            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(pts2);

            BoundingBox bBoxResult = bBox1.Union(bBox2);

            _testOutput.WriteLine(bBoxResult.ToString());
            Assert.True(bBoxResult.IsValid);
            Assert.Equal(pt2, bBoxResult.Min);
            Assert.Equal(pMax, bBoxResult.Max);
        }

        [Fact]
        public void BoundingBoxUnion_Return_ValidBoundingBox_IfOther_IsNotValid()
        {
            BoundingBox bBox1 = BoundingBox.Unset;
            BoundingBox bBox2 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            BoundingBox bBoxResult = BoundingBox.Union(bBox1, bBox2);

            Assert.Equal(bBoxResult, bBox2);
        }
    }
}
