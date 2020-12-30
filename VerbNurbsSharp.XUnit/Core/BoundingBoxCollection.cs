using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharpTest
{
    public static class BoundingBoxCollection
    {
        public static List<Point> BoundingBoxFrom5Points()
        {
            Point pt1 = new Point() { 0d, 0d, 0d };
            Point pt2 = new Point() { 10d, 0d, 0d };
            Point pt3 = new Point() { 10d, 10d, 0d };
            Point pt4 = new Point() { 5d, 5d, 0d };
            return new List<Point>() { pt1, pt2, pt3, pt4 };
        }

        public static List<Point> NegativeBoundingBox()
        {
            Point pt1 = new Point() { -15d, -15d, 0d };
            Point pt2 = new Point() { -5d, -5d, 0d };
            return new List<Point>() { pt1, pt2};
        }

        public static List<Point> NegativeMinValuePositiveMaxValue()
        {
            Point pt1 = new Point() { -12d, -17d, 0d };
            Point pt2 = new Point() { 10d, 13d, 0d };
            return new List<Point>() { pt1, pt2 };
        }

        public static List<Point> BoundingBoxWithZValue()
        {
            Point pt1 = new Point() { -12d, -17d, -10d };
            Point pt2 = new Point() { 10d, 13d, 5d };
            return new List<Point>() { pt1, pt2 };
        }

        public static IEnumerable<object[]> BoundingBoxCollections
        {
            get
            {
                yield return new object[]{BoundingBoxFrom5Points(), BoundingBoxFrom5Points()[0], BoundingBoxFrom5Points()[2]};
                yield return new object[]{NegativeBoundingBox(), NegativeBoundingBox()[0], NegativeBoundingBox()[1]};
                yield return new object[]{NegativeMinValuePositiveMaxValue(), NegativeMinValuePositiveMaxValue()[0], NegativeMinValuePositiveMaxValue()[1]};
                yield return new object[] { BoundingBoxWithZValue(), BoundingBoxWithZValue()[0], BoundingBoxWithZValue()[1]};
            }
        }

        public static IEnumerable<object[]> BoundingBoxAxisLength
        {
            get
            {
                yield return new object[] { BoundingBoxFrom5Points(), 1, 10d };
                yield return new object[] { NegativeBoundingBox(), 2, 0d }; //Index 2 is equal axis Z.
                yield return new object[] { NegativeMinValuePositiveMaxValue(), 0, 22d};
                yield return new object[] { BoundingBoxWithZValue(), 2, 15d};
                yield return new object[] { NegativeBoundingBox(), -2, 0};
                yield return new object[] { NegativeBoundingBox(), 4, 0 };
            }
        }

        public static IEnumerable<object[]> BoundingBoxIntersections
        {
            get
            {
                //yield return new object[] { BoundingBoxFrom5Points(), NegativeMinValuePositiveMaxValue(), true};
                yield return new object[] { BoundingBoxFrom5Points(), NegativeBoundingBox(), false};
            }
        }
    }
}
