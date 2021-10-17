using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Analyze
{
    /// <summary>
    /// Contains methods for analyzing surfaces.
    /// </summary>
    internal static class Surface
    {
        /// <summary>
        /// Computes the closest parameters on a surface to a given point.<br/>
        /// <em>Corresponds to page 244 chapter six from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface object.</param>
        /// <param name="point">Point to search from.</param>
        /// <returns>The closest parameter on the surface.</returns>
        internal static (double u, double v) ClosestParameter(NurbsSurface surface, Point3 point)
        {
            double minimumDistance = double.PositiveInfinity;
            (double u, double v) selectedUV = (0.5, 0.5);
            NurbsSurface splitSrf = surface;
            double param = 0.5;
            int maxIterations = 5;

            for (int i = 0; i < maxIterations; i++)
            {
                NurbsSurface[] surfaces = splitSrf.SplitAt(0.5, SplitDirection.Both);
                Point3[] pts = surfaces.Select(s => s.PointAt(0.5, 0.5)).ToArray();
                double[] distanceBetweenPts = pts.Select(point.DistanceTo).ToArray();
                if (distanceBetweenPts.All(d => d > minimumDistance)) break;

                (double, double)[] srfUV = DefiningUV(selectedUV, param);

                for (int j = 0; j < distanceBetweenPts.Length; j++)
                {
                    if (!(distanceBetweenPts[j] < minimumDistance)) continue;
                    minimumDistance = distanceBetweenPts[j];
                    selectedUV = srfUV[j];
                    splitSrf = surfaces[j];
                }

                param *= 0.5;
            }

            int t = 0;
            // Two zero tolerances can be used to indicate convergence:
            double tol1 = GSharkMath.MaxTolerance; // a measure of Euclidean distance;
            double tol2 = 0.0005; // a zero cosine measure.
            double minU = surface.KnotsU[0];
            double maxU = surface.KnotsU.Last();
            double minV = surface.KnotsV[0];
            double maxV = surface.KnotsV.Last();
            bool closedDirectionU = surface.IsClosed(SurfaceDirection.U);
            bool closedDirectionV = surface.IsClosed(SurfaceDirection.V);

            // To avoid infinite loop we limited the interaction.
            while (t < maxIterations)
            {
                // Get derivatives.
                var eval = Evaluate.Surface.RationalDerivatives(surface, selectedUV.u, selectedUV.v, 2);

                // Convergence criteria:
                // First condition, point coincidence:
                // |S(u,v) - p| < e1
                Vector3 diff = eval[0, 0] - new Vector3(point);
                double c1v = diff.Length;
                bool c1 = c1v <= tol1;

                // Second condition, zero cosine:
                // |Su(u,v)*(S(u,v) - P)|
                // ----------------------  < e2
                // |Su(u,v)| |S(u,v) - P|
                //
                // |Sv(u,v)*(S(u,v) - P)|
                // ----------------------  < e2
                // |Sv(u,v)| |S(u,v) - P|
                double c2an = eval[1, 0] * diff;
                double c2ad = eval[1, 0].Length * c1v;

                double c2bn = eval[0, 1] * diff;
                double c2bd = eval[0, 1].Length * c1v;

                double c2av = c2an / c2ad;
                double c2bv = c2bn / c2bd;

                bool c2a = c2av <= tol2;
                bool c2b = c2bv <= tol2;

                // If all the criteria are satisfied we are done.
                if (c1 && c2a && c2b)
                {
                    return selectedUV;
                }

                // Otherwise a new value ( Ui + 1, Vi + 1) is computed using Eq. 6.7
                var ct = NewtonIteration(selectedUV, eval, diff);

                // Ensure the parameters stay in range (Ui+1 E [minU, maxU] and Vi+1 E [minV, maxV]).
                if (ct.u < minU)
                {
                    ct = (closedDirectionU) ? (maxU - (minU - ct.u), ct.v) : (minU, ct.v);
                }

                if (ct.u > maxU)
                {
                    ct = (closedDirectionU) ? (minU + (ct.u - maxU), ct.v) : (maxU, ct.v);
                }

                if (ct.v < minV)
                {
                    ct = (closedDirectionV) ? (ct.u, maxV - (minV - ct.v)) : (ct.u, minV);
                }

                if (ct.v > maxV)
                {
                    ct = (closedDirectionV) ? (ct.u, minV + (ct.v - maxV)) : (ct.u, maxV);
                }

                // Parameters do not change significantly.
                double c3u = (eval[1, 0] * (ct.u - selectedUV.u)).Length;
                double c3v = (eval[0, 1] * (ct.v - selectedUV.v)).Length;

                if (c3u + c3v < tol1)
                {
                    return selectedUV;
                }

                selectedUV = ct;
                t++;
            }

            return selectedUV;
        }

        /// <summary>
        /// Newton iteration to minimize the distance between a point and a surface.
        /// <em>Corresponds to Eq. 6.5 at page 232 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="uv">The parameter uv obtained at the ith Newton iteration.</param>
        /// <param name="derivatives">Derivatives of the surface identify as S(u,v).</param>
        /// <param name="r">Representing the difference from S(u,v) - P.</param>
        /// <returns>The minimized parameter.</returns>
        private static (double u, double v) NewtonIteration((double u, double v) uv, Vector3[,] derivatives, Vector3 r)
        {
            Vector3 Su = derivatives[1, 0];
            Vector3 Sv = derivatives[0, 1];

            Vector3 Suu = derivatives[2, 0];
            Vector3 Svv = derivatives[0, 2];

            Vector3 Suv = derivatives[1, 1];
            Vector3 Svu = derivatives[1, 1];

            double f = Su * r;
            double g = Sv * r;

            // Eq. 6.5
            Vector k = new Vector { -f, -g };

            Matrix J = new Matrix
            {
                new List<double> {Su * Su + Suu * r, Su * Sv + Suv * r},
                new List<double> {Su * Sv + Svu * r, Sv * Sv + Svv * r}
            };

            // Eq. 6.6
            Matrix matrixLu = Matrix.Decompose(J, out int[] permutation);
            Vector d = Matrix.Solve(matrixLu, permutation, k);

            // Eq. 6.7
            return (d[0] + uv.u, d[1] + uv.v);
        }

        /// <summary>
        /// Defines the U and V parameters for a surface split in both direction, subtracting or adding half of the input parameter based on the quadrant.
        /// </summary>
        private static (double u, double v)[] DefiningUV((double u, double v) surfaceUV, double parameter)
        {
            double halfParameter = parameter * 0.5;
            var UV = new (double u, double v)[4]
            {
                (surfaceUV.u + halfParameter, surfaceUV.v - halfParameter),
                (surfaceUV.u + halfParameter, surfaceUV.v + halfParameter),
                (surfaceUV.u - halfParameter, surfaceUV.v - halfParameter),
                (surfaceUV.u - halfParameter, surfaceUV.v + halfParameter)
            };

            return UV;
        }

        /// <summary>
        /// Extracts the isoparametric curves (isocurves) at the given parameter and surface direction.
        /// </summary>
        /// <param name="surface">The surface object to extract the isocurve.</param>
        /// <param name="parameter">The parameter between 0.0 to 1.0 whether the isocurve will be extracted.</param>
        /// <param name="direction">The U or V direction whether the isocurve will be extracted.</param>
        /// <returns>The isocurve extracted.</returns>
        internal static NurbsCurve Isocurve(NurbsSurface surface, double parameter, SurfaceDirection direction)
        {
            KnotVector knots = (direction == SurfaceDirection.V) ? surface.KnotsV : surface.KnotsU;
            int degree = (direction == SurfaceDirection.V) ? surface.DegreeV : surface.DegreeU;

            Dictionary<double, int> knotMultiplicity = knots.Multiplicities();
            // If the knotVector already exists in the array, don't make duplicates.
            double knotKey = -1;
            foreach (KeyValuePair<double, int> keyValuePair in knotMultiplicity)
            {
                if (!(Math.Abs(parameter - keyValuePair.Key) < GSharkMath.Epsilon)) continue;
                knotKey = keyValuePair.Key;
                break;
            }

            int knotToInsert = degree + 1;
            if (knotKey >= 0)
            {
                knotToInsert = knotToInsert - knotMultiplicity[knotKey];
            }

            // Insert knots
            NurbsSurface refinedSurface = surface;
            if (knotToInsert > 0)
            {
                List<double> knotsToInsert = CollectionHelpers.RepeatData(parameter, knotToInsert);
                refinedSurface = KnotVector.Refine(surface, knotsToInsert, direction);
            }

            // Obtain the correct index of control points to extract.
            int span = knots.Span(degree, parameter);

            if (Math.Abs(parameter - knots[0]) < GSharkMath.Epsilon)
            {
                span = 0;
            }
            if (Math.Abs(parameter - knots.Last()) < GSharkMath.Epsilon)
            {
                span = (direction == SurfaceDirection.V)
                    ? refinedSurface.ControlPoints[0].Count - 1
                    : refinedSurface.ControlPoints.Count - 1;
            }

            return direction == SurfaceDirection.V
                ? new NurbsCurve(refinedSurface.DegreeU, refinedSurface.KnotsU, CollectionHelpers.Transpose2DArray(refinedSurface.ControlPoints)[span])
                : new NurbsCurve(refinedSurface.DegreeV, refinedSurface.KnotsV, refinedSurface.ControlPoints[span]);
        }
    }
}