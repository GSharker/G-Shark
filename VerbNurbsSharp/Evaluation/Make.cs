using VerbNurbsSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Evaluation
{
    public class Make
    {
        public static NurbsCurveData ClonedCurve(NurbsCurveData curve)
        {
            return new NurbsCurveData(curve.Degree, curve.Knots, curve.ControlPoints);
        }
    }
}
