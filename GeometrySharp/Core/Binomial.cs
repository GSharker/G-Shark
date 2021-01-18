using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometrySharp.Core
{
    public class Binomial
    {
        private static (int n, int k, double val) _storage = (0, 0, 0.0);

        /// <summary>
        /// Computes the binomial coefficient (denoted by *k choose n*).
        /// Please see the following website for details: http://mathworld.wolfram.com/BinomialCoefficient.html
        /// </summary>
        /// <param name="n">Size of the set of distinct elements.</param>
        /// <param name="k">Size of the subsets.</param>
        /// <returns>Combination of k and n</returns>
        public static double Get(int n, int k)
        {
            if (k == 0.0) return 1.0;
            if (n == 0 || k > n) return 0.0;
            if (k > n - k) k = n - k;

            if (_storage.n == n && _storage.k == k)
                return _storage.val;

            var r = 1.0;
            var n0 = n;

            for (int d = 1; d < k + 1; d++)
            {
                if (_storage.n == n0 && _storage.k == d)
                {
                    n--;
                    r = _storage.val;
                    continue;
                }

                r *= n--;
                r /= d;

                if (_storage.n == n0) continue;
                _storage.n = n0;
                _storage.k = d;
                _storage.val = r;
            }

            return r;
        }
    }
}
