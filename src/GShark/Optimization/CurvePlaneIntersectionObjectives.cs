using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation;

namespace GShark.Optimization
{
    /// <summary>
    /// The basic functions used into the minimization process, to define the intersection results between a curve and a plane.
    /// </summary>
    public class CurvePlaneIntersectionObjectives : IObjectiveFunction
    {
        private readonly NurbsCurve _curve;
        private readonly Plane _plane;

        /// <summary>
        /// Initializes the class, which collects the functions used for the minimization problem.
        /// </summary>
        /// <param name="crv">ICurve used in the intersection process.</param>
        /// <param name="plane">Plane used in the intersection process.</param>
        public CurvePlaneIntersectionObjectives(NurbsCurve crv, Plane plane)
        {
            _curve = crv;
            _plane = plane;
        }

        public double Value(Vector v)
        {
            var p0 = _curve.PointAt(v[0]);
            var p1 = _plane.ClosestPoint(p0, out _);

            var p0P1 = p0 - p1;

            return Vector3.DotProduct(p0P1, p0P1);
        }

        public Vector Gradient(Vector v)
        {
            var deriveC0 = Evaluation.RationalCurveDerivatives(_curve, v[0], 1);
            var r = deriveC0[0] - new Vector3(_plane.Origin);

            double f = Vector3.DotProduct(_plane.ZAxis, (Vector3)r);
            // Compute the derivative of function.
            double df = Vector3.DotProduct(_plane.ZAxis, deriveC0[1]);

            double value0 = 2 * (f / df);

            return new Vector { value0, value0 };
        }
    }
}
