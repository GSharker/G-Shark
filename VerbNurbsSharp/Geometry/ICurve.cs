using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VerbNurbsSharp.Core;
using NurbsCurve = GeometryLib.Geometry.NurbsCurve;

namespace VerbNurbsSharp.Geometry
{
    public interface ICurve : ISerializable
    {
        /// <summary>
        /// Provide the NURBS representation of the curve.
        /// </summary>
        /// <returns>A NurbsCurveData object representing the curve.</returns>
        public NurbsCurve AsNurbs();
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
        public Vector PointAt(double t);
        /// <summary>
        /// Evaluate the derivatives at a point on a curve.
        /// </summary>
        /// <param name="u">The parameter on the curve.</param>
        /// <param name="numberDerivs">The number of derivatives to evaluate on the curve.</param>
        /// <returns>An set of derivative vectors.</returns>
        public List<Vector> Derivatives(double u, int numberDerivs = 1);

    }
}
