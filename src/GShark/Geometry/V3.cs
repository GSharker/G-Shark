using GShark.Geometry.Interfaces;
using System;

namespace GShark.Geometry
{
    /// <summary>
    /// Vector3 represents a geometrical vector in the 3-dimensional space, defined by three coordinates xyz.
    /// </summary>
    public struct V3 : ICoordinateXYZ, IComparable<V3>, IEquatable<V3>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public int CompareTo(V3 other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(V3 other)
        {
            throw new NotImplementedException();
        }
    }
}
