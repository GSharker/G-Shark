using System;
using GeometrySharp.Geometry;

namespace GeometrySharp.Core.BoundingBoxTree
{
    public interface IBoundingBoxTree<T>
    {
        public BoundingBox BoundingBox();

        public Tuple<IBoundingBoxTree<T>, IBoundingBoxTree<T>> Split();

        public T Yield();

        public bool IsIndivisible(double tolerance);

        public bool IsEmpty();
    }
}
