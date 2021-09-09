using GShark.Geometry;

namespace GShark.Optimization
{
    /// <summary>
    /// Defines the minimum implementation for an objective function used by the <see cref="Minimizer"/>.
    /// </summary>
    internal interface IObjectiveFunction
    {
        /// <summary>
        /// Defines the objective function.
        /// </summary>
        double Value(Vector v);

        /// <summary>
        /// Defines the gradient function.
        /// </summary>
        Vector Gradient(Vector v);
    }
}
