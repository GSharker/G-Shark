using VerbNurbsSharp.Evaluation;
using System.Collections.Generic;
using VerbNurbsSharp.Core;

namespace GeometryLib.Geometry
{
    public class NurbsCurve : Serializable<NurbsCurve>
    {
        private readonly NurbsCurveData data;
        /// <summary>
        /// Construct a NurbsCurve by a NurbsCurveData object
        /// </summary>
        /// <param name="data">The data object</param>
        public NurbsCurve(NurbsCurveData data)
        {
            this.data = Check.isValidNurbsCurveData(data);
        }

        public NurbsCurve() { }

        public int Degree => data.Degree;
        public KnotArray Knots => data.Knots;
        public List<Point> ControlPoints => data.ControlPoints;
        public List<double> Weights => Eval.Weight1d(data.ControlPoints);

        public static NurbsCurve byKnotsControlPointsWeights(int degree, KnotArray knots, List<Point> controlPoints, List<double> weights = null) => null;
        public static NurbsCurve ByPoints(List<Point> points, int degree = 3) => new NurbsCurve(Make.RationalInterpCurve(points, degree));


        public NurbsCurve Clone() => new NurbsCurve(this.data);
        public Interval<double> Domain() => new Interval<double>(this.data.Knots[0], this.data.Knots[this.data.Knots.Count - 1]);
    }
}
