using GShark.Geometry;

namespace GShark.Intersection
{
    /// <summary>
    /// This is a POC class used to collect the result from curve-plane intersections.
    /// </summary>
    public class CurvePlaneIntersectionResult
    {
        /// <summary>
        /// Collects the result from an intersection.
        /// </summary>
        public CurvePlaneIntersectionResult(Point3 point, double t, Vector uv)
        {
            Point = point;
            CurveParameter = t;
            Coordinate = (uv[0], uv[1]);
        }

        /// <summary>
        /// Gets the curve's intersection point.
        /// </summary>
        public Point3 Point { get; }

        /// <summary>
        /// Gets the curve's parameter.
        /// </summary>
        public double CurveParameter { get; }

        /// <summary>
        /// Gets the coordinate of the project point on the plane.
        /// </summary>
        public (double U, double V) Coordinate { get; }

        /// <summary>
        /// Compose the curve intersection result as a text.
        /// </summary>
        /// <returns>The text format of the intersection.</returns>
        public override string ToString()
        {
            return $"pt: {Point} - t: {CurveParameter} - uv: {Coordinate}";
        }
    }
}
