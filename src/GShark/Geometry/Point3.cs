using GShark.Geometry.Interfaces;
using System;

namespace GShark.Geometry
{
    /// <summary>
    /// Point3 represents a geometrical point in 3-dimensional space, defined by three coordinates xyz.
    /// </summary>
    public struct Point3 : ICoordinateXYZ, IComparable<Point3>, IEquatable<Point3>
    {
        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        // Construct by a Point3, Point4, Vector3
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public int CompareTo(Point3 other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Point3 other)
        {
            throw new NotImplementedException();
        }

        // Implicit conversion to a control point and a vector.
    }
}