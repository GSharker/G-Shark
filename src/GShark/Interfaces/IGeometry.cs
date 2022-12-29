using System;
using System.Collections.Generic;
using System.Text;

namespace GShark.Interfaces
{
    public interface IGeometry<T> : IEquatable<T>, ITransformable<T>
    {
    }
}
