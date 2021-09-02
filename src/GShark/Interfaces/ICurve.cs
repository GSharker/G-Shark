using GShark.Core;
using GShark.Geometry;

namespace GShark.Interfaces
{
    public interface ICurve
    {
        /// <summary>
        /// Gets the length of the curve.
        /// </summary>
        public double Length { get; }

        /// <summary>
        /// Gets the domain of the curve.
        /// </summary>
        public Interval Domain { get; }

        /// <summary>
        /// Gets the starting point of the curve.
        /// </summary>
        public Point3 StartPoint { get; }

        /// <summary>
        /// Gets the middle point of the curve.
        /// </summary>
        public Point3 MidPoint { get; }

        /// <summary>
        /// Gets the end point of the curve.
        /// </summary>
        public Point3 EndPoint { get; }

        /// <summary>
        /// Gets the bounding box of the curve.
        /// </summary>
        public BoundingBox GetBoundingBox();

        /// <summary>
        /// Evaluates the length at the parameter t on the curve.
        /// </summary>
        /// <param name="t">Evaluation parameter. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The length of the curve coincides at the given parameter.</returns>
        public double LengthAt(double t);

        /// <summary>
        /// Evaluates the point at the parameter t on the curve.
        /// </summary>
        /// <param name="t">Evaluation parameter. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The point on the curve at the given parameter.</returns>
        public Point3 PointAt(double t);

        /// <summary>
        /// Evaluates the point at a certain length along the curve.
        /// </summary>
        /// <param name="length">Length along the curve between the start point and the returned point.</param>
        /// <returns>The point on the curve at the given length.</returns>
        public Point3 PointAtLength(double length);

        /// <summary>
        /// Evaluates the closest point on the curve that is close to the test point.
        /// </summary>
        /// <param name="pt">The test point.</param>
        /// <returns>The closest point.</returns>
        public Point3 ClosestPoint(Point3 pt);

        /// <summary>
        /// Evaluates the closest parameter on the curve that is close to the test point.
        /// </summary>
        /// <param name="pt">The test point.</param>
        /// <returns>The closest parameter between 0.0 and 1.0.</returns>
        public double ClosestParameter(Point3 pt);
    }
}
