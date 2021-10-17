using GShark.Geometry;

namespace GShark.Intersection
{
    /// <summary>
    /// This is a POC class used to collect the result from curves intersection.
    /// </summary>
    public class CurvesIntersectionResult
    {
        /// <summary>
        /// Collects the result from an intersection.
        /// </summary>
        public CurvesIntersectionResult(Point3 pointA, Point3 pointB, double parameterA, double parameterB)
        {
            PointA = pointA;
            PointB = pointB;
            ParameterA = parameterA;
            ParameterB = parameterB;
        }

        /// <summary>
        /// Gets the first curve's intersection point.
        /// </summary>
        public Point3 PointA { get; }

        /// <summary>
        /// Gets the second curve's intersection point.
        /// </summary>
        public Point3 PointB { get; }

        /// <summary>
        /// Gets the first curve's parameter on curve.
        /// </summary>
        public double ParameterA { get; }

        /// <summary>
        /// Gets the second curve's parameter on curve.
        /// </summary>
        public double ParameterB { get; }

        /// <summary>
        /// Compose the curve intersection result as a text.
        /// </summary>
        /// <returns>The text format of the intersection.</returns>
        public override string ToString()
        {
            return $"pointA: {PointA} - parameterA: {ParameterA} - pointB: {PointB} - parameterB: {ParameterB}";
        }
    }
}
