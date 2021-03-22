using GeometrySharp.Core;
using GeometrySharp.Geometry;

namespace GeometrySharp.Optimization
{
    /// <summary>
    /// Collects the minimization results.
    /// </summary>
    public class MinimizationResult
    {
        /// <summary>
        /// Initialize the class.
        /// </summary>
        public MinimizationResult(Vector3 solutionPoint, double initialGuess, Vector3 gradient, Matrix hessianMatrix, int iterations)
        {
            SolutionPoint = solutionPoint;
            InitialGuess = initialGuess;
            Gradient = gradient;
            HessianMatrix = hessianMatrix;
            Iterations = iterations;
        }

        public Vector3 SolutionPoint { get; private set; }
        public double InitialGuess { get; private set; }
        public Vector3 Gradient { get; private set; }
        public Matrix HessianMatrix { get; private set; }
        public int Iterations { get; private set; }
    }
}
