using System;
using GShark.Core;
using GShark.Geometry.Interfaces;

namespace GShark.Geometry
{
    /// <summary>
    /// Point4 represents a geometrical point in 3-dimensional space, defined by four coordinates xyzw.
    /// The fourth value (w) is considered the weight of the point.
    /// Point4 could be also identify as a control point.
    /// </summary>
    public struct Point4 : ICoordinateXYZ, IComparable<Point4>, IEquatable<Point4>
    {
        public Point4(double x, double y, double z, double w = 1.0)
        {
            W = Math.Abs(w - 1.0) < GeoSharpMath.MAX_TOLERANCE || w == 0.0 ? 1.0 : w;
            X = x * W;
            Y = y * W;
            Z = z * W;
        }
        public Point4(Point3 point3, double weight = 1.0) 
            : this(point3.X, point3.Y, point3.Z, weight)
        {
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public int CompareTo(Point4 other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Point4 other)
        {
            throw new NotImplementedException();
        }

        // Operations
        // Transform
    }
}