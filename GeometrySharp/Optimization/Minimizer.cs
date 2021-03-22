using System;
using GeometrySharp.Geometry;

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
            // Has to be implemented.
            throw new NotImplementedException();
        }

        public Vector3 NumericalGradient(Func<Vector3, double> func, Vector3 vec)
        {
            // Has to be implemented.
            return new Vector3();
        }
    }
}
