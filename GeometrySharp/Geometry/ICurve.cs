using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// ICurve is implemented by all curve types.
    /// ICurve is a series of methods that all the curves have.
    /// </summary>
    public interface ICurve
    {
        /// <summary>
        /// Calculate the length of the curve.
        /// </summary>
        /// <returns>The length of the curve.</returns>
        public double Length();

        /// <summary>
        /// Obtain the parametric domain of the curve.
        /// </summary>
        /// <returns>An Interval object containing the min and max of the domain.</returns>
        public Interval Domain();

        /// <summary>
        /// Evaluate a point on the curve.
        /// </summary>
        /// <param name="t">The parameter on the curve.</param>
        /// <returns>The evaluated point.</returns>
        public Vector3 PointAt(double t);

        /// <summary>
        /// Gets the BoundingBox in ascending fashion.
        /// </summary>
        public BoundingBox BoundingBox { get; }

        // TransformAt
        // Transform

        /// Note this method doesn't see necessary here, we keep it private and see in the future.
        /// <summary>
        /// Evaluate the derivatives at a point on a curve.
        /// </summary>
        /// <param name="parameter">The parameter on the curve.</param>
        /// <param name="numberDerivs">The number of derivatives to evaluate on the curve.</param>
        /// <returns>An set of derivative vectors.</returns>
        public List<Vector3> Derivatives(double parameter, int numberDerivs = 1);
    }
}
