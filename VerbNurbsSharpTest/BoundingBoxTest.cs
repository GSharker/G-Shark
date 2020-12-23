using System;
using System.Collections.Generic;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;
using Xunit;

namespace VerbNurbsSharpTest
{
    public class BoundingBoxTest
    {
        private static Point pt1 = new Point() { 0d, 0d, 0d };
        private static Point pt2 = new Point() { 10d, 0d, 0d };
        private static Point pt3 = new Point() { 10d, 10d, 0d };
        private static Point pt4 = new Point() { 5d, 5d, 0d };
        private readonly List<Point> pts = new List<Point>() { pt1, pt2, pt3, pt4 };

        [Fact]
        public void It_Create_A_BoundingBox_From_A_Collection_Of_Points()
        {
            BoundingBox bBox = new BoundingBox(pts);

            Assert.NotNull(bBox);
            Assert.Equal(bBox.Min, pt1);
            Assert.Equal(bBox.Max, pt3);
        }

        [Fact]
        public void It_Test_If_A_Point_Is_Contained_Into_The_BoundingBox()
        {
            Point conteinedPt = new Point() {2.5, 4.5, 0.0};

            BoundingBox bBox = new BoundingBox(pts);

            Assert.False(bBox.Contains(conteinedPt, false));
        }

        [Fact]
        public void It_Test_If_A_Point_Is_Outside_The_BoundingBox()
        {
            Point conteinedPt = new Point() { 12.4, 5.0, 0.0 };

            BoundingBox bBox = new BoundingBox(pts);

            Assert.False(bBox.Contains(conteinedPt, false));
        }
    }
}
