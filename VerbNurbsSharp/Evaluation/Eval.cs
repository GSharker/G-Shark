using VerbNurbsSharp.Core;
using System.Collections.Generic;
using System;

namespace VerbNurbsSharp.Evaluation
{
    public class Eval
    {
        public static int KnotSpanGivenN(int n, int degree, double u, List<double> knots)
        {
            if (u > knots[n + 1] - Constants.EPSILON)
                return n;
            if (u < knots[degree] + Constants.EPSILON)
                return degree;
            var low = degree;
            var high = n + 1;
            var mid = (int)Math.Floor((decimal)((low + high) / 2));
            while (u < knots[mid] || u >= knots[mid + 1])
            {
                if (u < knots[mid])
                    high = mid;
                else
                    low = mid;
                mid = (int)Math.Floor((decimal)((low + high) / 2));
            }
            return mid;
        }
    }
}
