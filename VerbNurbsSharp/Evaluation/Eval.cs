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

        public static List<double> BasisFunctionsGivenKnotSpanIndex(int knotSpan_index, double u, int degree, List<double> knots)
        {
            var basisFunctions = Vec.Zeros1d(degree + 1);
            var left = Vec.Zeros1d(degree + 1);
            var right = Vec.Zeros1d(degree + 1);
            double saved = 0.0;
            double temp = 0.0;

            basisFunctions[0] = 1.0;
            for (int j = 1; j < degree+1; j++)
            {
                left[j] = u - knots[knotSpan_index + 1 - j];
                right[j] = knots[knotSpan_index + j] - u;
                saved = 0.0;
                for (int r = 0; r < j; r++)
                {
                    temp = basisFunctions[r] / (right[r + 1] + left[j - r]);
                    basisFunctions[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }
                basisFunctions[j] = saved;
            }
            return basisFunctions;
        }
    }
}
