using GeometrySharp.Geometry;

namespace GeometrySharp.Core
{
    /// <summary>
    /// This is a POC class used to collect the result from curves intersection.
    /// </summary>
    public class CurveIntersectionResult
    {
        public CurveIntersectionResult()
        {
        }

        /// <summary>
        /// Collects the result from an intersection.
        /// </summary>
        /// <param name="point0">First curve's intersection point.</param>
        /// <param name="point1">Second curve's intersection point.</param>
        /// <param name="parameter0">First curve's t parameter on curve.</param>
        /// <param name="parameter1">Second curve's t parameter on curve.</param>
        public CurveIntersectionResult(Vector3 point0, Vector3 point1, double parameter0, double parameter1)
        {
            Point0 = point0;
            Point1 = point1;
            Parameter0 = parameter0;
            Parameter1 = parameter1;
        }

        /// <summary>
        /// Gets the first curve's intersection point.
        /// </summary>
        public Vector3 Point0 { get; private set; }

        /// <summary>
        /// Gets the second curve's intersection point.
        /// </summary>
        public Vector3 Point1 { get; private set; }

        /// <summary>
        /// Gets the first curve's t parameter on curve.
        /// </summary>
        public double Parameter0 { get; private set; }

        /// <summary>
        /// Gets the second curve's t parameter on curve.
        /// </summary>
        public double Parameter1 { get; private set; }

        /// <summary>
        /// Compose the curve intersection result as a text.
        /// </summary>
        /// <returns>The text format of the intersection.</returns>
        public override string ToString()
        {
            return $"pt0: {Point0} - t0: {Parameter0} - pt1: {Point1} - t1: {Parameter1}";
        }
    }
}
