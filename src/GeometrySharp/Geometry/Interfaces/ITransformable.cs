using GeometrySharp.Core;

namespace GeometrySharp.Geometry.Interfaces
{
    public interface ITransformable<T>
    {
        public T Transform(Transform transformation);
    }
}