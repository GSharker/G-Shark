#nullable enable
using GShark.Core;
using GShark.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// This class represents a NURBS curve.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Data/NurbsBaseCollection.cs?name=example)]
    /// </example>
    public class NurbsCurve : NurbsBase
    {
        /// <summary>
        /// Internal constructor, creates a NURBS curve.
        /// </summary>
        internal NurbsCurve(int degree, KnotVector knots, List<Point4> controlPoints) 
            : base(degree, knots, controlPoints)
        {
        }

        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="points">The points of the curve.</param>
        /// <param name="degree">The curve degree.</param>
        public NurbsCurve(List<Point3>? points, int degree)
            : this(degree, new KnotVector(degree, points!.Count), points.Select(p => new Point4(p)).ToList())
        {
        }

        /// <summary>
        /// Creates a NURBS curve.
        /// </summary>
        /// <param name="points">The points of the curve.</param>
        /// <param name="weights">The weights of each point.</param>
        /// <param name="degree">The curve degree.</param>
        public NurbsCurve(List<Point3>? points, List<double> weights, int degree)
            : this(degree, new KnotVector(degree, points!.Count), points.Select((p, i) => new Point4(p, weights[i])).ToList())
        {
        }
    }
}
