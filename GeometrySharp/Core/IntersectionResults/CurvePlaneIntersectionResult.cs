using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;

namespace GeometrySharp.Core.IntersectionResults
{
    /// <summary>
    /// This is a POC class used to collect the result from curve-plane intersections.
    /// </summary>
    public class CurvePlaneIntersectionResult
    {
        /// <summary>
        /// Collects the result from an intersection.
        /// </summary>
        /// <param name="point">Curve's intersection point.</param>
        /// <param name="t">Curve's parameter t on curve.</param>
        /// <param name="uv">Parameter uv on plane.</param>
        public CurvePlaneIntersectionResult(Vector3 point, double t, Vector3 uv)
        {
            Point = point;
            T = t;
            Uv = uv;
        }

        /// <summary>
        /// Gets the curve's intersection point.
        /// </summary>
        public Vector3 Point { get; }

        /// <summary>
        /// Gets the curve's parameter t.
        /// </summary>
        public double T { get; }

        /// <summary>
        /// Gets the parameter uv on plane.
        /// </summary>
        public Vector3 Uv { get; }

        /// <summary>
        /// Compose the curve intersection result as a text.
        /// </summary>
        /// <returns>The text format of the intersection.</returns>
        public override string ToString()
        {
            return $"pt: {Point} - t: {T} - uv: {Uv}";
        }
    }
}
