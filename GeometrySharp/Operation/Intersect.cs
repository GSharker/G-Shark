using System;
using GeometrySharp.Core;
using GeometrySharp.Geometry;

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
        // ToDo: Polyline-Plane
        // ToDo: Polyline-Polyline
        // ToDo: Polyline-Self
        // ToDo: Line-Plane
        // ToDo: Line-Line

        /// <summary>
        /// Solves the intersection between two planes.
        /// </summary>
        /// <param name="p1">The first plane.</param>
        /// <param name="p2">The second plane.</param>
        /// <returns>The intersection as <see cref="Ray"/>.</returns>
        public static Ray PlanePlane(Plane p1, Plane p2)
        {
            Vector3 plNormal1 = p1.Normal;
            Vector3 plNormal2 = p2.Normal;

            Vector3 directionVec = Vector3.Cross(plNormal1, plNormal2);
            if(Vector3.Dot(directionVec, directionVec) < GeoSharpMath.EPSILON)
                throw new Exception("The two planes are parallel.");

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
                pt.AddRange(new[] { 0.0, corX, corY });
            else if (largeIndex == 1)
                pt.AddRange(new[] { corX, 0.0, corY });
            else
                pt.AddRange(new[] { corX, corY, 0.0 });

            return new Ray(pt, directionVec.Unitize());
        }

        /// <summary>
        /// Find the point intersection of a line and a plane.
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="line">The segment to intersect.</param>
        /// <param name="plane">The plane has to be intersected.</param>
        /// <returns>The point representing the unique intersection.</returns>
        public static Vector3 LinePlane(Line line, Plane plane)
        {
            Vector3 lnDir = line.Direction;
            Vector3 ptPlane = plane.Origin - line.Start;

            double denominator = Vector3.Dot(plane.Normal, lnDir);
            double numeric = Vector3.Dot(plane.Normal, ptPlane);

            if (Math.Abs(denominator) < GeoSharpMath.EPSILON)
                throw new Exception("Segment parallel to the plane or lies in plane.");

            // Compute the intersect parameter.
            double t = numeric / denominator;

            return line.Start + lnDir * t;
        }
    }
}
