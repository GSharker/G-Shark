using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.Geometry;

namespace GeometrySharp.ExtendedMethods
{
    public static class ExtendedEnumerable
    {
        public static Vector3 ToVector(this IEnumerable<double> enumerable)
        {
            return new Vector3(enumerable.ToList());
        }

        public static Knot ToKnot(this IEnumerable<double> enumerable)
        {
            return new Knot(enumerable.ToList());
        }
    }
}
