using System.Collections.Generic;
using FluentAssertions;
using VerbNurbsSharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Geometry
{
    [Trait("Category", "BoundingBox")]
    public class BoundingBoxTest
    {
        private readonly ITestOutputHelper _testOutput;

        public BoundingBoxTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxCollections), MemberType = typeof(BoundingBoxCollection))]
        public void It_Create_A_BoundingBox_From_Points(List<Vector3> pts, Vector3 min, Vector3 max)
        {
            BoundingBox bBox = new BoundingBox(pts);

            _testOutput.WriteLine(bBox.ToString());
            bBox.Should().NotBeNull();
            bBox.Min.Should().BeEquivalentTo(min);
            bBox.Max.Should().BeEquivalentTo(max);
        }

        [Fact]
        public void ReturnTrue_IfAPoint_Is_Contained_Into_TheBoundingBox()
        {
            Vector3 conteinedPt = new Vector3() { 2.5, 4.5, 0.0 };

            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            bool containsResult = bBox.Contains(conteinedPt, false);

            containsResult.Should().BeTrue();
        }

        [Fact]
        public void ReturnFalse_IfAPoint_Is_Outside_TheBoundingBox()
        {
            Vector3 externalPt = new Vector3() { 12.4, 5.0, 0.0 };

            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            bool containsResult = bBox.Contains(externalPt, false);

            containsResult.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxIntersections), MemberType = typeof(BoundingBoxCollection))]
        public void ReturnTrue_If_TwoBoundingBoxes_Intersect(List<Vector3> ptsBBox1, List<Vector3> ptsBBox2, bool result)
        {
            BoundingBox bBox1 = new BoundingBox(ptsBBox1);
            BoundingBox bBox2 = new BoundingBox(ptsBBox2);

            bool intersectionResult = BoundingBox.AreOverlapping(bBox1, bBox2, 0.0);

            intersectionResult.Should().Be(result);
        }

        [Fact]
        public void Return_BooleanUnion_BetweenTwo_BoundingBoxes()
        {
            Vector3 pt1 = new Vector3() { 5d, 5d, 0d };
            Vector3 pt2 = new Vector3() { -15d, -13d, -5d };
            List<Vector3> pts = new List<Vector3>() { pt1, pt2 };
            Vector3 pMax = new Vector3() { 10, 10, 0 };

            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(pts);
            BoundingBox bBoxResult = bBox1.Union(bBox2);

            _testOutput.WriteLine(bBoxResult.ToString());
            bBoxResult.IsValid.Should().BeTrue();
            bBoxResult.Max.Should().BeEquivalentTo(pMax);
            bBoxResult.Min.Should().BeEquivalentTo(pt2);
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxIntersectionsUnset), MemberType = typeof(BoundingBoxCollection))]
        public void IntersectReturns_UnsetBBox_If_OneOfTheTwoBBoxes_IsNotInitialized_OrNotIntersection(BoundingBox bBox1, BoundingBox bBox2)
        {
            BoundingBox bBoxIntersect = bBox2.Intersect(bBox1);

            bBoxIntersect.IsValid.Should().BeFalse();
            bBoxIntersect.Max.Should().BeEquivalentTo(Vector3.Unset);
            bBoxIntersect.Min.Should().BeEquivalentTo(Vector3.Unset);
        }

        [Fact]
        public void IntersectReturns_BBox_As_Intersection_Of_Two_BBoxes()
        {
            Vector3 pt1 = new Vector3() { 5d, 5d, 0d };
            Vector3 pt2 = new Vector3() { 15d, 15d, 0d };
            List<Vector3> pts2 = new List<Vector3>() { pt1, pt2 };

            BoundingBox bBox1 = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());
            BoundingBox bBox2 = new BoundingBox(pts2);
            BoundingBox bBoxResult = bBox1.Intersect(bBox2);

            bBoxResult.IsValid.Should().BeTrue();
            bBoxResult.Max.Should().BeEquivalentTo(bBox1.Max);
            bBoxResult.Min.Should().BeEquivalentTo(bBox2.Min);
        }

        [Fact]
        public void Return_A_BBox_NotInitialized()
        {
            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxFrom5Points());

            bBox.Clear();

            bBox.IsValid.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(BoundingBoxCollection.BoundingBoxAxisLength), MemberType = typeof(BoundingBoxCollection))]
        public void Return_ACollection_Of_GetAxisLength(List<Vector3> pts, int index, double length)
        {
            BoundingBox bBox = new BoundingBox(pts);

            double lengthResult = bBox.GetAxisLength(index);

            lengthResult.Should().Be(length);
        }

        [Fact]
        public void Return_TheLongestAxis()
        {
            BoundingBox bBox = new BoundingBox(BoundingBoxCollection.BoundingBoxWithZValue());

            int longestAxis = bBox.GetLongestAxis();

            longestAxis.Should().Be(1);
        }

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
