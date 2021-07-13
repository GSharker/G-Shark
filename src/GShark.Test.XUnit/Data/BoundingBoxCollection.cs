using GShark.Geometry;
using System.Collections.Generic;
using Xunit;

namespace GShark.Test.XUnit.Data
{
    public class BoundingBoxCollection
    {
        public static Point3[] BoundingBox2D()
        {
            return new[]
            {
                new Point3(0d, 0d, 0),
                new Point3(10d, 0d, 0),
                new Point3(10d, 10d, 0),
                new Point3(5d, 5d, 0)
            };
        }
        public static Point3[] BoundingBox3D()
        {
            return new[]
            {
                new Point3( 74.264416, 36.39316, -1.884313),
                new Point3( 79.65881, 22.402983, 1.741763),
                new Point3( 97.679126, 13.940616, 3.812853),
                new Point3( 100.92443, 30.599893, -0.585116),
                new Point3( 78.805261, 45.16886, -4.22451),
                new Point3( 74.264416, 36.39316, -1.884313)
            };
        }

        public static Point3[] NegativeBoundingBox()
        {
            return new[]
            {
                new Point3(-15d, -15d, 0),
                new Point3(-5d, -5d, 0)
            };
        }

        public static Point3[] NegativeMinValuePositiveMaxValue()
        {
            return new[]
            {
                new Point3(-12d, -17d, 0),
                new Point3(10d, 13d, 0)
            };
        }

        public static Point3[] BoundingBoxWithZValue()
        {
            return new[]
            {
                new Point3(-12d, -17d, -10),
                new Point3(10d, 13d, 5)
            };
        }

        public static IEnumerable<object[]> BoundingBoxCollections
        {
            get
            {
                yield return new object[] {BoundingBox2D(), BoundingBox2D()[0], BoundingBox2D()[2]};
                yield return new object[] {BoundingBox3D(), new Point3(74.264416, 13.940616, -4.22451), new Point3(100.92443, 45.16886, 3.812853)};
                yield return new object[] {NegativeBoundingBox(), NegativeBoundingBox()[0], NegativeBoundingBox()[1]};
                yield return new object[] {
                    NegativeMinValuePositiveMaxValue(), NegativeMinValuePositiveMaxValue()[0],
                    NegativeMinValuePositiveMaxValue()[1]
                };
                yield return new object[] {BoundingBoxWithZValue(), BoundingBoxWithZValue()[0], BoundingBoxWithZValue()[1]};
            }
        }

        public static IEnumerable<object[]> BoundingBoxAxisLength
        {
            get
            {
                yield return new object[] {BoundingBox2D(), 1, 10d};
                yield return new object[] {NegativeBoundingBox(), 2, 0d}; //Index 2 is equal axis Z.
                yield return new object[] {NegativeMinValuePositiveMaxValue(), 0, 22d};
                yield return new object[] {BoundingBoxWithZValue(), 2, 15d};
                yield return new object[] {NegativeBoundingBox(), -2, 0};
                yield return new object[] {NegativeBoundingBox(), 4, 0};
            }
        }

        public static IEnumerable<object[]> BoundingBoxIntersections
        {
            get
            {
                yield return new object[] {BoundingBox2D(), NegativeMinValuePositiveMaxValue(), true};
                yield return new object[] {BoundingBox2D(), NegativeBoundingBox(), false};
            }
        }

        public static TheoryData<Point3[], BoundingBox> BoundingBoxIntersectionsUnset => 
            new TheoryData<Point3[], BoundingBox>
            {
                {BoundingBox2D(), BoundingBox.Unset},
                {BoundingBox2D(), new BoundingBox(NegativeBoundingBox())},
            };
    }
}
