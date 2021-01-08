using System.Collections.Generic;
using System.Linq;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.ExtendedMethods
{
    public static class ExtendedEnumerable
    {
        public static Vector ToVector(this IEnumerable<double> enumerable)
        {
            return new Vector(enumerable.ToList());
        }
    }
}
