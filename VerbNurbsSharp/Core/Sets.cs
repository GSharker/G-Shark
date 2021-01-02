using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public static class Sets
    {
        // ToDo This is a Range of a list of numbers.
        // https://www.statisticshowto.com/probability-and-statistics/statistics-definitions/range-statistics/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Domain(Vector a) => a.Last() - a.First();

        // ToDo use interval
        // ToDo make test for this class.
        /// <summary>
        /// Create a range of numbers. 
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static List<double> Range(int max)
        {
            var l = new Vector();
            double f = 0.0;
            for (int i = 0; i < max; i++)
            {
                l.Add(f);
                f += 1.0;
            }
            return l;
        }

        // ToDo use interval
        // ToDo make test for this class.
        /// <summary>
        /// Create a series of numbers.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Vector Span(double min, double max, double step)
        {
            if (step < Constants.EPSILON) return new Vector();
            if (min > max && step > 0.0) return new Vector();
            if (max > min && step < 0.0) return new Vector();

            var l = new Vector();
            var cur = min;
            while (cur <= max)
            {
                l.Add(cur);
                cur += step;
            }
            return l;
        }

        // ToDo will be integrated if necessary.
        public static double Min(Vector a) => throw new NotImplementedException();
        public static double Max(Vector a) => throw new NotImplementedException();
        public static bool All(List<bool> a) => throw new NotImplementedException();
    }
}
