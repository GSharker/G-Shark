using System;
using System.Collections.Generic;
using GeometrySharp.Geometry;

namespace GeometrySharp.Core
{
    public class Trig
    {

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
                if (System.Math.Abs(tripleProduct) > GeoSharpMath.EPSILON)
                    return false;
            }

            return true;
        }

        // ToDo Add method to check points are collinear.
    }
}
