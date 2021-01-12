using Newtonsoft.Json;
using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a polyline.
    /// PolylineData is useful, for example, as the result of a curve tessellation.
    /// </summary>
    public class Polyline : Serializable<Polyline>
    {
        public Polyline()
        {

        }
        /// <summary>
        /// The points in the polyline.
        /// </summary>
        public List<Vector3> Points { get; set; }
        /// <summary>
        /// The parameters of the individual points.
        /// </summary>
        public List<double> Parameters { get; set; }
        public Polyline(List<Vector3> points, List<double> parameters)
        {
            Points = points;
            Parameters = parameters;
        }

        /// <summary>
        /// Serialize a polyline to json
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);

        /// <summary>
        /// Create a polyline from a json
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override Polyline FromJson(string s)
        {
            throw new System.NotImplementedException();
        }
    }
}
