using System;
using System.Collections.Generic;
using System.Text;

namespace GShark.Interfaces
{
    public interface ICurve<T> : IEquatable<T>, ITransformable<T>
    {
    }
}
