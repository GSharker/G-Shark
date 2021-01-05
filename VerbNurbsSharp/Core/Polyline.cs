using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A simple data structure representing a polyline.
    /// PolylineData is useful, for example, as the result of a curve tessellation.
    /// </summary>
    public class Polyline : Serializable<Polyline>
    {
        /// <summary>
        /// The points in the polyline.
        /// </summary>
        public List<Point> Points { get; set; }
        /// <summary>
        /// The parameters of the individual points.
        /// </summary>
        public List<double> Parameters { get; set; }
        public PolylineData(List<Point> points, List<double> parameters)
        {
            Points = points;
            Parameters = parameters;
        }
    }
}
