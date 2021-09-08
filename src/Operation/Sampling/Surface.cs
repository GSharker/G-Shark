using GShark.Core;
using GShark.Enumerations;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Sampling
{
    /// <summary>
    /// Contains static algorithms for tessellation and division of NURBS surface.<br/>
    /// Some of these algorithms are "adaptive" - using certain heuristics to sample geometry where such samples make sense - while<br/>
    /// others are "regular" in that they sample regularly throughout a parametric domain.There are tradeoffs here.<br/>
    /// While adaptive algorithms can sometimes yield "better" results that are smaller or more economical, this can sometimes come at increased computational cost.
    /// For example, it is sometimes necessarily to compute higher order derivatives in order to<br/>
    /// obtain these more economical results.Your usage of these algorithms should consider these tradeoffs.
    /// </summary>
    internal static class Surface
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
        internal static NurbsSurface[] Split(NurbsSurface surface, double parameter, SplitDirection direction)
        {
            KnotVector knots = surface.KnotsV;
            int degree = surface.DegreeV;
            List<List<Point4>> srfCtrlPts = surface.ControlPoints;

            if (direction != SplitDirection.V)
            {
                srfCtrlPts = CollectionHelpers.Transpose2DArray(surface.ControlPoints);
                knots = surface.KnotsU;
                degree = surface.DegreeU;
            }

            List<double> knotsToInsert = CollectionHelpers.RepeatData(parameter, degree + 1);
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
                        new NurbsSurface(degree, surface.DegreeV, knotLeft, surface.KnotsV.Copy(), CollectionHelpers.Transpose2DArray(surfPtsLeft)),
                        new NurbsSurface(degree, surface.DegreeV, knotRight, surface.KnotsV.Copy(), CollectionHelpers.Transpose2DArray(surfPtsRight))
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
                        NurbsSurface srf1 = new NurbsSurface(degree, surface.DegreeV, knotLeft, surface.KnotsV.Copy(), CollectionHelpers.Transpose2DArray(surfPtsLeft));
                        NurbsSurface srf2 = new NurbsSurface(degree, surface.DegreeV, knotRight, surface.KnotsV.Copy(), CollectionHelpers.Transpose2DArray(surfPtsRight));

                        NurbsSurface[] split1 = Split(srf1, parameter, SplitDirection.V);
                        NurbsSurface[] split2 = Split(srf2, parameter, SplitDirection.V);

                        surfaceResult = split2.Concat(split1).ToArray();
                        break;
                    }
            }

            return surfaceResult;
        }
    }
}
