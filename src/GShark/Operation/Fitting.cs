using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Provides functions for interpolating and approximating NURBS curves and surfaces from points.<br/>
    /// Approximation uses least squares algorithm.
    /// </summary>
    public static class Fitting
    {
        // ToDo: this has to be implemented.
        public static NurbsCurve ApproximateCurve(List<Vector3> pts, int degree)
        {
            return new NurbsCurve();
        }

        /// <summary>
        /// Creates a set of interpolated cubic beziers through a set of points.
        /// </summary>
        /// <param name="pts">Set of points to interpolate.</param>
        /// <returns>A set of cubic beziers.</returns>
        public static List<NurbsCurve> BezierInterpolation(List<Vector3> pts)
        {
            if (pts.Count == 0)
            {
                throw new Exception("Collection of points is empty.");
            }

            List<NurbsCurve> beziers = new List<NurbsCurve>();
            (List<Vector3> ptsA, List<Vector3> ptsB) ctrlPts = SolveBezierCtrlPts(pts);

            for (int i = 0; i < pts.Count - 1; i++)
            {
                beziers.Add(new NurbsCurve(new List<Vector3> {pts[i], ctrlPts.ptsA[i], ctrlPts.ptsB[i], pts[i + 1]},
                    3));
            }

            return beziers;
        }

        /// <summary>
        /// Creates a interpolated curve through a set of points.<br/>
        /// <em>Refer to Algorithm A9.1 on The NURBS Book, pp.369-370 for details.</em>
        /// </summary>
        /// <param name="pts">The set of points to interpolate.</param>
        /// <param name="degree">The Curve degree.</param>
        /// <param name="startTangent">The tangent vector for the first point.</param>
        /// <param name="endTangent">The tangent vector for the last point.</param>
        /// <param name="centripetal">True use the chord as per knot spacing, false use the squared chord.</param>
        /// <returns>A the interpolated curve.</returns>
        public static NurbsCurve InterpolatedCurve(List<Vector3> pts, int degree, Vector3 startTangent = null,
            Vector3 endTangent = null, bool centripetal = false)
        {
            if (pts.Count < degree + 1)
            {
                throw new Exception($"You must supply at least degree + 1 points. You supplied {pts.Count} pts.");
            }

            // Gets uk parameters.
            List<double> uk = ParametersCurve(pts, centripetal);

            // Compute knot vectors.
            // Start knot vectors.
            Knot knotStart = Sets.RepeatData(0.0, degree + 1).ToKnot();

            // If we have tangent values we need two more control points and knots.
            bool hasTangents = startTangent != null && endTangent != null;
            int start = (hasTangents) ? 0 : 1;
            int end = (hasTangents) ? uk.Count - degree + 1 : uk.Count - degree;

            // Use averaging method (Eqn 9.8) to compute internal knots in the knot vector.
            for (int i = start; i < end; i++)
            {
                double weightSum = 0.0;
                for (int j = 0; j < degree; j++)
                {
                    weightSum += uk[i + j];
                }

                knotStart.Add((1.0 / degree) * weightSum);
            }

            // Add end knot vectors.
            Knot knots = knotStart.Concat(Sets.RepeatData(1.0, degree + 1)).ToKnot();

            // Global interpolation.
            // Build matrix of basis function coefficients.
            Matrix coeffMatrix = BuildCoefficientsMatrix(pts, degree, hasTangents, uk, knots);
            // Solve for each points.
            List<Vector3> ctrlPts = SolveCtrlPts(pts, degree, startTangent, endTangent, coeffMatrix, knots, hasTangents);

            return new NurbsCurve(degree, knots, ctrlPts);
        }

        /// <summary>
        /// Defines the control points.
        /// </summary>
        internal static List<Vector3> SolveCtrlPts(List<Vector3> pts, int degree, Vector3 startTangent, Vector3 endTangent, Matrix coeffMatrix,
            Knot knots, bool hasTangents)
        {
            Matrix matrixLu = Matrix.Decompose(coeffMatrix, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            // Equations 9.11
            double mult0 = knots[degree + 1] / degree;
            // Equations 9.12
            double mult1 = (1 - knots[knots.Count - degree - 2]) / degree;

            // Solve for each dimension.
            for (int i = 0; i < pts[0].Count; i++)
            {
                Vector3 b = new Vector3();
                if (!hasTangents)
                {
                    b = pts.Select(pt => pt[i]).ToVector();
                }
                else
                {
                    // Insert the tangents at the second and second to last index.
                    b.Add(pts[0][i]);
                    // Equations 9.11
                    b.Add(startTangent[i] * mult0);
                    b.AddRange(pts.Skip(1).Take(pts.Count - 2).Select(pt => pt[i]));
                    // Equations 9.12
                    b.Add(endTangent[i] * mult1);
                    b.Add(pts[^1][i]);
                }

                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            return ptsSolved.Transpose().Select(pt => pt.ToVector()).ToList();
        }

        /// <summary>
        /// Builds the coefficient matrix for global interpolation.
        /// </summary>
        internal static Matrix BuildCoefficientsMatrix(List<Vector3> pts, int degree, bool hasTangents, List<double> uk, Knot knots)
        {
            int dim = (hasTangents) ? pts.Count + 1 : pts.Count - 1;
            Matrix coeffMatrix = new Matrix();

            int dimEnd = (hasTangents) ? pts.Count - (degree - 1) : pts.Count - (degree + 1);

            foreach (double u in uk)
            {
                int span = knots.Span(dim, degree, u);
                List<double> basicFunction = Evaluation.BasicFunction(degree, knots, span, u);

                List<double> startRow = Sets.RepeatData(0.0, span - degree);
                List<double> endRow = Sets.RepeatData(0.0, dimEnd - (span - degree));

                coeffMatrix.Add(startRow.Concat(basicFunction).Concat(endRow).ToList());
            }

            if (hasTangents)
            {
                List<double> zeros = Sets.RepeatData(0.0, coeffMatrix[0].Count - 2);
                List<double> tangent = new List<double> { -1.0, 1.0 };

                coeffMatrix.Insert(1, tangent.Concat(zeros).ToList());
                coeffMatrix.Insert(coeffMatrix.Count - 1, zeros.Concat(tangent).ToList());
            }

            return coeffMatrix;
        }

        /// <summary>
        /// Refer to the Equations 9.4 and 9.5 for chord length parametrization, and Equation 9.6 for centripetal method
        /// on The NURBS Book(2nd Edition), pp.364-365.
        /// </summary>
        internal static List<double> ParametersCurve(List<Vector3> pts, bool centripetal = false)
        {
            List<double> chords = new List<double> { 0.0 };
            for (int i = 1; i < pts.Count; i++)
            {
                double chord = (centripetal) ? Math.Sqrt((pts[i] - pts[i - 1]).Length()) : (pts[i] - pts[i - 1]).Length();
                chords.Add(chord + chords.Last());
            }

            // Divide the individual chord length by the total chord length.
            List<double> uk = new List<double>();
            double maxChordLength = chords.Last();
            for (int i = 0; i < pts.Count; i++)
            {
                uk.Add(chords[i] / maxChordLength);
            }

            return uk;
        }

        internal static (List<Vector3> ptsA, List<Vector3> ptsB) SolveBezierCtrlPts(List<Vector3> pts, bool getsEndDerivatives = false)
        {
            int n = pts.Count - 1;

            // Build the coefficient matrix.
            Matrix m = Matrix.Identity(n, 4);
            Matrix coeffMatrix = m.FillDiagonal(0, 1, 1).FillDiagonal(1, 0, 1);
            coeffMatrix[0][0] = 2;
            coeffMatrix[n - 1][n - 1] = 7;
            coeffMatrix[n - 1][n - 2] = 2;

            // Build the vector points.
            List<Vector3> vecPts = (getsEndDerivatives) 
                ? Vector3.Zero2d(2, pts[0].Count)
                : Vector3.Zero2d(n, pts[0].Count);

            if (!getsEndDerivatives)
            {
                for (int i = 1; i < n - 1; i++)
                {
                    vecPts[i] = (pts[i] * 2 + pts[i + 1]) * 2;
                }
            }

            vecPts[0] = pts[0] + pts[1] * 2;
            vecPts[n - 1] = pts[n - 1] * 8 + pts[n];

            Matrix matrixLu = Matrix.Decompose(coeffMatrix, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            // Solve for each dimension.
            for (int i = 0; i < vecPts[0].Count; i++)
            {
                Vector3 b = new Vector3();
                b = vecPts.Select(pt => pt[i]).ToVector();

                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            List<Vector3> ctrlPtsA = ptsSolved.Transpose().Select(pt => pt.ToVector()).ToList();
            List<Vector3> ctrlPtsB = (getsEndDerivatives)
                ? Vector3.Zero2d(2, pts[0].Count)
                : Vector3.Zero2d(n, pts[0].Count);
            for (int i = 0; i < n - 1; i++)
            {
                ctrlPtsB[i] = pts[i + 1] * 2 - ctrlPtsA[i + 1];
            }

            ctrlPtsB[n - 1] = (ctrlPtsA[n - 1] + pts[n]) / 2;

            return (ctrlPtsA, ctrlPtsB);
        }
    }
}
