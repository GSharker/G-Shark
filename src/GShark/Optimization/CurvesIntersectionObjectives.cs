using GShark.Geometry;
using GShark.Operation;
using System.Collections.Generic;

namespace GShark.Optimization
{
    /// <summary>
    /// The basic functions used into the minimization process, to define the intersection results between curves.
    /// </summary>
    internal class CurvesIntersectionObjectives : IObjectiveFunction
    {
        private readonly NurbsBase _curve0;
        private readonly NurbsBase _curve1;

        /// <summary>
        /// Initialize the class, which collects the functions used for the minimization problem.
        /// </summary>
        /// <param name="curve0">First curve used in the intersection process.</param>
        /// <param name="curve1">First curve used in the intersection process.</param>
        internal CurvesIntersectionObjectives(NurbsBase curve0, NurbsBase curve1)
        {
            _curve0 = curve0;
            _curve1 = curve1;
        }

        public double Value(Vector v)
        {
            Vector p0 = Evaluate.Curve.PointAt(_curve0, v[0]);
            Vector p1 = Evaluate.Curve.PointAt(_curve1, v[1]);

            Vector p0P1 = p0 - p1;

            return Vector.Dot(p0P1, p0P1);
        }

        public Vector Gradient(Vector v)
        {
            List<Vector3> deriveC0 = Evaluate.Curve.RationalDerivatives(_curve0, v[0], 1);
            List<Vector3> deriveC1 = Evaluate.Curve.RationalDerivatives(_curve1, v[1], 1);

            Vector r = deriveC0[0] - deriveC1[0];
            Vector drDt = deriveC1[1] * -1.0;

            double value0 = 2.0 * Vector.Dot(deriveC0[1], r);
            double value1 = 2.0 * Vector.Dot(drDt, r);

            return new Vector { value0, value1 };
        }
    }
}
