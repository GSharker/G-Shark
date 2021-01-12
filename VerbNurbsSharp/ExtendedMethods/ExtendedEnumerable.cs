using System.Collections.Generic;
using System.Linq;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.ExtendedMethods
{
    public static class ExtendedEnumerable
    {
        public static Vector3 ToVector(this IEnumerable<double> enumerable)
        {
            return new Vector3(enumerable.ToList());
        }

        public static KnotArray ToKnot(this IEnumerable<double> enumerable)
        {
            return new KnotArray(enumerable.ToList());
        }
    }
}
