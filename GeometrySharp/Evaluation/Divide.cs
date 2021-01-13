using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;

//Divide provides various tools for dividing and splitting NURBS geometry.

namespace GeometrySharp.Evaluation
{
	public class Divide
	{
		/// <summary>
		/// Split a NURBS curve into two parts at a given parameter.
		/// </summary>
		/// <param name="curve">NurbsCurveData object representing the curve.</param>
		/// <param name="u">The parameter where to split the curve.</param>
		/// <returns>Two new curves, defined by degree, knots, and control points.</returns>
		public static List<NurbsCurve> CurveSplit(NurbsCurve curve, double u)
		{
			int degree = curve.Degree;
            Knot knots = curve.Knots;

            var knotsToInsert = Sets.RepeatData(u, degree + 1);

			NurbsCurve refinedCurve = Modify.CurveKnotRefine(curve, knotsToInsert);

		 	int s = Knot.Span(degree, u, knots);

			var knots0 = refinedCurve.Knots.ToList().GetRange(0, s + degree + 2).ToKnot();
            var knots1 = refinedCurve.Knots.GetRange(s + 1, refinedCurve.Knots.Count - (s + 1)).ToKnot();

			var controlPoints0 = refinedCurve.ControlPoints.GetRange(0, s + 1);
			var controlPoints1 = refinedCurve.ControlPoints.GetRange(s + 1, refinedCurve.ControlPoints.Count - (s + 1));

			return new List<NurbsCurve>() { new NurbsCurve(degree, knots0, controlPoints0), new NurbsCurve(degree, knots1, controlPoints1) };
		}


		//////////////////////////// =================================== not implemented yet ================================== ///////////////////


		public static List<NurbsSurface> SurfaceSplit(NurbsSurface surface, float u, bool useV = false)
		{
			throw new NotImplementedException();
			//			Knot knots;
			//			int degree;
			//			List<List<Vector3>> controlPoints;

			//			if (!useV)
			//			{
			//				controlPoints = Mat.Transpose(surface.ControlPoints);
			//				knots = surface.KnotsU;
			//				degree = surface.DegreeU;
			//			}
			//			else
			//			{
			//				controlPoints = surface.ControlPoints;
			//				knots = surface.KnotsU;
			//				degree = surface.DegreeU;
			//			}

			//			List<double> knots_to_insert = new List<double>();
			//			for (int i = 0; i < degree + 1; i++)
			//				knots_to_insert.Add(i);

			//			List<List<Vector3>> newpts0 = new List<List<Vector3>>();
			//			List<List<Vector3>> newpts1 = new List<List<Vector3>>();

			//			var s = Eval.knotSpan(degree, u, knots);
			//			NurbsCurveData res = null;

			//			foreach (List<Vector3> cps in controlPoints)
			//			{
			//				res = Modify.curveKnotRefine(new NurbsCurveData(degree, knots, cps), knots_to_insert);

			//				newpts0.Add(res.ControlPoints.GetRange(0, s + 1)/*.slice(0, s + 1)*/);
			//				newpts1.Add(res.ControlPoints.GetRange(0, s + 1)/*.slice(s + 1)*/);
			//			}

			//			Knot knots0 = (Knot)res.Knots.ToList().GetRange(0, s + degree + 2);//.slice(0, s + degree + 2);
			//			Knot knots1 = (Knot)res.Knots.ToList().GetRange(0, s + 1);//.slice(s + 1);

			//			if (!useV)
			//			{
			//				newpts0 = Mat.transpose(newpts0);
			//				newpts1 = Mat.transpose(newpts1);

			//				return new List<NurbsSurfaceData>(){ new NurbsSurfaceData(degree, surface.DegreeV, knots0, surface.KnotsV, newpts0),
			//				new NurbsSurfaceData(degree, surface.DegreeV, knots1, surface.KnotsV, newpts1)};
			//			}

			//			//v dir
			//			return new List<NurbsSurfaceData>(){new NurbsSurfaceData(surface.DegreeU, degree, surface.KnotsU, knots0, newpts0),
			//			new NurbsSurfaceData(surface.DegreeU, degree, surface.KnotsU, knots1, newpts1) };
		}


	}
}
