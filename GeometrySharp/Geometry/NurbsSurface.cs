using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a NURBS surface.
    /// NurbsSurfaceData does no checks for legality. You can use <see cref="GeometrySharp.Evaluation.Check"/> for that.
    /// </summary>
    public class NurbsSurface : Serializable<NurbsSurface>
    {
        public NurbsSurface(int degreeU, int degreeV, Knot knotsU, Knot knotsV, List<List<Vector3>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            KnotsU = knotsU;
            KnotsV = knotsV;
            ControlPoints = controlPoints;
        }
        /// <summary>
        /// Integer degree of surface in u direction.
        /// </summary>
        public int DegreeU { get; set; }
        /// <summary>
        /// Integer degree of surface in v direction.
        /// </summary>
        public int DegreeV { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in u direction.
        /// </summary>
        public Knot KnotsU { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in v direction.
        /// </summary>
        public Knot KnotsV { get; set; }
        /// <summary>
        /// 2d list of control points, the vertical direction (u) increases from top to bottom, the v direction from left to right,
        /// and where each control point is an list of length (dim).
        /// </summary>
        public List<List<Vector3>> ControlPoints { get; set; }

        public override NurbsSurface FromJson(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize a nurbs surface to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
