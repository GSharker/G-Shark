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
            return 0;
        }
    }
}
