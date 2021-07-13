using GShark.Core;
using GShark.Geometry;

namespace GShark.Optimization
{
    /// <summary>
    /// Collects the minimization results.
    /// </summary>
    public class MinimizationResult
    {
        /// <summary>
        /// Initialize the class.
        /// </summary>
        public MinimizationResult(Vector solutionPoint, double initialGuess, Vector gradient, Matrix hessianMatrix, int iterations, string message)
        {
            SolutionPoint = solutionPoint;
            InitialGuess = initialGuess;
            Gradient = gradient;
            HessianMatrix = hessianMatrix;
            Iterations = iterations;
            Message = message;
        }

        public Vector SolutionPoint { get; private set; }
        public double InitialGuess { get; private set; }
        public Vector Gradient { get; private set; }
        public Matrix HessianMatrix { get; private set; }
        public int Iterations { get; private set; }
        public string Message { get; private set; }
    }
}
