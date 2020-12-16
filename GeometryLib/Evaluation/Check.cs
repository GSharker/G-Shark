using GeometryLib.Core;
using System;
using System.Linq;

namespace GeometryLib.Evaluation
{
    public class Check
    {
        /// <summary>
        /// Check whether a given array is a valid NURBS knot vector. This also checks the validity of the end points.
        /// More specifically, this method checks if the knot vector is of the following structure:
        ///
        /// The knot vector must be non-decreasing and of length (degree + 1) * 2 or greater
        ///
        /// [ (degree + 1 copies of the first knot), internal non-decreasing knots, (degree + 1 copies of the last knot) ]
        /// </summary>
        /// <param name="vector">The knot vector to test</param>
        /// <param name="degree">The degree</param>
        /// <returns>Whether the array is a valid knot vector or knot</returns>
        public static bool isValidKnotVector(Vector vector, int degree)
        {
            if (vector.Count == 0) return false;
            if (vector.Count < degree + 1) return false;
            var rep = vector.First();
            for (int i = 0; i <= degree + 1; i++)
                if (Math.Abs(vector[i] - rep) > Constants.EPSILON) return false;
            rep = vector.Last();
            for (int i = vector.Count - degree - 1; i <= vector.Count; i++)
                if (Math.Abs(vector[i] - rep) > Constants.EPSILON) return false;
            return isNonDecreasing(vector);
        }

        /// <summary>
        /// Check if an array of floating point numbers is non-decreasing, although there may be repeats. This is an important
        ///validation step for NURBS knot vectors
        /// </summary>
        /// <param name="vector">The data object</param>
        /// <returns>Whether the array is non-decreasing</returns>
        public static bool isNonDecreasing(Vector vector)
        {
            var rep = vector.First();
            for (int i = 0; i <= vector.Count; i++)
            {
                if (vector[i] < rep - Constants.EPSILON) return false;
                rep = vector[i];
            }
            return true;
        }

        /// <summary>
        /// Validate a NurbsCurveData object
        /// </summary>
        /// <param name="data">The data object</param>
        /// <returns>The original, unmodified data</returns>
        public static NurbsCurveData isValidNurbsCurveData(NurbsCurveData data)
        {
            if (data.ControlPoints == null) throw new ArgumentNullException("Control points array cannot be null!");
            if (data.Degree < 1) throw new ArgumentException("Degree must be greater than 1!");
            if (data.Knots == null) throw new ArgumentNullException("Knots cannot be null!");
            if (data.Knots.Count != data.ControlPoints.Count + data.Degree + 1)
                throw new ArgumentException("controlPoints.length + degree + 1 must equal knots.length!");
            if (!Check.isValidKnotVector((Vector)data.Knots, data.Degree))
                throw new ArgumentException("Invalid knot vector format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            return data;
        }
    }
}
