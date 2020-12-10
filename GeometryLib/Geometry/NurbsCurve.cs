using GeometryLib.Evaluation;
using System.Collections.Generic;
using GeometryLib.Core;

namespace GeometryLib.Geometry
{
    public class NurbsCurve : ICurve
    {
        public NurbsCurve(NurbsCurveData data)
        {
            Data = Check.IsValidNurbsCurveData(data);
        }

        private NurbsCurveData Data { get; set; }
    }
}
