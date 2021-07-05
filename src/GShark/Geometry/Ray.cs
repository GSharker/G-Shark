using System;

namespace GShark.Geometry
{
    /// <summary>
    /// A ray is simply a position point and a direction.
    /// </summary>
    public class Ray : IEquatable<Ray>
    {
        /// <summary>
        /// Constructs the ray.
        /// </summary>
        /// <param name="position">The vector describing the direction of the ray.</param>
        /// <param name="direction">The point describing the origin of the ray.</param>
        public Ray(Vector position, Vector direction)
        {
            if (!position.IsValid())
            {
                throw new Exception("Point value is not valid.");
            }
            if (!direction.IsValid())
            {
                throw new Exception("Direction value is not valid.");
            }
            Direction = direction;
            Position = position;
        }

        /// <summary>
        /// Gets the vector, describing the ray direction.
        /// </summary>
        public Vector Direction { get; }

        /// <summary>
        /// Gets the position point of the ray.
        /// </summary>
        public Vector Position { get; }

        /// <summary>
        /// Calculates the point moved by a scalar value along a direction.
        /// </summary>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The point along the ray.</returns>
        public Vector OnRay(double amplitude)
        {
            return Position + Direction!.Amplify(amplitude);
        }

        /// <summary>
        /// Computes the closest point on a ray from a point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>The closest point on a ray from a point.</returns>
        public Vector ClosestPoint(Vector pt)
        {
            Vector rayDirNormalized = Direction!.Unitize();
            Vector rayOriginToPt = pt - Position!;
            double dotResult = Vector.Dot(rayOriginToPt, rayDirNormalized);
            Vector projectedPt = Position! + rayDirNormalized * dotResult;

            return projectedPt;
        }

        /// <summary>
        /// Computes the shortest distance between this ray and a test point.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <returns>The distance.</returns>
        public double DistanceTo(Vector pt)
        {
            Vector projectedPt = ClosestPoint(pt);
            Vector ptToProjectedPt = projectedPt - pt;
            return ptToProjectedPt.Length();
        }

        /// <summary>
        /// Evaluates a point along the ray.
        /// </summary>
        /// <param name="t">The t parameter.</param>
        /// <returns>A point at (Direction*t + Position).</returns>
        public Vector PointAt(double t)
        {
            return Position + Direction! * t;
        }

        /// <summary>
        /// Check if the ray is equal to the provided ray.<br/>
        /// Two ray are equal if position and direction are the same.
        /// </summary>
        /// <param name="other">The ray to compare.</param>
        /// <returns>True if position and direction are equal, otherwise false.</returns>
        public bool Equals(Ray other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return false;
            }

            return Equals(Direction, other.Direction) && Equals(Position, other.Position);
        }

        /// <summary>
        /// Gets the hash code for the ray.
        /// </summary>
        /// <returns>A unique hashCode of an ray.</returns>
        public override int GetHashCode()
        {
            return new[] { Position, Direction }.GetHashCode();
        }

        /// <summary>
        /// Constructs the string representation of the ray.
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return $"P:{Position} - D:{Direction}";
        }
    }
}
