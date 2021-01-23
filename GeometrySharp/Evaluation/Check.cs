using GeometrySharp.Core;
using System;
using System.Linq;
using GeometrySharp.Geometry;

namespace GeometrySharp.Evaluation
{
    public class Check
    {
        /// <summary>
        /// Check whether a given list is a valid NURBS knot knots. This also checks the validity of the end points.
        /// More specifically, this method checks if the knot knots is of the following structure:
        /// The knot knots must be non-decreasing and of length (degree + 1) * 2 or greater
        /// [ (degree + 1 copies of the first knot), internal non-decreasing knots, (degree + 1 copies of the last knot) ]
        /// </summary>
        /// <param name="knots">The knot knots to test</param>
        /// <param name="degree">The degree</param>
        /// <returns>Whether the array is a valid knot knots or knot</returns>
        public static bool AreValidKnots(Knot knots, int degree)
        {
            if (knots.Count == 0) return false;
            if (knots.Count < (degree + 1)*2) return false;

            var rep = knots.First();
            for (int i = 0; i < knots.Count; i++)
            {
                // Verb doesn't allow for periodic knots, these two ifs should be removed.
                if(i < degree + 1)
                    if (Math.Abs(knots[i] - rep) > GeoSharpMath.EPSILON) return false;

                if(i > knots.Count - degree - 1 && i < knots.Count)
                    if (Math.Abs(knots[i] - rep) > GeoSharpMath.EPSILON) return false;

                if (knots[i] < rep - GeoSharpMath.EPSILON) return false;
                rep = knots[i];
            }
            return true;
        }

        /// <summary>
        /// Validate a NurbsCurveData object.
        /// </summary>
        /// <param name="curve">The NurbsCurve object.</param>
        /// <returns>True if it is valid.</returns>
        public static bool IsValidNurbsCurve(NurbsCurve curve)
        {
            if (curve.ControlPoints == null) throw new ArgumentNullException(nameof(curve.ControlPoints));
            if (curve.Weights == null) throw new ArgumentNullException(nameof(curve.Weights));
            if (curve.Weights.Count != curve.ControlPoints.Count)
                throw new ArgumentException("Weights and ControlPoints must have the same dimension");
            if (curve.Degree < 1) throw new ArgumentException("Degree must be greater than 1!");
            if (curve.Knots == null) throw new ArgumentNullException("Knots cannot be null!");
            if (curve.Knots.Count != curve.ControlPoints.Count + curve.Degree + 1)
                throw new ArgumentException("controlPoints.length + degree + 1 must equal knots.length!");
            if (!Check.AreValidKnots(curve.Knots, curve.Degree))
                throw new ArgumentException("Invalid knot knots format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            return true;
        }

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
            if (!Check.AreValidKnots(data.KnotsU, data.DegreeU) || !Check.AreValidKnots(data.KnotsV, data.DegreeV))
                throw new ArgumentException("Invalid knot knots format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            return data;
        }
    }
}
