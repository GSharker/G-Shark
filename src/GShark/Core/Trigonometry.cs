using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GShark.Core
{
    /// <summary>
    /// Provides basic trigonometry methods.
    /// </summary>
    public class Trigonometry
    {
        /// <summary>
        /// Determines if the provide points are on the same plane within specified tolerance.
        /// </summary>
        /// <param name="points">Points to check.</param>
        /// <param name="tolerance">Tolerance.</param>
        /// <returns>True if all points are coplanar.</returns>
        public static bool ArePointsCoplanar(IList<Point3> points, double tolerance = GSharkMath.Epsilon)
        {
            // https://en.wikipedia.org/wiki/Triple_product
            if (points.Count <= 3)
            {
                return true;
            }

            var vec1 = points[1] - points[0];
            var vec2 = points[2] - points[0];

            for (int i = 3; i < points.Count; i++)
            {
                var vec3 = points[i] - points[0];
                double tripleProduct = Vector3.DotProduct(Vector3.CrossProduct(vec3, vec2), vec1);
                if (Math.Abs(tripleProduct) > GSharkMath.Epsilon)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if three points form a straight line (are collinear) within a given tolerance.<br/>
        /// Find the area of the triangle without using square root and multiply it for 0.5.<br/>
        /// http://www.stumblingrobot.com/2016/05/01/use-cross-product-compute-area-triangles-given-vertices/
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <param name="tol">Tolerance set per default as 1e-3</param>
        /// <returns>True if the three points are collinear.</returns>
        public static bool ArePointsCollinear(Point3 pt1, Point3 pt2, Point3 pt3, double tol = GSharkMath.MaxTolerance)
        {
            Vector3 pt1ToPt2 = pt2 - pt1;
            Vector3 pt1ToPt3 = pt3 - pt1;
            Vector3 norm = Vector3.CrossProduct(pt1ToPt2, pt1ToPt3);
            double area = Vector3.DotProduct(norm, norm);

            return area < tol;
        }

        /// <summary>
        /// Calculates the point at the equal distance from the three points, it can be also described as the center of a circle.
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <returns>The point at the same distance from the three points.</returns>
        public static Point3 EquidistantPoint(Point3 pt1, Point3 pt2, Point3 pt3)
        {
            if (PointOrder(pt1, pt2, pt3) == 0)
                throw new Exception("Points must not be collinear.");

            Vector3 v1 = pt2 - pt1;
            Vector3 v2 = pt3 - pt1;

            double v1V1 = Vector3.DotProduct(v1, v1);
            double v2V2 = Vector3.DotProduct(v2, v2);
            double v1V2 = Vector3.DotProduct(v1, v2);

            double a = 0.5 / (v1V1 * v2V2 - v1V2 * v1V2);
            double k1 = a * v2V2 * (v1V1 - v1V2);
            double k2 = a * v1V1 * (v2V2 - v1V2);

            return pt1 + v1 * k1 + v2 * k2;
        }

        /// <summary>
        /// Gets the orientation between tree points in the plane.<br/>
        /// https://math.stackexchange.com/questions/2386810/orientation-of-three-points-in-3d-space
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <param name="pt3">Third point.</param>
        /// <returns>0 if points are collinear, 1 if point order is clockwise, 2 if point order is counterclockwise.</returns>
        public static int PointOrder(Point3 pt1, Point3 pt2, Point3 pt3)
        {
            Plane pl = new Plane(pt1, pt2, pt3);
            Vector3 n = Vector3.CrossProduct(pt2 - pt1, pt3 - pt1);
            double result = Vector3.DotProduct(pl.ZAxis, n.Unitize());

            if (Math.Abs(result) < GSharkMath.Epsilon)
            {
                return 0;
            }

            return (result < 0) ? 1 : 2;
        }

        /// <summary>
        /// Finds the closest point on a segment.<br/>
        /// It is not a segment like a line or ray but the part that compose the segment.<br/>
        /// The segment is deconstruct in two points and two t values.
        /// </summary>
        /// <param name="point">Point to project.</param>
        /// <param name="segmentPt0">First point of the segment.</param>
        /// <param name="segmentPt1">Second point of the segment.</param>
        /// <param name="valueT0">First t value of the segment.</param>
        /// <param name="valueT1">Second t value of the segment.</param>
        /// <returns>Tuple with the closest point its corresponding tValue on the curve.</returns>
        public static (double tValue, Point3 pt) ClosestPointToSegment(Point3 point, Point3 segmentPt0, Point3 segmentPt1, double valueT0, double valueT1)
        {
            Vector3 direction = segmentPt1 - segmentPt0;
            double length = direction.Length;

            if (length < GSharkMath.Epsilon)
            {
                return (tValue: valueT0, pt: segmentPt0);
            }

            Vector3 vecUnitized = direction.Unitize();
            Vector3 ptToSegPt0 = point - segmentPt0;
            double dotResult = Vector3.DotProduct(ptToSegPt0, vecUnitized);

            if (dotResult < 0.0)
            {
                return (tValue: valueT0, pt: segmentPt0);
            }

            if (dotResult > length)
            {
                return (tValue: valueT1, pt: segmentPt1);
            }

            Point3 pointResult = segmentPt0 + (vecUnitized * dotResult);
            double tValueResult = valueT0 + (valueT1 - valueT0) * dotResult / length;
            return (tValue: tValueResult, pt: pointResult);
        }

       
    }
}
