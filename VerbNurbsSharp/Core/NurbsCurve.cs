using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A simple data structure representing a NURBS curve.
    /// NurbsCurveData does no checks for legality. You can use <see cref="VerbNurbsSharp.Evaluation.Check"/> for that.
    /// </summary>
    public class NurbsCurve : Serializable<NurbsCurve>
    {
        public NurbsCurve(int degree, KnotArray knots, List<Point> controlPoints)
        {
            Degree = degree;
            ControlPoints = controlPoints;
            Knots = knots;
        }
        /// <summary>
        /// Integer degree of curve.
        /// </summary>
        public int Degree { get; set; }
        /// <summary>
        /// 2d list of control points, where each control point is an list of length (dim).
        /// </summary>
        public List<Point> ControlPoints { get; set; }

        /// <summary>
        /// List of non-decreasing knot values.
        /// </summary>
        public KnotArray Knots { get; }
    }
}
