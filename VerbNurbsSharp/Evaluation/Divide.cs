using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerbNurbsSharp.Core;

// Divide provides various tools for dividing and splitting NURBS geometry.

namespace VerbNurbsSharp.Evaluation
{
	public class Divide
	{
		public static List<NurbsSurfaceData> surfaceSplit(NurbsSurfaceData surface, float u, bool useV = false)
		{
			KnotArray knots;
			int degree;
			List<List<Point>> controlPoints;

			if (!useV)
			{
				controlPoints = Mat.Transpose(surface.ControlPoints);
				knots = surface.KnotsU;
				degree = surface.DegreeU;
			}
			else
			{
				controlPoints = surface.ControlPoints;
				knots = surface.KnotsU;
				degree = surface.DegreeU;
			}

			List<double> knots_to_insert = new List<double>();
			for (int i = 0; i < degree + 1; i++)
				knots_to_insert.Add(i);

			List<List<Point>> newpts0 = new List<List<Point>>();
			List<List<Point>> newpts1 = new List<List<Point>>();

			var s = Eval.knotSpan(degree, u, knots);
			NurbsCurveData res = null;

			foreach (List<Point> cps in controlPoints)
			{
				res = Modify.curveKnotRefine(new NurbsCurveData(degree, knots, cps), knots_to_insert);

				newpts0.Add(res.ControlPoints.GetRange(0, s + 1)/*.slice(0, s + 1)*/);
				newpts1.Add(res.ControlPoints.GetRange(0, s + 1)/*.slice(s + 1)*/);
			}

			KnotArray knots0 = (KnotArray)res.Knots.ToList().GetRange(0, s + degree + 2);//.slice(0, s + degree + 2);
			KnotArray knots1 = (KnotArray)res.Knots.ToList().GetRange(0, s + 1);//.slice(s + 1);

			if (!useV)
			{
				newpts0 = Mat.Transpose(newpts0);
				newpts1 = Mat.Transpose(newpts1);

				return new List<NurbsSurfaceData>(){ new NurbsSurfaceData(degree, surface.DegreeV, knots0, surface.KnotsV, newpts0),
				new NurbsSurfaceData(degree, surface.DegreeV, knots1, surface.KnotsV, newpts1)};
			}

			//v dir
			return new List<NurbsSurfaceData>(){new NurbsSurfaceData(surface.DegreeU, degree, surface.KnotsU, knots0, newpts0),
			new NurbsSurfaceData(surface.DegreeU, degree, surface.KnotsU, knots1, newpts1) };
		}
	}
}
