using GShark.Core;
using GShark.Geometry;

namespace GShark.Optimization
{
    /// <summary>
    /// Collects the minimization results.
    /// </summary>
    internal class MinimizationResult
    {
        /// <summary>
        /// Initialize the class.
        /// </summary>
        internal MinimizationResult(Vector solutionPoint, double initialGuess, Vector gradient, Matrix hessianMatrix, int iterations, string message)
        {
            SolutionPoint = solutionPoint;
            InitialGuess = initialGuess;
            Gradient = gradient;
            HessianMatrix = hessianMatrix;
            Iterations = iterations;
            Message = message;
        }

        internal Vector SolutionPoint { get; private set; }
        internal double InitialGuess { get; private set; }
        internal Vector Gradient { get; private set; }
        internal Matrix HessianMatrix { get; private set; }
        internal int Iterations { get; private set; }
        internal string Message { get; private set; }
    }
}
