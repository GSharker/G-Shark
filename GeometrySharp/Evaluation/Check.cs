using GeometrySharp.Core;
using System;
using System.Linq;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
    public class Check
    {
        // ToDo This checks should be moved into the NurbsSurface construct, to avoid the creation of non valid surfaces.
        /// <summary>
        /// Validate a NurbsSurfaceData object
        /// </summary>
        /// <param name="data">The curve object</param>
        /// <returns>The original, unmodified curve</returns>
        public static NurbsSurface isValidNurbsSurfaceData(NurbsSurface data)
        {
            if (data.ControlPoints == null) throw new ArgumentNullException("Control points array connot be null!");
            if (data.DegreeU < 1) throw new ArgumentException("DegreeU must be greater than 1!");
            if (data.DegreeV < 1) throw new ArgumentException("DegreeV must be greater than 1!");
            if (data.KnotsU == null) throw new ArgumentNullException("KnotU cannot be null!");
            if (data.KnotsV == null) throw new ArgumentNullException("KnotV cannot be null!");

            if (data.KnotsU.Count != data.ControlPoints.Count + data.DegreeU + 1)
                throw new ArgumentException("controlPointsU.length + degreeU + 1 must equal knotsU.length!");
            if (data.KnotsV.Count != data.ControlPoints[0].Count + data.DegreeV + 1)
                throw new ArgumentException("controlPointsV.length + degreeV + 1 must equal knotsV.length!");
            //if (!Check.AreValidKnots(data.KnotsU, data.DegreeU) || !Check.AreValidKnots(data.KnotsV, data.DegreeV))
                //throw new ArgumentException("Invalid knot knots format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            return data;
        }
    }
}
