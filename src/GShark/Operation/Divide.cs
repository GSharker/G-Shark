using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Enum;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Provides various methods for dividing and splitting NURBS geometry.
    /// </summary>
    public class Divide
    {
        /// <summary>
		/// Splits a curve into two parts at a given parameter.
		/// </summary>
		/// <param name="curve">The curve object.</param>
		/// <param name="t">The parameter where to split the curve.</param>
		/// <returns>Two new curves, defined by degree, knots, and control points.</returns>
		public static List<ICurve> SplitCurve(ICurve curve, double t)
        {
            int degree = curve.Degree;

            List<double> knotsToInsert = Sets.RepeatData(t, degree + 1);

            ICurve refinedCurve = Modify.CurveKnotRefine(curve, knotsToInsert);

            int s = curve.Knots.Span(degree, t);

            KnotVector knots0 = refinedCurve.Knots.ToList().GetRange(0, s + degree + 2).ToKnot();
            KnotVector knots1 = refinedCurve.Knots.GetRange(s + 1, refinedCurve.Knots.Count - (s + 1)).ToKnot();

            List<Point4> controlPoints0 = refinedCurve.ControlPoints.GetRange(0, s + 1);
            List<Point4> controlPoints1 = refinedCurve.ControlPoints.GetRange(s + 1, refinedCurve.ControlPoints.Count - (s + 1));

            return new List<ICurve> { new NurbsCurve(degree, knots0, controlPoints0), new NurbsCurve(degree, knots1, controlPoints1) };
        }

        /// <summary>
        /// Splits (divides) the surface into two parts at the specified parameter
        /// </summary>
        /// <param name="surface">The NURBS surface to split.</param>
        /// <param name="parameter">The parameter at which to split the surface, parameter should be between 0 and 1.</param>
        /// <param name="direction">Where to split in the U or V direction of the surface.</param>
        /// <returns>If the surface is split vertically (U direction) the left side is returned as the first surface and the right side is returned as the second surface.<br/>
        /// If the surface is split horizontally (V direction) the bottom side is returned as the first surface and the top side is returned as the second surface.</returns>
        internal static NurbsSurface[] SplitSurface(NurbsSurface surface, double parameter, SplitDirection direction)
        {
            KnotVector knots = surface.KnotsV;
            int degree = surface.DegreeV;
            List<List<Point4>> pts2d = surface.ControlPoints;

            if (direction != SplitDirection.V)
            {
                pts2d = Sets.Reverse2DMatrixData(surface.ControlPoints);
                knots = surface.KnotsU;
                degree = surface.DegreeU;
            }

            List<double> knotsToInsert = Sets.RepeatData(parameter, degree + 1);
            int span = knots.Span(degree, parameter);

            List<List<Point4>> surfPtsLeft = new List<List<Point4>>();
            List<List<Point4>> surfPtsRight = new List<List<Point4>>();
            ICurve result = null;

            foreach (List<Point4> pts in pts2d)
            {
                NurbsCurve tempCurve = new NurbsCurve(degree, knots, pts);
                result = Modify.CurveKnotRefine(tempCurve, knotsToInsert);

                surfPtsLeft.Add(result.ControlPoints.GetRange(0, span + 1));
                surfPtsRight.Add(result.ControlPoints.GetRange(span + 1, span + 1));
            }

            if (result == null) throw new Exception("Could not solve the split.");

            KnotVector knotLeft = result.Knots.GetRange(0, span + degree + 2).ToKnot();
            KnotVector knotRight = result.Knots.GetRange(span + 1, span + degree + 2).ToKnot();

            if (direction != SplitDirection.V)
            {
                return new NurbsSurface[]
                {
                    new NurbsSurface(degree, surface.DegreeV, knotLeft, surface.KnotsV.Copy(), Sets.Reverse2DMatrixData(surfPtsLeft)),
                    new NurbsSurface(degree, surface.DegreeV, knotRight, surface.KnotsV.Copy(), Sets.Reverse2DMatrixData(surfPtsRight))
                };
            }

            return new NurbsSurface[]
            {
                new NurbsSurface(surface.DegreeU, degree, surface.KnotsU.Copy(), knotLeft, surfPtsLeft),
                new NurbsSurface(surface.DegreeU, degree, surface.KnotsU.Copy(), knotRight, surfPtsRight)
            };
        }

        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="divisions">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        internal static List<double> CurveByCount(ICurve curve, int divisions)
        {
            double approximatedLength = Analyze.CurveLength(curve);
            double arcLengthSeparation = approximatedLength / divisions;
            var divisionByLength = CurveByLength(curve, arcLengthSeparation);
            var tValues = divisionByLength.tValues;
            return tValues;
        }

        /// <summary>
        /// Divides a curve for a given length, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="length">The length separating the resultant samples.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        internal static (List<double> tValues, List<double> lengths) CurveByLength(ICurve curve, double length)
        {
            List<ICurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
            List<double> curveLengths = curves.Select(nurbsCurve => Analyze.BezierCurveLength(nurbsCurve)).ToList();
            double totalLength = curveLengths.Sum();

            List<double> tValues = new List<double> { curve.Knots[0] };
            List<double> divisionLengths = new List<double> { 0.0 };

            if (length > totalLength) return (tValues, divisionLengths);

            int i = 0;
            double sum = 0.0;
            double sum2 = 0.0;
            double segmentLength = length;

            while (i < curves.Count)
            {
                sum += curveLengths[i];

                while (segmentLength < sum + GeoSharkMath.Epsilon)
                {
                    double t = Analyze.BezierCurveParamAtLength(curves[i], segmentLength - sum2,
                        GeoSharkMath.MaxTolerance, curveLengths[i]); //***this is getting the parameter on the next curve. does this mean decompose into bezier maintains original params?

                    tValues.Add(t);
                    divisionLengths.Add(segmentLength);

                    segmentLength += length;
                }

                sum2 += curveLengths[i];
                i++;
            }

            return (tValues, divisionLengths);
        }
    }
}
