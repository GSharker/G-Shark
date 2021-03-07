using System.Collections.Generic;
using GeometrySharp.Geometry;
using Xunit;

namespace GeometrySharp.Test.XUnit.Data
{
    public class BoundingBoxCollection
    {
        public static Vector3[] BoundingBox2D()
        {
            return new[]
            {
                new Vector3 {0d, 0d, 0d},
                new Vector3 {10d, 0d, 0d},
                new Vector3 {10d, 10d, 0d},
                new Vector3 {5d, 5d, 0d}
            };
        }
        public static Vector3[] BoundingBox3D()
        {
            return new[]
            {
                new Vector3 { 74.264416, 36.39316, -1.884313},
                new Vector3 { 79.65881, 22.402983, 1.741763},
                new Vector3 { 97.679126, 13.940616, 3.812853},
                new Vector3 { 100.92443, 30.599893, -0.585116},
                new Vector3 { 78.805261, 45.16886, -4.22451},
                new Vector3 { 74.264416, 36.39316, -1.884313}
            };
        }

        public static Vector3[] NegativeBoundingBox()
        {
            return new[]
            {
                new Vector3 {-15d, -15d, 0d},
                new Vector3 {-5d, -5d, 0d}
            };
        }

        public static Vector3[] NegativeMinValuePositiveMaxValue()
        {
            return new[]
            {
                new Vector3 {-12d, -17d, 0d},
                new Vector3 {10d, 13d, 0d}
            };
        }

        public static Vector3[] BoundingBoxWithZValue()
        {
            return new[]
            {
                new Vector3 {-12d, -17d, -10d},
                new Vector3 {10d, 13d, 5d}
            };
        }

        public static IEnumerable<object[]> BoundingBoxCollections
        {
            get
            {
                yield return new object[] {BoundingBox2D(), BoundingBox2D()[0], BoundingBox2D()[2]};
                yield return new object[] { BoundingBox3D(), new Vector3 {74.264416, 13.940616, -4.22451}, new Vector3 {100.92443, 45.16886, 3.812853}};
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

        public static TheoryData<Vector3[], BoundingBox> BoundingBoxIntersectionsUnset => 
            new TheoryData<Vector3[], BoundingBox>
            {
                {BoundingBox2D(), BoundingBox.Unset},
                {BoundingBox2D(), new BoundingBox(NegativeBoundingBox())},
            };
    }
}
