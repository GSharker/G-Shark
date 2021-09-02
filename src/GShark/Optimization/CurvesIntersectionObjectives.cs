using GShark.Geometry;
using GShark.Operation;
using System.Collections.Generic;

namespace GShark.Optimization
{
    /// <summary>
    /// The basic functions used into the minimization process, to define the intersection results between curves.
    /// </summary>
    public class CurvesIntersectionObjectives : IObjectiveFunction
    {
        private readonly NurbsCurve _curve0;
        private readonly NurbsCurve _curve1;

        /// <summary>
        /// Initialize the class, which collects the functions used for the minimization problem.
        /// </summary>
        /// <param name="curve0">First curve used in the intersection process.</param>
        /// <param name="curve1">First curve used in the intersection process.</param>
        public CurvesIntersectionObjectives(NurbsCurve curve0, NurbsCurve curve1)
        {
            _curve0 = curve0;
            _curve1 = curve1;
        }

        public double Value(Vector v)
        {
            Vector p0 = Evaluation.CurvePointAt(_curve0, v[0]);
            Vector p1 = Evaluation.CurvePointAt(_curve1, v[1]);

            Vector p0P1 = p0 - p1;

            return Vector.Dot(p0P1, p0P1);
        }

        public Vector Gradient(Vector v)
        {
            List<Vector3> deriveC0 = Evaluation.RationalCurveDerivatives(_curve0, v[0], 1);
            List<Vector3> deriveC1 = Evaluation.RationalCurveDerivatives(_curve1, v[1], 1);

            Vector r = deriveC0[0] - deriveC1[0];
            Vector drDt = deriveC1[1] * -1.0;

            double value0 = 2.0 * Vector.Dot(deriveC0[1], r);
            double value1 = 2.0 * Vector.Dot(drDt, r);

            return new Vector { value0, value1 };
        }
    }
}
