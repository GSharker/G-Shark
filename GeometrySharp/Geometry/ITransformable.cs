using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    public interface ITransformable<T>
    {
        public T Transform(Transform transform);
    }
}