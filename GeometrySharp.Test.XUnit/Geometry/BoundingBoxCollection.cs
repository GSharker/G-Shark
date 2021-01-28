using System.Collections.Generic;
using GeometrySharp.Geometry;
using Xunit;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class BoundingBoxCollection
    {
        public static Vector3[] BoundingBoxFrom5Points = 
        {
            new Vector3() {0d, 0d, 0d},
            new Vector3() {10d, 0d, 0d},
            new Vector3() {10d, 10d, 0d},
            new Vector3() {5d, 5d, 0d}
        };

        public static Vector3[] NegativeBoundingBox =
        {
            new Vector3() {-15d, -15d, 0d},
            new Vector3() {-5d, -5d, 0d}
        };

        public static Vector3[] NegativeMinValuePositiveMaxValue =
        {
            new Vector3() {-12d, -17d, 0d},
            new Vector3() {10d, 13d, 0d}
        };

        public static Vector3[] BoundingBoxWithZValue =
        {
            new Vector3() {-12d, -17d, -10d},
            new Vector3() {10d, 13d, 5d}
        };

        public static IEnumerable<object[]> BoundingBoxCollections
        {
            get
            {
                yield return new object[] {BoundingBoxFrom5Points, BoundingBoxFrom5Points[0], BoundingBoxFrom5Points[2]};
                yield return new object[] {NegativeBoundingBox, NegativeBoundingBox[0], NegativeBoundingBox[1]};
                yield return new object[] {
                    NegativeMinValuePositiveMaxValue, NegativeMinValuePositiveMaxValue[0],
                    NegativeMinValuePositiveMaxValue[1]
                };
                yield return new object[] {BoundingBoxWithZValue, BoundingBoxWithZValue[0], BoundingBoxWithZValue[1]};
            }
        }

        public static IEnumerable<object[]> BoundingBoxAxisLength
        {
            get
            {
                yield return new object[] {BoundingBoxFrom5Points, 1, 10d};
                yield return new object[] {NegativeBoundingBox, 2, 0d}; //Index 2 is equal axis Z.
                yield return new object[] {NegativeMinValuePositiveMaxValue, 0, 22d};
                yield return new object[] {BoundingBoxWithZValue, 2, 15d};
                yield return new object[] {NegativeBoundingBox, -2, 0};
                yield return new object[] {NegativeBoundingBox, 4, 0};
            }
        }

        public static IEnumerable<object[]> BoundingBoxIntersections
        {
            get
            {
                yield return new object[] {BoundingBoxFrom5Points, NegativeMinValuePositiveMaxValue, true};
                yield return new object[] {BoundingBoxFrom5Points, NegativeBoundingBox, false};
            }
        }

        public static TheoryData<Vector3[], BoundingBox> BoundingBoxIntersectionsUnset = new TheoryData<Vector3[], BoundingBox>()
        {
            {BoundingBoxFrom5Points, BoundingBox.Unset},
            {BoundingBoxFrom5Points, new BoundingBox(NegativeBoundingBox)},
        };
    }
}
