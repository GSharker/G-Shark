using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;

namespace GeometrySharp.Optimization
{
    /// <summary>
    /// Minimizer functions collects the basic functions used into the minimization process,
    /// to define the intersection results between curves.
    /// </summary>
    public class ObjectiveFunction
    {
        private readonly NurbsCurve _curve0;
        private readonly NurbsCurve _curve1;

        /// <summary>
        /// Initialize the class, which collects the functions used for the minimization problem.
        /// </summary>
        /// <param name="curve0">First curve used in the intersection process.</param>
        /// <param name="curve1">First curve used in the intersection process.</param>
        public ObjectiveFunction(NurbsCurve curve0, NurbsCurve curve1)
        {
            _curve0 = curve0;
            _curve1 = curve1;
        }

        /// <summary>
        /// Gets the objective function.
        /// </summary>
        public double Value(Vector3 v)
        {
            Vector3 p0 = Evaluation.CurvePointAt(_curve0, v[0]);
            Vector3 p1 = Evaluation.CurvePointAt(_curve1, v[1]);

            Vector3 p0P1 = p0 - p1;

            return Vector3.Dot(p0P1, p0P1);
        }

        /// <summary>
        /// Gets the gradient function.
        /// </summary>
        public Vector3 Gradient(Vector3 v)
        {
            List<Vector3> deriveC0 = Evaluation.RationalCurveDerivatives(_curve0, v[0], 1);
            List<Vector3> deriveC1 = Evaluation.RationalCurveDerivatives(_curve1, v[1], 1);

            Vector3 r = deriveC0[0] - deriveC1[0];
            Vector3 drDt = deriveC1[1] * -1.0;

            double value0 = 2.0 * Vector3.Dot(deriveC0[1], r);
            double value1 = 2.0 * Vector3.Dot(drDt, r);

            return new Vector3{value0, value1};
        }

        /// <summary>
        /// Gets a numerical gradient function.
        /// </summary>
        public Vector3 NumericalGradient(Func<Vector3, double> func, Vector3 vec)
        {
            int n = vec.Count;
            double f0 = func(vec);

            if (double.IsNaN(f0))
            {
                throw new Exception("Gradient: func(x) is a NaN!");
            }

            Vector3 x0 = new Vector3(vec);
            double eps = 1e-3;
            int iteration = 0;
            double[] J = new double[n];

            for (int i = 0; i < n; i++)
            {
                double h = Math.Max(1e-6 * f0, 1e-8);

                while (true)
                {
                    iteration++;
                    if (iteration > 20)
                    {
                        throw new Exception("Numerical gradient fails");
                    }

                    x0[i] = vec[i] + h;
                    double f1 = func(x0);
                    x0[i] = vec[i] - h;
                    double f2 = func(x0);
                    x0[i] = vec[i];

                    if (double.IsNaN(f1) || double.IsNaN(f2))
                    {
                        h /= 16;
                        continue;
                    }

                    J[i] = (f1 - f2) / (2 * h);
                    double t0 = vec[i] - h;
                    double t1 = vec[i];
                    double t2 = vec[i] + h;
                    double d1 = (f1 - f0) / h;
                    double d2 = (f0 - f2) / h;

                    double[] tempMax0 = new[]
                    {
                        Math.Abs(J[i]),
                        Math.Abs(f0), Math.Abs(f1), Math.Abs(f2),
                        Math.Abs(t0), Math.Abs(t1), Math.Abs(t2),
                        1e-8
                    };

                    double N0 = tempMax0.Max();

                    double[] tempMax1 = new[]
                    {
                        Math.Abs(d1 - J[i]), Math.Abs(d2 - J[i]), Math.Abs(d1 - d2),
                    };

                    double N1 = tempMax1.Max();

                    double errest = Math.Min(N1 / N0, h / N0);

                    if (errest > eps)
                    {
                        h /= 16;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return J.ToVector();
        }
    }
}
