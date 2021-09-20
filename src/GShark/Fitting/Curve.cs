using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Fitting
{
    /// <summary>
    /// Provides functions for interpolating and approximating NURBS curves from points.<br/>
    /// Approximation uses least squares algorithm.
    /// </summary>
    public static class Curve
    {
        public static NurbsBase Approximate(List<Point3> pts, int degree, bool centripetal = false)
        {
            int numberCpts = pts.Count - 1;

            // Gets the parameters curve uk.
            List<double> uk = CurveHelpers.Parametrization(pts, centripetal);

            // Computes knot vectors.
            KnotVector knots = ComputeKnotsForCurveApproximation(uk, degree, numberCpts, pts.Count);

            // Compute matrix N
            Matrix matrixN = new Matrix();
            for (int i = 1; i < pts.Count - 1; i++)
            {
                List<double> tempRow = new List<double>();
                for (int j = 1; j < numberCpts - 1; j++)
                {
                    tempRow.Add(Evaluate.Curve.OneBasisFunction(degree, knots, j, uk[i]));
                }
                matrixN.Add(tempRow);
            }

            // Compute NT matrix.
            Matrix matrixNt = matrixN.Transpose();
            // Compute NTN matrix.
            Matrix matrixNtN = matrixNt * matrixN;

            // Computes Rk - Eqn 9.63.
            List<Point3> Rk = ComputesValuesRk(knots, uk, degree, pts, numberCpts);

            // Compute R - Eqn 9.67.
            var vectorR = ComputeValuesR(knots, uk, Rk, degree, numberCpts);

            // Computes control points, fixing the first and last point from the input points.
            List<Point4> ctrlPts = new List<Point4> { pts[0] };
            ctrlPts.AddRange(SolveCtrlPts(vectorR, matrixNtN));
            ctrlPts.Add(pts[pts.Count - 1]);

            return new NurbsCurve(degree, knots, ctrlPts);
        }

        /// <summary>
        /// Creates a set of interpolated cubic beziers through a set of points.
        /// </summary>
        /// <param name="pts">Set of points to interpolate.</param>
        /// <returns>A set of cubic beziers.</returns>
        public static List<NurbsBase> InterpolateBezier(List<Point3> pts)
        {
            if (pts.Count == 0)
            {
                throw new Exception("Collection of points is empty.");
            }

            List<NurbsBase> beziers = new List<NurbsBase>();
            var (ptsA, ptsB) = SolveBezierCtrlPts(pts);

            for (int i = 0; i < pts.Count - 1; i++)
            {
                beziers.Add(new NurbsCurve(new List<Point3> { pts[i], ptsA[i], ptsB[i], pts[i + 1] },
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
        public static NurbsBase Interpolated(List<Point3> pts, int degree, Vector3? startTangent = null,
            Vector3? endTangent = null, bool centripetal = false)
        {
            if (pts.Count < degree + 1)
            {
                throw new Exception($"You must supply at least degree + 1 points. You supplied {pts.Count} pts.");
            }

            // Gets uk parameters.
            List<double> uk = CurveHelpers.Parametrization(pts, centripetal);

            // Compute knot vectors.
            bool hasTangents = startTangent != null && endTangent != null;
            KnotVector knots = ComputeKnotsForInterpolation(uk, degree, hasTangents);

            // Global interpolation.
            // Build matrix of basis function coefficients.
            Matrix coeffMatrix = BuildCoefficientsMatrix(pts, degree, hasTangents, uk, knots);
            // Solve for each points.
            List<Point4> ctrlPts = (hasTangents)
                ? SolveCtrlPtsWithTangents(knots, pts, coeffMatrix, degree, new Vector3(startTangent), new Vector3(endTangent))
                : SolveCtrlPts(pts, coeffMatrix);

            return new NurbsCurve(degree, knots, ctrlPts);
        }

        /// <summary>
        /// Compute R - Eqn 9.67.
        /// </summary>
        private static List<Point3> ComputeValuesR(KnotVector knots, List<double> curveParameters, List<Point3> Rk, int degree, int numberOfCtrPts)
        {
            List<Vector> vectorR = new List<Vector>();
            for (int i = 1; i < numberOfCtrPts - 1; i++)
            {
                List<Vector> ruTemp = new List<Vector>();
                for (int j = 0; j < Rk.Count; j++)
                {
                    double tempBasisVal = Evaluate.Curve.OneBasisFunction(degree, knots, i, curveParameters[j + 1]);
                    ruTemp.Add(Rk[j] * tempBasisVal);
                }

                Vector tempVec = Vector.Zero1d(ruTemp[0].Count);
                for (int g = 0; g < ruTemp[0].Count; g++)
                {
                    foreach (Vector vec in ruTemp)
                    {
                        tempVec[g] += vec[g];
                    }
                }

                vectorR.Add(tempVec);
            }

            return vectorR.Select(v => new Point3(v[0], v[1], v[2])).ToList();
        }

        /// <summary>
        /// Computes Rk - Eqn 9.63.
        /// </summary>
        private static List<Point3> ComputesValuesRk(KnotVector knots, List<double> curveParameters, int degree, List<Point3> pts, int numberOfCtrPts)
        {
            Point3 pt0 = pts[0]; // Q0
            Point3 ptm = pts[pts.Count - 1]; // Qm
            List<Point3> Rk = new List<Point3>();
            for (int i = 1; i < pts.Count - 1; i++)
            {
                Point3 pti = pts[i];
                double n0p = Evaluate.Curve.OneBasisFunction(degree, knots, 0, curveParameters[i]);
                double nnp = Evaluate.Curve.OneBasisFunction(degree, knots, numberOfCtrPts - 1, curveParameters[i]);
                Point3 elem2 = pt0 * n0p;
                Point3 elem3 = ptm * nnp;

                Point3 tempVec = new Point3();
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
            KnotVector knots = CollectionHelpers.RepeatData(0.0, degree + 1).ToKnot();

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
            knots.AddRange(CollectionHelpers.RepeatData(1.0, degree + 1));
            return knots;
        }

        /// <summary>
        /// Computes the knots vectors used to calculate an approximate curve.
        /// </summary>
        private static KnotVector ComputeKnotsForCurveApproximation(List<double> curveParameters, int degree, int numberOfCtrPts, int numberOfPts)
        {
            // Start knot vectors.
            KnotVector knots = CollectionHelpers.RepeatData(0.0, degree + 1).ToKnot();

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
            knots.AddRange(CollectionHelpers.RepeatData(1.0, degree + 1));
            return knots;
        }

        /// <summary>
        /// Defines the control points.
        /// </summary>
        private static List<Point4> SolveCtrlPts(List<Point3> pts, Matrix coeffMatrix)
        {
            Matrix matrixLu = Matrix.Decompose(coeffMatrix, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            // Solve for each dimension.
            for (int i = 0; i < pts[0].Size; i++)
            {
                Vector b = new Vector();
                b = pts.Select(pt => pt[i]).ToVector();
                Vector solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }
            return ptsSolved.Transpose().Select(pt => new Point4(pt[0], pt[1], pt[2], 1)).ToList();
        }

        /// <summary>
        /// Defines the control points defining the tangent values for the first and last points.
        /// </summary>
        private static List<Point4> SolveCtrlPtsWithTangents(KnotVector knots, List<Point3> pts, Matrix coeffMatrix, int degree, Vector3 startTangent, Vector3 endTangent)
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
                Vector b = new Vector { pts[0][i], startTangent[i] * mult0 };
                // Insert the tangents at the second and second to last index.
                // Equations 9.11
                b.AddRange(pts.Skip(1).Take(pts.Count - 2).Select(pt => pt[i]));
                // Equations 9.12
                b.Add(endTangent[i] * mult1);
                b.Add(pts[pts.Count - 1][i]);

                Vector solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            return ptsSolved.Transpose().Select(pt => new Point4(pt[0], pt[1], pt[2], 1)).ToList();
        }

        /// <summary>
        /// Builds the coefficient matrix used to calculate a curve global interpolation.
        /// </summary>
        private static Matrix BuildCoefficientsMatrix(List<Point3> pts, int degree, bool hasTangents, List<double> curveParameters, KnotVector knots)
        {
            int dim = (hasTangents) ? pts.Count + 1 : pts.Count - 1;
            Matrix coeffMatrix = new Matrix();

            int dimEnd = (hasTangents) ? pts.Count - (degree - 1) : pts.Count - (degree + 1);

            foreach (double u in curveParameters)
            {
                int span = knots.Span(dim, degree, u);
                List<double> basicFunction = Evaluate.Curve.BasisFunction(degree, knots, span, u);

                List<double> startRow = CollectionHelpers.RepeatData(0.0, span - degree);
                List<double> endRow = CollectionHelpers.RepeatData(0.0, dimEnd - (span - degree));

                coeffMatrix.Add(startRow.Concat(basicFunction).Concat(endRow).ToList());
            }

            if (!hasTangents) return coeffMatrix;

            List<double> zeros = CollectionHelpers.RepeatData(0.0, coeffMatrix[0].Count - 2);
            List<double> tangent = new List<double> { -1.0, 1.0 };

            coeffMatrix.Insert(1, tangent.Concat(zeros).ToList());
            coeffMatrix.Insert(coeffMatrix.Count - 1, zeros.Concat(tangent).ToList());

            return coeffMatrix;
        }

        /// <summary>
        /// Solves finding the control points of a Bezier.
        /// </summary>
        private static (List<Point3> ptsA, List<Point3> ptsB) SolveBezierCtrlPts(List<Point3> pts, bool getsEndDerivatives = false)
        {
            int n = pts.Count - 1;

            // Build the coefficient matrix.
            Matrix m = Matrix.Identity(n, 4);
            Matrix coeffMatrix = m.FillDiagonal(0, 1, 1).FillDiagonal(1, 0, 1);
            coeffMatrix[0][0] = 2;
            coeffMatrix[n - 1][n - 1] = 7;
            coeffMatrix[n - 1][n - 2] = 2;

            // Build the vector points.
            List<Vector3> vecPts = (getsEndDerivatives) ? Enumerable.Repeat(new Vector3(), 2).ToList() : Enumerable.Repeat(new Vector3(), n).ToList();

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
                Vector b = new Vector();
                b = vecPts.Select(pt => pt[i]).ToVector();

                Vector solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            List<Point3> ctrlPtsA = ptsSolved.Transpose().Select(pt => new Point3(pt[0], pt[1], pt[2])).ToList();
            List<Point3> ctrlPtsB = (getsEndDerivatives) ? Enumerable.Repeat(new Point3(), 2).ToList() : Enumerable.Repeat(new Point3(), n).ToList();

            for (int i = 0; i < n - 1; i++)
            {
                ctrlPtsB[i] = pts[i + 1] * 2 - ctrlPtsA[i + 1];
            }

            ctrlPtsB[n - 1] = (ctrlPtsA[n - 1] + pts[n]) / 2;

            return (ctrlPtsA, ctrlPtsB);
        }
    }
}
