using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;

namespace VerbNurbsSharp.Core
{
    public static class Sets
    {
        /// <summary>
        /// The range of a set of number, or the distance between the smallest value to the biggest in the collection.
        /// </summary>
        /// <param name="a">Set of numbers.</param>
        /// <returns>The range.</returns>
        // https://www.statisticshowto.com/probability-and-statistics/statistics-definitions/range-statistics/
        public static double Dimension(IList<double> a)
        {
            var sortedSet = a.OrderBy(x => x);
            return sortedSet.Last() - sortedSet.First();
        }

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
        /// Returns a list of evenly spaced numbers over a specified interval.
        /// </summary>
        /// <param name="domain">Domain of numeric range.</param>
        /// <param name="step">Number of steps.</param>
        /// <returns>A list of equally spaced numbers.</returns>
        public static IList<double> LinearSpace(Interval domain, int step)
        {
            if(Math.Abs(domain.Min - domain.Max) <= Constants.EPSILON) return new List<double>(){ domain.Min };
            var linearSpace = new List<double>();

            if (step <= 1)
            {
                linearSpace.Add(domain.Min);
                return linearSpace;
            }

            var div = step - 1;
            var delta = domain.Max - domain.Min;
            for (int i = 0; i < step; i++)
                linearSpace.Add(domain.Min + (i * delta / div));
            return linearSpace;
        }

        /// <summary>
        /// Create a range of positive numbers, incrementing of one step and starting from 0.
        /// </summary>
        /// <param name="maxValue">The dimension of the range.</param>
        /// <returns>A range of numbers.</returns>
        public static IList<double> Range(int maxValue)
        {
            if (maxValue <= 0) throw new Exception("Max value range can not be negative or zero.");
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
            if (count <= 0) throw new Exception("Count can not be negative or zero.");
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
        // ToDo the original doesn't provide a set union, we have to keep an eye on this method.
        // A removed the Sorted from the name due to the method doesn't sort the final list.
        /// <summary>
        /// The set union of two sequences of numbers.
        /// </summary>
        /// <param name="a">First set.</param>
        /// <param name="b">Second set.</param>
        /// <returns>The set union.</returns>
        public static List<double> SetUnion(IList<double> a, IList<double> b)
        {
            if (a.Count == 0) return b.ToList();
            return b.Count == 0 ? a.ToList() : a.Union(b).ToList();
        }

        // A removed the Sorted from the name due to the method doesn't sort the final list.
        /// <summary>
        /// The set difference from two sequences of numbers, sorted.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<double> SetDifference(IList<double> a, IList<double> b)
        {
            if(a.Count == 0) throw new Exception("Set difference can't be computed, the first set is empty.");
            return a.Except(b).ToList();
        }

        /// <summary>
        /// Repeat data until it reaches the defined length.
        /// </summary>
        /// <param name="data">Data to repeat.</param>
        /// <param name="length">Length of the final set.</param>
        /// <returns>Set of repeated data.</returns>
        public static List<T> RepeatData<T>(T data, int length)
        {
            if (length < 0) throw new Exception("Length can not be negative.");
            List<T> list = new List<T>();
            for (int i = 0; i < length; i++)
                list.Add(data);
            return list;
        }

        // ToDo will be integrated if necessary.
        public static double Min(Vector a) => throw new NotImplementedException();
        public static double Max(Vector a) => throw new NotImplementedException();
        public static bool All(List<bool> a) => throw new NotImplementedException();
    }
}
