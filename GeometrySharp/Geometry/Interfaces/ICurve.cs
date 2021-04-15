using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry.Interfaces
{
    public interface ICurve
    {
        /// <summary>
        /// Integer degree of curve.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// 2d list of control points, where each control point is a list like (x,y,z) of length (dim).
        /// </summary>
        public List<Vector3> ControlPoints { get; }

        /// <summary>
        /// 2d list of points, where represented a set (wi*pi, wi) with length (dim+1).
        /// </summary>
        public List<Vector3> HomogenizedPoints { get; }

        /// <summary>
        /// List of non-decreasing knot values.
        /// </summary>
        public Knot Knots { get; }

        /// <summary>
        /// Gets the bounding box of the curve.
        /// </summary>
        public BoundingBox BoundingBox { get; }

        /// <summary>
        /// Computes the point at the parameter t on the curve,
        /// </summary>
        /// <param name="t">The parameter t on the curve.</param>
        /// <returns>The point on the curve.</returns>
        public Vector3 PointAt(double t);

        /// <summary>
        /// Computes the closest point on the curve that is close to the test point.
        /// </summary>
        /// <param name="pt">The test point.</param>
        /// <returns>The closest point.</returns>
        public Vector3 ClosestPt(Vector3 pt);
    }
}
