using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using GShark.Operation.Utilities;

namespace GShark.Operation
{
    /// <summary>
    /// Provides all of the core algorithms for evaluating points and derivatives on nurbs curves and surfaces.<br/>
    /// Many of these algorithms owe their implementation to The NURBS Book by Piegl and Tiller.
    /// </summary>
    public class Evaluation
    {
        /// <summary>
        /// Computes the non-vanishing basis functions.<br/>
        /// <em>Implementation of Algorithm A2.2 from The NURBS Book by Piegl and Tiller.<br/>
        /// Uses recurrence to compute the basis functions, also known as Cox - deBoor recursion formula.</em>
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="knot">knot value.</param>
        /// <returns>List of non-vanishing basis functions.</returns>
        public static List<double> BasicFunction(int degree, Knot knots, double knot)
        {
            int span = knots.Span(degree, knot);
            return BasicFunction(degree, knots, span, knot);
        }

        /// <summary>
        /// Computes the non-vanishing basis functions.<br/>
        /// <em>Implementation of Algorithm A2.2 from The NURBS Book by Piegl and Tiller.<br/>
        /// Uses recurrence to compute the basis functions, also known as Cox - deBoor recursion formula.</em>
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="span">Index span of knots.</param>
        /// <param name="knot">knot value.</param>
        /// <returns>List of non-vanishing basis functions.</returns>
        public static List<double> BasicFunction(int degree, Knot knots, int span, double knot)
        {
            Vector3 left = Vector3.Zero1d(degree + 1);
            Vector3 right = Vector3.Zero1d(degree + 1);
            // N[0] = 1.0 by definition;
            Vector3 N = Vector3.Zero1d(degree + 1);
            N[0] = 1.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = knot - knots[span + 1 - j];
                right[j] = knots[span + j] - knot;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    double temp = N[r] / (right[r + 1] + left[j - r]);
                    N[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                N[j] = saved;
            }

            return N;
        }

        /// <summary>
        /// Computes the value of a basis function for a single value.<br/>
        /// <em>Implementation of Algorithm A2.4 from The NURBS Book by Piegl and Tiller.</em><br/>
        /// </summary>
        /// <param name="degree">Degree of a curve.</param>
        /// <param name="knots">Set of knots.</param>
        /// <param name="span">Index span of knots.</param>
        /// <param name="knot">knot value.</param>
        /// <returns>The single parameter value of the basis function.</returns>
        public static double OneBasicFunction(int degree, Knot knots, int span, double knot)
        {
            // Special case at boundaries.
            if ((span == 0 && Math.Abs(knot - knots[0]) < GeoSharpMath.MAXTOLERANCE) ||
                (span == knots.Count - degree - 2) && Math.Abs(knot - knots[^1]) < GeoSharpMath.MAXTOLERANCE)
            {
                return 1.0;
            }
            // Local property, parameter is outside of span range.
            if (knot < knots[span] || knot >= knots[span + degree + 1])
            {
                return 0.0;
            }

            List<double> N = Sets.RepeatData(0.0, degree + span + 1);
            // Initialize the zeroth degree basic functions.
            for (int j = 0; j < degree + 1; j++)
            {
                if (knot >= knots[span + j] && knot < knots[span + j + 1])
                {
                    N[j] = 1.0;
                }
            }

            // Compute triangular table of basic functions.
            for (int k = 1; k < degree + 1; k++)
            {
                double saved = 0.0;
                if (N[0] != 0.0)
                {
                    saved = ((knot - knots[span]) * N[0]) / (knots[span + k] - knots[span]);
                }

                for (int j = 0; j < degree - k + 1; j++)
                {
                    double uLeft = knots[span + j + 1];
                    double uRight = knots[span + j + k + 1];

                    if (N[j + 1] == 0.0)
                    {
                        N[j] = saved;
                        saved = 0.0;
                    }
                    else
                    {
                        double temp = N[j + 1] / (uRight - uLeft);
                        N[j] = saved + (uRight - knot) * temp;
                        saved = (knot - uLeft) * temp;
                    }
                }
            }

            return N[0];
        }

        /// <summary>
        /// Computes a point on a non-uniform, non-rational b-spline curve.<br/>
        /// <em>Corresponds to algorithm 3.1 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="t">Parameter on the curve at which the point is to be evaluated</param>
        /// <returns>The evaluated point.</returns>
        public static Vector3 CurvePointAt(ICurve curve, double t)
        {
            int degree = curve.Degree;
            List<Vector3> curveHomogenizedPoints = curve.HomogenizedPoints;
            Knot knots = curve.Knots;

            if (!curve.Knots.AreValidKnots(degree, curveHomogenizedPoints.Count))
            {
                throw new ArgumentException("Invalid relations between control points, knot");
            }

            int n = knots.Count - degree - 2;

            int knotSpan = knots.Span(n, degree, t);
            List<double> basisValue = BasicFunction(degree, knots, knotSpan, t);
            Vector3 position = Vector3.Zero1d(curveHomogenizedPoints[0].Count);

            for (int i = 0; i < degree + 1; i++)
            {
                double valToMultiply = basisValue[i];
                Vector3 pt = curveHomogenizedPoints[knotSpan - degree + i];
                for (int j = 0; j < position.Count; j++)
                {
                    position[j] = position[j] + valToMultiply * pt[j];
                }
            }

            return position;
        }

        /// <summary>
        /// Computes a point on a non-uniform, non-rational B spline surface.<br/>
        /// <em>Corresponds to algorithm 3.5 from The NURBS book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="u">U parameter on the surface at which the point is to be evaluated</param>
        /// <param name="v">V parameter on the surface at which the point is to be evaluated</param>
        /// <returns>The evaluated point.</returns>
        public static Vector3 SurfacePointAt(NurbsSurface surface, double u, double v)
        {
            int n = surface.KnotsU.Count - surface.DegreeU - 2;
            int m = surface.KnotsV.Count - surface.DegreeV - 2;
            List<List<Vector3>> controlPoints = surface.ControlPoints;
            List<List<Vector3>> surfaceHomoPts = surface.HomogenizedPoints;
            int dim = controlPoints[0][0].Count;

            if (!surface.KnotsU.AreValidKnots(surface.DegreeU, surfaceHomoPts.Count))
            {
                throw new ArgumentException("Invalid relations between control points, knot in u direction");
            }

            if (!surface.KnotsV.AreValidKnots(surface.DegreeV, surfaceHomoPts[0].Count))
            {
                throw new ArgumentException("Invalid relations between control points, knot in v direction");
            }

            int knotSpanU = surface.KnotsU.Span(n, surface.DegreeU, u);
            int knotSpanV = surface.KnotsV.Span(m, surface.DegreeV, v);
            List<double> basisUValue = BasicFunction(surface.DegreeU, surface.KnotsU, knotSpanU, u);
            List<double> basisVValue = BasicFunction(surface.DegreeV, surface.KnotsV, knotSpanV, v);
            int uIndex = knotSpanU - surface.DegreeU;
            Vector3 position = Vector3.Zero1d(dim);
            for (int l = 0; l < surface.DegreeV + 1; l++)
            {
                var temp = Vector3.Zero1d(dim);
                var vIndex = knotSpanV - surface.DegreeV + l;
                for (int x = 0; x < surface.DegreeU + 1; x++)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        temp[j] = temp[j] + basisUValue[x] * controlPoints[uIndex + x][vIndex][j];
                    }
                }

                for (int j = 0; j < position.Count; j++)
                {
                    position[j] = position[j] + basisVValue[l] * temp[j];
                }
            }
            return position;

        }

        /// <summary>
        /// Extracts the iso-curve in u or v direction at a specified parameter.
        /// </summary>
        /// <param name="nurbsSurface">The surface to be evaluated</param>
        /// <param name="t">The parameter to be evaluated. Default value is 0.0 and will return the edge curve in the u direction</param>
        /// <param name="useU">Direction of the surface to be evaluated. Default value will consider the u direction.</param>
        /// <returns>Curve representing the iso-curve of the surface.</returns>
        public static ICurve SurfaceIsoCurve(NurbsSurface nurbsSurface, double t = 0, bool useU = true)
        {
            Knot knots = useU ? nurbsSurface.KnotsU : nurbsSurface.KnotsV;
            int degree = useU ? nurbsSurface.DegreeU : nurbsSurface.DegreeV;
            Dictionary<double, int> knotMults = knots.Multiplicities();

            int reqKnotIndex = -1;
            foreach (double i in knotMults.Keys)
            {
                if (Math.Abs(t - i) < GeoSharpMath.EPSILON)
                {
                    reqKnotIndex = knotMults.GetValueOrDefault(i);
                    break;
                }
            }

            int numKnotsToInsert = degree + 1;
            if (reqKnotIndex >= 0)
            {
                numKnotsToInsert -= knotMults.GetValueOrDefault(reqKnotIndex);
            }

            //Insert the knots
            NurbsSurface newSrf = numKnotsToInsert > 0 ? Modify.SurfaceKnotRefine(nurbsSurface, new Knot(Sets.RepeatData(t, numKnotsToInsert)), useU) : nurbsSurface;
            int span = knots.Span(degree, t);

            if (Math.Abs(t - knots[0]) < GeoSharpMath.EPSILON)
            {
                span = 0;
            }

            if (Math.Abs(t - knots[^1]) < GeoSharpMath.EPSILON)
            {
                span = useU ? newSrf.ControlPoints.Count - 1 : newSrf.ControlPoints[0].Count;
            }

            List<Vector3> ctrlPts = new List<Vector3>();
            if (!useU)
            {
                foreach (List<Vector3> row in newSrf.ControlPoints)
                {
                    ctrlPts.Add(row[span]);
                }

                return new NurbsCurve(newSrf.DegreeU, newSrf.KnotsU, ctrlPts);
            }
            return new NurbsCurve(newSrf.DegreeV, newSrf.KnotsV, newSrf.ControlPoints[span]);
        }

        /// <summary>
        /// Computes the tangent at a point on a curve.
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="t">The parameter on the curve.</param>
        /// <returns>The tangent vector at the given parameter.</returns>
        public static Vector3 RationalCurveTangent(ICurve curve, double t)
        {
            List<Vector3> derivatives = RationalCurveDerivatives(curve, t, 1);
            return derivatives[1];
        }

        /// <summary>
        /// Calculates the centroid averaging the points.  
        /// </summary>
        /// <param name="pts">The points collection to evaluate.</param>
        /// <returns>The centroid point.</returns>
        public static Vector3 CentroidByVertices(IList<Vector3> pts)
        {
            Vector3 centroid = new Vector3 { 0.0, 0.0, 0.0 };
            bool isClosed = pts[0] == pts[^1];
            int count = pts.Count;

            for (int i = 0; i < count && !(i == count - 1 & isClosed); i++)
            {
                centroid += pts[i];
            }

            return !isClosed ? centroid / count : centroid / (count - 1);
        }

        /// <summary>
        /// Calculates all the extrema on a curve.<br/>
        /// Extrema are calculated for each dimension, rather than for the full curve, <br/>
        /// so that the result is not the number of convex/concave transitions, but the number of those transitions for each separate dimension.
        /// </summary>
        /// <param name="derivPts">A collection of derivative coordinates.</param>
        /// <param name="order">Order of the curve.</param>
        /// <returns>The extrema </returns>
        public static Extrema ComputeExtrema(ICurve curve)
        {
            var derivPts = DerivativeCoordinates(curve.ControlPoints);
            Extrema extrema = new Extrema();

            int dim = derivPts[0][0].Count;

            for (int j = 0; j < dim; j++)
            {
                List<double> p0 = new List<double>();

                for (int i = 0; i < derivPts[0].Count; i++)
                {
                    p0.Add(derivPts[0][i][j]);
                }

                List<double> result = new List<double>(DerivRoots(p0));

                if (curve.Degree == 3)
                {
                    IList<double> p1 = new List<double>();

                    for (int i = 0; i < derivPts[1].Count; i++)
                    {
                        p1.Add(derivPts[1][i][j]);
                    }

                    result = result.Concat(DerivRoots(p1).ToList()).ToList();
                }

                result = result.Where((t) => t >= 0 && t <= 1).ToList();
                //ToDo result is a collection of doubles, t, therefore the comparison for Sort should be implicit using standard sorting methods in LINQ.
                result.Sort(GeoSharpMath.NumberSort);
                extrema[j] = result;
            }

            extrema.Values = extrema[0].Union(extrema[1]).Union(extrema[2]).OrderBy(x => x).ToList();
            return extrema;
        }

        /// <summary>
        /// Computes the derivative from the coordinate points.
        /// </summary>
        /// <param name="pts">The collection of coordinate points.</param>
        /// <returns>The derivative coordinates.</returns>
        internal static List<List<Vector3>> DerivativeCoordinates(List<Vector3> pts)
        {
            List<List<Vector3>> derivPts = new List<List<Vector3>>();

            List<Vector3> p = new List<Vector3>(pts);
            int d = p.Count;
            int c = d - 1;

            for (; d > 1; d--, c--)
            {
                List<Vector3> list = new List<Vector3>();
                for (int j = 0; j < c; j++)
                {
                    var dpt = (p[j + 1] - p[j]) * c;

                    list.Add(dpt);
                }

                derivPts.Add(list);
                p = list;
            }

            return derivPts;
        }

        /// <summary>
        /// Find all roots (derivative=0) for both derivatives.<br/>
        /// https://pomax.github.io/bezierinfo/#extremities
        /// </summary>
        /// <param name="derivs">The derivatives.</param>
        /// <returns>The roots values.</returns>
        internal static IList<double> DerivRoots(IList<double> derivs)
        {
            // quadratic roots
            if (derivs.Count == 3)
            {
                var a = derivs[0];
                var b = derivs[1];
                var c = derivs[2];
                var d = a - 2 * b + c;
                if (d != 0)
                {
                    var m1 = -Math.Sqrt(b * b - a * c);
                    var m2 = -a + b;
                    var v1 = -(m1 + m2) / d;
                    var v2 = -(-m1 + m2) / d;
                    return new[] { v1, v2 };
                }
                else if (b != c && d == 0)
                {
                    return new[] { (2 * b - c) / (2 * (b - c)) };
                }
                return Array.Empty<double>();
            }

            // linear roots
            if (derivs.Count == 2)
            {
                var a = derivs[0];
                var b = derivs[1];
                if (a != b)
                {
                    return new[] { a / (a - b) };
                }
                return Array.Empty<double>();
            }

            return Array.Empty<double>();
        }

        /// <summary>
        /// Determines the derivatives of a curve at a given parameter.<br/>
        /// <em>Corresponds to algorithm 4.2 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="parameter">Parameter on the curve at which the point is to be evaluated</param>
        /// <param name="numberDerivs">Number of derivatives to evaluate</param>
        /// <returns>The derivatives.</returns>
        public static List<Vector3> RationalCurveDerivatives(ICurve curve, double parameter, int numberDerivs = 1)
        {
            List<Vector3> derivatives = CurveDerivatives(curve, parameter, numberDerivs);
            // Array of derivate of A(t).
            // Where A(t) is the vector - valued function whose coordinates are the first three coordinates
            // of an homogenized pts.
            // Correspond in the book to Aders.
            List<Vector3> vecDers = LinearAlgebra.RationalPoints(derivatives);
            // Correspond in the book to wDers.
            List<double> weightDers = LinearAlgebra.GetWeights(derivatives);
            List<Vector3> CK = new List<Vector3>();

            for (int k = 0; k < numberDerivs + 1; k++)
            {
                Vector3 v = vecDers[k];

                for (int i = 1; i < k + 1; i++)
                {
                    double valToMultiply = LinearAlgebra.GetBinomial(k, i) * weightDers[i];
                    Vector3 pt = CK[k - i];
                    for (int j = 0; j < v.Count; j++)
                    {
                        v[j] = v[j] - valToMultiply * pt[j];
                    }
                }

                for (int j = 0; j < v.Count; j++)
                {
                    v[j] = v[j] * (1 / weightDers[0]);
                }

                CK.Add(v);
            }
            // Return C(t) derivatives.
            return CK;
        }

        /// <summary>
        /// Determines the derivatives of a NURBS curve at a given parameter.<br/>
        /// <em>Corresponds to algorithm 3.2 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curve">The curve object.</param>
        /// <param name="parameter">Parameter on the curve at which the point is to be evaluated.</param>
        /// <param name="numberDerivs">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <returns>The derivatives.</returns>
        public static List<Vector3> CurveDerivatives(ICurve curve, double parameter, int numberDerivs)
        {
            int degree = curve.Degree;
            List<Vector3> controlPoints = curve.HomogenizedPoints;
            Knot knots = curve.Knots;

            if (!curve.Knots.AreValidKnots(degree, controlPoints.Count))
            {
                throw new ArgumentException("Invalid relations between control points, knot");
            }

            int n = knots.Count - degree - 2;

            int ptDimension = controlPoints[0].Count;
            int derivateOrder = numberDerivs < degree ? numberDerivs : degree;
            List<Vector3> CK = Vector3.Zero2d(numberDerivs + 1, ptDimension);
            int knotSpan = knots.Span(n, degree, parameter);
            List<Vector3> derived2d = DerivativeBasisFunctionsGivenNI(knotSpan, parameter, degree, derivateOrder, knots);

            for (int k = 0; k < derivateOrder + 1; k++)
            {
                for (int j = 0; j < degree + 1; j++)
                {
                    double valToMultiply = derived2d[k][j];
                    Vector3 pt = controlPoints[knotSpan - degree + j];
                    for (int i = 0; i < CK[k].Count; i++)
                    {
                        CK[k][i] = CK[k][i] + (valToMultiply * pt[i]);
                    }
                }
            }
            return CK;
        }

        /// <summary>
        /// Computes the non-vanishing basis functions and their derivatives.<br/>
        /// <em>Corresponds to algorithm 2.3 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="span">Span index.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="degree">Curve degree.</param>
        /// <param name="order">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <param name="knots">Sets of non-decreasing knot values.</param>
        /// <returns>The derivatives at the given parameter.</returns>
        public static List<Vector3> DerivativeBasisFunctionsGivenNI(int span, double parameter, int degree,
            int order, Knot knots)
        {
            Vector3 left = Vector3.Zero1d(degree + 1);
            Vector3 right = Vector3.Zero1d(degree + 1);
            // N[0][0] = 1.0 by definition
            List<Vector3> ndu = Vector3.Zero2d(degree + 1, degree + 1);
            ndu[0][0] = 1.0;

            for (int j = 1; j < degree + 1; j++)
            {
                left[j] = parameter - knots[span + 1 - j];
                right[j] = knots[span + j] - parameter;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    ndu[j][r] = right[r + 1] + left[j - r];
                    double temp = ndu[r][j - 1] / ndu[j][r];

                    ndu[r][j] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                ndu[j][j] = saved;
            }

            // Load the basic functions.
            List<Vector3> ders = Vector3.Zero2d(order + 1, degree + 1);
            for (int j = 0; j < degree + 1; j++)
            {
                ders[0][j] = ndu[j][degree];
            }

            // Start calculating derivatives.
            List<Vector3> a = Vector3.Zero2d(2, degree + 1);
            // Loop over function index.
            for (int r = 0; r < degree + 1; r++)
            {
                // Alternate row in array a.
                int s1 = 0;
                int s2 = 1;
                a[0][0] = 1.0;

                // Loop to compute Kth derivative.
                for (int k = 1; k < order + 1; k++)
                {
                    double d = 0.0;
                    int rk = r - k;
                    int pk = degree - k;
                    int j1, j2;

                    if (r >= k)
                    {
                        a[s2][0] = a[s1][0] / ndu[pk + 1][rk];
                        d = a[s2][0] * ndu[rk][pk];
                    }

                    if (rk >= -1)
                    {
                        j1 = 1;
                    }
                    else
                    {
                        j1 = -rk;
                    }

                    if (r - 1 <= pk)
                    {
                        j2 = k - 1;
                    }
                    else
                    {
                        j2 = degree - r;
                    }

                    for (int j = j1; j < j2 + 1; j++)
                    {
                        a[s2][j] = (a[s1][j] - a[s1][j - 1]) / ndu[pk + 1][rk + j];
                        d += a[s2][j] * ndu[rk + j][pk];
                    }

                    if (r <= pk)
                    {
                        a[s2][k] = -a[s1][k - 1] / ndu[pk + 1][r];
                        d += a[s2][k] * ndu[r][pk];
                    }

                    ders[k][r] = d;

                    // Switch rows.
                    int tempVal = s1;
                    s1 = s2;
                    s2 = tempVal;
                }
            }

            // Multiply through by the the correct factors.
            int acc = degree;
            for (int k = 1; k < order + 1; k++)
            {
                for (int j = 0; j < degree + 1; j++)
                {
                    ders[k][j] *= acc;
                }

                acc *= degree - k;
            }

            return ders;
        }

        /// <summary>
        /// Computes the normal vector at uv parameter on a NURBS surface.
        /// </summary>
        /// <param name="nurbsSurface">The surface.</param>
        /// <param name="u">The u parameter.</param>
        /// <param name="v">the v parameter.</param>
        /// <returns>The normal vector at the given uv of the surface.</returns>
        public static Vector3 RationalSurfaceNormal(NurbsSurface nurbsSurface, double u, double v)
        {
            List<List<Vector3>> derivs = RationalSurfaceDerivatives(nurbsSurface, u, v);
            return Vector3.Cross(derivs[1][0], derivs[0][1]);
        }


        /// <summary>
        /// Computes the derivatives at a point on a NURBS surface.
        /// </summary>
        /// <param name="nurbsSurface">The surface.</param>
        /// <param name="u">The u parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The v parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">Number of derivatives to evaluate (default is 1)</param>
        /// <returns>The derivatives.</returns>
        public static List<List<Vector3>> RationalSurfaceDerivatives(NurbsSurface nurbsSurface, double u, double v, int numDerivs = 1)
        {
            List<List<Vector3>> ders = SurfaceDerivatives(nurbsSurface, u, v, numDerivs);
            List<List<Vector3>> Aders = LinearAlgebra.Rational2d(ders);
            List<List<double>> wders = LinearAlgebra.GetWeights2d(ders);
            List<List<Vector3>> SKL = new List<List<Vector3>>();
            int dim = Aders[0][0].Count;

            for (int k = 0; k < numDerivs + 1; k++)
            {
                SKL.Add(new List<Vector3>());
                for (int l = 0; l < numDerivs - k + 1; l++)
                {
                    Vector3 t1 = Aders[k][l];
                    for (int j = 1; j < l + 1; j++)
                    {
                        Vector3.SubMulMutate(t1, LinearAlgebra.GetBinomial(l, j) * wders[0][j], SKL[k][l - j]);
                    }

                    for (int i = 1; i < k + 1; i++)
                    {
                        Vector3.SubMulMutate(t1, LinearAlgebra.GetBinomial(k, i) * wders[i][0], SKL[k - i][l]);
                        Vector3 t2 = Vector3.Zero1d(dim);
                        for (int j = 1; j < l + 1; j++)
                        {
                            Vector3.AddMulMutate(t2, LinearAlgebra.GetBinomial(l, j) * wders[i][j], SKL[k - i][l - j]);
                        }

                        Vector3.SubMulMutate(t1, LinearAlgebra.GetBinomial(k, i), t2);
                    }
                    Vector3 t = t1 * (1 / wders[0][0]);
                    SKL[k].Add(t); //demogenize
                }
            }

            return SKL;
        }

        /// <summary>
        /// Computes the derivatives on a NURBS surface.
        /// </summary>
        /// <param name="nurbsSurface">Object representing the surface.</param>
        /// <param name="u">The u parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The v parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">Number of derivatives to evaluate.</param>
        /// <returns>A 2d collection representing the derivatives - u derivatives increase by row, v by column</returns>
        public static List<List<Vector3>> SurfaceDerivatives(NurbsSurface nurbsSurface, double u, double v, int numDerivs)
        {
            int n = nurbsSurface.KnotsU.Count - nurbsSurface.DegreeU - 2;
            int m = nurbsSurface.KnotsV.Count - nurbsSurface.DegreeV - 2;
            return SurfaceDerivativesGivenNM(nurbsSurface, n, m, u, v, numDerivs);
        }

        /// <summary>
        /// Computes the derivatives on a non-uniform, non-rational B spline surface.<br/>
        /// SKL is the derivative S(u,v) with respect to u K-times and v L-times.<br/>
        /// <em>Corresponds to algorithm 3.6 from The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="nurbsSurface">The surface.</param>
        /// <param name="n">Integer number of basis functions in u dir - 1 = knotsU.length - degreeU - 2.</param>
        /// <param name="m">Integer number of basis functions in v dir - 1 = knotsU.length - degreeU - 2.</param>
        /// <param name="u">The u parameter at which to evaluate the derivatives.</param>
        /// <param name="v">The v parameter at which to evaluate the derivatives.</param>
        /// <param name="numDerivs">The number of derivatives to evaluate.</param>
        /// <returns>A 2d collection representing the derivatives - u derivatives increase by row, v by column</returns>
        public static List<List<Vector3>> SurfaceDerivativesGivenNM(NurbsSurface nurbsSurface, int n, int m, double u, double v, int numDerivs)
        {
            int degreeU = nurbsSurface.DegreeU;
            int degreeV = nurbsSurface.DegreeV;
            List<List<Vector3>> ctrlPts = nurbsSurface.HomogenizedPoints;
            Knot knotsU = nurbsSurface.KnotsU;
            Knot knotsV = nurbsSurface.KnotsV;

            if (!knotsU.AreValidKnots(degreeU, ctrlPts.Count) || !knotsV.AreValidKnots(degreeV, ctrlPts[0].Count))
            {
                throw new ArgumentException("Invalid relations between control points, knot vector, and n");
            }

            //This should be always 3 
            int dim = ctrlPts[0][0].Count;
            int du = numDerivs < degreeU ? numDerivs : degreeU;
            int dv = numDerivs < degreeV ? numDerivs : degreeV;

            List<List<Vector3>> SKL = Vector3.Zero3d(numDerivs + 1, numDerivs + 1, dim);
            int knotSpanU = knotsU.Span(n, degreeU, u);
            int knotSpanV = knotsV.Span(m, degreeV, v);

            List<Vector3> uders = DerivativeBasisFunctionsGivenNI(knotSpanU, u, degreeU, n, knotsU);
            List<Vector3> vders = DerivativeBasisFunctionsGivenNI(knotSpanV, v, degreeV, m, knotsV);

            List<Vector3> temp = Vector3.Zero2d(degreeV + 1, dim);

            for (int k = 0; k < du + 1; k++)
            {
                for (int s = 0; s < degreeV + 1; s++)
                {
                    temp[s] = Vector3.Zero1d(dim);
                    for (int r = 0; r < degreeU + 1; r++)
                    {
                        Vector3.AddMulMutate(temp[s], uders[k][r], ctrlPts[knotSpanU - degreeU + r][knotSpanV - degreeV + s]);
                    }
                }
                int nk = numDerivs - k;
                var dd = nk < dv ? nk : dv;

                for (int l = 0; l < dd + 1; l++)
                {
                    SKL[k][l] = Vector3.Zero1d(dim);
                    for (int s = 0; s < degreeV + 1; s++)
                    {
                        Vector3.AddMulMutate(SKL[k][l], vders[l][s], temp[s]);
                    }
                }
            }
            return SKL;
        }
    }
}