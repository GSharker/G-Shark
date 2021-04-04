using GeometrySharp.Geometry;

namespace GeometrySharp.Optimization
{
    /// <summary>
    /// Interface defines the minimum implementation for an objective function used by the <see cref="Minimizer"/>.
    /// </summary>
    public interface IObjectiveFunction
    {
        /// <summary>
        /// Defines the objective function.
        /// </summary>
        double Value(Vector3 v);

        /// <summary>
        /// Defines the gradient function.
        /// </summary>
        Vector3 Gradient(Vector3 v);
    }
}
