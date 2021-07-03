using GShark.Geometry;

namespace GShark.Core.IntersectionResults
{
    /// <summary>
    /// This is a POC class used to collect the result from curve-plane intersections.
    /// </summary>
    public class CurvePlaneIntersectionResult
    {
        /// <summary>
        /// Collects the result from an intersection.
        /// </summary>
        /// <param name="point">ICurve's intersection point.</param>
        /// <param name="t">Curve's parameter t on curve.</param>
        /// <param name="uv">Parameter uv on plane.</param>
        public CurvePlaneIntersectionResult(Point3d point, double t, Vector3 uv)
        {
            Point = point;
            T = t;
            Uv = uv;
        }

        /// <summary>
        /// Gets the curve's intersection point.
        /// </summary>
        public Point3d Point { get; }

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
