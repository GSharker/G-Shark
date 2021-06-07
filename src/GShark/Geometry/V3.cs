using GShark.Geometry.Interfaces;
using System;
using GShark.Core;

namespace GShark.Geometry
{
    /// <summary>
    /// Vector3 represents a geometrical vector in the 3-dimensional space, defined by three coordinates xyz.
    /// </summary>
    public struct V3 : ICoordinateXYZ, IComparable<V3>, IEquatable<V3>
    {
        /// <summary>
        /// Creates the instance of a vector, by three components.
        /// </summary>
        /// <param name="x">The first component.</param>
        /// <param name="y">The second component.</param>
        /// <param name="z">The third component.</param>
        public V3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates the instance of a vector, coping the coordinate of a points.
        /// </summary>
        /// <param name="point">The point used to create the vector.</param>
        public V3(Point3 point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        /// <summary>
        /// Creates the instance of a vector, coping the components from another vector.
        /// </summary>
        /// <param name="vector">The vector used to copied the components.</param>
        public V3(V3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Gets the value of a point at location GeoSharpMath.UNSET_VALUE,GeoSharpMath.UNSET_VALUE,GeoSharpMath.UNSET_VALUE.
        /// </summary>
        public static V3 Unset => new V3(GeoSharpMath.UNSET_VALUE, GeoSharpMath.UNSET_VALUE, GeoSharpMath.UNSET_VALUE);

        /// <summary>
        /// Gets the value of the vector with components 1,0,0.
        /// </summary>
        public static V3 XAxis => new V3(1.0, 0.0, 0.0 );

        /// <summary>
        /// Gets the value of the vector with components 0,1,0.
        /// </summary>
        public static V3 YAxis => new V3( 0.0, 1.0, 0.0 );

        /// <summary>
        /// Gets the value of the vector with components 0,0,1.
        /// </summary>
        public static V3 ZAxis => new V3( 0.0, 0.0, 1.0 );

        /// <summary>
        /// Gets the X coordinate of the vector.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate of the vector.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the Z coordinate of the vector.
        /// </summary>
        public double Z { get; }

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
