﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using Microsoft.VisualBasic;

namespace GeometrySharp.Operation
{
    /// <summary>
    /// Fitting provides functions for interpolating and approximating nurbs curves and surfaces from points.
    /// Approximation uses least squares algorithm.
    /// </summary>
    public static class Fitting
    {
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
        /// Define the control points.
        /// </summary>
        internal static List<Vector3> SolveCtrlPts(List<Vector3> pts, int degree, Vector3 startTangent, Vector3 endTangent, Matrix coeffMatrix,
            Knot knots, bool hasTangents)
        {
            Matrix matrixLu = Matrix.Decompose(coeffMatrix, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            double mult0 = knots[degree + 1] / degree;
            double mult1 = (1 - knots[knots.Count - degree - 2]) / degree;

            // Solve for each dimension.
            for (int i = 0; i < pts[0].Count; i++)
            {
                Vector3 b = Vector3.Unset;
                if (!hasTangents)
                {
                    b = pts.Select(pt => pt[i]).ToVector();
                }
                else
                {
                    // Insert the tangents at the second and second to last index.
                    b.Add(pts[0][i]);
                    b.Add(startTangent[i] * mult0);
                    b.AddRange(pts.Skip(1).Take(pts.Count - 2).Select(pt => pt[i]));
                    b.Add(endTangent[i] * mult1);
                    b.Add(pts.Last()[i]);
                }

                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            return ptsSolved.Transpose().Select(pt => pt.ToVector()).ToList();
        }


        public static NurbsCurve ApproximateCurve(List<Vector3> pts, int degree)
        {
            return new NurbsCurve();
        }

        /// <summary>
        /// Builds the coefficient matrix for global interpolation.
        /// </summary>
        internal static Matrix BuildCoefficientsMatrix(List<Vector3> pts, int degree, bool hasTangents, List<double> uk, Knot knots)
        {
            int dim = (hasTangents) ? pts.Count + 1 : pts.Count - 1;
            Matrix coeffMatrix = new Matrix();

            int dimEnd = (hasTangents) ? pts.Count - (degree - 1) : pts.Count - (degree + 1);

            foreach (var u in uk)
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
        /// Please refer to the Equations 9.4 and 9.5 for chord length parametrization, and Equation 9.6 for centripetal method
        /// on The NURBS Book(2nd Edition), pp.364-365.
        /// </summary>
        internal static List<double> ParametersCurve(List<Vector3> pts, bool centripetal = false)
        {
            List<double> chords = new List<double> {0.0};
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
    }
}
