using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation.Enum;
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
        /// Splits (divides) the surface into two parts at the specified parameter
        /// </summary>
        /// <param name="surface">The NURBS surface to split.</param>
        /// <param name="parameter">The parameter at which to split the surface, parameter should be between 0 and 1.</param>
        /// <param name="direction">Where to split in the U or V direction of the surface.</param>
        /// <returns>If the surface is split vertically (U direction) the left side is returned as the first surface and the right side is returned as the second surface.<br/>
        /// If the surface is split horizontally (V direction) the bottom side is returned as the first surface and the top side is returned as the second surface.<br/>
        /// If the spit direction selected is both, the split computes first a U direction split and on the result a V direction split.</returns>
        internal static NurbsSurface[] SplitSurface(NurbsSurface surface, double parameter, SplitDirection direction)
        {
            KnotVector knots = surface.KnotsV;
            int degree = surface.DegreeV;
            List<List<Point4>> srfCtrlPts = surface.ControlPoints;

            if (direction != SplitDirection.V)
            {
                srfCtrlPts = Sets.Reverse2DMatrixData(surface.ControlPoints);
                knots = surface.KnotsU;
                degree = surface.DegreeU;
            }

            List<double> knotsToInsert = Sets.RepeatData(parameter, degree + 1);
            int span = knots.Span(degree, parameter);

            List<List<Point4>> surfPtsLeft = new List<List<Point4>>();
            List<List<Point4>> surfPtsRight = new List<List<Point4>>();
            NurbsCurve result = null;

            foreach (List<Point4> pts in srfCtrlPts)
            {
                NurbsCurve tempCurve = new NurbsCurve(degree, knots, pts);
                result = Modify.CurveKnotRefine(tempCurve, knotsToInsert);

                surfPtsLeft.Add(result.ControlPoints.GetRange(0, span + 1));
                surfPtsRight.Add(result.ControlPoints.GetRange(span + 1, span + 1));
            }

            if (result == null) throw new Exception($"Could not split {nameof(surface)}.");

            KnotVector knotLeft = result.Knots.GetRange(0, span + degree + 2).ToKnot();
            KnotVector knotRight = result.Knots.GetRange(span + 1, span + degree + 2).ToKnot();
            NurbsSurface[] surfaceResult = Array.Empty<NurbsSurface>();

            switch (direction)
            {
                case SplitDirection.U:
                    {
                        surfaceResult = new NurbsSurface[]
                        {
                        new NurbsSurface(degree, surface.DegreeV, knotLeft, surface.KnotsV.Copy(), Sets.Reverse2DMatrixData(surfPtsLeft)),
                        new NurbsSurface(degree, surface.DegreeV, knotRight, surface.KnotsV.Copy(), Sets.Reverse2DMatrixData(surfPtsRight))
                        };
                        break;
                    }
                case SplitDirection.V:
                    {
                        surfaceResult = new NurbsSurface[]
                        {
                        new NurbsSurface(surface.DegreeU, degree, surface.KnotsU.Copy(), knotLeft, surfPtsLeft),
                        new NurbsSurface(surface.DegreeU, degree, surface.KnotsU.Copy(), knotRight, surfPtsRight)
                        };
                        break;
                    }
                case SplitDirection.Both:
                    {
                        NurbsSurface srf1 = new NurbsSurface(degree, surface.DegreeV, knotLeft, surface.KnotsV.Copy(), Sets.Reverse2DMatrixData(surfPtsLeft));
                        NurbsSurface srf2 = new NurbsSurface(degree, surface.DegreeV, knotRight, surface.KnotsV.Copy(), Sets.Reverse2DMatrixData(surfPtsRight));

                        NurbsSurface[] split1 = SplitSurface(srf1, parameter, SplitDirection.V);
                        NurbsSurface[] split2 = SplitSurface(srf2, parameter, SplitDirection.V);

                        surfaceResult = split2.Concat(split1).ToArray();
                        break;
                    }
            }

            return surfaceResult;
        }

        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="divisions">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        internal static List<double> CurveByCount(NurbsCurve curve, int divisions)
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
        internal static (List<double> tValues, List<double> lengths) CurveByLength(NurbsCurve curve, double length)
        {
            List<NurbsCurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
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

                while (segmentLength < sum + GSharkMath.Epsilon)
                {
                    double t = Analyze.BezierCurveParamAtLength(curves[i], segmentLength - sum2,
                        GSharkMath.MaxTolerance, curveLengths[i]); //***this is getting the parameter on the next curve. does this mean decompose into bezier maintains original params?

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
