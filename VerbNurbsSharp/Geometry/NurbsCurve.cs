using System;
using VerbNurbsSharp.Evaluation;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VerbNurbsSharp.Core;
using Newtonsoft.Json;

namespace VerbNurbsSharp.Geometry
{
    public class NurbsCurve : Serializable<NurbsCurve>, ICurve, IEquatable<NurbsCurve>
    {
        /// <summary>
        /// A simple data structure representing a NURBS curve.
        /// NurbsCurve does no checks for legality. You can use <see cref="VerbNurbsSharp.Evaluation.Check"/> for that.
        /// </summary>
        public NurbsCurve(int degree, KnotArray knots, List<Vector> controlPoints, List<double> weights = null)
        {
            Degree = degree;
            ControlPoints = weights == null ? controlPoints : Eval.Homogenize1d(controlPoints, weights); 
            Knots = knots;
        }
        /// <summary>
        /// Construct a NurbsCurve by a NurbsCurve object.
        /// </summary>
        /// <param name="curve">The curve object</param>
        public NurbsCurve(NurbsCurve curve)
        {
            if (Check.IsValidNurbsCurve(curve))
            {
                Degree = curve.Degree;
                ControlPoints = curve.ControlPoints;
                Knots = curve.Knots;
            };
        }
        /// <summary>
        /// Integer degree of curve.
        /// </summary>
        public int Degree { get; set; }
        /// <summary>
        /// 2d list of control points, where each control point is a list of length (dim).
        /// </summary>
        public List<Vector> ControlPoints { get; set; }
        /// <summary>
        /// List of non-decreasing knot values.
        /// </summary>
        public KnotArray Knots { get; set; }
        /// <summary>
        /// Obtain a copy of the NurbsCurve.
        /// </summary>
        /// <returns>The copied curve.</returns>
        public NurbsCurve Clone()
        {
            return new NurbsCurve(this);
        }
        /// <summary>
        /// Check if the points represented a set (wi*pi, wi) with length (dim+1).
        /// </summary>
        /// <returns>Get the result.</returns>
        public bool AreControlPointsHomogenized() => this.ControlPoints.All(pt => pt.Count == 4);
        /// <summary>
        /// Transform a curve with the given matrix.
        /// </summary>
        /// <param name="mat">4d set representing the transform.</param>
        /// <returns>A new NurbsCurve transformed.</returns>
        public NurbsCurve Transform(Matrix mat) => new NurbsCurve(Modify.RationalCurveTransform(this, mat));
        // ToDo Implement the async method.
        /// <summary>
        /// Split the curve at the give parameter.
        /// </summary>
        /// <param name="t">The parameter at which to split the curve</param>
        /// <returns>Two curves - one at the lower end of the parameter range and one at the higher end.</returns>
        public List<NurbsCurve> Split(double t) => Divide.CurveSplit(this, t);
        // ToDo Implement the async method.

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }

        public NurbsCurve AsNurbs()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Determine the valid domain of the curve.
        /// </summary>
        /// <returns>representing the high and end point of the domain of the curve.</returns>
        public Interval Domain() => new Interval(this.Knots.First(), this.Knots.Last());

        public Vector PointAt(double t)
        {
            throw new System.NotImplementedException();
        }

        public List<Vector> Derivatives(double u, int numberDerivs = 1)
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(NurbsCurve other)
        {
            if (other == null) return false;
            return Degree == other.Degree && Equals(ControlPoints, other.ControlPoints) && Equals(Knots, other.Knots);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            var controlPts = string.Join("\n", this.ControlPoints.Select(first => $"({string.Join(",", first)})"));
            var knots = $"Knots = ({string.Join(",", this.Knots)})";
            var degree = $"CurveDegree = {Degree}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(knots);
            stringBuilder.AppendLine(degree);

            return stringBuilder.ToString();
        }

        public override NurbsCurve FromJson(string s) => throw new NotImplementedException();

        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
