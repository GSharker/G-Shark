using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GeometrySharp.Core;

// ToDo this class need to be tested.
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

            //if (data.ControlPoints == null) throw new ArgumentNullException("Control points array connot be null!");
            //if (data.DegreeU < 1) throw new ArgumentException("DegreeU must be greater than 1!");
            //if (data.DegreeV < 1) throw new ArgumentException("DegreeV must be greater than 1!");
            //if (data.KnotsU == null) throw new ArgumentNullException("KnotU cannot be null!");
            //if (data.KnotsV == null) throw new ArgumentNullException("KnotV cannot be null!");

            //if (data.KnotsU.Count != data.ControlPoints.Count + data.DegreeU + 1)
            //    throw new ArgumentException("controlPointsU.length + degreeU + 1 must equal knotsU.length!");
            //if (data.KnotsV.Count != data.ControlPoints[0].Count + data.DegreeV + 1)
            //    throw new ArgumentException("controlPointsV.length + degreeV + 1 must equal knotsV.length!");
            ////if (!Check.AreValidKnots(data.KnotsU, data.DegreeU) || !Check.AreValidKnots(data.KnotsV, data.DegreeV))
            ////throw new ArgumentException("Invalid knot knots format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            //return data;

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
