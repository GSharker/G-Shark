using System.Collections.Generic;
using GShark.Core;

namespace GShark.Geometry.Interfaces
{
    public interface ICurve
    {
        /// <summary>
        /// Gets the degree of curve.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// Gets the control points, where each control point is a list like (x,y,z).
        /// </summary>
        public List<Point3> ControlPoints { get; }

        /// <summary>
        /// Gets the homogenize points, where represented a set (wi*pi, wi).
        /// </summary>
        public List<Point4> HomogenizedPoints { get; }

        /// <summary>
        /// Gets the collection of non-decreasing knot values.
        /// </summary>
        public KnotVector Knots { get; }

        /// <summary>
        /// Gets the domain of the curve.
        /// </summary>
        public Interval Domain { get; }

        /// <summary>
        /// Gets the bounding box of the curve.
        /// </summary>
        public BoundingBox BoundingBox { get; }

        /// <summary>
        /// Computes the point at the parameter t on the curve,
        /// </summary>
        /// <param name="t">The parameter t on the curve.</param>
        /// <returns>The point on the curve.</returns>
        public Point3 PointAt(double t);

        /// <summary>
        /// Computes the closest point on the curve that is close to the test point.
        /// </summary>
        /// <param name="pt">The test point.</param>
        /// <returns>The closest point.</returns>
        public Point3 ClosestPoint(Point3 pt);
    }
}
