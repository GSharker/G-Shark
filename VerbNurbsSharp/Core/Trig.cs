using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static bool isPointOnPlane(Point pt, Plane plane, double tol)
        {
            return Math.Abs(Vector.Dot(new Vector(Constants.Subtraction(pt, plane.Origin)), plane.Normal)) < tol;
        }
        /// <summary>
        /// Get the closest point on a ray from a point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="ray">The ray on which to find the point.</param>
        /// <returns>Get the closest point on a ray from a point.</returns>
        public static Point RayClosestPoint(Point pt, Ray ray)
        {
            Vector rayDirNormalized = Vector.Normalized(ray.Direction);
            Vector rayOriginToPt = new Vector(Constants.Subtraction(pt, ray.Origin));
            double dotResult = Vector.Dot(rayOriginToPt, rayDirNormalized);
            Point projectedPt =
                new Point(Constants.Addition(ray.Origin, Constants.Multiplication(rayDirNormalized, dotResult)));
            return projectedPt;
        }

        /// <summary>
        /// Get the distance of a point to a ray.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <param name="ray">The ray from which to calculate the distance.</param>
        /// <returns>The distance.</returns>
        public static double DistanceToRay(Point pt, Ray ray)
        {
            Point projectedPt = RayClosestPoint(pt, ray);
            Vector ptToProjectedPt = new Vector(Constants.Subtraction(projectedPt, pt));
            return Vector.Length(ptToProjectedPt);
        }

        /// <summary>
        /// Determine if the provide points are on the same plane.
        /// </summary>
        /// <param name="points">Provided points.</param>
        /// <returns>Whether the point are coplanar.</returns>
        public static bool ArePointsCoplanar(IList<Point> points)
        {
            if (points.Count < 3) return true;

            Vector vec1 = new Vector(Constants.Subtraction(points[1], points[0]));
            Vector vec2 = new Vector(Constants.Subtraction(points[2], points[0]));

            for (int i = 3; i < points.Count; i++)
            {
                Vector vec3 = new Vector(Constants.Subtraction(points[i], points[0]));
                double tripleProduct = Vector.Dot(Vector.Cross(vec1, vec2), vec3);
                // https://en.wikipedia.org/wiki/Triple_product
                if (Math.Abs(tripleProduct) > Constants.EPSILON)
                    return false;
            }

            return true;
        }

        // ToDo these methods are similar to the one that use the ray, in this case using a segment, I don't think is necessary.
        // It required a segment that is not defined as a data.
        public static Point segmentClosestPoint() => throw new NotImplementedException();
        public static double distToSegment() => throw new NotImplementedException();
    }
}
