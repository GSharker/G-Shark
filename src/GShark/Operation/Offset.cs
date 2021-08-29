using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;

namespace GShark.Operation
{
    /// <summary>
    /// Collects a set of method for offsetting primitives and NURBS geometries.
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
            if (distance == 0.0)
            {
                return ln;
            }

            Vector3 vecOffset = Vector3.CrossProduct(ln.Direction, pln.ZAxis).Amplify(distance);
            return new Line(ln.StartPoint + vecOffset, ln.EndPoint + vecOffset);
        }

        /// <summary>
        /// Computes the offset of a circle.
        /// </summary>
        /// <param name="cl">The circle to offset.</param>
        /// <param name="distance">The distance of the offset.</param>
        /// <returns>The offset circle.</returns>
        public static Circle Circle(Circle cl, double distance)
        {
            if (distance == 0.0)
            {
                return cl;
            }

            return new Circle(cl.Plane, cl.Radius + distance);
        }

        /// <summary>
        /// Computes the offset of a curve.
        /// </summary>
        /// <param name="crv">The curve to offset.</param>
        /// <param name="distance">The distance of the offset.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset curve.</returns>
        public static NurbsCurve Curve(NurbsCurve crv, double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return crv;
            }

            var (tValues, pts) = Tessellation.CurveAdaptiveSample(crv);

            List<Point3> offsetPts = new List<Point3>();
            for (int i = 0; i < pts.Count; i++)
            {
                Vector3 tangent = Evaluation.RationalCurveTangent(crv, tValues[i]);
                Vector3 vecOffset = Vector3.CrossProduct(tangent, pln.ZAxis).Amplify(distance);
                offsetPts.Add(pts[i] + vecOffset);
            }

            return Fitting.InterpolatedCurve(offsetPts, 2);
        }

        /// <summary>
        /// Computes the offset of a polyline or a polygon.
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="distance">The distance of the offset.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset polyline.</returns>
        public static Polyline Polyline(Polyline poly, double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return poly;
            }

            int iteration = (poly.IsClosed) ? poly.Count : poly.Count - 1;

            Point3[] offsetPts = new Point3[poly.Count];
            List<Line> segments = poly.Segments;
            Line[] offsetSegments = new Line[segments.Count + 1];

            for (int i = 0; i < iteration; i++)
            {
                int k = (i == iteration - 1 && poly.IsClosed) ? 0 : i;
                if (i == iteration - 1 && k == 0)
                {
                    goto Intersection;
                }

                Vector3 vecOffset = Vector3.CrossProduct(segments[k].Direction, pln.ZAxis).Amplify(distance);
                Transform xForm = Transform.Translation(vecOffset);
                offsetSegments[k] = segments[k].Transform(xForm);

                if (i == 0 && poly.IsClosed)
                {
                    continue;
                }
                if (k == 0 && !poly.IsClosed)
                {
                    offsetPts[k] = offsetSegments[k].StartPoint;
                    continue;
                }

            Intersection:
                bool ccx = Intersect.LineLine(offsetSegments[(i == iteration - 1 && poly.IsClosed) ? iteration - 2 : k - 1], offsetSegments[k], out Point3 pt, out _, out _, out _);
                if (!ccx)
                {
                    continue;
                }

                offsetPts[k] = pt;

                if (i == iteration - 1)
                {
                    offsetPts[(poly.IsClosed) ? i : i + 1] = (poly.IsClosed) ? offsetPts[0] : offsetSegments[k].EndPoint;
                }
            }

            return new Polyline(offsetPts);
        }
    }
}
