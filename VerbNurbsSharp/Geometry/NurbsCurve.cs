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

        public NurbsCurve()
        {
        }

        public static NurbsCurve ByPoints(List<Point> points, int degree = 3) => new NurbsCurve(Make.RationalInterpCurve(points, degree));

        public int Degree => this.data.Degree;
        public List<double> Knots => this.data.Knots;
        public List<Point> ControlPoints => this.data.ControlPoints;

        public NurbsCurve Clone() => new NurbsCurve(this.data);
        public Interval<double> Domain() => new Interval<double>(this.data.Knots[0], this.data.Knots[this.data.Knots.Count - 1]);
    }
}
