using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a NURBS volume. This data structure is largely experimental in intent. Like CurveData
    /// and Surface, this data structure does no legality checks.
    /// </summary>
    public class Volume : Serializable<Volume>
    {
        /// <summary>
        /// Integer degree in u direction.
        /// </summary>
        public int DegreeU { get; set; }
        /// <summary>
        /// Integer degree in v direction.
        /// </summary>
        public int DegreeV { get; set; }
        /// <summary>
        /// Integer degree in w direction.
        /// </summary>
        public int DegreeW { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in u direction.
        /// </summary>
        public Knot KnotsU { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in v direction.
        /// </summary>
        public Knot KnotsV { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in w direction.
        /// </summary>
        public Knot KnotsW { get; set; }
        /// <summary>
        /// 3d list of control points, where rows are the u dir, and columns run along the positive v direction,
        /// and where each control point is an list of length (dim)
        /// </summary>
        public List<List<List<Vector3>>> ControlPoints { get; set; }
        public Volume(int degreeU, int degreeV, int degreeW, Knot knotsU, Knot knotsV, Knot knotsW, List<List<List<Vector3>>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            DegreeW = degreeW;
            KnotsU = knotsU;
            KnotsV = knotsV;
            KnotsW = knotsW;
            ControlPoints = controlPoints;
        }

        public override Volume FromJson(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize a volume to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);

    }
}
