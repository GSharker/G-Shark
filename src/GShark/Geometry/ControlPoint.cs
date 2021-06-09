using GShark.Core;
using GShark.Geometry.Interfaces;
using System;

namespace GShark.Geometry
{
    /// <summary>
    /// ControlPoint represents a geometrical point in 3-dimensional space, defined by four coordinates xyzw.
    /// The fourth value (w) is considered the weight of the point.
    /// ControlPoint could be also identify as a control point.
    /// </summary>
    public struct ControlPoint : IEquatable<ControlPoint>
    {
        /// <summary>
        /// Constructs a new instance of a control point by coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the control point.</param>
        /// <param name="y">Y coordinate of the control point.</param>
        /// <param name="z">Z coordinate of the control point.</param>
        /// <param name="w">Weight factor of the control point, set as per default at 1.0.</param>
        public ControlPoint(double x, double y, double z, double w = 1.0)
        {
            X = x;
            Y = y;
            Z = z;
            W = Math.Abs(w - 1.0) < GeoSharpMath.MaxTolerance || w == 0.0 ? 1.0 : w;
        }

        /// <summary>
        /// Constructs a new instance of a control point from an Euclidean point.
        /// </summary>
        /// <param name="point">The point defining the coordinate of the control point.</param>
        /// <param name="weight">Weight factor of the control point, set as per default at 1.0.</param>
        public ControlPoint(Point3 point, double weight = 1.0)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
            W = Math.Abs(weight - 1.0) < GeoSharpMath.MaxTolerance || weight == 0.0 ? 1.0 : weight;
        }

        /// <summary>
        /// Gets the X coordinate of the control point.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate of the control point.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the Z coordinate of the control point.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Gets the weight of the control point.
        /// </summary>
        public double W { get; }

        /// <summary>
        /// Checks if two control points are exactly the same.
        /// </summary>
        /// <param name="other">The other control point.</param>
        /// <returns>True if the other control point matches.</returns>
        public bool Equals(ControlPoint other)
        {
            return Math.Abs(other.X - X) < GeoSharpMath.MaxTolerance &&
                   Math.Abs(other.Y - Y) < GeoSharpMath.MaxTolerance &&
                   Math.Abs(other.Z - Z) < GeoSharpMath.MaxTolerance &&
                   Math.Abs(other.W - W) < GeoSharpMath.MaxTolerance;
        }
    }
}