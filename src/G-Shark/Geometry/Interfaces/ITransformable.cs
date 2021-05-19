using GShark.Core;

namespace GShark.Geometry.Interfaces
{
    public interface ITransformable<T>
    {
        public T Transform(Transform transformation);
    }
}