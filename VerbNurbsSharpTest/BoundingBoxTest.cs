using System;
using System.Collections.Generic;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;
using Xunit;

namespace VerbNurbsSharpTest
{
    public class BoundingBoxTest
    {
        [Fact]
        public void It_Create_A_BoundingBox_From_A_Collection_Of_Points()
        {
            //Assert
            Point pt1 = new Point() { 0d, 0d, 0d };
            Point pt2 = new Point() { 10d, 0d, 0d };
            Point pt3 = new Point() { 10d, 10d, 0d };
            Point pt4 = new Point() { 5d, 5d, 0d };
            List<Point> pts = new List<Point>() { pt1, pt2, pt3, pt4 };

            //Act
            BoundingBox bBox = new BoundingBox(pts);
            //Assert
            Assert.NotNull(bBox);
            Assert.Equal(bBox.Min, pt1);
            Assert.Equal(bBox.Max, pt3);
        }
    }
}
