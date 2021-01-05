using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
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
        public KnotArray KnotsU { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in v direction.
        /// </summary>
        public KnotArray KnotsV { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in w direction.
        /// </summary>
        public KnotArray KnotsW { get; set; }
        /// <summary>
        /// 3d list of control points, where rows are the u dir, and columns run along the positive v direction,
        /// and where each control point is an list of length (dim)
        /// </summary>
        public List<List<List<Point>>> ControlPoints { get; set; }
        public VolumeData(int degreeU, int degreeV, int degreeW, KnotArray knotsU, KnotArray knotsV, KnotArray knotsW, List<List<List<Point>>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            DegreeW = degreeW;
            KnotsU = knotsU;
            KnotsV = knotsV;
            KnotsW = knotsW;
            ControlPoints = controlPoints;
        }
    }
}
