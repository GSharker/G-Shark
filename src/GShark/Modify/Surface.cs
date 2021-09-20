using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using System;
using System.Collections.Generic;

namespace GShark.Modify
{
    /// <summary>
    /// Contains many fundamental algorithms for modifying the properties of a NURBS surface.<br/>
    /// These include algorithms for: knot insertion, knot refinement, degree elevation, re-parametrization.<br/>
    /// </summary>
    internal static class Surface
    {
        /// <summary>
        /// Performs a knot refinement on a surface by inserting knots at various parameters.<br/>
        /// <em>Implementation of Algorithm A5.5 of The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        internal static NurbsSurface SurfaceKnotRefine(NurbsSurface surface, IList<double> knotsToInsert, SurfaceDirection direction)
        {
            List<List<Point4>> modifiedControlPts = new List<List<Point4>>();
            List<List<Point4>> controlPts = surface.ControlPoints;
            KnotVector knots = surface.KnotsV;
            int degree = surface.DegreeV;

            if (direction != SurfaceDirection.V)
            {
                controlPts = CollectionHelpers.Transpose2DArray(surface.ControlPoints);
                knots = surface.KnotsU;
                degree = surface.DegreeU;
            }

            NurbsBase curve = null;
            foreach (List<Point4> pts in controlPts)
            {
                curve = Curve.KnotRefine(new NurbsCurve(degree, knots, pts), knotsToInsert);
                modifiedControlPts.Add(curve.ControlPoints);
            }

            if (curve == null)
            {
                throw new Exception(
                    "The refinement was not be able to be completed. A problem occur refining the internal curves.");
            }

            if (direction != SurfaceDirection.V)
            {
                var reversedControlPts = CollectionHelpers.Transpose2DArray(modifiedControlPts);
                return new NurbsSurface(surface.DegreeU, surface.DegreeV, curve.Knots, surface.KnotsV.Copy(),
                    reversedControlPts);
            }

            return new NurbsSurface(surface.DegreeU, surface.DegreeV, surface.KnotsU.Copy(), curve.Knots,
                modifiedControlPts);
        }
    }
}
