using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;

namespace GShark.Evaluate
{
    /// <summary>
    /// Provides all of the core algorithms for evaluating points and derivatives on surfaces.
    /// </summary>
    internal class Surface
    {
        /// <summary>
        /// Computes the derivatives at the given U and V parameters on a NURBS surface.<br/>
        /// <para>Returns a two dimensional array containing the derivative vectors.<br/>
        /// Increasing U partial derivatives are increasing row-wise.Increasing V partial derivatives are increasing column-wise.<br/>
        /// Therefore, the[0,0] position is a point on the surface, [n,0] is the nth V partial derivative, the[n,n] position is twist vector or mixed partial derivative UV.</para>
        /// <em>Corresponds to algorithm 4.4 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="u">The u parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The v parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">Number of derivatives to evaluate, set as default to 1.</param>
        /// <returns>The derivatives.</returns>
        internal static Vector3[,] RationalDerivatives(NurbsSurface surface, double u, double v, int numDerivs = 1)
        {
            if (u < 0.0 || u > 1.0)
            {
                throw new ArgumentException("The U parameter is not into the domain 0.0 to 1.0.");
            }

            if (v < 0.0 || v > 1.0)
            {
                throw new ArgumentException("The V parameter is not into the domain 0.0 to 1.0.");
            }

            var derivatives = Derivatives(surface, u, v, numDerivs);
            Vector3[,] SKL = new Vector3[numDerivs + 1, numDerivs + 1];

            for (int k = 0; k < numDerivs + 1; k++)
            {
                for (int l = 0; l < numDerivs - k + 1; l++)
                {
                    Vector3 t = derivatives.Item1[k, l];
                    for (int j = 1; j < l + 1; j++)
                    {
                        t -= SKL[k, l - j] * (GSharkMath.GetBinomial(l, j) * derivatives.Item2[0, j]);
                    }

                    for (int i = 1; i < k + 1; i++)
                    {
                        t -= SKL[k - i, l] * (GSharkMath.GetBinomial(k, i) * derivatives.Item2[i, 0]);
                        Vector3 t2 = Vector3.Zero;
                        for (int j = 1; j < l + 1; j++)
                        {
                            t2 += SKL[k - i, l - j] * (GSharkMath.GetBinomial(l, j) * derivatives.Item2[i, j]);
                        }

                        t -= t2 * GSharkMath.GetBinomial(k, i);
                    }
                    SKL[k, l] = t / derivatives.Item2[0, 0];
                }
            }

            return SKL;
        }

        /// <summary>
        /// Computes the derivatives on a non-uniform, non-rational B-spline surface.<br/>
        /// SKL is the derivative S(u,v) with respect to U K-times and V L-times.<br/>
        /// <em>Corresponds to algorithm 3.6 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="u">The U parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The V parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">Number of derivatives to evaluate, set as default to 1.</param>
        /// <returns>A tuple with 2D collection representing all derivatives(k,L) and weights(k,l): U derivatives increase by row, V by column.</returns>
        internal static Tuple<Vector3[,], double[,]> Derivatives(NurbsSurface surface, double u, double v, int numDerivs = 1)
        {
            // number of basis function.
            int n = surface.KnotsU.Count - surface.DegreeU - 2;
            int m = surface.KnotsV.Count - surface.DegreeV - 2;

            // number of derivatives.
            int du = Math.Min(numDerivs, surface.DegreeU);
            int dv = Math.Min(numDerivs, surface.DegreeV);

            int knotSpanU = surface.KnotsU.Span(n, surface.DegreeU, u);
            int knotSpanV = surface.KnotsV.Span(m, surface.DegreeV, v);

            List<Vector> uDerivs = Curve.DerivativeBasisFunctionsGivenNI(knotSpanU, u, surface.DegreeU, n, surface.KnotsU);
            List<Vector> vDerivs = Curve.DerivativeBasisFunctionsGivenNI(knotSpanV, v, surface.DegreeV, m, surface.KnotsV);

            int dim = surface.ControlPoints[0][0].Size;
            List<List<Vector>> SKLw = Vector.Zero3d(numDerivs + 1, numDerivs + 1, dim);
            List<Vector> temp = Vector.Zero2d(surface.DegreeV + 1, dim);
            Vector3[,] SKL = new Vector3[numDerivs + 1, numDerivs + 1];
            double[,] weights = new double[numDerivs + 1, numDerivs + 1];

            for (int k = 0; k < du + 1; k++)
            {
                for (int s = 0; s < surface.DegreeV + 1; s++)
                {
                    temp[s] = Vector.Zero1d(dim);
                    for (int r = 0; r < surface.DegreeU + 1; r++)
                    {
                        Vector.AddMulMutate(temp[s], uDerivs[k][r], surface.ControlPoints[knotSpanU - surface.DegreeU + r][knotSpanV - surface.DegreeV + s]);
                    }
                }

                int dd = Math.Min(numDerivs - k, dv);

                for (int l = 0; l < dd + 1; l++)
                {
                    SKLw[k][l] = Vector.Zero1d(dim);
                    for (int s = 0; s < surface.DegreeV + 1; s++)
                    {
                        Vector.AddMulMutate(SKLw[k][l], vDerivs[l][s], temp[s]);
                    }

                    SKL[k, l] = new Vector3(SKLw[k][l][0], SKLw[k][l][1], SKLw[k][l][2]); // Extracting the derivatives.
                    weights[k, l] = SKLw[k][l][3]; // Extracting the weights.
                }
            }

            return new Tuple<Vector3[,], double[,]>(SKL, weights);
        }

        /// <summary>
        /// Computes a point on a non-uniform, non-rational B-spline surface.<br/>
        /// The U and V parameters have to be between 0.0 to 1.0.<br/>
        /// <em>Corresponds to algorithm 3.5 from The NURBS book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The NURBS surface.</param>
        /// <param name="u">The U parameter on the surface at which the point is to be evaluated.</param>
        /// <param name="v">The V parameter on the surface at which the point is to be evaluated.</param>
        /// <returns>The evaluated point.</returns>
        internal static Point4 PointAt(NurbsSurface surface, double u, double v)
        {
            if (u < 0.0 || u > 1.0)
            {
                throw new ArgumentException("The U parameter is not into the domain 0.0 to 1.0.");
            }

            if (v < 0.0 || v > 1.0)
            {
                throw new ArgumentException("The V parameter is not into the domain 0.0 to 1.0.");
            }

            int n = surface.KnotsU.Count - surface.DegreeU - 2;
            int m = surface.KnotsV.Count - surface.DegreeV - 2;
            int knotSpanU = surface.KnotsU.Span(n, surface.DegreeU, u);
            int knotSpanV = surface.KnotsV.Span(m, surface.DegreeV, v);
            List<double> basisUValue = Curve.BasisFunction(surface.DegreeU, surface.KnotsU, knotSpanU, u);
            List<double> basisVValue = Curve.BasisFunction(surface.DegreeV, surface.KnotsV, knotSpanV, v);
            int uIndex = knotSpanU - surface.DegreeU;
            Point4 evaluatedPt = Point4.Zero;

            for (int l = 0; l < surface.DegreeV + 1; l++)
            {
                var temp = Point4.Zero;
                var vIndex = knotSpanV - surface.DegreeV + l;
                for (int k = 0; k < surface.DegreeU + 1; k++)
                {
                    temp.X += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].X;
                    temp.Y += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].Y;
                    temp.Z += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].Z;
                    temp.W += basisUValue[k] * surface.ControlPoints[uIndex + k][vIndex].W;
                }

                evaluatedPt.X += basisVValue[l] * temp.X;
                evaluatedPt.Y += basisVValue[l] * temp.Y;
                evaluatedPt.Z += basisVValue[l] * temp.Z;
                evaluatedPt.W += basisVValue[l] * temp.W;
            }
            return evaluatedPt;
        }
    }
}