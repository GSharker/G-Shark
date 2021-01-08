using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerbNurbsSharp.Geometry;
using VerbNurbsSharp.Core;

//Divide provides various tools for dividing and splitting NURBS geometry.

namespace VerbNurbsSharp.Evaluation
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
            KnotArray knots = curve.Knots;

			List<double> knots_to_insert = new List<double>();
			for (int i = 0; i <= degree; i++)
				knots_to_insert.Add(u);

			NurbsCurve refinedCurve = Modify.CurveKnotRefine(curve, knots_to_insert);

		 	int s = Eval.KnotSpan(degree, u, knots);

			KnotArray knots0 = (KnotArray)refinedCurve.Knots.ToList().GetRange(0, s + degree + 2);
			KnotArray knots1 = (KnotArray)refinedCurve.Knots.ToList().GetRange(0, s + 1);

			List<Vector> cpts0 = refinedCurve.ControlPoints.GetRange(0, s + 1);
			List<Vector> cpts1 = refinedCurve.ControlPoints.GetRange(0, s + 1);

			return new List<NurbsCurve>() { new NurbsCurve(degree, knots0, cpts0), new NurbsCurve(degree, knots1, cpts1) };
		}


		//////////////////////////// =================================== not implemented yet ================================== ///////////////////


		public static List<NurbsSurface> SurfaceSplit(NurbsSurface surface, float u, bool useV = false)
		{
			throw new NotImplementedException();
			//			KnotArray knots;
			//			int degree;
			//			List<List<Point>> controlPoints;

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

			//			List<List<Point>> newpts0 = new List<List<Point>>();
			//			List<List<Point>> newpts1 = new List<List<Point>>();

			//			var s = Eval.knotSpan(degree, u, knots);
			//			NurbsCurveData res = null;

			//			foreach (List<Point> cps in controlPoints)
			//			{
			//				res = Modify.curveKnotRefine(new NurbsCurveData(degree, knots, cps), knots_to_insert);

			//				newpts0.Add(res.ControlPoints.GetRange(0, s + 1)/*.slice(0, s + 1)*/);
			//				newpts1.Add(res.ControlPoints.GetRange(0, s + 1)/*.slice(s + 1)*/);
			//			}

			//			KnotArray knots0 = (KnotArray)res.Knots.ToList().GetRange(0, s + degree + 2);//.slice(0, s + degree + 2);
			//			KnotArray knots1 = (KnotArray)res.Knots.ToList().GetRange(0, s + 1);//.slice(s + 1);

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
