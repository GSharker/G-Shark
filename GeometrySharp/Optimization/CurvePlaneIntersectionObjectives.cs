using System.Collections.Generic;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;

namespace GeometrySharp.Optimization
{
    /// <summary>
    /// The basic functions used into the minimization process, to define the intersection results between a curve and a plane.
    /// </summary>
    public class CurvePlaneIntersectionObjectives : IObjectiveFunction
    {
        private readonly NurbsCurve _curve;
        private readonly Plane _plane;

        /// <summary>
        /// Initialize the class, which collects the functions used for the minimization problem.
        /// </summary>
        /// <param name="crv">Curve used in the intersection process.</param>
        /// <param name="plane">Plane used in the intersection process.</param>
        public CurvePlaneIntersectionObjectives(NurbsCurve crv, Plane plane)
        {
            _curve = crv;
            _plane = plane;
        }

        public double Value(Vector3 v)
        {
            Vector3 p0 = _curve.PointAt(v[0]);
            Vector3 p1 = _plane.ClosestPoint(p0, out _);

            Vector3 p0P1 = p0 - p1;

            return Vector3.Dot(p0P1, p0P1);
        }

        public Vector3 Gradient(Vector3 v)
        {
            List<Vector3> deriveC0 = Evaluation.RationalCurveDerivatives(_curve, v[0], 1);
            Vector3 r = deriveC0[0] - _plane.Origin;

            double f = Vector3.Dot(_plane.Normal, r);
            // Compute the derivative of function.
            double df = Vector3.Dot(_plane.Normal, deriveC0[1]);

            double value0 = 2 * (f / df);

            return new Vector3{value0, value0};
        }
    }
}
