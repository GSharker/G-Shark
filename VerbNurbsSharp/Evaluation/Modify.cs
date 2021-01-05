using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Evaluation
{
	public class Modify
	{
		/// <summary>
		/// Insert a collectioin of knots on a curve
		/// corresponds to Algorithm A5.4 (Piegl & Tiller)
		/// </summary>
		/// <param name="curve"></param>
		/// <param name="knotsToInsert"></param>
		/// <returns></returns>
		public static NurbsCurveData curveKnotRefine(NurbsCurveData curve, List<double> knotsToInsert)
		{
			if (knotsToInsert.Count == 0)
				return Make.ClonedCurve(curve);

			int degree = curve.Degree;
			List<Point> controlPoints = curve.ControlPoints;
			KnotArray knots = curve.Knots;

			return null;
		}
	}
}
