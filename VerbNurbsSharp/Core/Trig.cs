using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Trig
    {
        //public static Point RayClosestPoint
        // ToDo the original method used 3 points, but 3 points are always coplanar.
        /// <summary>
        /// Determine if the provide points are on the same plane.
        /// </summary>
        /// <param name="points">Provided points.</param>
        /// <returns>Whether the point are coplanar.</returns>
        public static bool ArePointsCoplanar(IList<Point> points)
        {
            if (points.Count < 3) return true;
            
            Vector vec1 = Vector.Subtraction(points[1], points[0]);
            Vector vec2 = Vector.Subtraction(points[2], points[0]);

            for (int i = 3; i < points.Count; i++)
            {
                Vector vec3 = Vector.Subtraction(points[i], points[0]);
                double tripleProduct = Vector.Dot(Vector.Cross(vec1, vec2), vec3);
                // https://en.wikipedia.org/wiki/Triple_product
                if (Math.Abs(tripleProduct) > Constants.EPSILON)
                    return false;
            }
            return true;
        }
    }
}
