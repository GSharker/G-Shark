using GeometrySharp.Core;
using System;
using Vector3 = GeometrySharp.Geometry.Vector3;

namespace GeometrySharp.Optimization
{
    /// <summary>
    /// The Minimizer solves for unconstrained problems, using a gradient descendent.
    /// This class is at the base of some intersection methods.
    /// </summary>
    public class Minimizer
    {
        private readonly Func<Vector3, double> _objectiveFunction;
        private readonly Func<Vector3, Vector3> _gradientFunc;

        /// <summary>
        /// Initializes the minimizer.
        /// </summary>
        /// <param name="objectiveFunction">The objective function used into the minimization process.</param>
        /// <param name="gradientFunc">The gradient function used into the minimization process.</param>
        public Minimizer(Func<Vector3, double> objectiveFunction, Func<Vector3, Vector3> gradientFunc)
        {
            _objectiveFunction = objectiveFunction;
            _gradientFunc = gradientFunc;
        }

        public MinimizationResult UnconstrainedMinimizer(Vector3 initialGuess, double gradientTolerance = 1e-8, int maxIteration = 1000)
        {
            Vector3 x0 = new Vector3(initialGuess);
            int n = x0.Count;
            double f0 = _objectiveFunction(x0);
            Vector3 x1 = null;
            Vector3 s = null;

            if (double.IsNaN(f0))
            {
                throw new Exception("Unconstrained Minimizer: f(x0) is a NaN!");
            }

            gradientTolerance = Math.Max(gradientTolerance, GeoSharpMath.EPSILON);
            Matrix H1 = Matrix.Identity(n);
            int iteration = 0;
            Vector3 g0 = _gradientFunc(x0);

            while (iteration < maxIteration)
            {
                ValidateGradient(g0);
                Vector3 step = Vector3.Reverse(g0 * H1);
                ValidateGradient(step);

                double lengthStep = step.Length();
                if (lengthStep < gradientTolerance)
                {
                    throw new Exception("Newton step smaller than tolerance.");
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
                    double f1 = _objectiveFunction(x1);

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
                    throw new Exception("Line search step size smaller than gradient tolerance.");
                }
                if (iteration > maxIteration)
                {
                    throw new Exception("Max iteration reached during line search.");
                }

                Vector3 g1 = _gradientFunc(x1);
                Vector3 y = g1 - g0;
                double ys = Vector3.Dot(y, s);
                Vector3 Hy = y * H1;


            }


            throw new NotImplementedException();
        }

        private void ValidateGradient(Vector3 eval)
        {
            foreach (double val in eval)
            {
                if (double.IsNaN(val) || double.IsInfinity(val))
                {
                    throw new Exception("Non-finite gradient returned.");
                }
            }
        }

        private Matrix Tensor(Vector3 vec0, Vector3 vec1)
        {
            int m = vec0.Count;
            int n = vec1.Count;
            int i = m - 1;
            Matrix A = new Matrix();

            while (i >= 0)
            {
                double xi = vec0[i];
                int j = n - 1;
                double[] Ai = new double[j];

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

                A[i] = Ai;
                i--;
            }

            return A;
        }
    }
}
