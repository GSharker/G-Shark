using VerbNurbsSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Evaluation
{
    //ToDO initialized the class Make
    public class Make
    {
        public Make()
        {

        }

        public static NurbsCurveData RationalBezierCurve(List<Point> controlPoints, List<double> weights = null)
        {
            var degree = controlPoints.Count - 1;
            var knots = new KnotArray();
            for (int i = 0; i < degree + 1; i++)
                knots.Add(0.0);
            for (int i = 0; i < degree + 1; i++)
                knots.Add(1.0);
            if (weights == null)
                weights = Sets.RepeatData(1.0, controlPoints.Count);
            return new NurbsCurveData(degree, knots, Eval.Homogenize1d(controlPoints, weights));
        }
    }
}
