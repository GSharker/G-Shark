using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Provides functions for interpolating and approximating NURBS curves and surfaces from points.<br/>
    /// Approximation uses least squares algorithm.
    /// </summary>
    public static class Fitting
    {
        public static NurbsCurve ApproximateCurve(List<Point3d> pts, int degree, bool centripetal = false)
        {
            int numberCpts = pts.Count - 1;

            // Gets the parameters curve uk.
            List<double> uk = CurveParameters(pts, centripetal);

            // Computes knot vectors.
            KnotVector knots = ComputeKnotsForCurveApproximation(uk, degree, numberCpts, pts.Count);

            // Compute matrix N
            Matrix matrixN = new Matrix();
            for (int i = 1; i < pts.Count - 1; i++)
            {
                List<double> tempRow = new List<double>();
                for (int j = 1; j < numberCpts - 1; j++)
                {
                    tempRow.Add(Evaluation.OneBasisFunction(degree, knots, j, uk[i]));
                }
                matrixN.Add(tempRow);
            }

            // Compute NT matrix.
            Matrix matrixNt = matrixN.Transpose();
            // Compute NTN matrix.
            Matrix matrixNtN = matrixNt * matrixN;

            // Computes Rk - Eqn 9.63.
            List<Point3d> Rk = ComputesValuesRk(knots, uk, degree, pts, numberCpts);

            // Compute R - Eqn 9.67.
            var vectorR = ComputeValuesR(knots, uk, Rk, degree, numberCpts);

            // Computes control points, fixing the first and last point from the input points.
            List<Point3d> ctrlPts = new List<Point3d> { pts[0] };
            ctrlPts.AddRange(SolveCtrlPts(knots, vectorR, matrixNtN));
            ctrlPts.Add(pts[^1]);
            return new NurbsCurve(degree, knots, ctrlPts);
        }

        /// <summary>
        /// Creates a set of interpolated cubic beziers through a set of points.
        /// </summary>
        /// <param name="pts">Set of points to interpolate.</param>
        /// <returns>A set of cubic beziers.</returns>
        public static List<NurbsCurve> BezierInterpolation(List<Point3d> pts)
        {
            if (pts.Count == 0)
            {
                throw new Exception("Collection of points is empty.");
            }

            List<NurbsCurve> beziers = new List<NurbsCurve>();
            (List<Point3d> ptsA, List<Point3d> ptsB) ctrlPts = SolveBezierCtrlPts(pts);

            for (int i = 0; i < pts.Count - 1; i++)
            {
                beziers.Add(new NurbsCurve(new List<Point3d> { pts[i], ctrlPts.ptsA[i], ctrlPts.ptsB[i], pts[i + 1] },
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
        public static NurbsCurve InterpolatedCurve(List<Point3d> pts, int degree, Vector3d? startTangent = null,
            Vector3d? endTangent = null, bool centripetal = false)
        {
            if (pts.Count < degree + 1)
            {
                throw new Exception($"You must supply at least degree + 1 points. You supplied {pts.Count} pts.");
            }

            // Gets uk parameters.
            List<double> uk = CurveParameters(pts, centripetal);

            // Compute knot vectors.
            bool hasTangents = startTangent != null && endTangent != null;
            KnotVector knots = ComputeKnotsForInterpolation(uk, degree, hasTangents);

            // Global interpolation.
            // Build matrix of basis function coefficients.
            Matrix coeffMatrix = BuildCoefficientsMatrix(pts, degree, hasTangents, uk, knots);
            // Solve for each points.
            List<Point3d> ctrlPts = (hasTangents)
                ? SolveCtrlPtsWithTangents(knots, pts, coeffMatrix, degree, new Vector3d(startTangent.Value), new Vector3d(endTangent.Value))
                : SolveCtrlPts(knots, pts, coeffMatrix);

            return new NurbsCurve(degree, knots, ctrlPts);
        }

        /// <summary>
        /// Compute R - Eqn 9.67.
        /// </summary>
        private static List<Point3d> ComputeValuesR(KnotVector knots, List<double> curveParameters, List<Point3d> Rk, int degree, int numberOfCtrPts)
        {
            List<Vector3> vectorR = new List<Vector3>();
            for (int i = 1; i < numberOfCtrPts - 1; i++)
            {
                List<Vector3> ruTemp = new List<Vector3>();
                for (int j = 0; j < Rk.Count; j++)
                {
                    double tempBasisVal = Evaluation.OneBasisFunction(degree, knots, i, curveParameters[j + 1]);
                    ruTemp.Add(Rk[j] * tempBasisVal);
                }

                Vector3 tempVec = Vector3.Zero1d(ruTemp[0].Count);
                for (int g = 0; g < ruTemp[0].Count; g++)
                {
                    foreach (Vector3 vec in ruTemp)
                    {
                        tempVec[g] += vec[g];
                    }
                }

                vectorR.Add(tempVec);
            }

            return vectorR.Select(v => new Point3d(v[0], v[1], v[2])).ToList();
        }

        /// <summary>
        /// Computes Rk - Eqn 9.63.
        /// </summary>
        private static List<Point3d> ComputesValuesRk(KnotVector knots, List<double> curveParameters, int degree, List<Point3d> pts, int numberOfCtrPts)
        {
            Point3d pt0 = pts[0]; // Q0
            Point3d ptm = pts[^1]; // Qm
            List<Point3d> Rk = new List<Point3d>();
            for (int i = 1; i < pts.Count - 1; i++)
            {
                Point3d pti = pts[i];
                double n0p = Evaluation.OneBasisFunction(degree, knots, 0, curveParameters[i]);
                double nnp = Evaluation.OneBasisFunction(degree, knots, numberOfCtrPts - 1, curveParameters[i]);
                Point3d elem2 = pt0 * n0p;
                Point3d elem3 = ptm * nnp;

                Point3d tempVec = new Point3d();
                for (int j = 0; j < 3; j++)
                {
                    tempVec[j] = (pti[j] - elem2[j] - elem3[j]);
                }

                tempVec.X = pti.X - elem2.X - elem3.X;
                tempVec.Y = pti.Y - elem2.Y - elem3.Y;
                tempVec.Z = pti.Z - elem2.Z - elem3.Z;

                Rk.Add(tempVec);
            }

            return Rk;
        }

        /// <summary>
        /// Computes the knot vectors used to calculate a curve global interpolation.
        /// </summary>
        private static KnotVector ComputeKnotsForInterpolation(List<double> curveParameters, int degree, bool hasTangents)
        {
            // Start knot vectors.
            KnotVector knots = Sets.RepeatData(0.0, degree + 1).ToKnot();

            // If we have tangent values we need two more control points and knots.
            int start = (hasTangents) ? 0 : 1;
            int end = (hasTangents) ? curveParameters.Count - degree + 1 : curveParameters.Count - degree;

            // Use averaging method (Eqn 9.8) to compute internal knots in the knot vector.
            for (int i = start; i < end; i++)
            {
                double weightSum = 0.0;
                for (int j = 0; j < degree; j++)
                {
                    weightSum += curveParameters[i + j];
                }

                knots.Add((1.0 / degree) * weightSum);
            }

            // Add end knot vectors.
            knots.AddRange(Sets.RepeatData(1.0, degree + 1));
            return knots;
        }

        /// <summary>
        /// Computes the knots vectors used to calculate an approximate curve.
        /// </summary>
        private static KnotVector ComputeKnotsForCurveApproximation(List<double> curveParameters, int degree, int numberOfCtrPts, int numberOfPts)
        {
            // Start knot vectors.
            KnotVector knots = Sets.RepeatData(0.0, degree + 1).ToKnot();

            // Compute 'd' value - Eqn 9.68.
            double d = (double)numberOfPts / (numberOfCtrPts - degree);

            // Find the internal knots - Eqn 9.69.
            for (int j = 1; j < numberOfCtrPts - degree; j++)
            {
                int i = (int)(j * d);
                double alpha = (j * d) - i;
                double knot = ((1.0 - alpha) * curveParameters[i - 1]) + (alpha * curveParameters[i]);
                knots.Add(knot);
            }

            // Add end knot vectors.
            knots.AddRange(Sets.RepeatData(1.0, degree + 1));
            return knots;
        }

        /// <summary>
        /// Defines the control points.
        /// </summary>
        private static List<Point3d> SolveCtrlPts(KnotVector knots, List<Point3d> pts, Matrix coeffMatrix)
        {
            Matrix matrixLu = Matrix.Decompose(coeffMatrix, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            // Solve for each dimension.
            for (int i = 0; i < pts[0].Size; i++)
            {
                Vector3 b = new Vector3();
                b = pts.Select(pt => pt[i]).ToVector();
                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }
            return ptsSolved.Transpose().Select(pt => new Point3d(pt[0], pt[1], pt[2])).ToList();
        }

        /// <summary>
        /// Defines the control points defining the tangent values for the first and last points.
        /// </summary>
        private static List<Point3d> SolveCtrlPtsWithTangents(KnotVector knots, List<Point3d> pts, Matrix coeffMatrix, int degree, Vector3d startTangent, Vector3d endTangent)
        {
            Matrix matrixLu = Matrix.Decompose(coeffMatrix, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            // Equations 9.11
            double mult0 = knots[degree + 1] / degree;
            // Equations 9.12
            double mult1 = (1 - knots[knots.Count - degree - 2]) / degree;

            // Solve for each dimension.
            for (int i = 0; i < 3; i++)
            {
                Vector3 b = new Vector3();
                // Insert the tangents at the second and second to last index.
                b.Add(pts[0][i]);
                // Equations 9.11
                b.Add(startTangent[i] * mult0);
                b.AddRange(pts.Skip(1).Take(pts.Count - 2).Select(pt => pt[i]));
                // Equations 9.12
                b.Add(endTangent[i] * mult1);
                b.Add(pts[^1][i]);
                
                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            return ptsSolved.Transpose().Select(pt => new Point3d(pt[0], pt[1], pt[2])).ToList();
        }

        /// <summary>
        /// Builds the coefficient matrix used to calculate a curve global interpolation.
        /// </summary>
        internal static Matrix BuildCoefficientsMatrix(List<Point3d> pts, int degree, bool hasTangents, List<double> curveParameters, KnotVector knots)
        {
            int dim = (hasTangents) ? pts.Count + 1 : pts.Count - 1;
            Matrix coeffMatrix = new Matrix();

            int dimEnd = (hasTangents) ? pts.Count - (degree - 1) : pts.Count - (degree + 1);

            foreach (double u in curveParameters)
            {
                int span = knots.Span(dim, degree, u);
                List<double> basicFunction = Evaluation.BasisFunction(degree, knots, span, u);

                List<double> startRow = Sets.RepeatData(0.0, span - degree);
                List<double> endRow = Sets.RepeatData(0.0, dimEnd - (span - degree));

                coeffMatrix.Add(startRow.Concat(basicFunction).Concat(endRow).ToList());
            }

            if (!hasTangents) return coeffMatrix;

            List<double> zeros = Sets.RepeatData(0.0, coeffMatrix[0].Count - 2);
            List<double> tangent = new List<double> { -1.0, 1.0 };

            coeffMatrix.Insert(1, tangent.Concat(zeros).ToList());
            coeffMatrix.Insert(coeffMatrix.Count - 1, zeros.Concat(tangent).ToList());

            return coeffMatrix;
        }

        /// <summary>
        /// Refer to the Equations 9.4 and 9.5 for chord length parametrization, and Equation 9.6 for centripetal method
        /// on The NURBS Book(2nd Edition), pp.364-365.
        /// </summary>
        internal static List<double> CurveParameters(List<Point3d> pts, bool centripetal = false)
        {
            List<double> chords = new List<double> { 0.0 };
            for (int i = 1; i < pts.Count; i++)
            {
                double chord = (centripetal) ? Math.Sqrt((pts[i] - pts[i - 1]).Length) : (pts[i] - pts[i - 1]).Length;
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

        /// <summary>
        /// Solves finding the control points of a Bezier.
        /// </summary>
        private static (List<Point3d> ptsA, List<Point3d> ptsB) SolveBezierCtrlPts(List<Point3d> pts, bool getsEndDerivatives = false)
        {
            int n = pts.Count - 1;

            // Build the coefficient matrix.
            Matrix m = Matrix.Identity(n, 4);
            Matrix coeffMatrix = m.FillDiagonal(0, 1, 1).FillDiagonal(1, 0, 1);
            coeffMatrix[0][0] = 2;
            coeffMatrix[n - 1][n - 1] = 7;
            coeffMatrix[n - 1][n - 2] = 2;

            // Build the vector points.
            List<Vector3d> vecPts = (getsEndDerivatives) ? Enumerable.Repeat(new Vector3d(), 2).ToList() : Enumerable.Repeat(new Vector3d(), n).ToList();

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
            for (int i = 0; i < vecPts[0].Size; i++)
            {
                Vector3 b = new Vector3();
                b = vecPts.Select(pt => pt[i]).ToVector();

                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            List<Point3d> ctrlPtsA = ptsSolved.Transpose().Select(pt => new Point3d(pt[0], pt[1], pt[2])).ToList();
            List<Point3d> ctrlPtsB = (getsEndDerivatives) ? Enumerable.Repeat(new Point3d(), 2).ToList() : Enumerable.Repeat(new Point3d(), n).ToList();

            for (int i = 0; i < n - 1; i++)
            {
                ctrlPtsB[i] = pts[i + 1] * 2 - ctrlPtsA[i + 1];
            }

            ctrlPtsB[n - 1] = (ctrlPtsA[n - 1] + pts[n]) / 2;

            return (ctrlPtsA, ctrlPtsB);
        }
    }
}
