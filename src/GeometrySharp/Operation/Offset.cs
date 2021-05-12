using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;

namespace GeometrySharp.Operation
{
    // ToDo: Add cap to the offset.
    /// <summary>
    /// The offset class collects a set of method for offsetting primitives and nurbs geometries.
    /// </summary>
    public static class Offset
    {
        /// <summary>
        /// Computes the offset of a line.
        /// </summary>
        /// <param name="ln">The line to offset.</param>
        /// <param name="distance">The distance of the offset.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset line.</returns>
        public static Line Line(Line ln, double distance, Plane pln)
        {
            Vector3 vecOffset = Vector3.Cross(ln.Direction, pln.Normal).Amplify(distance);
            return new Line(ln.Start + vecOffset, ln.End + vecOffset);
        }

        /// <summary>
        /// Computes the offset of a circle.
        /// </summary>
        /// <param name="cl">The circle to offset.</param>
        /// <param name="distance">The distance of the offset.</param>
        /// <returns>The offset circle.</returns>
        public static Circle Circle(Circle cl, double distance)
        {
            return new Circle(cl.Plane, cl.Radius + distance);
        }

        public static ICurve Curve(ICurve crv, double distance, Plane pln)
        {
            var subdivision = Tessellation.CurveAdaptiveSample(crv);

            List<Vector3> offsetPts = new List<Vector3>();
            for (int i = 0; i < subdivision.pts.Count; i++)
            {
                Vector3 tangent = Evaluation.RationalCurveTangent(crv, subdivision.tValues[i]);
                Vector3 vecOffset = Vector3.Cross(tangent, pln.Normal).Amplify(distance);
                offsetPts.Add(subdivision.pts[i] + vecOffset);
            }

            return Fitting.InterpolatedCurve(offsetPts, 2);
        }

        public static Polyline Polyline(Polyline pl, double distance, Plane pln)
        {
            //int iteration = (pl.IsClosed) ? pl.Count - 1 : pl.Count;

            //Vector3[] offsetPts = new Vector3[pl.Count];
            //Line[] segments = pl.Segments();
            //Line[] offsetSegments = new Line[segments.Length + 1];

            //for (int i = 0; i < iteration; i++)
            //{
            //    Vector3 vecOffset = Vector3.Cross(segments[i].Direction, pln.Normal).Amplify(distance);
            //    Transform xForm = Transform.Translation(vecOffset);
            //    offsetSegments[i] = segments[i].Transform(xForm);

            //    if (i == 0 && pl.IsClosed)
            //    {
            //        continue;
            //    }
            //    else if (i == 0 && !pl.IsClosed)
            //    {
            //        offsetPts[i] = offsetSegments[i].Start;
            //    }
            //    else if (i == iteration - 1 && !pl.IsClosed)
            //    {
            //        offsetPts[i] = offsetSegments[i].End;
            //    }
            //    else
            //    {
            //        Intersect.LineLine()
            //    }
            //}

            return null;
        }
    }
}
