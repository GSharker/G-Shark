using System;
using System.Collections.Generic;
using GeometrySharp.Geometry;

namespace GeometrySharp.Core
{
    /// <summary>
    /// Trigonometry provides basic trigonometry methods.
    /// </summary>
    // Note is this the best name?
    public class Trigonometry
    {
        // ToDo isPointOnPlane.

        /// <summary>
        /// Determine if the provide points are on the same plane.
        /// </summary>
        /// <param name="points">Provided points.</param>
        /// <returns>Whether the point are coplanar.</returns>
        public static bool ArePointsCoplanar(IList<Vector3> points)
        {
            // https://en.wikipedia.org/wiki/Triple_product
            if (points.Count < 3) return true;

            Vector3 vec1 = points[1] - points[0];
            Vector3 vec2 = points[2] - points[0];

            for (int i = 3; i < points.Count; i++)
            {
                Vector3 vec3 = points[i] - points[0];
                double tripleProduct = Vector3.Dot(Vector3.Cross(vec3, vec2), vec1);
                if (Math.Abs(tripleProduct) > GeoSharpMath.EPSILON)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determine if three points form a straight line (are collinear) within a given tolerance.
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <param name="tol">Tolerance.</param>
        /// <returns>True if the three points are collinear.</returns>
        public static bool AreThreePointsCollinear(Vector3 pt1, Vector3 pt2, Vector3 pt3, double tol)
        {
            // Find the area of the triangle without using square root and multiply it for 0.5
            // http://www.stumblingrobot.com/2016/05/01/use-cross-product-compute-area-triangles-given-vertices/
            var pt1ToPt2 = pt2 - pt1; 
            var pt1ToPt3 = pt3 - pt1;
            var norm = Vector3.Cross(pt1ToPt2, pt1ToPt3);
            var area = Vector3.Dot(norm, norm);

            return area < tol;
        }
    }
}
