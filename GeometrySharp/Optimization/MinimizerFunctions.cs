using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;

namespace GeometrySharp.Optimization
{
    public class MinimizerFunctions
    {
        private readonly NurbsCurve _curve0;
        private readonly NurbsCurve _curve1;
        public MinimizerFunctions(NurbsCurve curve0, NurbsCurve curve1)
        {
            _curve0 = curve0;
            _curve1 = curve1;
        }
        public double Objective(Vector3 v)
        {
            Vector3 p0 = Evaluation.CurvePointAt(_curve0, v[0]);
            Vector3 p1 = Evaluation.CurvePointAt(_curve1, v[1]);

            Vector3 p0P1 = p0 - p1;

            return Vector3.Dot(p0P1, p0P1);
        }

        public Vector3 Gradient(Vector3 v)
        {
            return new Vector3();
        }
    }
}
