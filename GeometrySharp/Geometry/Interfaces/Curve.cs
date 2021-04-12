using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry.Interfaces
{
    public abstract class Curve
    {
        /// <summary>
        /// Integer degree of curve.
        /// </summary>
        public int Degree { get; internal set; }

        /// <summary>
        /// 2d list of control points, where each control point is a list like (x,y,z) of length (dim).
        /// </summary>
        public List<Vector3> ControlPoints => LinearAlgebra.PointDehomogenizer1d(HomogenizedPoints);

        /// <summary>
        /// 2d list of points, where represented a set (wi*pi, wi) with length (dim+1).
        /// </summary>
        public List<Vector3> HomogenizedPoints { get; internal set; }

        /// <summary>
        /// List of non-decreasing knot values.
        /// </summary>
        public Knot Knots { get; internal set; }

        public abstract BoundingBox BoundingBox { get; }

        public abstract Vector3 PointAt(double t, bool parametrize = true);

        public abstract Vector3 TangentAt(double t, bool parametrize = true);

        public abstract Vector3 ClosestPt(Vector3 pt);
    }
}
