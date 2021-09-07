using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using GShark.Interfaces;

namespace GShark.Core
{
    /// <summary>
    /// Provides basic trigonometry methods.
    /// </summary>
    public class Trigonometry
    {
        /// <summary>
        /// Determines if the provide points are on the same plane.
        /// </summary>
        /// <param name="points">Provided points.</param>
        /// <returns>Whether the point are coplanar.</returns>
        public static bool ArePointsCoplanar(IList<Point3> points)
        {
            // https://en.wikipedia.org/wiki/Triple_product
            if (points.Count < 3)
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
        /// A quickSort algorithm used to order the curves in a sequential manner based on the distance between the first and last points of each curves.<br/>
        /// https://github.com/mcneel/opennurbs/blob/c20e599d1ff8f08a55d3dddf5b39e37e8b5cac06/opennurbs_curve.cpp#L3600 <br/>
        /// https://exceptionnotfound.net/quick-sort-csharp-the-sorting-algorithm-family-reunion/
        /// </summary>
        /// <param name="curves">The sets of curve to sort.</param>
        /// <returns>The set of curves sorted.</returns>
        public static List<NurbsCurve> QuickSortCurve(IList<NurbsCurve> curves)
        {
            if (curves == null || curves.Count == 0)
            {
                throw new Exception("The set of curves is empty.");
            }

            if (curves.Count == 1)
            {
                return curves.ToList();
            }

            // creates the sets of data required.
            List<Point3[]> lines = curves.Select(c => new []{c.StartPoint, c.EndPoint}).ToList();
            int[] indexes = new int[lines.Count];
            bool[] revers = new bool[lines.Count];
            for (int t = 0; t < lines.Count; t++)
            {
                revers[t] = false;
                indexes[t] = t;
            }

            // sort lines
            for (int ni = 1; ni < lines.Count; ni++)
            {
                int endI, endEnd, i;
                var startI = endI = ni;
                var startEnd = endEnd = 0;
                var startPoint = (revers[0]) ? lines[indexes[0]][1] : lines[indexes[0]][0];
                var endPoint = (revers[ni - 1]) ? lines[indexes[ni - 1]][0] : lines[indexes[ni - 1]][1];
                var startDistance = startPoint.DistanceTo(lines[indexes[startI]][0]);
                var endDistance = endPoint.DistanceTo(lines[indexes[endI]][0]);

                for (i = ni; i < lines.Count; i++)
                {
                    Point3 testingPoint = lines[indexes[i]][0];
                    for (int end = 0; end < 2; end++)
                    {
                        double testingDistance = startPoint.DistanceTo(testingPoint);
                        if (testingDistance < startDistance)
                        {
                            startI = i;
                            startEnd = end;
                            startDistance = testingDistance;
                        }

                        testingDistance = endPoint.DistanceTo(testingPoint);
                        if (testingDistance < endDistance)
                        {
                            endI = i;
                            endEnd = end;
                            endDistance = testingDistance;
                        }

                        testingPoint = lines[indexes[i]][1];
                    }
                }

                if (startDistance < endDistance)
                {
                    // N[index[startI]] will be first in list.
                    i = indexes[ni];
                    indexes[ni] = indexes[startI];
                    indexes[startI] = i;
                    startI = indexes[ni];
                    for (i = ni; i > 0; i--)
                    {
                        indexes[i] = indexes[i - 1];
                        revers[i] = revers[i - 1];
                    }
                    indexes[0] = startI;
                    revers[0] = (startEnd != 1);
                }
                else
                {
                    // N[index[endI]] will be next in the list.
                    i = indexes[ni];
                    indexes[ni] = indexes[endI];
                    indexes[endI] = i;
                    revers[ni] = (endEnd == 1);
                }
            }

            List<NurbsCurve> sortedCurves = indexes.Select(i => (revers[i]) ? curves[i].Reverse() : curves[i]).ToList();
            return sortedCurves;
        }
    }
}
