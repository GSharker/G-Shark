using GShark.Core;
using GShark.Core.BoundingBoxTree;
using GShark.Core.IntersectionResults;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Operation
{
    /// <summary>
    /// Provides various tools for all kinds of intersection between NURBS and primitive.
    /// </summary>
    public class Intersect
    {
        /// <summary>
        /// Solves the intersection between two planes.<br/>
        /// This method returns true if intersection is found, false if the planes are parallel.
        /// </summary>
        /// <param name="p1">The first plane.</param>
        /// <param name="p2">The second plane.</param>
        /// <param name="line">The intersection as <see cref="Line"/>.</param>
        /// <returns>True if the intersection success.</returns>
        public static bool PlanePlane(Plane p1, Plane p2, out Line line)
        {
            Vector3 plNormal1 = p1.Normal;
            Vector3 plNormal2 = p2.Normal;
            line = new Line(new Vector3 { 0, 0, 0 }, new Vector3 { 0, 0, 1 });

            Vector3 directionVec = Vector3.Cross(plNormal1, plNormal2);
            if (Vector3.Dot(directionVec, directionVec) < GeoSharpMath.EPSILON)
            {
                return false;
            }

            // Find the largest index of directionVec of the line of intersection.
            int largeIndex = 0;
            double ai = Math.Abs(directionVec[0]);
            double ay = Math.Abs(directionVec[1]);
            double az = Math.Abs(directionVec[2]);

            if (ay > ai)
            {
                largeIndex = 1;
                ai = ay;
            }

            if (az > ai)
            {
                largeIndex = 2;
            }

            double a1, b1, a2, b2;

            if (largeIndex == 0)
            {
                a1 = plNormal1[1];
                b1 = plNormal1[2];
                a2 = plNormal2[1];
                b2 = plNormal2[2];
            }
            else if (largeIndex == 1)
            {
                a1 = plNormal1[0];
                b1 = plNormal1[2];
                a2 = plNormal2[0];
                b2 = plNormal2[2];
            }
            else
            {
                a1 = plNormal1[0];
                b1 = plNormal1[1];
                a2 = plNormal2[0];
                b2 = plNormal2[1];
            }

            double dot1 = -Vector3.Dot(p1.Origin, plNormal1);
            double dot2 = -Vector3.Dot(p2.Origin, plNormal2);

            double denominator = a1 * b2 - a2 * b1;

            double corX = (b1 * dot2 - b2 * dot1) / denominator;
            double corY = (a2 * dot1 - a1 * dot2) / denominator;

            Vector3 pt = new Vector3();

            if (largeIndex == 0)
            {
                pt.AddRange(new[] { 0.0, corX, corY });
            }
            else if (largeIndex == 1)
            {
                pt.AddRange(new[] { corX, 0.0, corY });
            }
            else
            {
                pt.AddRange(new[] { corX, corY, 0.0 });
            }

            line = new Line(pt, pt + directionVec.Unitize());
            return true;
        }

        /// <summary>
        /// Finds the unique point intersection of a line and a plane.<br/>
        /// This method returns true if intersection return the unique point,<br/>
        /// it returns false if the segment is parallel to the plane or lies in plane.<br/>
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="line">The segment to intersect. Assumed as infinite.</param>
        /// <param name="plane">The plane has to be intersected.</param>
        /// <param name="pt">The point representing the unique intersection.</param>
        /// <param name="t">The parameter on the line between 0.0 to 1.0</param>
        /// <returns>True if the intersection success.</returns>
        public static bool LinePlane(Line line, Plane plane, out Vector3 pt, out double t)
        {
            Vector3 lnDir = line.Direction;
            Vector3 ptPlane = plane.Origin - line.Start;
            double segmentLength = line.Length;

            double denominator = Vector3.Dot(plane.Normal, lnDir);
            double numerator = Vector3.Dot(plane.Normal, ptPlane);

            if (Math.Abs(denominator) < GeoSharpMath.EPSILON)
            {
                pt = Vector3.Unset;
                t = 0.0;
                return false;
            }

            // Compute the intersect parameter.
            double s = numerator / denominator;
            pt = line.Start + lnDir * s;
            // Parametrize the t value between 0.0 to 1.0.
            t = s / segmentLength;
            return true;
        }

        /// <summary>
        /// Solves the intersection between two lines, assumed as infinite.<br/>
        /// Returns as outputs two points describing the minimum distance between the two lines.<br/>
        /// Returns false if the segments are parallel.<br/>
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="ln0">The first line.</param>
        /// <param name="ln1">The second line.</param>
        /// <param name="pt0">The output point of the first line.</param>
        /// <param name="pt1">The output point of the second line.</param>
        /// <param name="t0">The parameter on the first line between 0.0 to 1.0</param>
        /// <param name="t1">The parameter on the second line between 0.0 to 1.0</param>
        /// <returns>True if the intersection succeed.</returns>
        public static bool LineLine(Line ln0, Line ln1, out Vector3 pt0, out Vector3 pt1, out double t0, out double t1)
        {
            double ln0Length = ln0.Length;
            double ln1Length = ln1.Length;
            Vector3 lnDir0 = ln0.Direction;
            Vector3 lnDir1 = ln1.Direction;
            Vector3 ln0Ln1Dir = ln0.Start - ln1.Start;

            double a = Vector3.Dot(lnDir0, lnDir0);
            double b = Vector3.Dot(lnDir0, lnDir1);
            double c = Vector3.Dot(lnDir1, lnDir1);
            double d = Vector3.Dot(lnDir0, ln0Ln1Dir);
            double e = Vector3.Dot(lnDir1, ln0Ln1Dir);
            double div = a * c - b * b;

            if (Math.Abs(div) < GeoSharpMath.EPSILON)
            {
                pt0 = Vector3.Unset;
                pt1 = Vector3.Unset;
                t0 = 0.0;
                t1 = 0.0;
                return false;
            }

            double s = (b * e - c * d) / div;
            double t = (a * e - b * d) / div;

            pt0 = ln0.Start + lnDir0 * s;
            pt1 = ln1.Start + lnDir1 * t;
            t0 = s / ln0Length;
            t1 = t / ln1Length;
            return true;
        }

        /// <summary>
        /// Computes the intersection between a polyline and a plane.<br/>
        /// Under the hood, is intersecting each segment with the plane and storing the intersection point into a collection.<br/>
        /// If no intersections are found a empty collection is returned.<br/>
        /// </summary>
        /// <param name="poly">The polyline to intersect with.</param>
        /// <param name="pl">The section plane.</param>
        /// <returns>A collection of the unique intersection points.</returns>
        public static List<Vector3> PolylinePlane(Polyline poly, Plane pl)
        {
            List<Vector3> intersectionPts = new List<Vector3>();
            Line[] segments = poly.Segments();

            foreach (Line segment in segments)
            {
                if (!LinePlane(segment, pl, out Vector3 pt, out double t))
                {
                    continue;
                }

                if (t >= 0.0 && t <= 1.0)
                {
                    intersectionPts.Add(pt);
                }
            }

            return intersectionPts;
        }

        /// <summary>
        /// Computes the intersection between a circle and a line.<br/>
        /// If the intersection is computed the result points can be 1 or 2 depending on whether the line touches the circle tangentially or cuts through it.<br/>
        /// The intersection result false if the line misses the circle entirely.<br/>
        /// http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        /// </summary>
        /// <param name="cl">The circle for intersection.</param>
        /// <param name="ln">The line for intersection.</param>
        /// <param name="pts">Output the intersection points.</param>
        /// <returns>True if intersection is computed.</returns>
        public static bool LineCircle(Circle cl, Line ln, out Vector3[] pts)
        {
            Vector3 pt0 = ln.Start;
            Vector3 ptCircle = cl.Center;
            Vector3 lnDir = ln.Direction;
            Vector3 pt0PtCir = pt0 - ptCircle;

            double a = Vector3.Dot(lnDir, lnDir);
            double b = Vector3.Dot(lnDir, pt0PtCir) * 2;
            double c = Vector3.Dot(pt0PtCir, pt0PtCir) - (cl.Radius * cl.Radius);

            double det = b * b - 4 * a * c;
            double t;

            if ((a <= GeoSharpMath.MAXTOLERANCE) || (det < 0))
            {
                pts = new Vector3[] { };
                return false;
            }
            else if (Math.Abs(det) < GeoSharpMath.MAXTOLERANCE)
            {
                t = -b / (2 * a);
                Vector3 intersection = pt0 + lnDir * t;
                pts = new Vector3[] { intersection };
                return true;
            }
            else
            {
                t = (-b + Math.Sqrt(det)) / (2 * a);
                double t1 = (-b - Math.Sqrt(det)) / (2 * a);
                Vector3 intersection0 = pt0 + lnDir * t;
                Vector3 intersection1 = pt0 + lnDir * t1;
                pts = new Vector3[] { intersection0, intersection1 };
                return true;
            }
        }

        /// <summary>
        /// Computes the intersection between a plane and a circle.<br/>
        /// If the intersection is computed the result points can be 1 or 2 depending on whether the plane touches the circle tangentially or cuts through it.<br/>
        /// The intersection result false if the plane is parallel to the circle or misses the circle entirely.
        /// </summary>
        /// <param name="pl">The plane for intersection.</param>
        /// <param name="cl">The circle for intersection.</param>
        /// <param name="pts">Output the intersection points.</param>
        /// <returns>True if intersection is computed.</returns>
        public static bool PlaneCircle(Plane pl, Circle cl, out Vector3[] pts)
        {
            pts = new Vector3[] { };
            Vector3 clPt = cl.Center;

            Vector3 cCross = Vector3.Cross(pl.Origin, clPt);
            if (Math.Abs(cCross.Length()) < GeoSharpMath.EPSILON)
            {
                return false;
            }

            bool intersection = PlanePlane(pl, cl.Plane, out Line intersectionLine);
            Vector3 closestPt = intersectionLine.ClosestPt(clPt);
            double distance = clPt.DistanceTo(intersectionLine);

            if (Math.Abs(distance) < GeoSharpMath.EPSILON)
            {
                Vector3 pt = cl.ClosestPt(closestPt);
                pts = new[] { pt };
                return true;
            }

            return LineCircle(cl, intersectionLine, out pts);
        }

        /// <summary>
        /// Computes the intersection between a curve and a line.
        /// </summary>
        /// <param name="crv">The curve to intersect.</param>
        /// <param name="ln">The line to intersect with.</param>
        /// <returns>A collection of <see cref="CurvesIntersectionResult"/>.</returns>
        public static List<CurvesIntersectionResult> CurveLine(ICurve crv, Line ln)
        {
            return CurveCurve(crv, ln);
        }

        /// <summary>
        /// Computes the intersection between two curves.
        /// </summary>
        /// <param name="crv1">First curve to intersect.</param>
        /// <param name="crv2">Second curve to intersect.</param>
        /// <param name="tolerance">Tolerance set per default at 1e-6.</param>
        /// <returns>If intersection found a collection of <see cref="CurvesIntersectionResult"/> otherwise the result will be empty.</returns>
        public static List<CurvesIntersectionResult> CurveCurve(ICurve crv1, ICurve crv2, double tolerance = 1e-6)
        {
            List<Tuple<ICurve, ICurve>> bBoxTreeIntersections = BoundingBoxOperations.BoundingBoxTreeIntersection(new LazyCurveBBT(crv1), new LazyCurveBBT(crv2), 0);
            List<CurvesIntersectionResult> intersectionResults = bBoxTreeIntersections
                .Select(x => IntersectionRefiner.CurvesWithEstimation(crv1, crv2, x.Item1.Knots[0], x.Item2.Knots[0], tolerance))
                .Where(crInRe => (crInRe.Pt0 - crInRe.Pt1).SquaredLength() < tolerance)
                .Unique((a, b) => Math.Abs(a.T0 - b.T0) < tolerance * 5);

            return intersectionResults;
        }

        /// <summary>
        /// Computes the intersection between a curve and a plane.<br/>
        /// https://www.parametriczoo.com/index.php/2020/03/31/plane-and-curve-intersection/
        /// </summary>
        /// <param name="crv">The curve to intersect.</param>
        /// <param name="pl">The plane to intersect with the curve.</param>
        /// <param name="tolerance">Tolerance set per default at 1e-6.</param>
        /// <returns>If intersection found a collection of <see cref="CurvePlaneIntersectionResult"/> otherwise the result will be empty.</returns>
        public static List<CurvePlaneIntersectionResult> CurvePlane(ICurve crv, Plane pl, double tolerance = 1e-6)
        {
            List<ICurve> bBoxRoot = BoundingBoxOperations.BoundingBoxPlaneIntersection(new LazyCurveBBT(crv), pl);
            List<CurvePlaneIntersectionResult> intersectionResults = bBoxRoot.Select(
                x => IntersectionRefiner.CurvePlaneWithEstimation(crv, pl, x.Knots[0], x.Knots[0], tolerance)).ToList();

            return intersectionResults;
        }

        /// <summary>
        /// Computes the self intersections of a curve.
        /// </summary>
        /// <param name="crv">The curve for self-intersections.</param>
        /// <param name="tolerance">Tolerance set per default at 1e-6.</param>
        /// <returns>If intersection found a collection of <see cref="CurvesIntersectionResult"/> otherwise the result will be empty.</returns>
        public static List<CurvesIntersectionResult> CurveSelf(NurbsCurve crv, double tolerance = 1e-6)
        {
            List<Tuple<ICurve, ICurve>> bBoxTreeIntersections = BoundingBoxOperations.BoundingBoxTreeIntersection(new LazyCurveBBT(crv), 0);
            List<CurvesIntersectionResult> intersectionResults = bBoxTreeIntersections
                .Select(x => IntersectionRefiner.CurvesWithEstimation(x.Item1, x.Item2, x.Item1.Knots[0], x.Item2.Knots[0], tolerance))
                .Where(crInRe => Math.Abs(crInRe.T0 - crInRe.T1) > tolerance)
                .Unique((a, b) => Math.Abs(a.T0 - b.T0) < tolerance * 5);

            return intersectionResults;
        }
    }
}
