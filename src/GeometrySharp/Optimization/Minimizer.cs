using GeometrySharp.Core;
using System;
using System.Linq;
using GeometrySharp.Geometry;

namespace GeometrySharp.Optimization
{
    /// <summary>
    /// The Minimizer solves for unconstrained problems, using a gradient descendent.
    /// This class is at the base of some intersection methods.
    /// </summary>
    public class Minimizer
    {
        private readonly IObjectiveFunction _objectiveFunction;

        /// <summary>
        /// Minimizer initialization.
        /// </summary>
        /// <param name="objectiveFunction">The objective functions used into the minimization process.</param>
        public Minimizer(IObjectiveFunction objectiveFunction)
        {
            _objectiveFunction = objectiveFunction;
        }

        /// <summary>
        /// The Minimizer solves for unconstrained problems, using a gradient descendent.
        /// </summary>
        /// <param name="initialGuess">The vector initial guess.</param>
        /// <param name="gradientTolerance">The gradient tolerance set per default to 1e-8.</param>
        /// <param name="maxIteration">The number of iteration used, set per default 1000.</param>
        /// <returns>The minimization result, <see cref="MinimizationResult"/>.</returns>
        public MinimizationResult UnconstrainedMinimizer(Vector3 initialGuess, double gradientTolerance = 1e-8, int maxIteration = 1000)
        {
            Vector3 x0 = new Vector3(initialGuess);
            int n = x0.Count;
            double f0 = _objectiveFunction.Value(x0);
            double f1 = 0.0;
            Vector3 x1 = null;
            Vector3 s = null;
            string message = string.Empty;

            if (double.IsNaN(f0))
            {
                throw new Exception("Unconstrained Minimizer: f(x0) is a NaN!");
            }

            gradientTolerance = Math.Max(gradientTolerance, GeoSharpMath.EPSILON);
            Matrix H1 = Matrix.Identity(n);
            int iteration = 0;
            Vector3 g0 = _objectiveFunction.Gradient(x0);

            while (iteration < maxIteration)
            {
                if (g0.Any(val => double.IsNaN(val) || double.IsInfinity(val)))
                {
                    message = "Gradient has Infinity or NaN.";
                    break;
                }
                Vector3 step = Vector3.Reverse(g0 * H1);
                if (step.Any(val => double.IsNaN(val) || double.IsInfinity(val)))
                {
                    message = "Search direction has Infinity or NaN";
                    break;
                }

                double lengthStep = step.Length();
                if (lengthStep < gradientTolerance)
                {
                    message = "Newton step smaller than tolerance.";
                    break;
                }

                double t = 1.0;
                double df0 = Vector3.Dot(g0, step);

                // Line search.
                while (iteration < maxIteration)
                {
                    if (t * lengthStep < gradientTolerance)
                    {
                        break;
                    }

                    s = step * t;
                    x1 = x0 + s;
                    f1 = _objectiveFunction.Value(x1);

                    if (f1 - f0 >= 0.1 * t * df0 || double.IsNaN(f1))
                    {
                        t *= 0.5;
                        iteration++;
                        continue;
                    }

                    break;
                }

                if (t * lengthStep < gradientTolerance)
                {
                    message = "Line search step size smaller than gradient tolerance.";
                    break;
                }
                if (iteration > maxIteration)
                {
                    message = "Max iteration reached during line search.";
                    break;
                }

                Vector3 g1 = _objectiveFunction.Gradient(x1);
                Vector3 y = g1 - g0;
                double ys = Vector3.Dot(y, s);
                Vector3 Hy = y * H1;

                Matrix T0 = Tensor(s, s);
                Matrix T1 = Tensor(Hy, s);
                Matrix T2 = Tensor(s, Hy);

                H1 = (H1 + (T0 * (ys + Vector3.Dot(y, Hy)) / (ys * ys))) - (T1 + T2) / ys;
                x0 = x1;
                f0 = f1;
                g0 = g1;
                iteration++;
            }

            return new MinimizationResult(x0, f0, g0, H1, iteration, message);
        }

        private Matrix Tensor(Vector3 vec0, Vector3 vec1)
        {
            int m = vec0.Count;
            int n = vec1.Count;
            int i = m - 1;
            Matrix A = Matrix.Identity(m);

            while (i >= 0)
            {
                double xi = vec0[i];
                int j = n - 1;
                double[] Ai = new double[n];

                while (j >= 3)
                {
                    Ai[j] = xi * vec1[j];
                    j--;
                    Ai[j] = xi * vec1[j];
                    j--;
                    Ai[j] = xi * vec1[j];
                    j--;
                    Ai[j] = xi * vec1[j];
                    j--;
                }

                while (j >= 0)
                {
                    Ai[j] = xi * vec1[j];
                    j--;
                }

                A[i] = Ai.ToList();
                i--;
            }

            return A;
        }
    }
}
