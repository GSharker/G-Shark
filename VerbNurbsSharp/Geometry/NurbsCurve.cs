using VerbNurbsSharp.Evaluation;
using System.Collections.Generic;
using VerbNurbsSharp.Core;
using Newtonsoft.Json;

namespace GeometryLib.Geometry
{
    public class NurbsCurve : Serializable<NurbsCurve>
    {
        public override NurbsCurve FromJson(string s)
        {
            throw new System.NotImplementedException();
        }

        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
