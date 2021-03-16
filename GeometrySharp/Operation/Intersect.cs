using GeometrySharp.Core;
using GeometrySharp.Geometry;
using System;
using System.Collections.Generic;

namespace GeometrySharp.Operation
{
    /// <summary>
    /// Intersection provides various tools for all kinds of intersection
    /// </summary>
    public class Intersect
    {
        // ToDo: Curve-Plane
        // ToDo: Curve-Curve
        // ToDo: Curve-Line
        // ToDo: Curve-Self
        // ToDo: Polyline-Polyline
        // ToDo: Polyline-Self

        /// <summary>
        /// Solves the intersection between two planes.
        /// This method returns true if intersection is found, false if the
        /// planes are parallel.
        /// </summary>
        /// <param name="p1">The first plane.</param>
        /// <param name="p2">The second plane.</param>
        /// <param name="ray">The intersection as <see cref="Ray"/>.</param>
        /// <returns>True if the intersection success.</returns>
        public static bool PlanePlane(Plane p1, Plane p2, out Ray ray)
        {
            Vector3 plNormal1 = p1.Normal;
            Vector3 plNormal2 = p2.Normal;

            Vector3 directionVec = Vector3.Cross(plNormal1, plNormal2);
            if (Vector3.Dot(directionVec, directionVec) < GeoSharpMath.EPSILON)
            {
                ray = new Ray(Vector3.Unset, Vector3.Unset);
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

            ray = new Ray(pt, directionVec.Unitize());
            return true;
        }

        /// <summary>
        /// Finds the unique point intersection of a line and a plane.
        /// This method returns true if intersection return the unique point, it returns
        /// false if the segment is parallel to the plane or lies in plane.
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="line">The segment to intersect. Assumed as infinite.</param>
        /// <param name="plane">The plane has to be intersected.</param>
        /// <param name="pt">The point representing the unique intersection.</param>
        /// <param name="t">The parameter on the line between 0.0</param>
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
        /// Solves the intersection between two lines, assumed as infinite.
        /// Returns as outputs two points describing the minimum distance between the two lines.
        /// Returns false if the segments are parallel.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="ln0">The first line.</param>
        /// <param name="ln1">The second line.</param>
        /// <param name="pt0">The output point of the first line.</param>
        /// <param name="pt1">The output point of the second line.</param>
        /// <returns>True if the intersection succeed.</returns>
        public static bool LineLine(Line ln0, Line ln1, out Vector3 pt0, out Vector3 pt1)
        {
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
                return false;
            }

            double s = (b * e - c * d) / div;
            double t = (a * e - b * d) / div;

            pt0 = ln0.Start + lnDir0 * s;
            pt1 = ln1.Start + lnDir1 * t;
            return true;
        }

        /// <summary>
        /// Computes the intersection between a polyline and a plane.
        /// Under the hood, is intersecting each segment with the plane and storing the intersection point into a collection.
        /// If no intersections are found a empty collection is returned.
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
                if (!LinePlane(segment, pl, out Vector3 pt, out double t)) continue;
                if(t >= 0.0 && t <= 1.0)
                {
                    intersectionPts.Add(pt);
                }
            }

            return intersectionPts;
        }
    }
}
