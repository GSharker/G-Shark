using System;
using System.Collections.Generic;
using VerbNurbsSharp.Geometry;

namespace VerbNurbsSharp.Core
{
    public class Trig
    {
        /// <summary>
        /// Determinate if the provided point lies on the plane.
        /// </summary>
        /// <param name="pt">The point to check if it lies on the plane.</param>
        /// <param name="plane">The plane on which to find if the point lies on.</param>
        /// <param name="tol">Tolerance value.</param>
        /// <returns>Whether the point is on the plane.</returns>
        public static bool isPointOnPlane(Vector3 pt, Plane plane, double tol)
        {
            return System.Math.Abs(Vector3.Dot(pt - plane.Origin , plane.Normal)) < tol;
        }
        /// <summary>
        /// Get the closest point on a ray from a point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="ray">The ray on which to find the point.</param>
        /// <returns>Get the closest point on a ray from a point.</returns>
        public static Vector3 RayClosestPoint(Vector3 pt, Ray ray)
        {
            Vector3 rayDirNormalized = Vector3.Normalized(ray.Direction);
            Vector3 rayOriginToPt = pt - ray.Origin;
            double dotResult = Vector3.Dot(rayOriginToPt, rayDirNormalized);
            Vector3 projectedPt = ray.Origin + (rayDirNormalized * dotResult);
            return projectedPt;
        }

        /// <summary>
        /// Get the distance of a point to a ray.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <param name="ray">The ray from which to calculate the distance.</param>
        /// <returns>The distance.</returns>
        public static double DistanceToRay(Vector3 pt, Ray ray)
        {
            Vector3 projectedPt = RayClosestPoint(pt, ray);
            Vector3 ptToProjectedPt = projectedPt -  pt;
            return Vector3.Length(ptToProjectedPt);
        }

        /// <summary>
        /// Determine if the provide points are on the same plane.
        /// </summary>
        /// <param name="points">Provided points.</param>
        /// <returns>Whether the point are coplanar.</returns>
        public static bool ArePointsCoplanar(IList<Vector3> points)
        {
            if (points.Count < 3) return true;

            Vector3 vec1 = points[1] - points[0];
            Vector3 vec2 = points[2] - points[0];

            for (int i = 3; i < points.Count; i++)
            {
                Vector3 vec3 = points[i] - points[0];
                double tripleProduct = Vector3.Dot(Vector3.Cross(vec1, vec2), vec3);
                // https://en.wikipedia.org/wiki/Triple_product
                if (System.Math.Abs(tripleProduct) > Math.EPSILON)
                    return false;
            }

            return true;
        }

        // ToDo these methods are similar to the one that use the ray, in this case using a segment, I don't think is necessary.
        // It required a segment that is not defined as a data.
        public static Vector3 segmentClosestPoint() => throw new NotImplementedException();
        public static double distToSegment() => throw new NotImplementedException();
    }
}
