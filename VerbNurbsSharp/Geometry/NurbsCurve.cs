using VerbNurbsSharp.Evaluation;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Geometry
{
    public class NurbsCurve : Serializable<NurbsCurve>, ICurve
    {
        private readonly NurbsCurve _curve;
        /// <summary>
        /// A simple data structure representing a NURBS curve.
        /// NurbsCurve does no checks for legality. You can use <see cref="VerbNurbsSharp.Evaluation.Check"/> for that.
        /// </summary>
        public NurbsCurve(int degree, KnotArray knots, List<Vector> controlPoints)
        {
            Degree = degree;
            ControlPoints = controlPoints;
            Knots = knots;
        }
        /// <summary>
        /// Construct a NurbsCurve by a NurbsCurve object.
        /// </summary>
        /// <param name="curve">The curve object</param>
        public NurbsCurve(NurbsCurve curve)
        {
            this._curve = Check.IsValidNurbsCurve(curve);
        }
        /// <summary>
        /// Integer degree of curve.
        /// </summary>
        public int Degree { get; set; }
        /// <summary>
        /// 2d list of control points, where each control point is an list of length (dim).
        /// </summary>
        public List<Vector> ControlPoints { get; set; }

        /// <summary>
        /// List of non-decreasing knot values.
        /// </summary>
        public KnotArray Knots { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }

        public NurbsCurve AsNurbs()
        {
            throw new System.NotImplementedException();
        }

        public Interval Domain()
        {
            throw new System.NotImplementedException();
        }

        public Vector PointAt(double t)
        {
            throw new System.NotImplementedException();
        }

        public List<Vector> Derivatives(double u, int numberDerivs = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
