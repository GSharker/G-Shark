using Newtonsoft.Json;
using System;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo this class need to be tested.
    // ToDo this class need to be commented.
    // Note this class can be developed bit more looking Hypar or RhinoCommon.
    // Note Ray and Plane are the same.

    /// <summary>
    /// A Ray is simply an position point and normal.
    /// </summary>
    public class Ray
    {
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Direction = direction;
            this.Position = position;
        }

        /// <summary>
        /// Gets the point moved by a scalar value along a direction.
        /// </summary>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The point along the ray.</returns>
        public Vector3 OnRay(double amplitude)
        {
            var vectorAmplified = this.Direction.Normalized() * amplitude;
            return this.Position + vectorAmplified;
        }

        /// <summary>
        /// Get the closest point on a ray from a point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>Get the closest point on a ray from a point.</returns>
        public Vector3 ClosestPoint(Vector3 pt)
        {
            var rayDirNormalized = this.Direction.Normalized();
            var rayOriginToPt = pt - this.Position;
            var dotResult = Vector3.Dot(rayOriginToPt, rayDirNormalized);
            var projectedPt = this.Position + rayDirNormalized * dotResult;

            return projectedPt;
        }

        /// <summary>
        /// Compute the shortest distance between this ray and a test point.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <returns>The distance.</returns>
        public double DistanceTo(Vector3 pt)
        {
            var projectedPt = ClosestPoint(pt);
            var ptToProjectedPt = projectedPt - pt;
            return ptToProjectedPt.Length();
        }

        /// <summary>
        /// Evaluates a point along the ray.
        /// </summary>
        /// <param name="t">The t parameter.</param>
        /// <returns>A point at (Direction*t + Position).</returns>
        public Vector3 PointAt(double t)
        {
            return !this.Position.IsValid() || !this.Direction.IsValid()
                ? Vector3.Unset
                : this.Position + this.Direction * t;
        }
    }
}
