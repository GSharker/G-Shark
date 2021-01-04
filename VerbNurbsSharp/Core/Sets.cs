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

        /// <summary>
        /// Create a range of numbers. 
        /// </summary>
        /// <param name="domain">Domain of numeric range.</param>
        /// <param name="step">Number of steps</param>
        /// <returns>A range of numbers.</returns>
        public static IList<double> Range(Interval domain, int step)
        {
            if (step <= 0) return new List<double>() { domain.Min, domain.Max };
            var l = new List<double>();
            double f = 0.0;

            while (f <= step)
            {
                double normalizedParam = f / step;
                double number = domain.ParameterAt(normalizedParam);
                l.Add(number);
                ++f;
            }

            return l;
        }

        /// <summary>
        /// Create a range of positive numbers, incrementing of one step and starting from 0.
        /// </summary>
        /// <param name="maxValue">The dimension of the range.</param>
        /// <returns>A range of numbers.</returns>
        public static IList<double> Range(int maxValue)
        {
            if (maxValue <= 0) throw new Exception("Zero or negative value is not accepted");
            var l = new List<double>();
            double f = 0.0;

            while (f <= maxValue)
            {
                l.Add(f);
                ++f;
            }

            return l;
        }

        /// <summary>
        /// Create a series of numbers. 
        /// </summary>
        /// <param name="domain">First number in the series.</param>
        /// <param name="step">Step size for each successive number.</param>
        /// <param name="count">Number of values in the series.</param>
        /// <returns>Series of numbers.</returns>
        public static IList<double> Span(double start, double step, int count)
        {
            if (count <= 0) throw new Exception("Count as zero or negative is not accepted");
            var l = new List<double>();
            double counter = 0.0;
            double number1 = start;

            while (counter <= count)
            {
                l.Add(number1);
                number1 += step;
                ++counter;
            }

            return l;
        }

        public static Vector SortedSetUnion(Vector a, Vector b) => throw new NotImplementedException();
        public static Vector SortedSetSub(Vector a, Vector b) => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="num"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static List<T> Rep<T>(int num, T ele)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < num; i++)
                list.Add(ele);
            return list;
        }

        // ToDo will be integrated if necessary.
        public static double Min(Vector a) => throw new NotImplementedException();
        public static double Max(Vector a) => throw new NotImplementedException();
        public static bool All(List<bool> a) => throw new NotImplementedException();
    }
}
