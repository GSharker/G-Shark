using System;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;
using Xunit;

namespace VerbNurbsSharpTest
{
    public class BoundingBoxTest
    {
        [Fact]
        public void It_Create_A_BoundingBox()
        {
            Point pt1 = new Point() {0, 0, 0};
            Point pt2 = new Point() {10, 0, 0};

            Line line = new Line(pt1, pt2);
            Assert.NotNull(line);
        }
    }
}
