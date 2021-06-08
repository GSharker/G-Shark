using GShark.Geometry.Interfaces;
using System;
using GShark.Core;

namespace GShark.Geometry
{
    /// <summary>
    /// Point3 represents a geometrical point in 3-dimensional space, defined by three coordinates xyz.
    /// </summary>
    public struct Point3 : IVector3, ITransformable<Point3>, IComparable<Point3>, IEquatable<Point3>
    {
        /// <summary>
        /// Creates an instance of a point by three coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates an instance of a point cooping the coordinate from another point.
        /// </summary>
        /// <param name="point">The point used to create the new point.</param>
        public Point3(Point3 point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        /// <summary>
        /// Creates an instance of a point cooping the components of a vector.
        /// </summary>
        /// <param name="vector">The vector used to create the new point.</param>
        public Point3(Vector3d vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Gets the X coordinate of the point.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate of the point.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the Z coordinate of the point.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Gets a point located at (0,0,0).
        /// </summary>
        public Point3 Default => new Point3(0.0, 0.0, 0.0);

        /// <summary>
        /// Gets a value indicating whether this vector is valid.<br/>
        /// A valid vector must be formed of finite numbers.
        /// </summary>
        /// <param name="a">The vector to be valued.</param>
        /// <returns>True if the vector is valid.</returns>
        public bool IsValid => GeoSharpMath.IsValidDouble(X) && GeoSharpMath.IsValidDouble(Y) && GeoSharpMath.IsValidDouble(Z);

        // ToDo: Implicit conversion to a control point and a vector.
        // ToDo: Operations.
        // ToDo: DistanceTo(Line).
        // ToDo: IsOnPlane

        /// <summary>
        /// Calculates the distance from this point to another point.
        /// </summary>
        /// <param name="point">The target point.</param>
        /// <returns>The distance between this point and the provided point.</returns>
        public double DistanceTo(Point3 point)
        {
            return Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2) + Math.Pow(point.Z - Z, 2));
        }

        /// <summary>
        /// Transforms the point using a transformation matrix.
        /// </summary>
        /// <param name="t">The transformation matrix.</param>
        /// <returns>The transformed point as a new instance.</returns>
        public Point3 Transform(Transform t)
        {
            double x = 0.0;
            double y = 0.0;
            double z = 0.0;
            double w = 0.0;

            //ToDO Convert to Matrix multiplication! Create a column row vector from the point. i.e. IVector

            x = t[0][0] * X + t[0][1] * Y + t[0][2] * Z + t[0][3];
            y = t[1][0] * X + t[1][1] * Y + t[1][2] * Z + t[1][3];
            z = t[2][0] * X + t[2][1] * Y + t[2][2] * Z + t[2][3];
            w = t[3][0] * X + t[3][1] * Y + t[3][2] * Z + t[3][3];

            if (!(w > 0.0)) return new Point3(x, y, z);

            double w2 = 1.0 / w;
            x *= w2;
            y *= w2;
            z *= w2;

            return new Point3(x, y, z);
        }

        public int CompareTo(Point3 other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Point3 other)
        {
            throw new NotImplementedException();
        }

    }
}