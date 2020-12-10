using GeometryLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using GeometryLib.Core;

namespace GeometryLib.Evaluation
{
    class Check
    {
        internal static NurbsCurveData IsValidNurbsCurveData(NurbsCurveData data)
        {
            if (data.ControlPoints == null) throw new Exception("Control points array cannot be null!");
            //if (data.Degree == null) throw new Exception("Degree cannot be null!");
            if (data.Degree < 1) throw new Exception("Degree must be greater than 1!");
            if (data.Knots == null) throw new Exception("Knots cannot be null!");

            if (data.Knots.Count != data.ControlPoints.Count + data.Degree + 1)
                throw new Exception("controlPoints.length + degree + 1 must equal knots.length!");

            if (!Check.IsValidKnotVector(data.Knots, data.Degree))
                throw new Exception("Invalid knot vector format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");

            return data;
        }

        internal static bool IsValidKnotVector(List<double> knots, int degree)
        {
            if (knots.Count == 0) return false;
            if (knots.Count <= (degree + 1) * 2) return false;

            var rep = knots.First();

            for (int i=0;i <= degree + 1; i++)
                if (Math.Abs(knots[i] - rep) > Constants.EPSILON) return false;

            rep = knots.Last();

            for (int i = knots.Count - degree - 1; i < knots.Count; i++)
                if (Math.Abs(knots[i] - rep) > Constants.EPSILON) return false;

            return IsNonDecreasing(knots);
        }

        internal static bool IsNonDecreasing(List<double> knots)
        {
            var rep = knots.First();
            for (int i = 0; i<= knots.Count; i++ )
            {
                if (knots[i] < rep - Constants.EPSILON) return false;
                rep = knots[i];
            }
            return true;
        }
    }
}
